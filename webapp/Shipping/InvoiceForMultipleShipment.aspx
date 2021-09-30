<%@ Page Language="C#" Theme="DefaultTheme"  AutoEventWireup="true" CodeBehind="InvoiceForMultipleShipment.aspx.cs" Inherits="com.next.isam.webapp.shipping.InvoiceForMultipleShipment"  Title="Invoice for Multiple Shipment"%>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc3" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Invoice for Multiple Shipment</title>
    <link href="../includes/RevampStyle.css" type="text/css" rel="stylesheet"/>
    <script src="../common/common.js" type="text/javascript" ></script>   
</head>
<body onload="init();">


<form id="frm_InvoiceForMultipleShipment" runat="server"  >
<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>


<script type="text/javascript">

    function validateDate(obj) {
        var dt="";
        
        dt = obj.value;
        if (isDateValid(dt) || dt.trim() == "")
            return true;
        else {
            alert("Invalid Date");
            obj.select();
            return false;
        }
    }
    
    
    function validateInt(obj) {
        var num;

        if (obj.value == "")
            return true;
        num = getObjInt(obj);
        if (isNaN(num) || num < 0) {
            alert("Invalid number");
            obj.select();
            return false;
        }
        else {
            obj.value = num;
            return true;
        }
    }


    function validateDecimal(obj) {
        var num;

        if (obj.value == "")
            return true;
        num = getObjDecimal(obj);
        if (isNaN(num) || num < 0) {
            alert("Invalid number");
            obj.select();
            return false;
        }
        else {
            obj.value = num;
            return true;
        }
    }


    function validateInvoiceNo(obj) {
        var inv;
        var invStr;
        var pfx, seq, year;

        invStr = obj.value.trim();
        if (invStr != "") {
            inv = invStr.split("/");
            //alert(inv);
            if (inv.length != 3 || isNaN(parseInt(inv[1])) || isNaN(parseInt(inv[2]))) {
                alert("Invalid Invoice Number format.");
                obj.select();
                return false;
            }
            else
                if (inv[1] <= 0 || inv[1] > 99999 || inv[2] <= 0 || inv[2] > 9999) {
                alert("Invalid Invoice Number format.");
                obj.select();
                return false;
            }
            pfx = inv[0].toUpperCase().trim();
            seq = "00000" + inv[1].trim();
            seq = seq.substring(seq.length - 5, seq.length);
            year = "0000" + inv[2].trim();
            year = year.substring(year.length - 4, year.length);
            obj.value = pfx + "/" + seq + "/" + year;
        }
        return true;
    }


    function getObjInt(obj) {
        var numStr;
        var commaPos;
        var num;

        numStr=obj.value.trim();
        if (numStr == "")
            num = 0;
        else
        { 
            while ((commaPos = numStr.indexOf(",")) >= 0)
                numStr = numStr.substring(0, commaPos) + numStr.substring(commaPos + 1, numStr.length);
            num = parseInt(numStr);
        }
        return num;
    }


    function getObjDecimal(obj) {
        var numStr;
        var commaPos;
        var num;

        numStr = obj.value.trim();
        if (numStr == "")
            num = 0;
        else {
            while ((commaPos = numStr.indexOf(",")) >= 0)
                numStr = numStr.substring(0, commaPos) + numStr.substring(commaPos + 1, numStr.length);
            num = parseFloat(numStr);
        }
        return num;
    }


    function RefreshShipmentTotal(SeqNo) {
        var i;
        var obj;
        var rowPrefix;
        var totalRowPrefix;
        var qtyTextBoxName;
        var textBoxNameStartPos;
        var SellingAmt;
        var SupplierAmt;
        var TotalQty;
        var shippedQty;
        var amount;

        qtyTextBoxName = "txt_ShippedQty";
        totalRowPrefix = "";
        SellingAmt = 0;
        SupplierAmt = 0;
        TotalQty = 0;
        
        var inputElement = document.getElementsByTagName("input");
        for (i = 0; i < inputElement.length; i++) {
            obj = inputElement[i];
            if (obj.SequenceNo == SeqNo) {
                textBoxNameStartPos = obj.id.indexOf(qtyTextBoxName);
                if (textBoxNameStartPos >= 0) { // Shipped Qty text box for the specified sequence no.
                    if (obj.SellingPrice != "") {
                        shippedQty = parseInt(obj.value);
                        TotalQty += shippedQty;
                        amount = Math.round(shippedQty * parseFloat(obj.SellingPrice) * 100) / 100;
                        if (!isNaN(amount)) {
                            SellingAmt += amount;
                        }
                        amount = Math.round(shippedQty * parseFloat(obj.SupplierPrice) * 100) / 100;
                        if (!isNaN(amount)) {
                            SupplierAmt += amount;
                        }
                    }
                    else {   // Shipment Total
                        totalRowPrefix = obj.id.substring(0, textBoxNameStartPos);
                    }
                }   // Shipped Qty text box
            }   // Same Seq No.
        }
        document.getElementById(totalRowPrefix + "lbl_SellingAmt").innerText = SellingAmt.toLocaleString();
        document.getElementById(totalRowPrefix + "lbl_SupplierAmt").innerText = SupplierAmt.toLocaleString();
        document.getElementById(totalRowPrefix + "txt_ShippedQty").value = TotalQty.toString();
        document.getElementById(totalRowPrefix + "lbl_ShippedQty").innerText = TotalQty.toString();

        return true;
    }
    
    
    function CopyQtyToShippedQty(QtyType, Caller) {
        var fromTextBox = "txt_" + QtyType;
        var targetBox = "txt_ShippedQty";
        var objShipQty;
        var rowPrefix;
        var amount;
        var obj;
        var tmpCount = 0;
        
        var inputElement = document.getElementsByTagName("input");
        for (i = 0; i < inputElement.length; i++) {
            if (inputElement[i].type == "text") {
                if (inputElement[i].id.indexOf(targetBox) >= 0) {
                    objShipQty = inputElement[i];
                    if (objShipQty.Locked == "False" && (Caller == "USER" || (Caller == "SYSTEM" && objShipQty.Invoiced == "False" && objShipQty.InitialTotalShippedQty == "0"))) {
                        if (Caller == "USER" || (Caller == "SYSTEM" && (objShipQty.value == "" || parseInt(objShipQty.value) == 0))) {
                            if (QtyType == "OrderQty")
                                objShipQty.value = objShipQty.OrderQty;
                            else {
                                if (QtyType == "POQty")
                                    objShipQty.value = objShipQty.POQty;
                                else
                                    return false;
                            }
                        }
                        rowPrefix = objShipQty.id.substring(0, objShipQty.id.indexOf(targetBox));
                        if (objShipQty.sellingPrice != "" && objShipQty.SupplierPrice != "") {
                            amount = Math.round(parseFloat(objShipQty.SellingPrice) * parseInt(objShipQty.value) * 100) / 100;
                            document.getElementById(rowPrefix + "lbl_SellingAmt").innerText = amount.toLocaleString();
                            objShipQty.SellingAmt = amount;

                            amount = Math.round(parseFloat(objShipQty.SupplierPrice) * parseInt(objShipQty.value) * 100) / 100;
                            document.getElementById(rowPrefix + "lbl_SupplierAmt").innerText = amount.toLocaleString();
                            objShipQty.SupplierAmt = amount;
                        }
                    }
                } // Current element is ShippedQty
            }
        }
        for (i = 0; i < inputElement.length; i++) {
            if (inputElement[i].type == "text") {
                if (inputElement[i].id.indexOf(targetBox) >= 0) {
                    objShipQty = inputElement[i];
                    //if (objShipQty.SellingPrice == "" && objShipQty.SupplierPrice == "") {   // Row of Shipment Total
                    if (objShipQty.SubTotalRow == "true") {
                        if (objShipQty.Locked == "False")
                            RefreshShipmentTotal(objShipQty.SequenceNo);
                    }
                }
            }
        }
        return true;
    }


    function CopyPOQtyToShippedQty() {
        CopyQtyToShippedQty("POQty", "USER");
        return;
    }


   function CopyOrderQtyToShippedQty() {
       CopyQtyToShippedQty("OrderQty", "USER");
       return;
   }


   function calcTotalAmt(obj) {
       var objSellingAmt;
       var objSupplierAmt;
       var amount;
       var num;

        num = getObjInt(obj);
        if (isNaN(num) || num<0)
        {
            alert("Invalid number");
            obj.select();
            return false;
        }
        else
        {
            obj.value = num;
            objSellingAmt = document.getElementById(obj.id.replace("txt_ShippedQty", "lbl_SellingAmt"));
            objSupplierAmt = document.getElementById(obj.id.replace("txt_ShippedQty", "lbl_SupplierAmt"));
            amount = Math.round(num * parseFloat(obj.SellingPrice) * 100) / 100;
            objSellingAmt.innerText = amount.toLocaleString();
            amount = Math.round(num * parseFloat(obj.SupplierPrice) * 100) / 100;
            objSupplierAmt.innerText = amount.toLocaleString();
            RefreshShipmentTotal(obj.SequenceNo);
            return true;
        }
   }

   function changeDestinationStatus(obj) {
       var ddl, txt;
       
       if (obj.id.indexOf("ddl_") >= 0) {
           // hide ddl, show txt
           ddl = obj;
           txt = document.getElementById(obj.id.replace("ddl_", "txt_"));
           txt.value = ddl[ddl.selectedIndex].text;
           ddl.style.display = "none";
           txt.style.display = "block";
       }
       else
           if (obj.id.indexOf("txt_") >= 0) {
           // hide txt, show ddl
               txt = obj;
               ddl = document.getElementById(obj.id.replace("txt_", "ddl_"));
               txt.style.display = "none";
               ddl.style.display = "block";
               ddl.select;
               ddl.focus;
               ddl.click;
           }
   return;
   }

     
   function cancel() {
       window.close();
   }

   function initCopyButton() {
       var targetBox = "txt_ShippedQty";
       var objShipQty;
       var obj;

       var lockButton = false;
       var inputElement = document.getElementsByTagName("input");
        for (i = 0; i < inputElement.length; i++) {
           if (inputElement[i].type == "text") {
               if (inputElement[i].id.indexOf(targetBox) >= 0) {
                   objShipQty = inputElement[i];
                   if (objShipQty.Locked == "True") {
                       lockButton = true;
                       break;
                   }// Current element is ShippedQty
               }
           } 
       }

       if (lockButton) {
           document.getElementById('btn_CopyOrderQty').disabled = true;
           document.getElementById('btn_CopyPOQty').disabled = true;
       }
       else {
           document.getElementById('btn_CopyOrderQty').disabled = false;
           document.getElementById('btn_CopyPOQty').disabled = false;
       }
   }

    function init()
    {
        initCopyButton();
        return;
    }

    function confirmIfTotalShippedQtyZero(allowZeroQty) {
        var totalQty = 0, qty = 0;
        var anyShippedQty = false;
        var invNo = '', invDate = '';

        if (document.all.txt_InvoiceDate!=null)
            invDate = document.all.txt_InvoiceDate.value;
        if (document.all.txt_InvoiceNo!=null)
            invNo = document.all.txt_InvoiceNo.innerText;
        if (invNo != '' || invDate != '') {
            var nodeList = document.getElementsByTagName('input');
            for (i = 0, anyInvalid = false; i < nodeList.length && !anyInvalid; i++) {

                if (nodeList[i].name.indexOf("txt_ShippedQty") > 0) {
                    anyShippedQty = true;
                    str = nodeList[i].value.replace(",", "").trim();
                    qty = parseInt(str, 10);
                    if (qty >= 0 && str.replace(qty.toString(), "") == "")
                        totalQty += qty;
                    else
                        anyInvalid = true;
                }
            }
            if (anyInvalid) {
                alert('Invalid shipped quantity.');
                setSaveMode('OFF');
                return false;
            }
            else
                if (anyShippedQty && totalQty == 0) {
                    if (allowZeroQty) {
                        if (confirm('Total Shipped Quantity is zero. Confirm to save?'))
                            return true;
                        else {
                            setSaveMode('OFF');
                            return false;
                        }
                    }
                    else {
                        alert('Total Shipped Quantity cannot be zero.');
                        setSaveMode('OFF');
                        return false;
                    }
                }
        }
        return true;
    }

    // Save Action Control
    function enableButtons(flag) {
        document.getElementById('btn_Save').disabled = !flag;
        document.getElementById('btn_Reset').disabled = !flag;
        document.getElementById('btn_Print').disabled = !flag;
        document.getElementById('btn_Close').disabled = !flag;
    }

    function setSaveMode(state) {
        var saveButton = document.getElementById('btn_Save');
        switch (state) {
            case 'OFF':
                enableButtons(true);
                saveButton.clicked = false;
                break;
            case 'ON':
                enableButtons(false);
                saveButton.clicked = true;
                saveButton.disabled = false; // Allow to process the save action in server side
                break;
            case 'FREEZE':
                enableButtons(false);   // Do not allow to process the server side action
                saveButton.clicked = true; 
                break;
        }
    }

    function allowToSave() {
        var saveButton = document.getElementById('btn_Save');
        if (saveButton.clicked == null || saveButton.clicked == false) { // Have not click on the SAVE button
            setSaveMode('ON');
            return true;
        }
        else // Reclick on SAVE button
            setSaveMode('FREEZE');
        return false;
    }

    function suppressSpecialKey(evt) {
        evt = evt || window.event;
        var target = evt.target || evt.srcElement;
        if (evt.type == "keydown")
            switch (evt.keyCode) {
            case 8: // BackSpace
                var isEditable = !target.disabled && !(target.readOnly == null ? false : target.readOnly);
                if (!(isEditable && /input|textarea/i.test(target.nodeName) && /text|password/i.test(target.type))) {
                    // not in an active input area, block the key
                    evt.keyCode = 0;
                    evt.returnValue = false;
                }
                break;
            case 116: // 'F5' - Refresh Page, block the key
                evt.keyCode = 0;
                evt.returnValue = false;
                break;
        }
    }
    document.onkeydown = suppressSpecialKey;
    document.onkeypress = suppressSpecialKey;

</script>



<table width='100%' cellpadding='0' cellspacing='0'>
<tr>
    <td class="tableHeader" style=' font-size:80%;color:Black;font-weight:bold;display:none;' >Invoice to Mulitple Shipment</td>
</tr>
</table>
  
<asp:HiddenField ID='hid_WindowOnLoadAction' runat='server' Value='' />

<asp:Panel ID='pnl_MainBlock' runat='server' Visible='true' style='cursor:auto'>

    <table cellspacing='0' cellpadding='2' border='0' style='font-size: 14px; width: 790px;'>
         <tr>
            <td class="FieldLabel2" >
                <asp:Label runat='server'>Invoice No.</asp:Label>
            </td>
            <td >
                <asp:TextBox ID ='txt_InvoiceNo' runat='server' style='width:90px;' onblur="validateInvoiceNo(this);" />
            </td>
            <td align='right' class="FieldLabel2">
                &nbsp;&nbsp;<asp:Label ID='lbl_InvoiceDate' runat='server'>Invoice Date</asp:Label>&nbsp;&nbsp;
            </td>
            <td >
                 <asp:TextBox ID ='txt_InvoiceDate' runat='server' style='width:60px;' onblur='validateDate(this);' />
            </td>
            <td>
                &nbsp;
                <asp:Button ID='btn_Save'  runat='server' Text='Save'  skinid="SButton" onclick="btn_Save_Click"/>&nbsp;&nbsp;
                <asp:Button ID='btn_Reset' runat='server' Text='Reset' skinid="SButton"  onclick="btn_Reset_Click"/>&nbsp;&nbsp;
                <asp:Button ID='btn_Print' runat='server' Text='Print Invoice' SkinID="LButton" OnClick='btn_Print_Click'  Enabled='false' />&nbsp;&nbsp;
                <asp:Button ID='btn_Close' runat='server' Text='Close' skinid="SButton" OnClientClick='window.close();return false;' />
            </td>
           </tr>
    </table>
    <asp:HiddenField ID='hid_AllAreMockShopOrder_ToBeRemoved' Value='false' runat='server'/>

    <br/>
   
    <table border='0' style='display:block;'>
        <tr >
            <td>
                <asp:GridView ID="gv_MultiShipmentInvoice" runat="server" AutoGenerateColumns="false"  CssClass='TableWithGridLines'
                    onrowdatabound="gv_MultiShipmentInvoice_RowDataBound" Visible="true"  >
                    <Columns>
                        <asp:TemplateField headertext="Seq. No." >
                            <ItemTemplate >
                            <asp:Label ID='lbl_RowIndex' runat='server' style='display:none;'></asp:Label>
                            <asp:Label ID='lbl_SequenceNo' runat='server'></asp:Label>
                            <asp:Label ID='lbl_LineNo' runat='server' style='display:none;'></asp:Label>
                            <asp:Label ID='lbl_Locked' runat='server' style='display:none;'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Contract No." >
                            <ItemTemplate>
                                <asp:LinkButton ID="lnk_ContractNo" runat="server" Text='' PostBackUrl="~/Shipping/ShipmentDetail.aspx?ShipmentId=?"  />
                                <asp:Panel id='pnl_EditBlock' runat='server' style='border-style:ridge;border-width:thin;width:380px;font-size:12px;' Visible='false'>
                                    <table cellpadding=1 cellspacing=1 border=0 
                                    style="border-style:none;font-size: 12px; text-align:left; display:block;" width='100%'>
                                        <col /><col width='60px'/><col /><col width='80px'/><col />
                                        <tr>
                                            <td style="border-style: none;">Supplier<br />Invoice No. </td>
                                            <td style="border-style: none;" colspan='2'><asp:TextBox ID='txt_SupplierInvNo' runat='server' Text='' style="width: 120px"/></td>
                                            <td style="border-style: none;">Actual AWH Date </td>
                                            <td style="border-style: none;"><asp:TextBox ID='txt_ActualAwhDate' runat='server' Text='' style='width:55px' onblur='validateDate(this);'/></td>
                                        </tr>
                                        <tr>
                                            <td style="border-style: none;">Item Colour</td>
                                            <td style="border-style: none;" colspan='4'><asp:TextBox ID='txt_ItemColor' runat='server' Text='' style="width: 285px" /></td>
                                        </tr>
                                        <tr valign='top' style='display:block;'>
                                            <td style="border-style: none;">Item<br />Description</td>
                                            <td style="border-style: none;" colspan='4'>
                                                <asp:TextBox ID='txt_ItemDesc1' runat='server' Text='' style="width: 285px;"/><br/>
                                                <asp:TextBox ID='txt_ItemDesc2' runat='server' Text='' style="width: 285px;"/><br/>
                                                <asp:TextBox ID='txt_ItemDesc3' runat='server' Text='' style="width: 285px;"/><br/>
                                                <asp:TextBox ID='txt_ItemDesc4' runat='server' Text='' style="width: 285px;"/><br/>
                                                <asp:TextBox ID='txt_ItemDesc5' runat='server' Text='' style="width: 285px;"/><br/>
                                            </td>
                                        </tr>
                                        <tr id='tr_NoOfPackAndProdCode' >
                                            <td style='border-style:none;'><asp:panel id='pnl_labelNoOfPack' runat='server'>No. of Pack</asp:panel></td>
                                            <td style="border-style:none;" colspan='2'>
                                                <asp:Panel ID='pnl_textNoOfPack' style='width:120px;' runat='server'>
                                                    <div>
                                                        <asp:TextBox ID ="txt_PiecesPerUnit" runat="server" style='width:40px;' onblur='validateInt(this);' />
                                                        <span style="font-size: 14px; text-align: center; vertical-align: top;">/</span> 
                                                        <asp:DropDownList runat='server' id='ddl_PackingUnit' style='width:60px;height:17px;'>
                                                            <asp:ListItem Value='2' Text='Carton' Selected='True'></asp:ListItem>
                                                            <asp:ListItem Value='1' Text='Hanger'></asp:ListItem>
                                                        </asp:DropDownList> 
                                                    </div>
                                                </asp:Panel>
                                            </td>
                                            <td style="border-style: none;"><asp:panel ID='pnl_labelUkProdCode' runat='server'>&nbsp;UK Prod. Code</asp:panel></td>
                                            <td style="border-style: none;"><asp:TextBox ID ="txt_UKProductGroupCode" runat="server" style='width:55px;'/></td>
                                        </tr>
                                        <tr>
                                            <td style='border-style:none;' colspan='1'><asp:panel ID='pnl_CourierCharge' runat='server' >Courier Charge To NUK</asp:panel></td>
                                            <td style='border-style:none;' colspan='2'><asp:TextBox ID='txt_CourierChargeToNUK' runat='server' onblur='validateDecimal(this);' style="width:112px"/></td>
                                            <td style='border-style:none;' colspan='1'><asp:panel ID='pnl_NSComm' runat='server' >&nbsp;NS Commission</asp:panel></td>
                                            <td style='border-style:none;'><asp:TextBox ID='txt_NSComm' runat='server' style="width:55px;" value=''/></td>
                                        </tr>
                                        <tr>
                                            <td style="border-style:none;"><asp:panel ID='pnl_Destination' runat='server'>Destination</asp:panel></td>
                                            <td style="border-style:none;" colspan='4'>
                                                <cc3:SmartDropDownList ID="ddl_FinalDestination" runat="server"  style='display:block;width:290px;height:17px;'  />
                                                <asp:TextBox ID='txt_FinalDestination' Text='' runat='server' style='display:none;width:135px;vertical-align:bottom;' ></asp:TextBox>
                                                <input type='text' id='txt_test'  style='display:none;' disabled='disabled'  />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dly No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_DeliveryNo" runat="server" Text='' style="text-align:center;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ItemNo" runat="server" Text='' style="text-align:left;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField headertext="Supplier" >
                            <ItemTemplate>
                                <asp:Label ID="lbl_Vendor" runat='server' Width='200' style='text-align:left;'/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ccy." >
                            <ItemTemplate>
                                <asp:Label ID="lbl_Currency" runat="server" Text='' style="text-align:center;"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Colour" >
                            <ItemTemplate>
                                <asp:Label ID="lbl_Color" runat="server" Text='' style="text-align:center;"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Option No.">
                            <ItemTemplate >
                                <asp:Label ID="lbl_Option" runat="server" Text='' style="text-align:center;"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Size" >
                            <ItemTemplate>
                                <asp:Label ID="lbl_Size" runat="server" Text='' style="text-align:center;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<input type='button' id='btn_CopyOrderQty' value='Copy' class='style_button40' onclick='CopyOrderQtyToShippedQty();' /><br/> Order Qty."  ItemStyle-HorizontalAlign='Right'>
                            <ItemTemplate>
                                <asp:Label ID="lbl_OrderQty" runat="server" Text='' Value='' style="text-align:right;"  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<input type='button' id='btn_CopyPOQty' value='Copy' class='style_button40' onclick='CopyPOQtyToShippedQty();'/><br/> PO.&nbsp; Qty."  ItemStyle-HorizontalAlign='Right'>
                            <ItemTemplate>
                                <asp:Label ID="lbl_POQty" runat="server" Text='' Value='' style="text-align:right;"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Shipped Qty."  ItemStyle-HorizontalAlign='Right'>
                            <ItemTemplate>
                                <asp:Label ID='lbl_ShippedQty' runat="server" Text='' Value='' style='width:40px;text-align:right;color:Blue;' />
                                <asp:TextBox ID='txt_ShippedQty' runat="server" Text=''  style='width:40px;text-align:right;color:Blue;'  
                                     OnBlur='return validateInt(this);' OnChange='return calcTotalAmt(this);'
                                     SubTotalRow='' RowIdx='' SequenceNo='' Locked='' Invoiced=''
                                     OrderQty='' POQty='' InitialShippedQty='' InitialTotalShippedQty=''
                                     SellingPrice='' SupplierPrice='' SellingAmt='' SupplierAmt='' 
                                />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Selling Price"  ItemStyle-HorizontalAlign='Right'>
                            <ItemTemplate>
                                <asp:Label ID="lbl_SellingPrice" runat="server" Text='' Value='' style="text-align:right;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Selling Amount"  ItemStyle-HorizontalAlign='Right'>
                            <ItemTemplate>
                                <asp:Label ID="lbl_SellingAmt" runat="server" Text=''  style="text-align:right;" SequenceNo="" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Price"  ItemStyle-HorizontalAlign='Right'>
                            <ItemTemplate>
                                <asp:Label ID="lbl_SupplierPrice" runat="server" Text='' Value='' style="text-align:right;color:Green;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Amount"  ItemStyle-HorizontalAlign='Right'>
                            <ItemTemplate>
                                <asp:Label ID="lbl_SupplierAmt" runat="server" Text=''  style="text-align:right;color:Green;" SequenceNo=""/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <span style="font-family: Arial; font-size: medium; font-weight: bold; background-color: #FFFFCC;">No record found.</span>
                    </EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID='lbl_ShipmentNotFound' runat='server' style='font-family: Arial; font-size: medium; font-weight: bold; background-color: #FFFFCC;' Visible='false'>Shipment not found.</asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>    


<asp:Panel id='pnl_AccessDenied' runat='server' Visible='false'>
    <table style='width:98%;font-size:medium;font-weight:bold;'>
        <tr>
            <td width='100px'></td>
            <td>
                You are not authorized to access this page at this moment, <br /><br />
            </td>
            <td width='100px'></td>
        </tr>
        <tr>
            <td width='100px'></td>
            <td>
                Please contact Help Desk.<br /><br />
            </td>
            <td width='100px'></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td></td>
            <td align='center'>
                <asp:Button ID='btn_AccessDeniedClose' runat='server' Text='Close' style='width:80px;' OnClientClick='window.close();return false;' />
            </td>
            <td></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
    </table>
</asp:Panel>
    
    </form>
    
</body>
</html>