<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="ShipmentSearch.aspx.cs" Inherits="com.next.isam.webapp.shipping.ShipmentSearch" Title="Shipping Module" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--    
    <img src="../images/banner_shipping_search.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Shipping">Search Engine</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >

    <script type="text/javascript" >

        function open_window(url, windowName, parameterList) {
            // Pre-NOW version
            window.open(url, windowName, parameterList);
            //NOW version
            //openPopupWindow(url, windowName, parameterList);
        }

        function searchFormValidate() {
            if (document.getElementById("ctl00_ContentPlaceHolder1_txtItemNo_q").value == "" &&
        document.getElementById("ctl00_ContentPlaceHolder1_txtContractNo_q").value == "" &&
        document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNo_q").value == "") {
                alert("Please enter search criteria.");
                return false;
            }

            if (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNo_q").value != "" &&
            !isInvoiceNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNo_q").value.trim())) {
                alert("Invoice number format invalid");
                return false;
            }
            return true;
        }

        function validateAdvanceSearchCriteria() {
            if (document.getElementById("ctl00_ContentPlaceHolder1_txt_DocNo").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txtAtWHDateFrom_txtAtWHDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txtInvoiceUploadDateFrom_txtInvoiceUploadDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txtInvoiceDateFrom_txtInvoiceDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txtSupplierInvoiceNoFrom").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txtNSLInvoiceNoFrom").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txtItemNo").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txtContractNo").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_ILSInWHDateFrom_txt_ILSInWHDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_ILSInWHDateTo_txt_ILSInWHDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceSentDateFrom_txt_InvoiceSentDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceSentDateTo_txt_InvoiceSentDateTo_textbox").value == ""
            ) {
                alert("Please enter search criteria on one of below search criteria.\r\n- Contract No.\r\n" +
                "- Item No.\r\n- NSL Invoice No.\r\n- Supplier Invoice No.\r\n- NSL Invoice Date\r\n" +
                "- NSL Upload Date\r\n- Cus. AWH Date\r\n- Document No.\r\n- ILS In-WH Date" +
                "- Invoice Sent Date");
                return false;
            }

            return true;
        }

        function copyNSLInvoiceNo() {
            document.getElementById("ctl00_ContentPlaceHolder1_txtNSLInvoiceNoFrom").value = document.getElementById("ctl00_ContentPlaceHolder1_txtNSLInvoiceNoFrom").value.toUpperCase();
            document.getElementById("ctl00_ContentPlaceHolder1_txtNSLInvoiceNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txtNSLInvoiceNoFrom").value;
            if (document.getElementById("ctl00_ContentPlaceHolder1_txtNSLInvoiceNoFrom").value != "") {
                var d = new Date();
                document.getElementById("ctl00_ContentPlaceHolder1_txt_FiscalYear").value = d.getFullYear();
            }
            else
                document.getElementById("ctl00_ContentPlaceHolder1_txt_FiscalYear").value = "";
        }

        function copySupplierInvoiceNo() {
            document.getElementById("ctl00_ContentPlaceHolder1_txtSupplierInvoiceNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txtSupplierInvoiceNoFrom").value;
        }

        function cbxOfficeOnChange() {
            if (typeof (getTxtName("ctl00_ContentPlaceHolder1_uclProductTeam")) != 'undefined') {
                var txtName = getTxtName("ctl00_ContentPlaceHolder1_uclProductTeam");
                var txtOldName = getTxtOldName("ctl00_ContentPlaceHolder1_uclProductTeam");
                var txtId = getTxtId("ctl00_ContentPlaceHolder1_uclProductTeam");
                var cbxList = getCbxList("ctl00_ContentPlaceHolder1_uclProductTeam");

                txtName.value = "";
                txtOldName.value = "";
                txtId.value = "";
                if (cbxList.length > 0) cbxList.remove(0);
            }

        }

        function isReadyToPrint() {
            if (GetCheckBoxSelectedCount('ckb_Print') == 0) {
                alert('Please select one invoice at least.');
                return false;
            }
            return true;
        }


        function openShipment(parameter) {
            var docRcptDate = document.getElementById("ctl00_ContentPlaceHolder1_txt_DefaultDocReceiptDate_txt_DefaultDocReceiptDate_textbox");
            if (docRcptDate != null) {
                if (docRcptDate.value != "" && !isDateValid(docRcptDate.value)) {
                    alert("Invalid document receipt date.");
                    return false;
                }
            }
            this.open_window("ShipmentDetail.aspx?" + parameter, 'ShipmentDetail', 'width=800,height=600, top=0, left=0,scrollbars=1,resizable=1,status=1');
        }

        function EditMultipleShipment(parameter) {
            var i, obj, Ccy, inputElement, shipmentIdList, CustomerDestination;
            Ccy = "";
            CustomerDestination = "";
            shipmentIdList = "";
            inputElement = document.getElementsByTagName("input");
            for (i = 0; i < inputElement.length; i++) {
                obj = inputElement[i];
                if (obj.id.indexOf("ckb_Print") >= 0) {
                    if (obj.checked) {
                        if (Ccy == "")
                            Ccy = obj.CurrencyCode;
                        if (CustomerDestination == "")
                            CustomerDestination = obj.CustomerDestination;
                        if (Ccy != obj.CurrencyCode || CustomerDestination != obj.CustomerDestination)
                            break;
                    }
                }
            }
            if (i >= inputElement.length) {
                if (parameter != "")
                    this.open_window("InvoiceForMultipleShipment.aspx?" + parameter, 'InvoiceForMultipleShipment', 'width=800,height=600,scrollbars=1,resizable=1,status=1');
            }
            else
                alert('For editing multiple shipment, all selected shipments must in same Currency and Destination.')
            return;
        }
        
     </script>
     
    <table width="900px" cellspacing="2" cellpadding="2">
        <tr>
            <td>
                <table id="tb_QuickSearch" runat="server" width="100%">
                    <tr>
                        <td class="FieldLabel2">&nbsp;Contract No.&nbsp;</td>
                        <td><asp:TextBox ID="txtContractNo_q" runat="server"  /></td>
                        <td class="FieldLabel2">&nbsp;Item No.&nbsp;</td>
                        <td><asp:TextBox ID="txtItemNo_q" runat="server" /></td>
                        <td class="FieldLabel2">&nbsp;NSL Invoice No.&nbsp;</td>
                        <td><asp:TextBox ID="txt_InvoiceNo_q" runat="server" /></td>
                        <td></td>
                    </tr>                       
                    <tr><td><asp:LinkButton ID="btn_Advance" runat="server" Text="Advanced Search" 
                                onclick="btn_Advance_Click" /></td></tr>
                    <tr><td>&nbsp;</td></tr>

                    <tr>
                        <td colspan="7"><asp:Button ID="btn_QuickSearch" runat="server" Text="Search" 
                                onclick="btn_QuickSearch_Click" OnClientClick="return searchFormValidate();"/>&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btn_QuickReset" runat="server" Text="Reset" 
                                onclick="btn_QuickReset_Click" /></td>
                    </tr>
                    <tr><td>&nbsp;</td></tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table id="tb_AdvanceSearch" runat="server" visible="false" width="100%" cellspacing="0" cellpadding="0">
                    <tr>
                        <td colspan="5">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2" style="width:100px;">
                            <asp:Label ID="lblContractNo" runat="server" Text="&nbsp;Contract No.&nbsp;"></asp:Label> 
                        </td>
                        <td>
                            <asp:TextBox ID="txtContractNo" runat="server" Width="120px"></asp:TextBox>
                        </td>
                        <td style="width:1px;">&nbsp;</td>
                        <td class="FieldLabel2" style="width:100px;">
                            &nbsp;NSL Invoice No.&nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNSLInvoiceNoFrom" runat="server" 
                                onblur = "copyNSLInvoiceNo();" MaxLength="9" />&nbsp;To&nbsp;
                            <asp:TextBox ID="txtNSLInvoiceNoTo" runat="server" MaxLength="9" />
                            <asp:TextBox ID="txt_FiscalYear" runat="server" SkinID="SmallTextBox"  MaxLength="4" Width="40px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Delivery No.</td>
                        <td><asp:TextBox ID="txtDeliveryNo" runat="server" /></td>
                        <td></td>
                        <td class="FieldLabel2">&nbsp;Supplier Invoice No.</td>
                        <td><asp:TextBox ID="txtSupplierInvoiceNoFrom" onblur="copySupplierInvoiceNo();" runat="server" />&nbsp;To&nbsp;
                            <asp:TextBox ID="txtSupplierInvoiceNoTo" runat="server" /></td>
                    </tr>                    
                    <tr>
                        <td class="FieldLabel2">
                            <asp:Label ID="lblItemNo" runat="server" Text="Item No."></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtItemNo" runat="server" />
                        </td>
                        <td>&nbsp;</td>
                        <td class="FieldLabel2">
                            &nbsp;NSL Invoice Date
                        </td>
                        <td>
                            <cc2:SmartCalendar ID="txtInvoiceDateFrom" FromDateControl="txtInvoiceDateFrom" 
                            runat="server" ToDateControl="txtInvoiceDateTo" />
                            &nbsp;To&nbsp;
                            <cc2:SmartCalendar ID="txtInvoiceDateTo" FromDateControl="txtInvoiceDateFrom"
                                ToDateControl="txtInvoiceDateTo" runat="server" />  
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Customer</td>
                        <td>
                            <cc2:smartdropdownlist id="ddl_Customer" runat="server" Width="140px" CssClass="cbx" />
                        </td>
                        <td>&nbsp;</td>
                        <td class="FieldLabel2">&nbsp;NSL Upload Date</td>
                        <td>
                            <cc2:SmartCalendar ID="txtInvoiceUploadDateFrom" FromDateControl="txtInvoiceUploadDateFrom" 
                                ToDateControl="txtInvoiceUploadDateTo" runat="server" />
                            &nbsp;To&nbsp;
                            <cc2:SmartCalendar ID="txtInvoiceUploadDateTo" FromDateControl="txtInvoiceUploadDateFrom"
                                ToDateControl="txtInvoiceUploadDateTo" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Final Destination</td>
                        <td><cc2:SmartDropDownList ID="ddl_FinalDestination" runat="server" /></td>
                        <td>&nbsp;</td>
                        <td class="FieldLabel2"><span title="Customer Scheduled At-Warehouse Date">&nbsp;Cus. AWH Date</span></td>
                        <td> 
                            <cc2:SmartCalendar ID="txtAtWHDateFrom" FromDateControl="txtAtWHDateFrom" 
                                ToDateControl="txtAtWHDateTo" runat="server" />
                            &nbsp;To&nbsp;
                            <cc2:SmartCalendar ID="txtAtWHDateTo" FromDateControl="txtAtWHDateFrom"
                                ToDateControl="txtAtWHDateTo" runat="server" />
                         </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;OPR Type</td>
                        <td>                            
                            <cc2:smartdropdownlist id="ddl_OPRType" runat="server"></cc2:smartdropdownlist>
                        </td>
                        <td>&nbsp;</td>
                        <td class="FieldLabel2"><span>&nbsp;ILS In-WH Date</span></td>
                        <td><cc2:SmartCalendar ID="txt_ILSInWHDateFrom" FromDateControl="txt_ILSInWHDateFrom" 
                                ToDateControl="txt_ILSInWHDateTo" runat="server" />
                            &nbsp;To&nbsp;
                            <cc2:SmartCalendar ID="txt_ILSInWHDateTo" FromDateControl="txt_ILSInWHDateFrom"
                                ToDateControl="txt_ILSInWHDateTo" runat="server" />   
                         </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Order Type</td>
                        <td><cc2:smartdropdownlist id="ddl_OrderType" runat="server" AutoPostBack="True"
                                onselectedindexchanged="ddl_OrderType_SelectedIndexChanged" /></td>
                        <td>&nbsp;</td>
                        <td class="FieldLabel2">&nbsp;Invoice Sent Date</td>
                        <td><cc2:SmartCalendar ID="txt_InvoiceSentDateFrom" FromDateControl="txt_InvoiceSentDateFrom" 
                                ToDateControl="txt_InvoiceSentDateTo" runat="server" />
                            &nbsp;To&nbsp;
                            <cc2:SmartCalendar ID="txt_InvoiceSentDateTo" FromDateControl="txt_InvoiceSentDateFrom"
                                ToDateControl="txt_InvoiceSentDateTo" runat="server" />   
                        </td>                       
                    </tr>
                    <tr>
                   <td class="FieldLabel2">&nbsp;Purchase Term</td>
                        <td><cc2:smartdropdownlist ID="ddl_TermOfPurchase" runat="server" /></td>
                        <td>&nbsp;</td>
                         <td class="FieldLabel2">&nbsp;Office</td>
                        <td><cc2:smartdropdownlist id="ddl_Office" runat="server" Width="70px"></cc2:smartdropdownlist>
                            <cc2:SmartDropDownList ID="ddl_ShippingUser" runat="server" Visible="false"  />
                        </td>                                                                         
                    </tr>
                    <tr>
                         <td class="FieldLabel2">&nbsp;Document No.</td>
                        <td><asp:TextBox ID="txt_DocNo" runat="server" /></td>
                        <td>&nbsp;</td>
                        <td class="FieldLabel2">&nbsp;Product Team</td>
                        <td>                            
                              <uc1:uclsmartselection id="uclProductTeam" runat="server"></uc1:uclsmartselection>
                        </td>   
                    </tr>
                    <tr>                                                                              
                        <td class="FieldLabel2">&nbsp;Country of Origin</td>
                        <td><cc2:SmartDropDownList ID="ddl_CO" runat="server" /></td> 
                        <td>&nbsp;</td>                        
                        <td class="FieldLabel2">&nbsp;Supplier Name</td>
                        <td>
                            <uc1:UclSmartSelection ID="txt_SupplierName" runat="server" />
                         </td>  
                    </tr>
                    
                    <tr>
                        <td colspan="5">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">
                            <asp:Label ID="lblShipmentMethod" runat="server" Text="&nbsp;Shipment Method" />
                        </td>
                        <td colspan="4" class="CellWithBorder" height="30px">
                            <asp:CheckBox ID="ckbAir" runat="server" Text="AIR" Checked="true" />&nbsp;&nbsp;&nbsp;
                            <asp:CheckBox ID="ckbEcoAir" runat="server" Text="EcoAIR" Checked="true" />&nbsp;&nbsp;&nbsp;
                            <asp:CheckBox ID="ckbSea" runat="server" Text="SEA" Checked="true" />&nbsp;&nbsp;&nbsp;
                            <asp:CheckBox ID="ckbSeaAir" runat="server" Text="SEA/AIR" Checked="true" />&nbsp;&nbsp;&nbsp;
                            <asp:CheckBox ID="ckbTruck" runat="server" Text="TRUCK" Checked="true" />                            
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Status</td>
                        <td  colspan="4"  height="30px" style="border-top-style: none; border-top-width: 0px;" class="CellWithBorder">             
                        <asp:CheckBox id="ckb_Status_Enquiry" runat="server" Text="ENQUIRY" Width="100" Checked="true"  />
                            <asp:CheckBox ID="ckb_Status_Draft" runat="server" Text="DRAFT" Width="100" Checked="true"  />
                            <asp:CheckBox ID="ckb_Status_pend4Appraval" runat="server" Text="PENDING FOR APPROVAL" Width="150" Checked="true"  />
                            <asp:CheckBox ID="ckb_Status_Pend4Cancel" runat="server" Text="PENDING FOR CANCEL" Width="150" Checked="true"  /><br />
                            <asp:CheckBox ID="ckb_Status_AMENDED" runat="server" Text="AMENDED" Width="100" Checked="true"  />
                            <asp:CheckBox ID="ckb_Status_Reject" runat="server" Text="REJECTED" Width="100" Checked="true"  />
                            <asp:CheckBox ID="ckb_Status_Approved" runat="server" Text="APPROVED" Width="150" Checked="true"  />
                            <asp:CheckBox ID="ckb_Status_POGen" runat="server" Text="PO GENERATED" Width="150" Checked="true"  /><br />
                            <asp:CheckBox ID="ckb_Status_Invoiced" runat="server" Text= "INVOICED" Width="100" Checked="true"  />
                            <asp:CheckBox ID="ckb_Status_Cancelled" runat="server" Text="CANCELLED" Width="100" Checked="true" />
                            <asp:LinkButton ID="lnk_StatusAll"  runat="server" OnClientClick="CheckAll('ckb_Status');return false;" Text="ALL" />&nbsp;&nbsp;&nbsp;
                            <asp:LinkButton ID="lnk_StatusNone" runat="server" OnClientClick="UncheckAll('ckb_Status'); return false;" Text="NONE" />
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td colspan="4">
                           <asp:CheckBox ID="ckbSplitContractOnly" runat="server" Text ="Split Contract Only "/>&nbsp;
                           <asp:CheckBox ID="ckb_IsSZOrder" runat="server" Text="NSL (SZ) Order Only "/>&nbsp;
                           <asp:CheckBox ID="ckb_LDP" runat="server" Text="LDP Order "/>&nbsp;
                           <asp:CheckBox ID="ckb_QCCharge" runat="server" Text="With QC Charge "/>&nbsp;
                           <asp:CheckBox ID="ckb_IsSample" runat="server" Text="Mock Shop / Press / Studio Sample "/>&nbsp;
                           <asp:CheckBox ID="ckb_ReprocessGoods" runat="server" Text="Reprocess Goods"/>&nbsp;
                           <br />
                           <asp:CheckBox ID="ckb_GBTest" runat="server" Text="GB Test Required" />
                           <!---<asp:CheckBox ID="ckb_QCCInspection" runat="server" Text="QCC Inspection" />--->
                           <asp:CheckBox ID="ckb_IsTradingAF" runat="server" Text="Trading A/F" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" style="text-align :right ;"></td>
                    </tr>
                    <tr>
                        <td><asp:LinkButton ID="btn_Quick" runat="server" Text="Quick Search" 
                                onclick="btn_Quick_Click" /></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                    </tr>
                    <tr>
                        <td colspan="5">&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="5">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" 
                                onclick="btnSearch_Click" OnClientClick="return validateAdvanceSearchCriteria();" />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnReset" runat="server" Text="Reset" 
                                onclick="btnReset_Click" />
                        </td>
                    </tr>
                </table>
            
            </td>
        </tr>
    </table>
    <asp:Panel ID="pnl_Result" runat="server" Visible="false">
    <asp:Label ID="lbl_RowCount" runat="server" style="color:#ff9900; font-weight :bolder;" />
    <br /><br />
    <table width="500px">
    <tr>
        <td valign="middle"><img src="../images/icon_DualSourcing.gif" alt="" /></td><td valign="middle">Dual Sourcing</td>
        <td valign="middle"><img src="../images/icon_ReleaseLock.gif" alt="" /></td><td valign="middle">Lock Released</td>
        <td valign="middle"><img src="../images/icon_SZOrder.gif" alt="" /></td><td valign="middle">NSL (SZ) Order</td>
        <td valign="middle"><img src="../images/icon_OPRFabricOrder.gif" alt="" /></td><td valign="middle">OPR Order</td>
        <td valign="middle"><img src="../images/icon_LDP.gif" alt="" /></td><td valign="middle">LDP Order</td>
        <td valign="middle"><img src="../images/icon_Test.png" alt="" /></td><td valign="middle">GB Test Required</td>
        <td valign="middle"><img src="../images/icon_pass.png" alt="" /></td><td valign="middle">GB Test Pass</td>
        <td valign="middle"><img src="../images/icon_fail.png" alt="" /></td><td valign="middle">GB Test Fail (Release Payment)</td>
        <td valign="middle"><img src="../images/icon_air.gif" alt="" /></td><td valign="middle">Air Shipment</td>
        <td valign="middle"><img src="../images/icon_air_Purple.gif" alt="" /></td><td valign="middle">Air Shipment(Payment% > 0)</td>
    </tr>
    <tr>
        <td valign="middle"><img src="../images/icon_Uturn.gif" alt="" /></td><td valign="middle">U-Turn Order</td>
        <td valign="middle"><img src="../images/icon_MockShop.gif" alt="" /></td><td valign="middle">Mock Shop Sample</td>
        <td valign="middle"><img src="../images/icon_PressSample.gif" alt="" /></td><td valign="middle">Press Sample</td>
        <td valign="middle"><img src="../images/icon_StudioSample.gif" alt="" /></td><td valign="middle">Studio Sample</td>
        <td valign="middle"><img src="../images/icon_DiscountOrder.gif" alt="" /></td><td valign="middle">UK Discount</td>
        <td valign="middle"><img src="../images/icon_QCCharge.gif" alt="" /></td><td valign="middle">With QC Charge</td>
        <td valign="middle"><img src="../images/icon_ReprocessGoods.gif" alt="" /></td><td valign="middle">Reprocess Goods</td>
        <td valign="middle"><img src="../images/icon_fail_orange.png" alt="" /></td><td valign="middle">GB&nbsp;Test&nbsp;Failed (Hold Payment)</td>
        <td valign="middle"><img src="../images/icon_fail_red.png" alt="" /></td><td valign="middle">GB&nbsp;Test&nbsp;Failed (Cannot Release Payment)</td>
        <!---<td valign="middle"><img src="../images/icon_QCCInspection.png" alt="" /></td><td valign="middle">QCC Inspection</td>--->
        <td valign="middle"><img src="../images/icon_air_Red.gif" alt="" /></td><td valign="middle">Trading Air Freight</td>
    </tr>    
    <tr>
        <td colspan="2" valign="top">
            <asp:Button ID="btn_Print" runat="server" Text="  Print  " OnClick="btn_Print_Click" OnClientClick="return isReadyToPrint();" />&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_Edit" runat="server" Text="Edit" Visible="false" OnClick="btn_Edit_Click" />&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_Send" runat="server" Text="Send" Visible="false" OnClick="btn_Send_Click" OnClientClick='return isReadyToPrint();' />
        </td>
     </tr>
    </table>       
    <br />
    <table border="0">
        <tr>
            <td>
                <asp:ImageButton ID="lnk_SelectAll" runat="server" OnClientClick="CheckAll('ckb_Print');return false;" ImageUrl="../images/icon_selectall.jpg" AlternateText="select all"/>&nbsp;&nbsp;&nbsp;
                <asp:ImageButton ID="lnk_DeselectAll" runat="server" OnClientClick="UncheckAll('ckb_Print');return false;" ImageUrl="../images/icon_deselectall.jpg" AlternateText="de-select all" />&nbsp;&nbsp;&nbsp;
            </td>
            <td>
                <table class="CellWithBorder" cellpadding="3" id="tb_DefaultReceiptDate" runat="server" visible="false">
                    <tr>
                        <td><b>Default Doc Receipt Date</b></td>
                        <td><cc2:SmartCalendar ID="txt_DefaultDocReceiptDate" runat="server" /></td>                                            
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:GridView ID="gvInvoice" runat="server" AllowPaging="True" AllowSorting="True" 
        AutoGenerateColumns="False" OnSorting="gvInvoiceOnSort" OnRowCommand="InvoiceRowCommand"
        OnRowDataBound ="InvoiceDataBound" PageSize="100" OnPageIndexChanging="gvInvoicePageIndexChanged">
        <Columns>
            <asp:TemplateField ItemStyle-VerticalAlign="Middle" >
                <ItemTemplate>                    
                    <asp:ImageButton ID="btnView" runat="server" ToolTip="View Detail" CommandName="cmdView" ImageAlign="Middle" ImageUrl="~/images/btn_view.gif" />&nbsp;                    
                    <input type="checkbox" id="ckb_Print"  onclick="if (this.alt != '') SelectAllWithSameType('ckb_Print', this.alt);" runat="server" visible="false" disabled="disabled"  />
                    <asp:HiddenField ID="hid_ShipmentId" value="" runat="server" />
                    <%--<asp:CheckBox ID="ckb_Print" onclick="alert(this.alt);" runat="server" Visible ="false"  ToolTip="Print Invoice" Enabled="false"  />--%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Contract No." SortExpression="ContractNo" HeaderStyle-ForeColor="CornflowerBlue">
                <ItemTemplate >
                    <asp:Label ID="lbl_ContractNo" Text='<%# Eval("ContractNo") %>' runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="NSL Dly No." SortExpression="DeliveryNo" HeaderStyle-ForeColor="CornflowerBlue">
                <ItemTemplate >
                    <asp:Label ID="lbl_DeliveryNo" Text='<%# Eval("DeliveryNo") %>' runat="server" />
                </ItemTemplate>
                <HeaderStyle Width="20px" />
                <ControlStyle Width="10px" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Image ID="img_ReadDmsDoc" ImageUrl="~/images/icon_edit.gif" Visible="false" runat="server" style="cursor:hand;" ToolTip="Read DMS Doc."/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="100px">
                <ItemTemplate>
                    <asp:Image ID="img_dualSourcing" ImageUrl="~/images/icon_DualSourcing.gif" Visible="false" runat="server" />
                    <asp:Image ID="img_SZOrder" ImageUrl="~/images/icon_SZOrder.gif" Visible="false" runat="server" />
                    <asp:Image ID="img_discount" ImageUrl="~/images/icon_DiscountOrder.gif" Visible="false" runat="server" />
                    <asp:Image ID="img_OPRFabric" ImageUrl="~/images/icon_OPRFabricOrder.gif" Visible="false" runat="server" />
                    <asp:Image ID="img_releaseLock" ImageUrl="~/images/icon_ReleaseLock.gif" Visible="false" runat="server" />
                    <asp:Image ID="img_UTurn" imageUrl="~/images/icon_UTurn.gif" Visible="false" runat="server" />
                    <asp:Image ID="img_MockShop" ImageUrl="~/images/icon_MockShop.gif" Visible="false" runat="server" />
                    <asp:Image ID="img_PressSample" ImageUrl="~/images/icon_PressSample.gif" Visible="false" runat="server" />
                    <asp:Image ID="img_StudioSample" ImageUrl="~/images/icon_StudioSample.gif" Visible="false" runat="server" />
                    <asp:Image ID="img_LDP" ImageUrl="~/images/icon_LDP.gif" Visible="false" runat="server" />
                    <asp:Image ID="img_WithQCCharge" ImageUrl="~/images/icon_QCCharge.gif" Visible="false" runat="server"/>
                    <asp:Image ID="img_AirShipment" ImageUrl="~/images/icon_air.gif" Visible="false" ToolTip="Air Freight" runat="server"/>
                    <asp:Image ID="img_AirFreightWithPay" ImageUrl="~/images/icon_air_Purple.gif"  Visible="false" ToolTip="Air Freight(Payment% > 0)" runat="server" />
                    <asp:Image ID="img_TradingAirFreight" ImageUrl="~/images/icon_air_Red.gif"  Visible="false" ToolTip="Trading Air Freight" runat="server" />
                    <asp:Image ID="img_ReprocessGoods" ImageUrl="~/images/icon_ReprocessGoods.gif" Visible="false" runat="server"/>
                    <asp:Image ID="img_GBTestRequired" ImageUrl="~/images/icon_Test.png" ToolTip="GB Test Required" Visible="false" runat="server" />
                    <asp:Image ID="img_GBTestPassed" ImageUrl="~/images/icon_Pass.png" ToolTip="GB Test Pass" Visible="false" runat="server" />
                    <asp:Image ID="img_GBTestFailedRelease" ImageUrl="~/images/icon_Fail.png" ToolTip="GB Test Fail (Release Payment)" Visible="false" runat="server" />
                    <asp:Image ID="img_GBTestFailedHold" ImageUrl="~/images/icon_Fail_orange.png" Visible="false" ToolTip="GB Test Fail (Hold Payment)" runat="server" />
                    <asp:Image ID="img_GBTestFailedCannotRelease" ImageUrl="~/images/icon_Fail_red.png" Visible="false" ToolTip="GB Test Fail (Cannot Release Payment)" runat="server" />
                    <asp:Image ID="img_QCCInspection" ImageUrl="~/images/icon_QCCInspection.png" Visible="false" ToolTip="Cambodia QC Center Order" runat="server" />
                </ItemTemplate>
                <ItemStyle Width="100px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Prod Team" >
                <ItemTemplate >
                    <asp:Label ID="lbl_ProductTeam" Text='<%# Eval("ProductTeam.Code") %>' runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Supplier" SortExpression="Supplier" HeaderStyle-ForeColor="CornflowerBlue">
                <ItemTemplate >
                    <asp:Label ID="lbl_Vendor" Text='<%# Eval("Vendor.Name") %>' runat="server" />
                </ItemTemplate>
                <ControlStyle Width="120px" />
            </asp:TemplateField>            
            <asp:TemplateField HeaderText="Item No." SortExpression="ItemNo" HeaderStyle-ForeColor="CornflowerBlue">
                <ItemTemplate>
                    <asp:Label ID="lbl_ItemNo" Text='<%# Eval("ItemNo") %>' runat="server" />
                </ItemTemplate>
                <ControlStyle Width="40px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Customer Dly Date" SortExpression="CustomerDlyDate" HeaderStyle-ForeColor="CornflowerBlue">
                <ItemTemplate>
                    <asp:Label ID="lbl_AtWHDate" Text='<%# Eval("CustomerAgreedAtWarehouseDate", "{0:d}") %>' runat="server" />
                </ItemTemplate>                 
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Final Dest.">
                <ItemTemplate >
                    <asp:Label ID="lbl_Destination" runat="server" Text='<%# Eval("CustomerDestination.DestinationCode") %>' />
                </ItemTemplate>                
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Customer">
                <ItemTemplate>
                    <asp:Label ID="lbl_Customer" runat="server" Text='<%# Eval("Customer.CustomerCode") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="PO Qty">
                <ItemTemplate>
                    <asp:Label ID="lbl_POQty" Text='<%# Eval("TotalPOQuantity","{0:#,##0}") %>' runat="server" />
                </ItemTemplate>                
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Shipped Qty">
                <ItemTemplate>
                    <asp:Label ID="lbl_ShippedQty" Text='<%# Eval("TotalShippedQuantity","{0:#,##0}") %>' runat="server" />
                </ItemTemplate>                
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Ccy">
                <ItemTemplate>
                    <asp:Label ID="lbl_Currency2" Text='<%# Eval("SellCurrency.CurrencyCode") %>' runat="server" />
                </ItemTemplate>
                <ControlStyle Width="20px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Invoice Amt">
                <ItemTemplate>
                    <asp:Label ID="lbl_InvoicedAmt" Text='<%# Eval("InvoiceAmount","{0:#,##0.00}") %>' runat="server" />
               </ItemTemplate>
               <ItemStyle HorizontalAlign="Right" />               
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Invoice No." SortExpression="InvoiceNo" HeaderStyle-ForeColor="CornflowerBlue">
                <ItemTemplate>
                    <asp:Label ID="lbl_InvoiceNo" Text='<%# Eval("InvoiceNo") %>' runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Invoice Date">
                <ItemTemplate>
                    <asp:Label ID="lbl_InvoiceDate" Text='<%# Eval("InvoiceDate", "{0:d}") %>' runat="server" />
                </ItemTemplate>                
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CTS">
                <ItemTemplate >
                    <asp:Label ID="lbl_CTS" Text='<%# Convert.ToBoolean(Eval("isConfirmedToShip")) ? "YES" : "NO" %>' runat="server" />
                </ItemTemplate>
                <ControlStyle Width="20px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="No. of Split">
                <ItemTemplate >
                        <asp:Label ID="lbl_NoOfSplit" runat="server" Text='<%# Eval("SplitCount") %>' />
               </ItemTemplate>
                <HeaderStyle Width="30px" />
                <ControlStyle Width="20px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <asp:Label ID="lbl_Status" Text='<%# Eval("WorkflowStatus.Name") %>' runat="server" />
                </ItemTemplate>
                <ControlStyle Width="100px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Shipping User" SortExpression="ShippingUser" HeaderStyle-ForeColor="CornflowerBlue">
                <ItemTemplate>
                    <asp:Label ID="lbl_ShipUser" Text='<%# Eval("ShippingUser.DisplayName") %>' runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns> 
        <EmptyDataTemplate>
            <span style="font-family: Arial; font-size: medium; font-weight: bold; background-color: #FFFFCC;">No  record found.</span>
        </EmptyDataTemplate>
    </asp:GridView>
    </asp:Panel>

</asp:Content>

