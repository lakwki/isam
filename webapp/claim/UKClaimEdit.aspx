<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="UKClaimEdit.aspx.cs" Inherits="com.next.isam.webapp.claim.UKClaimEdit" Title="Next Claim Update" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Next Claim - Edit</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" language="javascript">
        function chkHasUKDN_OnChange(a) {
            var ckbHasDN = document.getElementById("<%=chk_HasUKDN.ClientID %>");
            var updUKDN = document.getElementById("<%=upd_UKDebitNote.ClientID %>");
            var txtDNNo = document.getElementById("<%=txt_UKDNNo.ClientID %>");
            var txtDNDate = document.getElementById("<%=txt_UKDNDate.ClientID %>_txt_UKDNDate_textbox")
            var txtDNRcvDate = document.getElementById("<%=txt_UKDNReceivedDate.ClientID %>_txt_UKDNReceivedDate_textbox")
            var imgDNDate = getFirstTag("div_UKDNDate", "img");
            var imgDNRcvDate = getFirstTag("div_UKDNReceivedDate", "img");

            if (!ckbHasDN.disabled) {
                if (ckbHasDN.checked) {
                    txtDNNo.disabled = false;
                    txtDNDate.disabled = false;
                    txtDNRcvDate.disabled = false;
                    if (updUKDN != null)
                        updUKDN.disabled = false;
                }
                else {
                    txtDNNo.value = "";
                    txtDNDate.value = "";
                    txtDNRcvDate.value = "";
                    txtDNNo.disabled = true;
                    txtDNDate.disabled = true;
                    txtDNRcvDate.disabled = true;

                    if (updUKDN != null)
                        updUKDN.disabled = true;
                }
            }
            else {
                txtDNDate.disabled = txtDNDate.readOnly;
                txtDNRcvDate.disabled = txtDNRcvDate.readOnly;
            }
            if (imgDNDate != null) {
                imgDNDate.disabled = txtDNDate.disabled;
                imgDNDate.style.cursor = (imgDNDate.disabled ? "auto" : "hand");
            }
            if (imgDNRcvDate != null) {
                imgDNRcvDate.disabled = txtDNRcvDate.disabled;
                imgDNRcvDate.style.cursor = (imgDNRcvDate.disabled ? "auto" : "hand");
            }
        }

        function getRefundResult(result) {
            //result : the string includes ReceivedDate, Amount & Remark (the first character is the delimiter)
            //alert(result);
            oResult = document.getElementById("<%=hid_RefundResult.ClientID %>");
            oResult.value = (result == undefined ? "" : result);
        }

        function getFirstTag(parentName, tag) {
            var obj = document.getElementById(parentName);
            var returnObj = null;
            var element = obj.getElementsByTagName(tag);
            for (i = 0; i < element.length; i++) {
                returnObj = element[i];
                break;
            }
            return returnObj;
        }

        function selectRequest(oSelected) {
            //alert(oSelected);
            var gvReq = document.getElementById("<%=gv_Request.ClientID %>");
            var element = gvReq.getElementsByTagName("input");
            for (var i = 0; i < element.length; i++) {
                req = element[i];
                if (req.name.indexOf("ClaimRequestGroup"))
                    req.checked=false;
            }
            oSelected.checked = true;
        }
        /*
        function getCalendarIcon(objSmartCalendarTextBox) {
            var calendar = null;
            var name = objSmartCalendarTextBox.id;
            imgElement = document.getElementsByTagName("img");
            for (i = 0; i < imgElement.length; i++) {
                //if (imgElement[i].src.toLowerCase().indexOf("calendar") > 0) {
                    if (imgElement[i].onclick != null) {
                        if (imgElement[i].onclick.toString().indexOf(name) > 0) {
                            calendar = imgElement[i];
                            break;
                        }
                    }
                //}
            }
            return calendar;
        }

        
        function initSmartCalendar(objName) {
            var calendar = null;
            var txBox = null;
            imgElement = document.getElementsByTagName("img");
            for (i = 0; i < imgElement.length; i++) {
                if (imgElement[i].src.toLowerCase().indexOf("calendar") > 0) {
                    if (imgElement[i].onclick != null) {
                        if (imgElement[i].onclick.toString().indexOf(objName) > 0) {
                            calendar = imgElement[i];
                            inputElement = document.getElementsByTagName("input");
                            for (j = 0; j < inputElement.length; j++)
                                if (inputElement[j].id.toLowerCase().indexOf(objName) > 0) {
                                    txBox = inputElement[j];
                                    break;
                                }
                            if (txBox != null) {
                                calendar.name = txBox.name.replace(objName + "_textbox", objName + "_img");
                                calendar.id = txBox.id.replace(objName + "_textbox", objName + "_img");
                            }
                            break;
                        }
                    }
                }
            }
            return calendar;
        }
        */

        function showProgressBarX() {
            var left = window.screen.availWidth / 3 - 100;
            var top = window.screen.availHeight / 3 - 100

            waitDialog = window.showModelessDialog("../common/ProgressBarX.htm", window, "dialogHeight: 30px; dialogWidth: 250px; center:yes; edge: sunken; center: Yes; help: No; resizable: No; status: No; scroll: No; dialogLeft:" + left + "px; dialogTop:" + top + "px;");
        }

        function updateHandlingOffice() {
            if (parseInt(document.all.<%= ddl_Office.ClientID %>.value) != 17 
                && parseInt(document.all.<%= ddl_Office.ClientID %>.value) != 1
                && parseInt(document.all.<%= ddl_Office.ClientID %>.value) != 2
                && parseInt(document.all.<%= ddl_Office.ClientID %>.value) != 16)
            {
                document.all.<%= ddl_HandlingOffice.ClientID %>.value = document.all.<%= ddl_Office.ClientID %>.value;
            }
            else if (parseInt(document.all.<%= ddl_Office.ClientID %>.value) == 17)
            {
                if (parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != -1 
                    && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 17 
                    && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 1 
                    && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 2 
                    && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 16)
                    document.all.<%= ddl_HandlingOffice.ClientID %>.value = "-1";
            }
            else if (parseInt(document.all.<%= ddl_Office.ClientID %>.value) == 1)
            {
                if (parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != -1 
                    && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 17 
                    && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 1)
                    document.all.<%= ddl_HandlingOffice.ClientID %>.value = "-1";
            }
            else if (parseInt(document.all.<%= ddl_Office.ClientID %>.value) == 2)
            {
                if (parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != -1 
                    && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 17 
                    && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 2)
                    document.all.<%= ddl_HandlingOffice.ClientID %>.value = "-1";
            }
            else if (parseInt(document.all.<%= ddl_Office.ClientID %>.value) == 16)
            {
                if (parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != -1 
                    && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 17 
                    && parseInt(document.all.<%= ddl_HandlingOffice.ClientID %>.value) != 16)
                    document.all.<%= ddl_HandlingOffice.ClientID %>.value = "-1";
            }
        }

        function confirmToSave() {
            return confirm("Confirm that the NEXT Claim information is correct.")
        }

        function openAttachments(o, sid) {
            window.open('ClaimRequestAttachmentList.aspx?requestId=' + sid, 'ClaimRequestAttachmentList', 'status=1,width=400,height=500');
        }       
</script>


<table width="800px">
    <tr>
        <td colspan="3"><b><asp:validationsummary id="vs" runat="server"></asp:validationsummary></b></td>
    </tr>
    <tr id ="tr_UKClaimMainInfo" >
        <td colspan="2">
            <table>  
                <tr>
                    <td colspan="4">
                        <table id="tb_MainClaimInfo" style="margin-left:10px;" cellpadding="5" cellspacing="0" >
                            <tr style="display:none;">
                                <td width="100px"></td><td width="160px"></td><td width="100px"></td><td width="160px"></td>
                            </tr>
                            <tr>
                                <td colspan="4"><asp:CheckBox ID="chk_HasUKDN" runat="server" Text="Next Debit Note Available"  OnClick="chkHasUKDN_OnChange();"  OnChange="chkHasUKDN_OnChange();" checked="true" /></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">Next D/N No</td>
                                <td>
                                    <asp:TextBox ID="txt_UKDNNo" runat="server" />
                                </td>
                                <td class="FieldLabel2">Status</td>
                                <td>
                                    <div style="border:thin solid silver;width:150px;height:18px;display:none;">
                                        <b><asp:Label ID="lbl_Status" runat="server" style="position:relative;top:2px;left:2px;"></asp:Label></b>
                                    </div>
                                    <asp:TextBox ID="txt_Status" runat="server" style="font-weight:600;width:150px; "  readonly="true" TabIndex=-1  />
                                    <asp:HiddenField ID="hid_Status" runat="server" Value="" />
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">Next D/N Date</td>
                                <td>
                                    <div id="div_UKDNDate" style="display:block;">
                                        <cc2:SmartCalendar ID="txt_UKDNDate" runat="server"/>
                                    </div>
                                </td>
                                <td class="FieldLabel2">Next D/N Received Date</td>
                                <td>
                                    <div id="div_UKDNReceivedDate" style="display:block;">
                                        <cc2:SmartCalendar ID="txt_UKDNReceivedDate" runat="server"/>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4"><hr /></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2" >Claim Type</td>
                                <td colspan="3">
                                    <cc2:SmartDropDownList ID="ddl_ClaimType" runat="server" width="125px" 
                                        AutoPostBack="true" 
                                        onselectedindexchanged="ddl_ClaimType_SelectedIndexChanged"/>
                                    &nbsp;&nbsp;
                                    <span id="divMFRN" runat="server">
                                        <asp:TextBox runat="server" ID="txtMonth" style="width:100px;"/> (YYYYMM)
                                    </span>
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2" >Contract No</td>
                                <td>
                                    <asp:TextBox ID="txt_ContractNo" runat="server" style="width:140px;" 
                                        AutoPostBack="True" ontextchanged="txt_ContractNo_TextChanged"/>
                                </td>          
                                <td class="FieldLabel2">Item No</td>
                                <td>
                                    <asp:TextBox ID="txt_ItemNo" runat="server" OnTextChanged="txt_ItemNo_OnTextChanged" AutoPostBack="true"  />
                                    <asp:HiddenField ID="hid_ItemNo" runat="server"/>
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">Supplier</td>
                                <td colspan="3">
                                    <cc2:SmartDropDownList ID="ddl_Supplier" runat="server" Enabled="false" 
                                        Width="425px" onselectedindexchanged="ddl_Supplier_SelectedIndexChanged" AutoPostBack="true"/>
                                    <uc1:uclsmartselection  ID="txt_SZSupplier" runat="server" width="300px"/>
                                    <uc1:uclsmartselection  ID="txt_Supplier" runat="server" width="300px"/>
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">Office / H. Office / Term / PT</td>
                                <td colspan="3" valign="top">
                                    <table>
                                        <tr>
                                            <td>
                                                <cc2:SmartDropDownList ID="ddl_Office" runat="server" width="55px" Enabled="false"/>&nbsp;&nbsp;
                                            </td>
                                            <td>
                                                <cc2:SmartDropDownList ID="ddl_HandlingOffice" runat="server" width="55px"/>&nbsp;&nbsp;
                                            </td>
                                            <td>
                                                <cc2:SmartDropDownList ID="ddl_TermOfPurchase" runat="server" width="80px" Enabled="false"/>&nbsp;&nbsp;
                                            </td>
                                            <td>
                                                <uc1:UclSmartSelection ID="uclProductTeam" runat="server" Enabled="false"/>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">Qty</td>
                                <td>
                                    <asp:TextBox ID="txt_Qty" runat="server" />
                                </td>
                                <td class="FieldLabel2">Amount</td>
                                <td>
                                    <cc2:SmartDropDownList ID="ddl_Currency" runat="server" width="45px"/>&nbsp;
                                    <asp:TextBox ID="txt_Amt" runat="server"  style="Width:88px;"/>
                                </td>          
                            </tr>

                            <tr>
                                <td class="FieldLabel2">Comments</td>
                                <td colspan="3">
                                    <asp:TextBox ID="txt_Desc" runat="server" style="width:420px;"  />
                                </td>
                            </tr>
                            <tr>
                                <td />
                                <td colspan="3"><asp:TextBox TextMode="MultiLine" Rows="4" id="txt_RemarkHistory" runat="server" style="width:420px;background-color:lightgrey" readonly="true"/></td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">Settlement Option</td>
                                <td><cc2:SmartDropDownList ID="ddl_SettlementOption" runat="server" width="100px" /></td>
                                <td class="FieldLabel2">Payment Office</td>
                                <td><cc2:SmartDropDownList ID="ddl_PaymentOffice" runat="server" width="120px"/>&nbsp;&nbsp;</td>
                            </tr>
                            <tr>
                                <td />
                                <td colspan="3"><asp:CheckBox runat="server" ID="chkIsReadyForSettlement" Text="Ready For Settlement" /></td>
                            </tr>
                            <tr id="tr_DNCopyUpload" runat="server">
                                <td class="FieldLabel2">D/N Copy</td>
                                <td colspan="3">
                                    <asp:FileUpload ID="upd_UKDebitNote" runat="server" Width="430px" />
                                    <asp:HiddenField ID="hid_UKDebitNoUploadFile" runat="server" />
                                </td>
                            </tr>
                       </table>            
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <table id="tb_DNToSupplierInfo" runat="server" style="margin-left:10px;" cellpadding="5" cellspacing="0" >
                            <tr>
                                <td width="100px"></td>
                                <td width="160px"></td>
                                <td width="100px"></td>
                                <td width="160px"></td>
                                <td width="50px"></td>
                            </tr>
                           <tr>
                                <td colspan="5"  style="border-top: thin solid #CCCCCC;">
                                    <asp:Panel ID="pnl_DNToSupInfo" runat="server" skinid="sectionHeader2"  Height="22" >
                                        <div style="padding:3px;" class="header2">DN To Supplier Info</div>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">Debit Note No</td>
                                <td>
                                    <asp:TextBox ID="txt_DebitNoteNo" runat="server" Enabled="false" backcolor="ControlLight" />
                                </td>
                                <td class="FieldLabel2">Debit Note Date</td>
                                <td>
                                    <asp:TextBox ID="txt_DebitNoteDate" runat="server" Enabled="false" BackColor="ControlLight" />
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel2">Settlement Date</td>
                                <td>
                                    <asp:TextBox ID="txt_DebitNoteSettlementDate" runat="server" Enabled="false" backcolor="ControlLight" />
                                </td>
                                <td class="FieldLabel2" style="display:none;">Debit Note Amount</td>
                                <td style="display:none;">
                                    <asp:TextBox ID="txt_DebitNoteAmount" runat="server" Enabled="false" backcolor="ControlLight" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>        
        </td>
    </tr>
</table>

<asp:Panel runat="server" ID="pnlClaimRequestMapping">
    <table id="Table5" runat="server" style="margin-left:10px;" cellpadding="5" cellspacing="0" >
        <tr>
            <td width="100px"></td>
            <td width="160px"></td>
            <td width="100px"></td>
            <td width="160px"></td>
            <td width="50px"></td>
        </tr>
        <tr>
            <td colspan="5" style="border-top: thin solid #CCCCCC;"> </td>
        </tr>
    </table>

    <asp:Panel ID="Panel2" runat="server" skinid="sectionHeader1" Width="700px" height="24" >
        <div style="padding-top:4px;font-size:9pt;">&nbsp;Claim Request Detail</div>
    </asp:Panel>

    <asp:GridView ID="gv_Request" runat="server" AutoGenerateColumns="false" 
        onrowdatabound="gv_Request_RowDataBound" >
        <Columns>
           <asp:TemplateField HeaderText="" >
                <ItemTemplate> 
                    <div style="display:none;">
                        <asp:RadioButton GroupName="ClaimRequestGroup" id="radClaimRequest" runat="server" AutoPostBack="false" onClick="selectRequest(this);" />
                        <asp:HiddenField ID="hid_ClaimRequestId" runat="server" Value="" />
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
           <asp:TemplateField HeaderText="Claim Type">
                <ItemTemplate>
                    <asp:Label ID="lbl_ClaimType" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Item/Contract No">
                <ItemTemplate>
                    <asp:Label ID="lbl_ItemContractNo" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Form No">
                <ItemTemplate>
                    <asp:Label ID="lbl_FormNo" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Form Issue Date">
                <ItemTemplate>
                    <asp:Label ID="lbl_FormIssueDate" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Month">
                <ItemTemplate>
                    <asp:Label ID="lbl_Month" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="NS %">
                <ItemTemplate>
                    <asp:Label ID="lbl_NSRechargePercent" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Supplier %">
                <ItemTemplate>
                    <asp:Label ID="lbl_SupplierRechargePercent" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <asp:Label ID="lbl_Status" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:LinkButton Text="Attachments" runat="server" ID="lnk_Attachment" CommandName="ListAttachments"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>   
    <br />
</asp:Panel>

<asp:Panel runat="server" ID="pnlClaimRefund">
    <table id="Table4" runat="server" style="margin-left:10px;" cellpadding="5" cellspacing="0" >
        <tr>
            <td width="100px"></td>
            <td width="160px"></td>
            <td width="100px"></td>
            <td width="160px"></td>
            <td width="50px"></td>
        </tr>
        <tr>
            <td colspan="5" style="border-top: thin solid #CCCCCC;"> </td>
        </tr>
    </table>

    <asp:Panel ID="Panel4" runat="server" skinid="sectionHeader1" Width="700px" height="28" >
            <table border="0"  cellpadding="0" cellspacing="0"  >
                <tr >
                    <td>
                           <div style="padding-top:2px;font-size:9pt;">&nbsp;Next Claim Refund&nbsp;&nbsp;</div>
                    </td>
                    <td>
                        <div style="padding-top:1px;">
                        &nbsp;&nbsp;
                            <asp:Button ID="btn_AddRefund" runat="server" Text="Add" onclick="btn_AddRefund_Click"  CausesValidation="false"/>
                            <asp:HiddenField ID="hid_RefundResult" runat="server" Value="" />
                        </div>
                    </td>
                </tr>
            </table>
    </asp:Panel>
    <asp:GridView ID="gv_Refund" runat="server" AutoGenerateColumns="false" 
        OnRowCommand="gv_Refund_RowCommand" onrowdatabound="gv_Refund_RowDataBound"  RowStyle-VerticalAlign="Top">
        <Columns>

           <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:ImageButton ID="imgEditRefund" runat="server" ImageUrl="~/images/icon_edit.gif"  ToolTip="Edit Claim Refund"  CommandName="EditUKClaimRefund"/>
                    <asp:ImageButton ID="imgDeleteRefund" runat="server" ImageUrl="~/images/icon_delete.gif" ToolTip="Delete Next Claim Refund" CommandName="DeleteUKClaimRefund" />
                </ItemTemplate>
           </asp:TemplateField>
            <asp:TemplateField HeaderText="Received Date" ItemStyle-Width="90px"  ItemStyle-Height="30px">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" style="position:absolute; width:520px; text-align:left;">
                        <table>
                            <tr><td height="15px"></td></tr>
                            <tr>
                                <td><b>Remark : </b></td>
                                <td>&nbsp;</td>
                                <td style="width:500px;border:1px solid Silver ; background-color:White; z-index:10; text-align:left;">
                                       <asp:Label ID="lbl_Remark" runat="server" style="padding-left:5px;"/>
                                </td>
                            </tr>
                        </table>
                    </asp:Label><asp:Label ID="lbl_ReceivedDate" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Amount" ItemStyle-Width="90px">
                <ItemTemplate>
                    <asp:Label ID="lbl_Amount" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
           <asp:TemplateField HeaderText="Credit Note No." >
                <ItemTemplate>
                    <asp:Label ID="lbl_CreditNoteNo" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Credit Note Date" ItemStyle-Width="100px">
                <ItemTemplate>
                    <asp:Label ID="lbl_CreditNoteDate" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Credit Note Amount" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center" Visible="false">
                <ItemTemplate>
                    <asp:Label ID="lbl_CreditNoteAmount" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
           <asp:TemplateField HeaderText="Settlement Option" >
                <ItemTemplate>
                    <asp:Label ID="lbl_SettlementOption" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
           <asp:TemplateField HeaderText="Ready For Settlement" >
                <ItemTemplate>
                    <asp:Label ID="lbl_IsReadyForSettlement" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
           <asp:TemplateField HeaderText="Settlement Date" >
                <ItemTemplate>
                    <asp:Label ID="lbl_RefundSettlementDate" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView> 
    <br />  
</asp:Panel>

<asp:Panel runat="server" ID="pnlBIAMapping">
    <table id="Table3" runat="server" style="margin-left:10px;" cellpadding="5" cellspacing="0" >
        <tr>
            <td width="100px"></td>
            <td width="160px"></td>
            <td width="100px"></td>
            <td width="160px"></td>
            <td width="50px"></td>
        </tr>
        <tr>
            <td colspan="5" style="border-top: thin solid #CCCCCC;"> </td>
        </tr>
    </table>

    <asp:Panel ID="Panel5" runat="server" skinid="sectionHeader1" Width="610px" height="24" >
        <div style="padding-top:4px;font-size:9pt;">&nbsp;This BIA covers below NEXT claim(s)</div></asp:Panel><asp:GridView 
            ID="gv_BIAMapping" runat="server" AutoGenerateColumns="false" ShowFooter="true" 
            onrowdatabound="gv_BIAMapping_RowDataBound"><Columns>
           <asp:TemplateField HeaderText="Claim Type">
                <ItemTemplate>
                    <asp:Label ID="lbl_ClaimType" runat="server" />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lbl_TotalCnt" runat="server" />
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Item/Contract No">
                <ItemTemplate>
                    <asp:Label ID="lbl_ItemContractNo" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Month">
                <ItemTemplate>
                    <asp:Label ID="lbl_Month" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Next D/N No.">
                <ItemTemplate>
                    <asp:Label ID="lbl_UKDNNo" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Next D/N Date">
                <ItemTemplate>
                    <asp:Label ID="lbl_UKDNDate" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Amount">
                <ItemTemplate>
                    <asp:Label ID="lbl_Amt" runat="server" />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Label ID="lbl_TotalAmt" runat="server" />
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <asp:Label ID="lbl_Status" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:LinkButton Text="Attachments" runat="server" ID="lnk_Attachment" CommandName="ListAttachments"></asp:LinkButton></ItemTemplate></asp:TemplateField></Columns></asp:GridView><br /></asp:Panel><asp:Panel runat="server" ID="pnl_BIADiscrepancy">
    <table id="Table1" runat="server" style="margin-left:10px;" cellpadding="5" cellspacing="0" >
        <tr>
            <td width="100px"></td>
            <td width="160px"></td>
            <td width="100px"></td>
            <td width="160px"></td>
            <td width="50px"></td>
        </tr>
        <tr>
            <td colspan="5" style="border-top: thin solid #CCCCCC;">
                <div style="padding:3px;" class="header2">BIA Discrepancy</div></td></tr><tr>
            <td class="FieldLabel2">Handling Method</td><td>
                <cc2:SmartDropDownList ID="ddl_Action" runat="server" />
            </td>
            <td class="FieldLabel2">Amount</td><td>
                <asp:TextBox ID="txt_BIADiscrepancyAmt" runat="server" Enabled="false"/>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">Remark</td><td>
                <asp:TextBox ID="txt_BIADiscrepancyRemark" runat="server" width="150px" ToolTip="This will be displayed on the attached D/N"/>
            </td>
            <td colspan="3">
                <asp:CheckBox runat="server" Text="Mark As Complete" ID="chkBIADiscrepancyComplete" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">DR/CR Note No</td><td><asp:TextBox ID="txt_BIADiscrepancyDNNo" runat="server" Enabled="false"/></td>
            <td class="FieldLabel2">DR/CR Note Date</td><td><asp:TextBox ID="txt_BIADiscrepancyDNDate" runat="server" Enabled="false"/></td>
        </tr>
    </table>
</asp:Panel>
<table id="Table2" runat="server" style="margin-left:10px;" cellpadding="5" cellspacing="0" >
    <tr>
        <td width="100px"></td>
        <td width="160px"></td>
        <td width="100px"></td>
        <td width="160px"></td>
        <td width="50px"></td>
    </tr>
    <tr>
        <td colspan="5" style="border-top: thin solid #CCCCCC;"> </td>
    </tr>
</table>

    <asp:CustomValidator ID="valCustom" runat="server" Display="None" 
        onservervalidate="valCustom_ServerValidate"></asp:CustomValidator><asp:Button ID="btn_Save" runat="server" Text="Save" 
        onclick="btn_Save_Click" onClientClick="if (!confirmToSave()) return false;showProgressBarX();" CausesValidation="false" />&nbsp;&nbsp; <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" onclick="btn_Cancel_Click" CausesValidation="false"/>&nbsp;&nbsp; <asp:Button ID="btn_ViewLog" runat="server" Text="View Log" SkinID="MButton"  CausesValidation="false"/>&nbsp;&nbsp; <asp:Button ID="btn_ViewAttachment" runat="server" Text="View Attachment" SkinID="LButton"  CausesValidation="false"/>
        <asp:Button ID="btn_New" runat="server" Text="New Next Claim" SkinID="LButton" onclick="btn_New_Click"/>&nbsp;&nbsp; <asp:Button 
        ID="btn_GoToBIA" runat="server" Text="Handle BIA Discrepancy" 
        SkinID="XLButton_Hint" Visible="false" onclick="btn_GoToBIA_Click" />
    <script language="javascript" type="text/javascript">
        //initSmartCalendar("txt_UKDNDate");
        //initSmartCalendar("txt_UKDNReceivedDate");
        chkHasUKDN_OnChange()
    </script>
        <br />
            <br />
        <div style="border:1px,1px,1px,1px;background-color:Aqua;color:black;" runat="server" id="divNoteForTR">
        <b>Note: </b><br />
        <ul>
            <li>Office Turkey & Egypt allows prompt debit note to supplier (Next Claim D/N received on or after 03/06/2013</li><li>Office Shanghai allows prompt debit note to supplier (MFRN Only)</li><li>i.e. No need to wait for Factory-Sign-Off</li></ul><br />
        </div>

</asp:Content>
