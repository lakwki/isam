<%@ Page Language="C#" AutoEventWireup="true" Theme="DefaultTheme" CodeBehind="LCBatchUpdate.aspx.cs" Inherits="com.next.isam.webapp.shipping.LCBatchUpdate" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>ISAM - LC Batch Update</title>
    <script src="../common/common.js" type="text/javascript" ></script>   
    <script type="text/javascript">

        function CopyTextToBlank(obj, textboxName, dataType, action) {
            var i; 
            var nodeList = document.getElementsByTagName("input");
            var validMsg = "";

            if (dataType == "N" )
            {
                if ((isNaN(parseFloat(obj.value)) && obj.value != "") || parseFloat(obj.value)<0)
                    validMsg = "Invalid number!";
                else 
                {
                    if (obj.value != "")
                        obj.value = parseFloat(obj.value);
                }
            }

            if (dataType == "D") 
            {
                validMsg = "";
                if (obj.value != "" && !isDateValid(obj.value))
                    validMsg = "Invalid Date Format (DD/MM/YYYY)";
            }

            if (validMsg == "") 
            {
                if (document.all.hid_ValidMsg.value != "" && action == "blur") return false;
                else {
                    resetValidMsg();
                    for (i = 0; i < nodeList.length; i++) {
                        if (nodeList[i].type == "text")
                            if (nodeList[i].name.indexOf(textboxName) != -1)
                                if (!nodeList[i].disabled) 
                                {
                                    if (action == "blur" && nodeList[i].updateStatus == "blur")
                                        nodeList.updateStatus = "";
                                    if ((nodeList[i].value == null || nodeList[i].value == "") || (dataType == "N" && parseFloat(nodeList[i].value) == 0 && obj.value > 0)  //) || nodeList[i].updateStatus=="ToBeConfirm")
                                    || (nodeList[i].updateStatus == "blur" && action == "change")) {
                                        nodeList[i].value = obj.value;
                                        nodeList[i].updateStatus = (action == "change" ? "" : action);
                                    }
                                }
                    }
                }
            }

             //alert(obj.name + " | " + action + " | " + obj.updateStatus + " | " + validMsg);
            if (validMsg != "") {
                if ((action == "change" && obj.updateStatus != "blur") || (action == "blur" && obj.updateStatus == "")) {
                    obj.updateStatus = "VALIDATED";
                    alert(validMsg);
                    document.all.hid_ValidMsg.value = validMsg;
                }
                obj.select();
                return false;
            }
            else {
                obj.updateStatus = "";
                return true;
            }
        }

        function resetValidMsg() {
            if (document.all.hid_ValidMsg.value != "") {
                document.all.hid_ValidMsg.value = ""
                return false;
            }
            else
                return true;
        }

        function clearColumnStatus() {
            var i;
            var textboxName = "";
            var obj = event.srcElement || event.target;
            var inputList = document.getElementsByTagName("input");
            for (i = 1; i < obj.id.length - 1 && i < obj.name.length - 1; i++)
                if (obj.id.charAt(obj.id.length - i) != obj.name.charAt(obj.name.length - i)) break;
                else textboxName = obj.id.charAt(obj.id.length - i) + textboxName;
            for (i = 0; i < inputList.length; i++)
                if (inputList[i].type == "text" && inputList[i].name.indexOf(textboxName) != -1 && !inputList[i].disabled && inputList[i].id != obj.id)
                    inputList[i].updateStatus = "";
        }

        function CopyToBlank(dataType) {
            var i;
            var textboxName = "";
            var obj = event.srcElement || event.target;
            var action = event.type;
            for (i = 1; i < obj.id.length - 1 && i < obj.name.length - 1; i++)
                if (obj.id.charAt(obj.id.length - i) != obj.name.charAt(obj.name.length - i)) break;
                else textboxName = obj.id.charAt(obj.id.length - i) + textboxName;
            return CopyTextToBlank(obj, textboxName, dataType, action);
        }


        function ClearRow(obj) {
            var nodeList = document.getElementsByTagName("input");
            for (i = 0; i < nodeList.length; i++) {
                if (nodeList[i].type == "text")
                    if (nodeList[i].id.indexOf(obj.id.replace(/btn_Clear/, "")) != -1) {
                        nodeList[i].value = "";
                        //nodeList[i].updateStatus = "ToBeConfirm";
                    }
            }

        }

        function changeRowStatus() {
            var target = event.target || event.srcElement;
            if (target.type == "checkbox")
                setRowStatus(target, target.checked);
        }

        function setRowStatus(ckb, active) {
            var i, lcCancelled, node;
            var gridRow = ckb.offsetParent.parentElement;
            var prefix = ckb.id.replace(/ckb_Selected/, "");
            node = gridRow.getElementsByTagName("SPAN");
            for (i = node.length - 1, lcCancelled = false; i >= 0 && !lcCancelled; i--)
                lcCancelled = (node[i].innerText.toUpperCase() == "LC CANCELLED");
            node = gridRow.getElementsByTagName("INPUT");
            for (i = 0; i < node.length; i++)
                if (node[i].id.indexOf(prefix) != -1 && node[i].type == "text" && node[i].title == "")
                    node[i].disabled = !active;
            ckb.checked = active;
        }

        function setAllCheckBox(action) {
            var checked = (action == "Checked");
            var node = document.all.gv_ShipmentLC.getElementsByTagName("INPUT");
            for (i = 0; i < node.length; i++)
                if (node[i].type == "checkbox")
                    setRowStatus(node[i], checked);
        }

        function validateCancellation() {
            var i, j, ckb, alert = "";
            var input = document.all.gv_ShipmentLC.getElementsByTagName("INPUT");
            for (i = 0; i < input.length; i++)
                if (input[i].type == "checkbox") {
                    if ((ckb = input[i]).checked) {
                        var contract = "", lcNo = "";
                        var gridRow = ckb.offsetParent.parentElement;
                        var label = gridRow.getElementsByTagName("SPAN");
                        var txt = gridRow.getElementsByTagName("INPUT");
                        for (j = 0; j < label.length; j++)
                            if (label[j].id.indexOf("lbl_ContractNo") != -1)
                                contract = label[j].innerText + contract
                            else if (label[j].id.indexOf("lbl_DlyNo") != -1)
                                contract += "-" + label[j].innerText;
                            for (j = 0; j < txt.length && lcNo == ""; j++)
                                if (txt[j].id.indexOf("txt_LCNo") != -1)
                                    lcNo = txt[j].value;
                        if (lcNo == "")
                            alert += String.fromCharCode(13) + "L/C Application for contract " + contract + " cannot be cancelled (No L/C number)";
                    }
                }
                if (alert != "")
                    lbl_Alert.innerText = alert + String.fromCharCode(13);
            return (alert == "")
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div>
    <table width="600" >
        <tr>
            <asp:TextBox ID="txt_LCBatchId" style="display:none;" runat="server" Text=""/>
            <asp:TextBox ID="txt_UserId" style="display:none;" runat="server" Text=""/>
            <td class="FieldLabel3">L/C Batch No.</td>
            <td><asp:TextBox ID="txt_LCBatchNo" CssClass="nonEditableTextbox" runat="server" ReadOnly="true"  Text="" /></td>
            <td class="FieldLabel3">Application</td>
            <td colspan="3">
                <asp:TextBox ID="txt_AppliedBy" CssClass="nonEditableTextbox" runat="server" ReadOnly="true"  Text="" />&nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="txt_AppDate" CssClass="nonEditableTextbox" SkinID="DateTextBox" runat="server" ReadOnly="true"  Text=""/>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel3">Office</td>
            <td><asp:TextBox ID="txt_Office" CssClass="nonEditableTextbox" runat="server" ReadOnly="true"  Text="" /></td>
            <td class="FieldLabel3">Currency</td>
            <td><asp:TextBox ID="txt_Currency" CssClass="nonEditableTextbox" SkinID="SmallTextBox" runat="server" ReadOnly="true" Text="" /></td>
            <td class="FieldLabel3">P.O. Amount</td>
            <td><asp:TextBox ID="txt_POAmt" CssClass="nonEditableTextbox" runat="server" ReadOnly="true"  Text="" /></td>
        </tr>
        <tr>
            <td class="FieldLabel3">Supplier</td>
            <td colspan="3"><asp:TextBox ID="txt_Supplier" CssClass="nonEditableTextbox" SkinID="TextBox300" runat="server" ReadOnly="true" Text="" /></td>
            <td class="FieldLabel3">Status</td>
            <td><asp:TextBox ID="txt_Status" CssClass="nonEditableTextbox" runat="server" ReadOnly="true"  Text="" /></td>            
        </tr>
    </table>
    <br />
    <asp:LinkButton ID="btn_SelectAll" runat="server" Text="Select All" OnClientClick="setAllCheckBox('Checked');return false;" />&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="btn_DeselectAll" runat="server" Text="Deselect All"  OnClientClick="setAllCheckBox('Unchecked');return false;" />

    <asp:GridView ID ="gv_ShipmentLC" runat="server" AutoGenerateColumns="false" onrowdatabound="gv_ShipmentLC_RowDataBound"  OnSorting="gv_ShipmentLC_OnSort" AllowSorting="True"  >
        <Columns>
            <asp:TemplateField >
                <ItemTemplate>
                    <asp:CheckBox ID="ckb_Selected" runat="server" Checked="true" onclick="changeRowStatus();" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Contract No." SortExpression="ContractNo" HeaderStyle-ForeColor="CornflowerBlue">
                <ItemTemplate >
                    <asp:Label ID="lbl_ContractNo" runat="server" Text='' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Dly No.">
                <ItemTemplate >
                    <asp:Label ID="lbl_DlyNo" runat="server" Text='' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Item No." SortExpression="ItemNo" HeaderStyle-ForeColor="CornflowerBlue">
                <ItemTemplate >
                    <asp:Label ID="lbl_ItemNo" runat="server" Text='' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Purchase Term">
                <ItemTemplate >
                    <asp:Label ID="lbl_PurchaseTerm" runat="server" Text='' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Purchase Location">
                <ItemTemplate >
                    <asp:Label ID="lbl_PurchaseLocation" runat="server" Text='' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Ccy">
                <ItemTemplate >
                    <asp:Label ID="lbl_Currency" runat="server" Text='' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="PO Qty">
                <ItemTemplate >
                    <asp:Label ID="lbl_POQty" runat="server" Text='' style="text-align:right;"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="PO Amt">
                <ItemTemplate>
                    <asp:Label ID="lbl_POAmt" runat="server" Text='' style="text-align:right;"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="L/C No.">
                <ItemTemplate >
                    <asp:TextBox ID="txt_LCNo" runat="server" SkinID="DateTextBox" onChange="return CopyToBlank('T');" onBlur="return CopyToBlank('T');" updateStatus=''/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Issued Date">
                <ItemTemplate >
                    <asp:TextBox ID="txt_IssuedDate" runat="server" SkinID="DateTextBox" onChange="return CopyToBlank('D');" onBlur="return CopyToBlank('D');" updateStatus=''/> 
                    <cc1:CalendarExtender ID="ce_IssueDate"  FirstDayOfWeek="Sunday" Format="dd/MM/yyyy"
                        TargetControlID="txt_IssuedDate" runat="server"  />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText ="Expiry Date">
                <ItemTemplate>
                    <asp:TextBox id="txt_ExpiryDate" runat="server" SkinID="DateTextBox" onChange="return CopyToBlank('D');" onBlur="return CopyToBlank('D');" updateStatus=''/>
                    <cc1:CalendarExtender ID="ce_ExpiryDate"  FirstDayOfWeek="Sunday"  Format="dd/MM/yyyy" 
                        TargetControlID="txt_ExpiryDate" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="L/C Amount">
                <ItemTemplate >
                    <asp:TextBox ID="txt_LCAmount" SkinID="DateTextBox" runat="server" onChange="return CopyToBlank('N');" onBlur="return CopyToBlank('N');" updateStatus=''/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField >
                <ItemTemplate >
                    <asp:ImageButton ID="btn_Clear" runat="server" OnClientClick="ClearRow(this); return false;" ImageUrl="~/images/icon_clear.gif" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Contract Status">
                <ItemTemplate >
                    <asp:Label ID="lbl_ContractStatus" runat="server" Text='' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="L/C Application Status">
                <ItemTemplate >
                    <asp:Label ID="lbl_LCStatus" runat="server" Text='' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText ="L/C Cancellation Date">
                <ItemTemplate>
                    <asp:TextBox id="txt_LcCancelDate" runat="server" SkinID="DateTextBox" onChange="return CopyToBlank('D');" onBlur="return CopyToBlank('D');"  onselect="clearColumnStatus();" onclick="clearColumnStatus();" updateStatus=''/>
                    <cc1:CalendarExtender ID="ce_LcCancelDate"  FirstDayOfWeek="Sunday"  Format="dd/MM/yyyy"   PopupPosition="BottomRight" TargetControlID="txt_LcCancelDate" runat="server"  />
                </ItemTemplate>
             </asp:TemplateField>
        </Columns>        
    </asp:GridView>   
    <asp:label ID="lbl_Alert" runat="server" text=""  style="font-size:medium; color:Red;"/>
    <input type="hidden" id="hid_ValidMsg" value="" />
    <br />
    <asp:Button ID="btn_Update" runat="server" Text="Update" onclick="btn_Update_Click" />&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" onclick="btn_Cancel_Click" />&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btn_Close" runat="server" Text="Close" OnClientClick="window.close();return false;" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btn_CancelLC" runat="server" Text="Cancel L/C" onclick="btn_CancelLC_Click"
         OnClientClick="return validateCancellation();" 
      SkinID="MButton" />
    </div>
   
</form>
</body>

</html>
