<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="ShippingDocumentControl.aspx.cs" Inherits="com.next.isam.webapp.shipping.ShippingDocumentControl" Title="Shipping Document Control"%>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<%@ Register Src="~/usercontrol/UclSortingOrder.ascx" TagName="UclSortingOrder" TagPrefix="uso1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">   <!-- cph_ImageHeader -->
    <img src="" runat="server" id="imgHeaderText" alt="workplace"  visible='false'  />

    <script type="text/javascript" >

        function isDate(DMY_Date) {
            if (DMY_Date == "")
                return true;
            else
                return isDateValid(DMY_Date);
        }

        function inputValidation(oCaller, sName) {
            var sActualAtWHDateFrom;
            var sActualAtWHDateTo;
            var sILSActualAtWHDateFrom;
            var sILSActualAtWHDateTo;
            var sContractNo;
            var sItemNo;
            var sLCNoFrom;
            var sLCNoTo;

            var prefix;

            prefix = oCaller.id.replace(sName, "");

            sActualAtWHDateFrom = (document.getElementById(prefix + "txt_ActualAtWHDateFrom_txt_ActualAtWHDateFrom_textbox").value);
            sActualAtWHDateTo = (document.getElementById(prefix + "txt_ActualAtWHDateTo_txt_ActualAtWHDateTo_textbox").value);

            sILSActualAtWHDateFrom = (document.getElementById(prefix + "txt_ILSActualAtWHDateFrom_txt_ILSActualAtWHDateFrom_textbox").value)
            sILSActualAtWHDateTo = (document.getElementById(prefix + "txt_ILSActualAtWHDateTo_txt_ILSActualAtWHDateTo_textbox").value)

            sContractNo = document.getElementById(prefix + "txt_ContractNo").value;
            sItemNo = document.getElementById(prefix + "txt_ItemNo").value;

            sLCNoFrom = document.getElementById(prefix + "txt_LCNoFrom").value;
            sLCNoTo = document.getElementById(prefix + "txt_LCNoTo").value;

            if (sContractNo=="" && sItemNo=="" && sLCNoFrom == "" && sLCNoTo == "" && (sActualAtWHDateFrom == "" && sActualAtWHDateTo == "") && (sILSActualAtWHDateFrom == "" && sILSActualAtWHDateTo == "")) {
                alert("Please input any of the followings:\n\r - Contract No.;\n\r - Item No.;\n\r - L/C No.;\n\r - ILS Stock to Warehouse Date;\n\r - Stock to Warehouse Date");
                return false;
            }
            if (!isDate(sActualAtWHDateFrom) || !isDate(sActualAtWHDateTo) || !isDate(sILSActualAtWHDateFrom) || !isDate(sILSActualAtWHDateTo)) {
                alert("Invalid Date Format.");
                return false;
            }
            refreshPrintButton(oCaller, sName);
            
            return true;
        }


        function ClearSearchingCriteria(oCaller, sName) {
            var prefix;

            prefix = oCaller.id.replace(sName, "");
            document.getElementById(prefix + "pnl_SearchResult").style.display = "none";
            //txt_ContractNo
            //txt_ItemNo
            //txt_LCNoFrom
            //txt_ILSActualAtWHDateFrom_txt_ILSActualAtWHDateFrom_textbox
            //txt_ILSActualAtWHDateTo_txt_ILSActualAtWHDateTo_textbox
            //txt_ActualAtWHDateFrom_txt_ActualAtWHDateFrom_textbox
            //txt_ActualAtWHDateTo_txt_ActualAtWHDateTo_textbox
            
        }

        function refreshPrintButton(oCaller, sName) {
            var oLcNoFrom, oLcNoTo, oVendorName, oPayCheckDate;
            var btn;

            return true;
             //alert(oCaller.id);
            //alert(prefix);
            prefix = oCaller.id.replace(sName, "");
            btn = document.getElementById(prefix + "btn_Print");
            if (btn != null) {
                oLcNoFrom = document.getElementById(prefix + "txt_LCNoFrom");
                oLcNoTo = document.getElementById(prefix + "txt_LCNoTo");
                oVendorName = document.getElementById(prefix + "txt_VendorName_txtName");
                oPayCheckDate = document.getElementById(prefix + "txt_LcPaymentCheckDate_txt_LcPaymentCheckDate_textbox");
                if (oLcNoFrom.value == "" || oLcNoTo.value == "" || oVendorName.value == "" || oPayCheckDate.value == "") {
                    btn.disabled = true;
                    btn.title = "To print L/C Status Report,\nPlease specify the 'L/C No.', 'Vendor' and 'L/C Payment Check Date'";
                    btn.style.display = "none";
                }
                else {
                    btn.disabled = false;
                    btn.title = "Print L/C Status Report";
                    if (sName == "btn_Update")
                        btn.style.display = "block";
                }
                if (sName == "btn_Search")
                    btn.style.display = "none";
            }   
            return true;
        }

        function btnUpdateValidChecking() {
            var anyChecked = false;
            var valid = true;
            var billRef = null;
            var lcNo = null;

            inputElement = document.getElementsByTagName("input");
            for (i = 0; i < inputElement.length; i++) {
                if (billRef == null)
                    if (inputElement[i].id.indexOf("txt_LCBillRefNoSequence") > 0)
                        billRef = (inputElement[i].value.trim() == "" ? "" : inputElement[i].value.trim());
                if (lcNo == null)
                    if (inputElement[i].id.indexOf("txt_LCNoFrom") > 0 || inputElement[i].id.indexOf("txt_LCNoTo") > 0)
                        lcNo = (inputElement[i].value.trim() == "" ? "" : inputElement[i].value.trim());

                if (!anyChecked)
                    if (inputElement[i].id.indexOf("ckb_Selected") > 0)
                        anyChecked = inputElement[i].checked;

                if (billRef != null && lcNo != null && anyChecked)
                    break;
            }
            if (anyChecked && billRef == "" && lcNo != "") {
                alert("L/C Bill Reference No. is not inputted.\n\n   You cannot update the shipment!");
                valid = false;
            }

            return valid;
        }

        function SubmitApplication(obj) {
            obj.disabled = true;
            document.all.ctl00_ContentPlaceHolder1_btn_Apply.click();
            return true;
        }

        function WhoAmI(obj) {
            alert(obj.id);
            return true;
        }

        function ckb_Selected_Click(obj) {
            alert(obj.id);
            alert(obj.checked);
            return true;
        }

        function CheckAllRow(sName) {
            var inputElement;

            CheckAll(sName);
            inputElement = document.getElementsByTagName("input");
            for (i = 0; i < inputElement.length; i++) {
                if (inputElement[i].id.indexOf(sName) > 0) {
                    if (inputElement[i].enabled == true)
                        highlightEntry(inputElement[i]);
                }
            }
        }

        function UncheckAllRow(sName) {
            var inputElement;

            UncheckAll(sName);
            inputElement = document.getElementsByTagName("input");
            for (i = 0; i < inputElement.length; i++) {
                if (inputElement[i].id.indexOf(sName)>0)
                    highlightEntry(inputElement[i]);
            }
        }
        
    </script>
<asp:Label ID='lbl_view' runat='server' Text='VIEW' Visible='false'></asp:Label>
<asp:Label ID='lbl_Update' runat='server' Text='UPDATE' Visible='false'></asp:Label>
<asp:Label ID='lbl_Print' runat='server' Text='PRINT' Visible='false'></asp:Label>

<asp:Panel ID='pnl_PageBody' runat='server'>
<!--
<DIV class='tableHeader' style='width:800px;'>&nbsp;&nbsp;Payment Control&nbsp;&nbsp;</DIV>
-->
<asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Shipping">Payment Control</asp:Panel>


<!--
##############################
##   Searching Criteria     ##
##############################
-->
    <table width="850px" cellspacing="0" cellpadding="2" border='0' >
        <col width="160"/><col width="120"/><col width="120"/><col width="50"/><col width="240"/><col />
        <tr style='display:block;'>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="FieldLabel2">Contract No.</td>
            <td><asp:TextBox ID="txt_ContractNo" runat="server" style='width:100px;'/></td>
            <td class="FieldLabel2" align='right'>Item No.&nbsp;&nbsp;</td>
            <td><asp:TextBox ID="txt_ItemNo" runat="server" style='width:70px;'/></td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="FieldLabel2" >L/C No.</td>
            <td colspan="3">
                <asp:TextBox ID="txt_LCNoFrom" runat="server" style='width:100px;' onblur='refreshPrintButton(this, "txt_LCNoFrom")' />&nbsp;&nbsp;to&nbsp;&nbsp;
		    	<asp:TextBox ID="txt_LCNoTo" runat="server" style='width:100px;' onblur='refreshPrintButton(this, "txt_LCNoTo")' />
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="FieldLabel2"><span>ILS Stock to Whse Date</span></td>
            <td colspan="3">
                <cc2:smartcalendar id="txt_ILSActualAtWHDateFrom" runat="server" 
                    ToDateControl="txt_ILSActualAtWHDateTo" RequiredFieldEnabled="False" 
                    RequiredFieldText="Please Input ILS Stock to Warehouse Date here" />
                &nbsp;&nbsp;&nbsp;&nbsp;to&nbsp;&nbsp;
			    <cc2:smartcalendar id="txt_ILSActualAtWHDateTo" runat="server"  
                    FromDateControl="txt_ILSActualAtWHDateFrom" RequiredFieldEnabled="False" 
                    RequiredFieldText="Please Input ILS stock to Warehouse Date here" />
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="FieldLabel2"><span>Stock to Whse Date</span></td>
            <td colspan="3">
                <cc2:smartcalendar id="txt_ActualAtWHDateFrom" runat="server" 
                    ToDateControl="txt_ActualAtWHDateTo" RequiredFieldEnabled="False" 
                    RequiredFieldText="Please Input Stock to Warehouse Date here" />
                &nbsp;&nbsp;&nbsp;&nbsp;to&nbsp;&nbsp;
			    <cc2:smartcalendar id="txt_ActualAtWHDateTo" runat="server"  
                    FromDateControl="txt_ActualAtWHDateFrom" RequiredFieldEnabled="False" 
                    RequiredFieldText="Please Input Stock to Warehouse Date here" />
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="FieldLabel2">Supplier</td>
            <td colspan="4">
                <uc1:UclSmartSelection  ID="txt_VendorName" runat="server"  onblur='refreshPrintButton(this, "txt_VendorName")'/>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">Office</td>
            <td colspan='2'>
                <cc2:SmartDropDownList ID='ddl_Office' runat='server' width="150" AutoPostBack="false" />
            </td>
            <td>&nbsp;</td>
            <td style='display:none;'>
                <cc2:SmartDropDownList ID='ddl_ProductTeam' runat='server' AutoPostBack="True" Visible='false'/>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">Shipping User</td>
            <td colspan='2'>
                <cc2:SmartDropDownList ID='ddl_ShippingUser' runat='server' width="150" Autopostback='false'/>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="FieldLabel2_T" valign="top">Customer</td>
            <td colspan='3'>
              <div class="CellWithBorder" style='width:550px;'>
                <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="100%" RepeatLayout="Table" RepeatColumns="4">
                </asp:CheckBoxList>
              </div>  
            </td>    
            <td align="left" valign="bottom">
                <asp:ImageButton ID="btn_Clear1" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_Customer'); return false;" />&nbsp;
                <asp:ImageButton ID="btn_All1" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_Customer'); return false;" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2_T" valign="top">Status</td>
            <td colspan="4">
              <div class='CellWithBorder' style='width:550px;'>
                <table>
                    <tr>
                        <td>Shipping Document Receipt Date : &nbsp;&nbsp;</td>
                        <td>&nbsp;&nbsp;</td>
                        <td><asp:CheckBox runat="server" id="cbx_DocReceiptDateNotUpdated"  Checked="true" /> Not Updated&nbsp;&nbsp;</td>
                        <td>&nbsp;&nbsp;</td>
                        <td><asp:CheckBox runat="server" id="cbx_DocReceiptDateUpdated" /> Updated&nbsp;&nbsp;</td>
                    </tr>
                    <tr id='tr_LCPaymentChecked' runat='server'>
                        <td>L/C Payment Checked : &nbsp;&nbsp;</td>
                        <td>&nbsp;&nbsp;</td>
                        <td><asp:CheckBox runat="server" id="cbx_LCPaymentCheckedNotUpdated" checked="true"/> Not Updated&nbsp;&nbsp;</td>
                        <td>&nbsp;&nbsp;</td>
                        <td><asp:CheckBox runat="server" id="cbx_LCPaymentCheckedUpdated"/> Updated&nbsp;&nbsp;</td>
                    </tr>
                </table>
              </div>  
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2_T" valign="top">Sort By</td>
            <td class="CellWithBorder" colspan='2'>
                <uso1:UclSortingOrder ID="ucl_SortingOrder" runat="server" />       
            </td>
        </tr>

        <tr><td colspan="5">&nbsp;</td></tr>
        <tr>
            <td>&nbsp;</td>
            <td colspan="4">
                <asp:Button ID="btn_Search" runat="server" Text="Search" toolTip='Search for record' onclick="btn_Search_Click" style='width:80px;' 
                    OnClientClick='return inputValidation(this,"btn_Search");' />
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_Reset" runat="server" Text="Reset"  ToolTip='Reset searching criteria' onclick="btn_Reset_Click" />
                 &nbsp;&nbsp;&nbsp;&nbsp;
            </td>
        </tr>
    </table>

<!--
######################
##  Update panel    ##
######################
-->
<asp:Panel ID='pnl_Update' runat='server' Visible='false'>
<div class="CellWithBorder" style='width:800px;'>
  <table width='100%' cellspacing='1' cellpadding='1'>
    <tr style="display:none;">
        <td></td>
        <td style='width:150px'></td>
        <td style='width:100px'></td>
        <td></td>
        <td style='width:150px'></td>
        <td style='width:100px'></td>
        <td></td>
        <td></td>
    </tr>
    <tr style='border-style:inset;' valign='top' >
        <td></td>
        <td colspan="5">
            <table>
                <tr>
                    <td><div style="width:120px;">Document Receipt Date<br />and Amount Checked</div></td>
                    <td style="width:90px;" ><cc2:SmartCalendar ID="txt_ShippingDocReceiptDate" runat="server" /></td>
                    <td></td>
                    <td id='td_PayDocCheckDateTitle' runat='server'>
                        <div style="width:145px;">L/C Payment Check Date and<br />Shipping Document Checked</div>
                    </td>
                    <td id='td_PayDocCheckDateInput' runat='server' style="width:90px;">
                        <cc2:SmartCalendar ID="txt_LcPaymentCheckDate" runat="server" onblur='refreshPrintButton(this,"txt_LcPaymentCheckDate")' SkinID="DateTextBox" />
                    </td>
                    <td></td>
                    <td id='td_LcBillRefNo' runat='server'>
                        <asp:Table ID='tbl_LcBillRefNo' runat='server' CellSpacing='0' CellPadding='0'>
                            <asp:TableRow>
                                <asp:TableCell>L/C Bill Ref. No.&nbsp;</asp:TableCell>
                                <asp:TableCell><asp:TextBox ID='txt_LCBillRefNoPrefix' runat='server'/></asp:TableCell>
                                <asp:TableCell><asp:TextBox ID='txt_LCBillRefNoSequence' runat='server'/></asp:TableCell>
                                <asp:TableCell><asp:TextBox ID='txt_LCBillRefNoSuffix' runat='server'/></asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Button ID="btn_Update" runat="server" Text="Update" SkinID="SButton" OnClick='btn_Update_Click'  OnClientClick='return btnUpdateValidChecking();' toolTip='Update the [Document Receipt Date]/[L/C Payment Checked Date] for the selected shipment' style='display:block;'/>&nbsp;&nbsp;    
                        <asp:Button ID="btn_Export" runat="server" Text="Export L/C Status" onclick="btn_Export_Click" title='To export L/C Status Report, please specify the L/C No.' SkinID="LButton" style='width:130px;display:none;'/>
                        <asp:Button ID="btn_Print"  runat="server" Text="Print L/C Status" SkinID="MButton"  onclick="btn_Print_Click" title='To print L/C Status Report, please specify the L/C No.' style='display:none;'/>&nbsp;&nbsp;
                    </td>
                </tr>
            </table>
        </td>
    </tr>
  </table>
</div>

</asp:Panel>
<!--
##########################
##  Searching Result    ##
##########################
-->
    <asp:Panel ID="pnl_SearchResult" runat="server" Visible="false">
        <table width='600px' >
            <tr>
                <td style='width:60px;'></td>
                <td style='width:5px;'></td>
                <td style='width:70px;'></td>
                <td style='width:200px;'></td>
                <td style='width:220px;'></td>
            </tr>
            <tr>
                <td colspan='5'>
                    <asp:Label ID="lbl_RowCount" runat="server" style="color:#ff9900; font-weight :bolder;" text="" />
                </td>
            </tr>
            <tr>
                <td><asp:LinkButton ID="btn_SelectAll" runat="server" Text="Select All" OnClientClick="CheckAllRow('ckb_Selected');return false;"  /></td>
                <td></td>
                <td><asp:LinkButton ID="btn_DeselectAll" runat="server" Text="Deselect All" OnClientClick="UncheckAllRow('ckb_Selected');return false;" visible="true"/></td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>
      <asp:UpdatePanel runat="server" >
         <ContentTemplate>
            <asp:GridView ID="gv_Shipment" runat="server" AutoGenerateColumns="false"  OnRowDataBound='gv_Shipment_RowDataBound' RowStyle-Font-Bold='false'>
                <Columns>
                    <asp:TemplateField ItemStyle-VerticalAlign="Middle">
                        <ItemTemplate>
                            <asp:CheckBox ID='ckb_Selected' runat='server' Enabled='true' onclick='highlightEntry(this)' Text=""/>
                            <asp:Panel ID='pnl_RowRemark' runat='server' style='display:none; vertical-align:middle; padding-top:8px;' ToolTip='You are not allow to update because the Shipping Document Status is []!'>
                            <b style="color:Red; font-size:larger;">!</b>
                            </asp:Panel>&nbsp;
                            <asp:HiddenField ID='hid_ShipmentId' runat='server'  Value=''  />
                            <asp:TextBox ID='txt_ShipDocReadyToCheck' runat='server'  Value='' style="display:none;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Contract No.">
                        <ItemTemplate>
                            <asp:LinkButton ID="btn_ContractNo" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="NSL Dly No.">
                        <ItemTemplate>
                            <asp:Label ID="lbl_DlyNo" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image ID="img_ReadDmsDoc" ImageUrl="~/images/icon_edit.gif" runat="server" style="cursor:hand;" ToolTip="Read DMS Doc."/>
                            <asp:textbox></asp:textbox>
                            <asp:table id="tab" runat="server">
                            </asp:table>
                            <table border="0" cellpadding="0" cellspacing="0">
                                <tr style="vertical-align:middle;">
                                    <td><asp:Image ID="img_GBTestRequire" ImageUrl="~/images/icon_Test.png" runat="server" ToolTip="GB Test is Required" visible="false"  /></td>
                                    <td><asp:Image ID="img_GBTestPass" ImageUrl="~/images/icon_Pass.png" runat="server" ToolTip="GB Test Pass" visible="false"/></td>
                                    <td><asp:Image ID="img_GBTestFailRelease" ImageUrl="~/images/icon_Fail.png" runat="server" ToolTip="GB Test Fail (Can Release Payment)" visible="false"/></td>
                                    <td><asp:Image ID="img_GBTestFailHold" ImageUrl="~/images/icon_Fail_Orange.png" runat="server" ToolTip="GB Test Fail (Hold)" visible="false"/></td>
                                    <td><asp:Image ID="img_GBTestFailNotRelease" ImageUrl="~/images/icon_Fail_Red.png" runat="server" ToolTip="GB Test Fail (Cannot Release Payment)" visible="false"/></td>
                                    <td><asp:Image ID="img_QccInspection" ImageUrl="~/images/icon_QccInspection.png" runat="server" ToolTip="QCC Inspection" visible="false"/></td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Supplier">
                        <ItemTemplate>
                            <asp:Label ID="lbl_Supplier" runat="server" Width='120px' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Item No.">
                        <ItemTemplate>
                            <asp:Label ID="lbl_ItemNo" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Final Dest.">
                        <ItemTemplate>
                            <asp:Label ID="lbl_Destination" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Customer">
                        <ItemTemplate>
                            <asp:Label ID="lbl_Customer" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Shipped Qty." ItemStyle-HorizontalAlign='Right' >
                        <ItemTemplate>
                            <asp:Label ID="lbl_TotalShippedQuantity" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Ccy" >
                        <ItemTemplate>
                            <asp:Label ID="lbl_Currency" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Supplier Invoice Net Amt." ItemStyle-HorizontalAlign='Right' >
                        <ItemTemplate>
                            <asp:Label ID="lbl_SupplierInvoiceNetAmount" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Supplier Invoice No." >
                        <ItemTemplate>
                            <asp:Label ID="lbl_SupplierInvoiceNo" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Invoice No." >
                        <ItemTemplate>
                            <asp:Label ID="lbl_InvoiceNo" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Invoice Date" >
                        <ItemTemplate>
                            <asp:Label ID="lbl_InvoiceDate" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="No. of Split" >
                        <ItemTemplate>
                            <asp:Label ID="lbl_NoOfSplit" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <asp:Label ID='lbl_ShipmentWorkflowStatus' runat="server"/>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField headerstyle-borderstyle='none' ItemStyle-BorderStyle='none' Visible='false'>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Shipping User" >
                        <ItemTemplate>
                            <asp:Label ID='lbl_ShippingUser' runat="server" Text="Yui"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <span style='font-family: Arial; font-size: medium; font-weight: bold; background-color: #FFFFCC;width:400px'>No record found.</span>
                </EmptyDataTemplate>
            </asp:GridView>

         </ContentTemplate>
      </asp:UpdatePanel>
      <br />
    </asp:Panel>
</asp:Panel>

<script type="text/javascript" >

    function setTextBoxWidth(objectId, width) {
        //debugger;
        var obj;
        input = document.getElementsByTagName("input");
        for (i = 0; i < input.length; i++)
            if ((input[i].id).indexOf(objectId) >= 0) {
                obj = input[i];
                break;
            }
        if (obj!=null)
            obj.style.width = width.toString() + "px";
    }
    setTextBoxWidth("txt_ShippingDocReceiptDate", 55);
    setTextBoxWidth("txt_LcPaymentCheckDate", 55);
//    setTextBoxWidth("txt_LCBillRefNo", 60);
    
</script>

</asp:Content>
