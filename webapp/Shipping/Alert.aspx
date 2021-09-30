<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="Alert.aspx.cs" Inherits="com.next.isam.webapp.shipping.Alert" Title="Alert and Notification" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_workplace_alert.gif" runat="server" id="imgHeaderText" alt="" />
-->
<asp:Panel ID="panel1" runat="server" SkinID="sectionheader_MyWorkplace">Alert & Notification</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 
 
    <script type="text/javascript">
        var Delimiter = "|";
        
        function openAttachments(o, sid) {
            window.open('../account/AttachmentList.aspx?shipmentId=' + sid, 'attachmentlist', 'scrollbars=1,status=0,width=400,height=300');
        }

        function changeDisplayStyle(sTagName, sState) {
            var obj;
            var ExpandIcon;
            var CollapseIcon;

            obj = document.getElementById("tr_" + sTagName);
            if (obj != null) {
                if (sState == "")
                    if (obj.style.display == "none")
                        obj.style.display = "block";
                    else
                        obj.style.display = "none";
                else
                    obj.style.display = sState;
            }

            CollapseIcon = document.getElementById("img_" + sTagName + "_Collapse");
            ExpandIcon = document.getElementById("img_" + sTagName + "_Expand");
            if (CollapseIcon != null && ExpandIcon != null) {
                if (obj.style.display == "none") {
                    CollapseIcon.style.display = "none";
                    ExpandIcon.style.display = "block";
                }
                else {
                    CollapseIcon.style.display = "block";
                    ExpandIcon.style.display = "none";
                }
            }
            return obj.style.display;
        }


        function loadDataGrid(AlertName) {
            var ShowList;
            changeDisplayStyle(AlertName, "");

            // Keep the display status of the alert list, the display status will be restore while processing the window ONLOAD event
            ShowList = "";
            if (tr_InvUploadFail.style.display != "none") ShowList += Delimiter + "InvUploadFail";
            if (tr_MissingAdvBankVendor.style.display != "none") ShowList += Delimiter + "MissingAdvBankVendor";
            if (tr_UTOrderOSInv.style.display != "none") ShowList += Delimiter + "UTOrderOSInv";
            if (tr_OSBooking.style.display != "none") ShowList += Delimiter + "OSBooking";
            if (tr_OSShipDoc.style.display != "none") ShowList += Delimiter + "OSShipDoc";
            if (tr_OSShipDocOffshore.style.display != "none") ShowList += Delimiter + "OSShipDocOffshore";
            if (tr_OSResubmitDoc.style.display != "none") ShowList += Delimiter + "OSResubmitDoc";
            //if (tr_MissingSunAccCode.style.display != "none") ShowList += Delimiter + "MissingSunAccCode";
            if (tr_MissingEpicorSupplierId.style.display != "none") ShowList += Delimiter + "MissingEpicorSupplierId";
            if (tr_MissingPayAdvEMail.style.display != "none") ShowList += Delimiter + "MissingPayAdvEMail";
            if (tr_EziBuyOSPaymentList.style.display != "none") ShowList += Delimiter + "EziBuyOSPaymentList";
            if (tr_OSNTInvDeptApv.style.display != "none") ShowList += Delimiter + "OSNTInvDeptApv";
            if (tr_OSNTInvAccLvl1Apv.style.display != "none") ShowList += Delimiter + "OSNTInvAccLvl1Apv";
            if (tr_OSNTInvAccLvl2Apv.style.display != "none") ShowList += Delimiter + "OSNTInvAccLvl2Apv";
            if (tr_NewNTVendorApproval.style.display != "none") ShowList += Delimiter + "NewNTVendorApproval";
            if (tr_NTVendorAmendment.style.display != "none") ShowList += Delimiter + "NTVendorAmendment";

            document.getElementById("ctl00_ContentPlaceHolder1_txt_AlertList").value = ShowList;
            if (document.getElementById("ctl00_ContentPlaceHolder1_lb_" + AlertName) != null)
            // Load the data into the grid
                __doPostBack("ctl00$ContentPlaceHolder1$lb_" + AlertName, "");

            return true;
        }


        function emailUserToFollowUp(contractDelivery, invoiceNo, message) {
            window.location = "mailto:?subject=Pending Invoice " + invoiceNo
                + "   (Shipment : " + contractDelivery + ")"
                + "&body=" + message;

        }

        function assignTextValue(fromObj, targetObjId, textValue) {
            var prefix;
            var segment;
            var target;
            var targetId;

            segment = fromObj.name.split("$");
            targetId = segment[segment.length - 1];
            prefix = fromObj.id.replace(targetId, "");
            targetId = prefix + targetId;
            target = document.getElementById(targetId);
            target.value = textValue;
            alert(target.value);
        }

        function Say(Something) {
            alert("Javascript : \n\r" + Something);
        }


        function tr_InvUploadFail_onload() {
            alert("tr_InvUploadFail ONLOAD");
        }


        function window_onload() {
            alert("Window ONLOAD");
        }

        function SetFilterArea(status) {
            if (status == null || status == "") {
                if (document.all.tr_Filter.style.display == "none") {
                    document.all.tr_Filter.style.display = "block";
                    document.all.btn_SetFilter.innerText = "Hide Data Filter";
                }
                else {
                    document.all.tr_Filter.style.display = "none";
                    document.all.btn_SetFilter.innerText = "Show Data Filter";
                }
            }
            else {
                if (status.toString().toUpperCase() == "ON") {
                    document.all.tr_Filter.style.display = "block";
                    document.all.btn_SetFilter.innerText = "Hide Data Filter";
                }
                else {
                    if (status.toString().toUpperCase() == "OFF") {
                        document.all.tr_Filter.style.display = "none";
                        document.all.btn_SetFilter.innerText = "Show Data Filter";
                    }
                    else
                        document.all.tr_Filter.style.display = status;
                }
            }
        }
        
    </script>
    
    
    <script type="text/vbscript">
    function SetGridStatus_old(tagName, status)
    
    if document.all.item("ctl00_ContentPlaceHolder1_tr_" & tagName & "_Header") is Nothing then
        setGridStatus = ""
    else    
        if status="none" then
            document.all.item("tr_" & tagName).style.display="none"
            document.all.item("img_" & tagName & "_Expand").style.display = "block"
            document.all.item("img_" & tagName & "_Collapse").style.display = "none"
            setGridStatus = ""
        else    
            document.all.item("tr_" & tagName).style.display="block"
            document.all.item("img_" & tagName & "_Expand").style.display = "none"
            document.all.item("img_" & tagName & "_Collapse").style.display = "block"
            setGridStatus = Delimiter & tagName
        end if
    end if    
    
    end function

    
    function SetGridStatus(tagName)
        if document.all.item("ctl00_ContentPlaceHolder1_tr_" & tagName & "_Header") is Nothing then
            setGridStatus = ""
        else
            AlertList = document.getElementById("ctl00_ContentPlaceHolder1_txt_AlertList").value
            if instr(AlertList & Delimiter, tagName & Delimiter)=0 then
                document.all.item("tr_" & tagName).style.display="none"
                document.all.item("img_" & tagName & "_Expand").style.display = "block"
                document.all.item("img_" & tagName & "_Collapse").style.display = "none"
                setGridStatus = ""
            else    
                document.all.item("tr_" & tagName).style.display="block"
                document.all.item("img_" & tagName & "_Expand").style.display = "none"
                document.all.item("img_" & tagName & "_Collapse").style.display = "block"
                setGridStatus = Delimiter & tagName
            end if
        end if    
    
    end function


    Sub Window_OnLoad()
        AlertList = document.getElementById("ctl00_ContentPlaceHolder1_txt_AlertList").value
        ShowList = ""
        ShowList = ShowList + SetGridStatus ("InvUploadFail")
        ShowList = ShowList + SetGridStatus ("MissingAdvBankVendor")
        ShowList = ShowList + SetGridStatus ("UTOrderOSInv")
        ShowList = ShowList + SetGridStatus ("OSBooking")
        ShowList = ShowList + SetGridStatus ("OSShipDoc")
        ShowList = ShowList + SetGridStatus ("OSShipDocOffshore")
        ShowList = ShowList + SetGridStatus ("OSResubmitDoc")
        ShowList = ShowList + SetGridStatus ("MissingSunAccCode")
        ShowList = ShowList + SetGridStatus ("MissingEpicorSupplierId")
        ShowList = ShowList + SetGridStatus ("MissingPayAdvEMail")
        ShowList = ShowList + SetGridStatus ("EziBuyOSPaymentList")
        ShowList = ShowList + SetGridStatus ("OSNTInvDeptApv")
        ShowList = ShowList + SetGridStatus ("OSNTInvAccLvl1Apv")
        ShowList = ShowList + SetGridStatus ("OSNTInvAccLvl2Apv")
        ShowList = ShowList + SetGridStatus ("NewNTVendorApproval")
        ShowList = ShowList + SetGridStatus ("NTVendorAmendment")
        ShowList = ShowList + SetGridStatus ("NTInvAccApv")
        document.getElementById("ctl00_ContentPlaceHolder1_txt_AlertList").value = ShowList
    End Sub


    function Say(Something)
        msgbox Something,,"VBScript"
        Say = Something
    end function

    </script>


    <asp:TextBox ID='txt_AlertList' runat='server' Text='' style="width:400;display:none;" Width='300px' ></asp:TextBox>

    <table width="800px" cellpadding="2" cellspacing="0" border='0' >
        <tr>
            <td class="tableHeader" visible='false' style='display:none;' >Alert and Notification</td>
        </tr>
        <tr>
            <td >
                <table width="600px" style="border-width: 1px; border-spacing: 3px; border-style: solid; border-color: green; background-color: white;">
                    <tr >
                        <td style='display:block;' >
                        <a onclick="SetFilterArea();return false;" id="btn_SetFilter" style="font-weight:bold; cursor:pointer;">Show Data Filter</a>
                        </td>
                    </tr>
                    <tr>
                        <td width="100%">
                        <table id='tbl_Filter' width='100%' >
                            <tr id='tr_Filter' style='display:none;'>
                                <td>
                                    <b>Office</b>&nbsp;:&nbsp;<cc2:smartdropdownlist id="ddl_Office" runat="server" Width="100px"></cc2:smartdropdownlist>
                                </td>
                                <td style='display:block;'>
                                    <b>Department</b>&nbsp;:&nbsp;
                                    <cc2:smartdropdownlist id="ddl_Department" runat="server" Width="120px">
                                    <asp:ListItem Value=''>--All--</asp:ListItem>
                                    </cc2:smartdropdownlist>
                                </td>
                                <td style='display:none;'>
                                    <b>Product Team</b>&nbsp;:&nbsp;<cc2:smartdropdownlist id="ddl_ProductTeam" runat="server" Width="120px"></cc2:smartdropdownlist>
                                </td>
                                <td width='100'>&nbsp;</td>
                                
                                <td>
                                <asp:Button ID="btn_Apply" runat="server" Text="Apply" OnClick='btn_Apply_Click'  On_Client_Click='document.all.ctl00_ContentPlaceHolder1_txt_AlertList.value="";' />
                                &nbsp;&nbsp;
                                </td>
                            </tr>
                        </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>    
            
        <asp:TableRow  ID="tr_InvUploadFail_Header" runat="server" Visible="false">
            <asp:TableCell id="td_InvUploadFail" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header3" ID="lb_InvUploadFail" name="lb_InvUploadFail" style="" onclick='lb_InvUploadFail_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_InvUploadFail_Expand' onclick="loadDataGrid('InvUploadFail');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_InvUploadFail_Collapse' onclick="loadDataGrid('InvUploadFail');" style='cursor:hand;display:none;' src="../images/Collapse.jpg"  alt='Hide the detail'/>
                    </td>
                    <td>
                        <a class ="header2" id="sp_InvUploadFail" onclick="loadDataGrid('InvUploadFail');" >Self-Billed Invoice Upload Failure</a>&nbsp;
                        <asp:label ID="lbl_InvUploadFail" class="header3" runat="server" text="" style="border:thin none;font-weight:normal;" ></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_uploadFailedInfo" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP"  />
                    </td>
                    <td>
                        <div id="flyout1" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                        <div id="div_UploadFailInfo" class="animationLayer" style="width:250px; height:80px;">
                            <a onclick="div_UploadFailInfo.style.visibility='hidden';" style="cursor:pointer;float:right ;" ><img src="../images/close.png" alt="" /></a>
                            NUK self-billed invoice cannot be uploaded into ISAM due to mentioned reason. Please contact corresponding merchandiser to follow up.
                        </div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id="tr_InvUploadFail" visible="false" style="display:none;">
            <td>
                <asp:GridView ID="gv_InvUploadFail" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_InvUploadFail_RowDataBound" visible="false"  >            
                    <Columns>
                        <asp:TemplateField HeaderText="Contract No.">
                            <ItemTemplate >
                                <asp:LinkButton ID="lnk_ContractNo" runat="server" Text='' />
    <!--
                                    Post_Back_Url="~/Shipping/ShipmentDetail.aspx?ShipmentId=309182"  />
                                    -->
                                    
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dly No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_DlyNo"   runat="server" Text='' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_InvNo" runat="server" Text='' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice Date">
                            <ItemTemplate>
                                <asp:Label ID="lbl_InvDate" runat="server" Text='' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lbl_WorkflowStatus" runat="server" Text='' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Merchandiser">
                            <ItemTemplate>
                                <asp:Label ID="lbl_MerchName" runat="server" Text=''  style="text-align:left;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fail Reason">
                            <ItemTemplate>
                                <asp:Label ID="lbl_FailReason" runat="server" Text='' style="text-align:left;"  /> 
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        No outstanding invoice.
                    </EmptyDataTemplate>

                </asp:GridView>
            </td>
        </tr>


        <asp:TableRow  ID="tr_MissingAdvBankVendor_Header" runat="server" Visible="false">
           <asp:TableCell id="td_MissingAdvBankVendor" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header3" ID="lb_MissingAdvBankVendor" name="lb_MissingAdvBankVendor"  style="" onclick='lb_MissingAdvBankVendor_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_MissingAdvBankVendor_Expand' onclick="loadDataGrid('MissingAdvBankVendor');"  style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_MissingAdvBankVendor_Collapse' onclick="loadDataGrid('MissingAdvBankVendor');"  style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class="header2" onclick="loadDataGrid('MissingAdvBankVendor');" >Vendor with Missing L/C Advising Bank Information</a>&nbsp;
                        <asp:label ID="lbl_MissingAdvBankVendor" class="header3" runat="server" text="" style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="lnk_Vendor" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_Vendor" class="animationLayer" style="width:250px; height:60px; ">
                            <a onclick="div_Vendor.style.visibility='hidden';" style="cursor:pointer; float:right ;" ><img src="../images/close.png" alt="" /></a>
                            List out supplier with L/C Payment Term shipment but not yet setup Advising Bank.                    
                        </div>
                        <div id="flyout2" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id="tr_MissingAdvBankVendor" style="display:none;">
           <td>
                <asp:GridView ID="gv_MissingAdvBankVendor" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_MissingAdvBankVendor_RowDataBound" Visible="false" >
                    <Columns>
                        <asp:TemplateField HeaderText="Vendor">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Vendor" runat="server" style="text-align:left;"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Contract No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ContractNo" runat="server" visible="false"/>
                            </ItemTemplate>
                            <ItemTemplate >
                                <asp:LinkButton ID="lnk_ContractNo" runat="server" 
                                    PostBackUrl="~/Shipping/ShipmentDetail.aspx?ShipmentId=?"  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Delivery No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_DeliveryNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ItemNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Customer Dly Date">
                            <ItemTemplate>
                                <asp:Label ID="lbl_CustDlyDate" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Status" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Merchandiser">
                            <ItemTemplate>
                                <asp:Label ID="lbl_MerName" runat="server" style="text-align:left;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                    No outstanding vendor.
                    </EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>


        <asp:TableRow  ID="tr_UTOrderOSInv_Header" runat="server" Visible="false">
           <asp:TableCell id="td_UTOrderOSInv" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header3" ID="lb_UTOrderOSInv" name="lb_UTOrderOSInv" style="" onclick='lb_UTOrderOSInv_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_UTOrderOSInv_Expand'  onclick="loadDataGrid('UTOrderOSInv');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_UTOrderOSInv_Collapse'  onclick="loadDataGrid('UTOrderOSInv');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class="header2" onclick="loadDataGrid('UTOrderOSInv');" >UT Order Outstanding to Invoice</a>&nbsp;
                        <asp:label ID="lbl_UTOrderOSInv" class="header3" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_UTOrderInfo" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_UTOrderInfo" class="animationLayer" style="width:250px; height:130px;">
                            <a onclick="div_UTOrderInfo.style.visibility='hidden';" style="cursor:pointer; float:right;" >
                            <img src="../images/close.png" alt="" /></a>
                            This alert list is designed for NSL SZ office to detect shipments which waiting for issue invoice.
                            Shipments will be shown on this list under below conditions.<br />
                            &nbsp; (1)	UT Order;<br />
                            &nbsp; (2)	STW Date is input; <br />
                            &nbsp; (3)	Invoice Date is empty;<br />
                        </div>
                        <div id="flyout3" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_UTOrderOSInv' style="display:none;">
            <td>
                <asp:GridView ID="gv_UTOrderOSInv" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_UTOrderOSInv_RowDataBound" Visible="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Contract No.">
                            <ItemTemplate >
                                <asp:Label ID="lbl_ContractNo" runat="server" visible="false"/>
                            </ItemTemplate>
                            <ItemTemplate >
                                <asp:LinkButton ID="lnk_ContractNo" runat="server" 
                                    PostBackUrl="~/Shipping/ShipmentDetail.aspx?ShipmentId=?"  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dly No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_DlyNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="STW Date">
                            <ItemTemplate>
                                <asp:Label ID="lbl_STWDate" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ItemNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Supplier" runat="server" style="text-align:left;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Status" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                    No outstanding order.
                    </EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>

        
        <asp:TableRow  ID="tr_OSBooking_Header" runat="server" Visible="false">
           <asp:TableCell id="td_OSBooking" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header3" ID="lb_OSBooking" name="lb_OSBooking" style="" onclick='lb_OSBooking_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_OSBooking_Expand'  onclick="loadDataGrid('OSBooking');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_OSBooking_Collapse'  onclick="loadDataGrid('OSBooking');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="sp_OSBooking" onclick="loadDataGrid('OSBooking');" >Outstanding Booking</a>&nbsp;
                        <asp:label ID="lbl_OSBooking" class="header3" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_BookingInfo" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_BookingInfo" class="animationLayer" style="width:250px; height:130px; ">
                            <a onclick="div_BookingInfo.style.visibility='hidden';" style="cursor:pointer;float:right;" >
                            <img src="../images/close.png" alt="" /></a>
                            Outstanding Booking List is an alert list design for China and Thailand region. Shipments will be classified as Outstanding Booking Shipment under below conditions.<br />
                            &nbsp; (1)	Shipment is not INVOICED.; <br />
                            &nbsp; (2)	Customer Dly Date <= Today + 7;<br />
                            &nbsp; (3)	STW Date is empty; OR Booked WH Date is empty + S/O No. is empty; <br />
                        </div>
                        <div id="flyout4" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>       
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_OSBooking' style="display:none;">
            <td>
                <asp:GridView ID="gv_OSBooking" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_OSBooking_RowDataBound" Visible="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Contract No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ContractNo" runat="server" visible="false"/>
                            </ItemTemplate>
                            <ItemTemplate >
                                <asp:LinkButton ID="lnk_ContractNo" runat="server" 
                                    PostBackUrl="~/Shipping/ShipmentDetail.aspx?ShipmentId=?"  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dly No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_DlyNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ItemNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier">
                            <ItemTemplate>
                                <asp:Label ID="lbl_supplier" runat="server" style="text-align:left;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Customer Dly Date">
                            <ItemTemplate >
                                <asp:Label ID="lbl_CustDlyDate" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Status" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Shipping User" >
                            <ItemTemplate>
                                <asp:Label ID="lbl_ShipUser" runat="server" style="text-align:left;"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate >
                        No outstanding booking.
                    </EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>


        <asp:TableRow  ID="tr_OSShipDoc_Header" runat="server" Visible="false">
           <asp:TableCell id="td_OSShipDoc" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header2" ID="lb_OSShipDoc" name="lb_OSShipDoc" style="" onclick='lb_OSShipDoc_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_OSShipDoc_Expand'  onclick="loadDataGrid('OSShipDoc');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_OSShipDoc_Collapse'  onclick="loadDataGrid('OSShipDoc');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="sp_OSShipDoc" onclick="loadDataGrid('OSShipDoc');" >Outstanding Document to be Presented to Accounts</a>&nbsp;
                        <asp:label ID="lbl_OSShipDoc" class="header2" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_DocInfo" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_DocInfo" class="animationLayer" style="width:250px; height:130px;">
                        <a onclick="div_DocInfo.style.visibility='hidden';" style="cursor:pointer; float:right;" ><img src="../images/close.png" alt="" /></a>
                        Outstanding Document to be presented to Accounts; This alert list is designed for China and Thailand Region to detect which supplier invoice has been collected and waiting to present to Accounts<br />
                        &nbsp; (1)	Today – Shipment Document Received Date >= 4;<br />
                        &nbsp; (2)	Account Received Date is empty<br />
                        </div>
                        <div id="flyout5" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_OSShipDoc' style="display:none;">
            <td>
                <asp:GridView ID="gv_OSShipDoc" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_OSShipDoc_RowDataBound" Visible="false" >
                    <Columns>
                        <asp:TemplateField HeaderText="Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Contract No." >
                            <ItemTemplate >
                                <asp:Label ID="lbl_ContractNo" runat="server" visible="false"/>
                            </ItemTemplate>
                            <ItemTemplate >
                                <asp:LinkButton ID="lnk_ContractNo" runat="server" Text=''
                                    PostBackUrl="~/Shipping/ShipmentDetail.aspx?ShipmentId=?"  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dly No." >
                            <ItemTemplate>
                                <asp:Label ID="lbl_DlyNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item No.">
                            <ItemTemplate >
                                <asp:Label ID="lbl_ItemNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier" ItemStyle-HorizontalAlign='Left' >
                            <ItemTemplate>
                                <asp:Label ID="lbl_Supplier" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Date of Document Rcpt" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_DocRcptDate" runat ="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Shipping User"  ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate >
                                <asp:Label ID="lbl_ShipUser" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate >No outstanding document.</EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>

        <asp:TableRow  ID="tr_OSShipDocOffshore_Header" runat="server" Visible="false">
           <asp:TableCell id="td_OSShipDocOffshore" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header2" ID="lb_OSShipDocOffshore" name="lb_OSShipDocOffshore" style="" onclick='lb_OSShipDocOffshore_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_OSShipDocOffshore_Expand'  onclick="loadDataGrid('OSShipDocOffshore');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_OSShipDocOffshore_Collapse'  onclick="loadDataGrid('OSShipDocOffshore');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="sp_OSShipDocOffshore" onclick="loadDataGrid('OSShipDocOffshore');" >Outstanding Document to be Presented to Accounts (Offshore)</a>&nbsp;
                        <asp:label ID="lbl_OSShipDocOffshore" class="header2" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_DocOffshoreInfo" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_DocOffshoreInfo" class="animationLayer" style="width:250px; height:130px;">
                        <a onclick="div_DocOffshoreInfo.style.visibility='hidden';" style="cursor:pointer; float:right;" ><img src="../images/close.png" alt="" /></a>
                        Outstanding Document to be Presented to Accounts (Offshore); This alert list is designed for LK, IN, ND Office to detect which supplier invoice has been collected and waiting to present to Accounts<br />
                        </div>
                        <div id="flyout9" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_OSShipDocOffshore' style="display:none;">
            <td>
                <asp:GridView ID="gv_OSShipDocOffshore" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_OSShipDocOffshore_RowDataBound" Visible="false" >
                    <Columns>
                        <asp:TemplateField HeaderText="Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Contract No." >
                            <ItemTemplate >
                                <asp:Label ID="lbl_ContractNo" runat="server" visible="false"/>
                            </ItemTemplate>
                            <ItemTemplate >
                                <asp:LinkButton ID="lnk_ContractNo" runat="server" PostBackUrl="~/Shipping/ShipmentDetail.aspx?ShipmentId=?"  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dly No." >
                            <ItemTemplate>
                                <asp:Label ID="lbl_DlyNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item No.">
                            <ItemTemplate >
                                <asp:Label ID="lbl_ItemNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice No.">
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice Date">
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceDate" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier" ItemStyle-HorizontalAlign="Left" >
                            <ItemTemplate>
                                <asp:Label ID="lbl_Supplier" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Inv. No.">
                            <ItemTemplate >
                                <asp:Label ID="lbl_SupplierInvoiceNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Date of Document Rcpt" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_DocRcptDate" runat ="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Shipping User"  ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate >
                                <asp:Label ID="lbl_ShipUser" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="DMS Doc." >
                            <ItemTemplate >
                                <asp:Image ID="img_Doc" runat ="server" ImageUrl="~/images/icon_edit.gif" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Follow-Up" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate >
                                <asp:Label ID="lbl_FollowUp" runat ="server" />
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                    <EmptyDataTemplate >No outstanding document.</EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>


        <asp:TableRow  ID="tr_OSResubmitDoc_Header" runat="server" Visible="false">
           <asp:TableCell id="td_OSResubmitDoc" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header2" ID="lb_OSResubmitDoc" name="lb_OSResubmitDoc" style="" onclick='lb_OSResubmitDoc_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_OSResubmitDoc_Expand'  onclick="loadDataGrid('OSResubmitDoc');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_OSResubmitDoc_Collapse'  onclick="loadDataGrid('OSResubmitDoc');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="sp_OSResubmitDoc" onclick="loadDataGrid('OSResubmitDoc');" >Outstanding Document to be Re-submitted to Accounts</a>&nbsp;
                        <asp:label ID="lbl_OSResubmitDoc" class="header2" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_ResubmitInfo" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_ResubmitInfo" class="animationLayer" style="width:250px; height:130px;">
                        <a onclick="div_ResubmitInfo.style.visibility='hidden';" style="cursor:pointer; float:right;" ><img src="../images/close.png" alt="" /></a>
                        Accounts rejected shipping uploaded document (including NS Invoice, Supplier Invoice, etc) will be reflected on below Alert and Notification List
                        in order to remind HK Shipping to re-submit the invoice and supplier invoice to Accounts follow-up payment.<br />
                        </div>
                        <div id="flyout8" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_OSResubmitDoc' style="display:none;">
            <td>
                <asp:GridView ID="gv_OSResubmitDoc" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_OSResubmitDoc_RowDataBound" Visible="false" >
                    <Columns>
                        <asp:TemplateField HeaderText="Office ">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier" ItemStyle-HorizontalAlign="Left" >
                            <ItemTemplate>
                                <asp:Label ID="lbl_Supplier" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Contract No. " >
                            <ItemTemplate >
                                <asp:Label ID="lbl_ContractNo" runat="server" />
                            </ItemTemplate>
                            <ItemTemplate >
                                <asp:LinkButton ID="lnk_ContractNo" runat="server" PostBackUrl="~/Shipping/ShipmentDetail.aspx?ShipmentId=?"  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dly<br>No." >
                            <ItemTemplate>
                                <asp:Label ID="lbl_DlyNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item No.">
                            <ItemTemplate >
                                <asp:Label ID="lbl_ItemNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice No ">
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Invoice No "  ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate >
                                <asp:Label ID="lbl_SupInvNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Reject Reason"  ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate >
                                <asp:Label ID="lbl_RejectReason" runat="server" Width="150" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate >No outstanding document.</EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>


        <asp:TableRow  ID="tr_MissingSunAccCode_Header" runat="server" Visible="false">
           <asp:TableCell id="td_MissingSunAccCode" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header3" ID="lb_MissingSunAccCode" name="lb_MissingSunAccCode" style="" onclick='lb_MissingSunAccCode_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_MissingSunAccCode_Expand'  onclick="loadDataGrid('MissingSunAccCode');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_MissingSunAccCode_Collapse'  onclick="loadDataGrid('MissingSunAccCode');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="sp_MissingSunAccCode" onclick="loadDataGrid('MissingSunAccCode');" >Supplier Missing SUN Account Code</a>&nbsp;
                        <asp:label ID="lbl_MissingSunAccCode" class="header3" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_MissingSunAccCode" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_MissingSunAccCode" class="animationLayer" style="width:250px; height:130px;">
                             <a onclick="div_MissingSunAccCode.style.visibility='hidden';" style="cursor:pointer; float:right;" ><img src="../images/close.png" alt="" /></a>
                            Supplier Missing SUN Account Code;<br />
                            This alert list is designed for Accounts users to detect which supplier has NOT been assigned SUN Account Code for interfacing.<br />
                            &nbsp; (1)	Supplier SUN Account Code;<br />
                            &nbsp; (2)	Supplier with invoiced shipment.
                        </div>
                        <div id="flyout6" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_MissingSunAccCode' style="display:none;">
            <td>
                <asp:GridView ID="gv_MissingSunAccCode" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_MissingSunAccCode_RowDataBound" Visible="false" >
                    <Columns>
                        <asp:TemplateField HeaderText="Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Purchase Term" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_PurchaseTerm" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Supplier" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate >No outstanding supplier.</EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>    

        <asp:TableRow  ID="tr_MissingEpicorSupplierId_Header" runat="server" Visible="false">
           <asp:TableCell id="td_MissingEpicorSupplierId" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header3" ID="lb_MissingEpicorSupplierId" name="lb_MissingEpicorSupplierId" style="" onclick='lb_MissingEpicorSupplierId_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_MissingEpicorSupplierId_Expand'  onclick="loadDataGrid('MissingEpicorSupplierId');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_MissingEpicorSupplierId_Collapse'  onclick="loadDataGrid('MissingEpicorSupplierId');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="sp_MissingEpicorSupplierId" onclick="loadDataGrid('MissingEpicorSupplierId');" >Supplier Missing Profile in Epicor</a>&nbsp;
                        <asp:label ID="lbl_MissingEpicorSupplierId" class="header3" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_MissingEpicorSupplierId" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_MissingEpicorSupplierId" class="animationLayer" style="width:250px; height:130px;">
                             <a onclick="div_MissingEpicorSupplierId.style.visibility='hidden';" style="cursor:pointer; float:right;" ><img src="../images/close.png" alt="" /></a>
                            Supplier Missing Profile in Epicor<br />
                            This alert list is design for Accounts users to detect which supplier profile has NOT been created profile in Epicor system under below conditions.<br />
                                    &nbsp;(1)	Supplier with invoiced shipment; OR has scheduled shipment within 30 days.<br />
                                    &nbsp;(2)	VPS profile’s “Supplier Profile has been created in Epicor” checkbox is NOT tick.
                        </div>
                        <div id="flyout16" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_MissingEpicorSupplierId' style="display:none;">
            <td>
                <asp:GridView ID="gv_MissingEpicorSupplierId" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_MissingEpicorSupplierId_RowDataBound" Visible="false" >
                    <Columns>
                        <asp:TemplateField HeaderText="Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Purchase<br> Term" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_PurchaseTerm" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Supplier" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Epicor <br>Supplier ID">
                            <ItemTemplate>
                                <asp:Label ID="lbl_SupplierId" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate >No outstanding supplier.</EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>    


        <asp:TableRow  ID="tr_MissingPayAdvEMail_Header" runat="server" Visible="false">
           <asp:TableCell id="td_MissingPayAdvEMail" runat="server">
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header3" ID="lb_MissingPayAdvEMail" name="lb_MissingPayAdvEMail" style="" onclick='lb_MissingPayAdvEMail_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id="img_MissingPayAdvEMail_Expand"  onclick="loadDataGrid('MissingPayAdvEMail');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id="img_MissingPayAdvEMail_Collapse"  onclick="loadDataGrid('MissingPayAdvEMail');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="sp_MissingPayAdvEMail" onclick="loadDataGrid('MissingPayAdvEMail');" >Supplier Missing Payment Advice E-mail Address</a>&nbsp;
                        <asp:label ID="lbl_MissingPayAdvEMail" class="header3" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_MissingPayAdvEMail" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_MissingPayAdvEMail" class="animationLayer" style="width:250px; height:130px;">
                             <a onclick="div_MissingPayAdvEMail.style.visibility='hidden';" style="cursor:pointer; float:right;" ><img src="../images/close.png" alt="" /></a>
                            Supplier Missing Payment Advice E-mail Address;<br />
                            This alert list is designed for Accounts users to detect which supplier has NOT been assigned Payment Advice eMail Address for sending payment advice.<br />
                            &nbsp; (1)	Supplier Payment Advice eMail Address is empty;<br />
                            &nbsp; (2)	Supplier with invoiced shipment.
                        </div>
                        <div id="flyout7" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_MissingPayAdvEMail' style="display:none;">
            <td>
                <asp:GridView ID="gv_MissingPayAdvEMail" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_MissingPayAdvEMail_RowDataBound" Visible="false" >
                    <Columns>
                        <asp:TemplateField HeaderText="Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier"  ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Supplier" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate >No outstanding supplier.</EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>    


        <asp:TableRow  ID="tr_EziBuyOSPaymentList_Header" runat="server" Visible="false">
           <asp:TableCell id="td_EziBuyOSPaymentList" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header2" ID="lb_EziBuyOSPaymentList" name="lb_EziBuyOSPaymentList" style="" onclick='lb_EziBuyOSPaymentList_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_EziBuyOSPaymentList_Expand'  onclick="loadDataGrid('EziBuyOSPaymentList');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_EziBuyOSPaymentList_Collapse'  onclick="loadDataGrid('EziBuyOSPaymentList');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="sp_EziBuyOSPaymentList" onclick="loadDataGrid('EziBuyOSPaymentList');" >EziBuy - Outstanding Payment and Cargo Delivery List</a>&nbsp;
                        <asp:label ID="lbl_EziBuyOSPaymentList" class="header2" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                        &nbsp;&nbsp;
                        <asp:ImageButton ID="btn_EziBuyOSPaymentListInExcel" runat="server" OnClick="btn_EziBuyOSPaymentListInExcel_click" title='Export in EXCEL format' ImageUrl="~/images/Icon_Excel.jpg" />
                    </td>
                    <td>&nbsp;&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_EziBuyOSPaymentList" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_EziBuyOSPaymentList" class="animationLayer" style="width:250px; height:140px;">
                        <a onclick="div_EziBuyOSPaymentList.style.visibility='hidden';" style="cursor:pointer; float:right;" ><img src="../images/close.png" alt="" /></a>
                        This alert list is designed for customer EziBuy to detect the outstanding shipments. 
                            Shipments will be shown on this list under below conditions.
                        <br />
                        &nbsp; (1)	EziBuy Order;<br />
                        &nbsp; (2)	Shipment is INVOICED<br />
                        &nbsp; (2)	Actual In-Warehouse Date or Sales Settlement Date is empty<br />
                        </div>
                        <div id="flyout8" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_EziBuyOSPaymentList' style="display:none;">
            <td>
                <asp:GridView ID="gv_EziBuyOSPaymentList" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_EziBuyOSPaymentList_RowDataBound" Visible="false" >
                    <Columns>
                        <asp:TemplateField HeaderText="NSL<br>Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Inbound<br>Dly No." >
                            <ItemTemplate>
                                <asp:Label ID="lbl_InboundDlyNo" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item No.">
                            <ItemTemplate >
                                <asp:Label ID="lbl_ItemNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Customer<br>Dly Date" >
                            <ItemTemplate>
                                <asp:Label ID="lbl_CustomerDlydate" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="NSL Invoice No." >
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceNo" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Inv. Amount <br>in USD"  ItemStyle-HorizontalAlign="right">
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceAmount" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>    
                        <asp:TemplateField HeaderText="Invoice Date">
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceDate" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice<br>Sent Date" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceSentDate" runat ="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actual<br>In-WH Date" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_ActualInWHDate" runat ="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sales <br>Settlement Date">
                            <ItemTemplate >
                                <asp:Label ID="lbl_SettlementDate" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Booked <br>In-WH Date">
                            <ItemTemplate >
                                <asp:Label ID="lbl_BookInWHDate" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Gold Seal <br>Approval Date">
                            <ItemTemplate >
                                <asp:Label ID="lbl_GoldSealApprovalDate" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate >No outstanding document.</EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>


        <asp:TableRow  ID="tr_OSNTInvDeptApv_Header" runat="server" Visible="false">
           <asp:TableCell id="td_OSNTInvDeptApvList" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header2" ID="lb_OSNTInvDeptApv" name="lb_OSNTInvDeptApv" style="" onclick='lb_OSNTInvDeptApv_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_OSNTInvDeptApv_Expand'  onclick="loadDataGrid('OSNTInvDeptApv');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_OSNTInvDeptApv_Collapse'  onclick="loadDataGrid('OSNTInvDeptApv');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="sp_OSNTInvDeptApv" onclick="loadDataGrid('OSNTInvDeptApv');" >Outstanding Non-Trade Expense Invoice Pending for Department Approval</a>&nbsp;
                        <asp:label ID="lbl_OSNTInvDeptApv" class="header2" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_OSNTInvDeptApvInfo" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_OSNTInvDeptApv" class="animationLayer" style="width:250px; height:130px;">
                        <a onclick="div_OSNTInvDeptApv.style.visibility='hidden';" style="cursor:pointer; float:right;" ><img src="../images/close.png" alt="" /></a>
                        It will show Non-Trade Expense Invoice with below status<br />
                        &nbsp; (1)	Pending for Approval : Waiting for Department approver for approval;<br />
                        </div>
                        <div id="flyout10" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_OSNTInvDeptApv' style="display:none;">
            <td>
                <asp:GridView ID="gv_OSNTInvDeptApv" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_OSNTInvDeptApv_RowDataBound" Visible="false" >
                    <Columns>
                        <asp:TemplateField HeaderText="Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Invoice No. / Account No.">
                            <ItemTemplate >
                                <asp:LinkButton ID="lnk_InvoiceAccountNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice Date" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceDate" runat ="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vendor" ItemStyle-HorizontalAlign='Left' >
                            <ItemTemplate>
                                <asp:Label ID="lbl_Vendor" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Due Date" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_DueDate" runat ="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Currency">
                            <ItemTemplate >
                                <asp:Label ID="lbl_Currency" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                       <asp:TemplateField HeaderText="Amount"  ItemStyle-HorizontalAlign="right">
                            <ItemTemplate >
                                <asp:Label ID="lbl_Amount" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>    
                        <asp:TemplateField HeaderText="Approver" ItemStyle-HorizontalAlign='Left' >
                            <ItemTemplate>
                                <asp:Label ID="lbl_Approver" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice Status"  ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceStatus" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate >No outstanding Invoice.</EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>


        <asp:TableRow  ID="tr_OSNTInvAccLvl1Apv_Header" runat="server" Visible="false">
           <asp:TableCell id="td_OSNTInvAccLvl1ApvList" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header2" ID="lb_OSNTInvAccLvl1Apv" name="lb_OSNTInvAccLvl1Apv" style="" onclick='lb_OSNTInvAccLvl1Apv_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_OSNTInvAccLvl1Apv_Expand'  onclick="loadDataGrid('OSNTInvAccLvl1Apv');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_OSNTInvAccLvl1Apv_Collapse'  onclick="loadDataGrid('OSNTInvAccLvl1Apv');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="sp_OSNTInvAccLvl1Apv" onclick="loadDataGrid('OSNTInvAccLvl1Apv');" >Outstanding Non-Trade Expense Invoice Pending for Accounts 1st Level Approval</a>&nbsp;
                        <asp:label ID="lbl_OSNTInvAccLvl1Apv" class="header2" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_OSNTInvAccLvl1ApvInfo" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_OSNTInvAccLvl1Apv" class="animationLayer" style="width:250px; height:130px;">
                        <a onclick="div_OSNTInvAccLvl1Apv.style.visibility='hidden';" style="cursor:pointer; float:right;" ><img src="../images/close.png" alt="" /></a>
                        It will show Non-Trade Expense Invoice with below status<br />
                        &nbsp; (1)	Department Approved	: Department head approved and waiting for Accounts;<br />
                        &nbsp; (2)	Accounts Evaluating	: Invoice returned by 2nd level Accounts Approver to 1st level Accounts Approver follow-up.<br />
                        </div>
                        <div id="flyout11" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_OSNTInvAccLvl1Apv' style="display:none;">
            <td>
                <asp:GridView ID="gv_OSNTInvAccLvl1Apv" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_OSNTInvAccLvl1Apv_RowDataBound" Visible="false" >
                    <Columns>
                        <asp:TemplateField HeaderText="Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Invoice No. /<br> Account No.">
                            <ItemTemplate >
                                 <asp:LinkButton ID="lnk_InvoiceAccountNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice Date" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceDate" runat ="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vendor" ItemStyle-HorizontalAlign='Left' >
                            <ItemTemplate>
                                <asp:Label ID="lbl_Vendor" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Due Date" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_DueDate" runat ="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Currency">
                            <ItemTemplate >
                                <asp:Label ID="lbl_Currency" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                       <asp:TemplateField HeaderText="Amount"  ItemStyle-HorizontalAlign="right">
                            <ItemTemplate >
                                <asp:Label ID="lbl_Amount" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>    
                        <asp:TemplateField HeaderText="Invoice Status"  ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceStatus" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate >No outstanding Invoice.</EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>

    
        <asp:TableRow  ID="tr_OSNTInvAccLvl2Apv_Header" runat="server" Visible="false">
           <asp:TableCell id="td_OSNTInvAccLvl2ApvList" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header2" ID="lb_OSNTInvAccLvl2Apv" name="lb_OSNTInvAccLvl2Apv" style="" onclick='lb_OSNTInvAccLvl2Apv_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_OSNTInvAccLvl2Apv_Expand'  onclick="loadDataGrid('OSNTInvAccLvl2Apv');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_OSNTInvAccLvl2Apv_Collapse'  onclick="loadDataGrid('OSNTInvAccLvl2Apv');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="sp_OSNTInvAccLvl2Apv" onclick="loadDataGrid('OSNTInvAccLvl2Apv');" >Outstanding Non-Trade Expense Invoice Pending for Accounts 2nd Level Approval</a>&nbsp;
                        <asp:label ID="lbl_OSNTInvAccLvl2Apv" class="header2" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_OSNTInvAccLvl2ApvInfo" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_OSNTInvAccLvl2Apv" class="animationLayer" style="width:250px; height:130px;">
                        <a onclick="div_OSNTInvAccLvl2Apv.style.visibility='hidden';" style="cursor:pointer; float:right;" ><img src="../images/close.png" alt="" /></a>
                        It will show Non-Trade Expense Invoice with below status<br />
                        &nbsp; (1)	Accounts Received		: 1st level Accounts Approvers approved invoice<br />
                        </div>
                        <div id="flyout12" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_OSNTInvAccLvl2Apv' style="display:none;">
            <td>
                <asp:GridView ID="gv_OSNTInvAccLvl2Apv" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_OSNTInvAccLvl2Apv_RowDataBound" Visible="false" >
                    <Columns>
                        <asp:TemplateField HeaderText="Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Invoice No. /<br> Account No.">
                            <ItemTemplate >
                                 <asp:LinkButton ID="lnk_InvoiceAccountNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice Date" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceDate" runat ="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vendor" ItemStyle-HorizontalAlign='Left' >
                            <ItemTemplate>
                                <asp:Label ID="lbl_Vendor" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Due Date" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_DueDate" runat ="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Currency">
                            <ItemTemplate >
                                <asp:Label ID="lbl_Currency" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                       <asp:TemplateField HeaderText="Amount"  ItemStyle-HorizontalAlign="right">
                            <ItemTemplate >
                                <asp:Label ID="lbl_Amount" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>    
                        <asp:TemplateField HeaderText="Invoice Status"  ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceStatus" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate >No outstanding Invoice.</EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>

        
        <asp:TableRow  ID="tr_NTInvAccApv_Header" runat="server" Visible="false">
           <asp:TableCell id="td_NTInvAccApvList" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header2" ID="lb_NTInvAccApv" name="lb_NTInvAccApv" style="" onclick='lb_NTInvAccApv_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_NTInvAccApv_Expand'  onclick="loadDataGrid('NTInvAccApv');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_NTInvAccApv_Collapse'  onclick="loadDataGrid('NTInvAccApv');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="A2" onclick="loadDataGrid('NTInvAccApv');" >Non-Trade Expense Invoice Pending for Interface Generation</a>&nbsp;
                        <asp:label ID="lbl_NTInvAccApv" class="header2" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_NTInvAccApvInfo" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_NTInvAccApv" class="animationLayer" style="width:250px; height:130px;">
                        <a onclick="div_NTInvAccApv.style.visibility='hidden';" style="cursor:pointer; float:right;" ><img src="../images/close.png" alt="" /></a>
                        It shows the Non-Trade Expense Invoice with below status<br />
                        &nbsp; (1)	Accounts Approved	: it is pending for generating Interface;<br />
                        </div>
                        <div id="flyout13" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_NTInvAccApv' style="display:none;">
            <td>
                <asp:GridView ID="gv_NTInvAccApv" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_NTInvAccApv_RowDataBound" Visible="false" >
                    <Columns>
                        <asp:TemplateField HeaderText="Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Invoice No. /<br> Account No.">
                            <ItemTemplate >
                                 <asp:LinkButton ID="lnk_InvoiceAccountNo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice Date" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceDate" runat ="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vendor" ItemStyle-HorizontalAlign='Left' >
                            <ItemTemplate>
                                <asp:Label ID="lbl_Vendor" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Due Date" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_DueDate" runat ="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Currency">
                            <ItemTemplate >
                                <asp:Label ID="lbl_Currency" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                       <asp:TemplateField HeaderText="Amount"  ItemStyle-HorizontalAlign="right">
                            <ItemTemplate >
                                <asp:Label ID="lbl_Amount" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>    
                        <asp:TemplateField HeaderText="Invoice Status"  ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate >
                                <asp:Label ID="lbl_InvoiceStatus" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate >No outstanding Invoice.</EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>

        
        <asp:TableRow  ID="tr_NewNTVendorApproval_Header" runat="server" Visible="false">
           <asp:TableCell id="td_NewNTVendorApprovalList" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header2" ID="lb_NewNTVendorApproval" name="lb_NewNTVendorApproval" style="" onclick='lb_NewNTVendorApproval_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_NewNTVendorApproval_Expand'  onclick="loadDataGrid('NewNTVendorApproval');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_NewNTVendorApproval_Collapse'  onclick="loadDataGrid('NewNTVendorApproval');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="sp_NewNTVendorApproval" onclick="loadDataGrid('NewNTVendorApproval');" >New Non-Trade Vendor Pending for Approval</a>&nbsp;
                        <asp:label ID="lbl_NewNTVendorApproval" class="header2" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_NewNTVendorApprovalInfo" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_NewNTVendorApproval" class="animationLayer" style="width:250px; height:130px;">
                        <a onclick="div_NewNTVendorApproval.style.visibility='hidden';" style="cursor:pointer; float:right;" ><img src="../images/close.png" alt="" /></a>
                        It will show the new Non-Trade vendor with below status<br />
                        &nbsp; (1)	Pending		: Pending for Accounts approval<br />
                        </div>
                        <div id="flyout14" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_NewNTVendorApproval' style="display:none;">
            <td>
                <asp:GridView ID="gv_NewNTVendorApproval" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_NewNTVendorApproval_RowDataBound" Visible="false" >
                    <Columns>
                        <asp:TemplateField HeaderText="Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vendor" ItemStyle-HorizontalAlign='Left'>
                            <ItemTemplate >
                                 <asp:LinkButton ID="lnk_NTVendor" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vendor Address" >
                            <ItemTemplate >
                                <asp:Label ID="lbl_NTVendorAddress" runat ="server"  Width="200px"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Default Expense Type" >
                            <ItemTemplate>
                                <asp:Label ID="lbl_ExpenseType" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate >No vendor.</EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>

    
        <asp:TableRow  ID="tr_NTVendorAmendment_Header" runat="server" Visible="false">
           <asp:TableCell id="td_NTVendorAmendmentList" runat="server" >
                <table>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <asp:LinkButton class="header2" ID="lb_NTVendorAmendment" name="lb_NTVendorAmendment" style="" onclick='lb_NTVendorAmendment_click' runat='server' Enabled='true'></asp:LinkButton>
                        <img id='img_NTVendorAmendment_Expand'  onclick="loadDataGrid('NTVendorAmendment');" style='cursor:hand;display:block;' src="../images/Expand.jpg" alt='Show the detail'/> 
                        <img id='img_NTVendorAmendment_Collapse'  onclick="loadDataGrid('NTVendorAmendment');" style='cursor:hand;display:none;' src="../images/Collapse.jpg" alt='Hide the detail'/> 
                    </td>
                    <td>
                        <a class ="header2" id="A1" onclick="loadDataGrid('NTVendorAmendment');" >Non-Trade Vendor Amendment Detail</a>&nbsp;
                        <asp:label ID="lbl_NTVendorAmendment" class="header2" runat="server" text=""  style="border:thin none;font-weight:normal;"></asp:label>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:ImageButton ID="btn_NTVendorAmendmentInfo" runat="server" OnClientClick="return false;" ImageUrl="~/images/help.BMP" />
                    </td>
                    <td>
                        <div id="div_NTVendorAmendment" class="animationLayer" style="width:250px; height:130px;">
                        <a onclick="div_NTVendorAmendment.style.visibility='hidden';" style="cursor:pointer; float:right;" ><img src="../images/close.png" alt="" /></a>
                        It shows the amendment detail of Non-Trade Expense Vendor for follow-up<br />
                        </div>
                        <div id="flyout15" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>
                    </td>
                </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <tr id='tr_NTVendorAmendment' style="display:none;">
            <td>
                <asp:GridView ID="gv_NTVendorAmendment" runat="server" AutoGenerateColumns="false" 
                    onrowdatabound="gv_NTVendorAmendment_RowDataBound" Visible="false" >
                     <Columns>
                        <asp:TemplateField HeaderText="Office">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Office" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vendor" ItemStyle-HorizontalAlign='Left'>
                            <ItemTemplate >
                                 <asp:LinkButton ID="lnk_NTVendor" runat="server" />&nbsp;&nbsp;
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Amendment Detail" ItemStyle-HorizontalAlign='Left'>
                            <ItemTemplate >
                                <asp:Label ID="lbl_NTVendorAmendment" runat ="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate >No vendor.</EmptyDataTemplate>
                </asp:GridView>
            </td>
        </tr>
   
    </table>

         <cc1:AnimationExtender id="ae_UploadFail" runat="server" TargetControlID="btn_uploadFailedInfo">
            <Animations>
                <OnClick>
                    <Sequence>
                        <%-- Disable the button so it can't be clicked again --%>
                        <EnableAction Enabled="false" />
                        
                        <%-- Position the wire frame on top of the button and show it --%>
                        <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_uploadFailedInfo'), $get('flyout1'));" />
                        <StyleAction AnimationTarget="flyout1" Attribute="display" Value="block"/>
                        
                        <%-- Move the wire frame from the button's bounds to the info panel's bounds --%>
                        <Parallel AnimationTarget="flyout1" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="90" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <%-- Move the info panel on top of the wire frame, fade it in, and hide the frame --%>
                        <ScriptAction Script="Cover($get('flyout1'), $get('div_UploadFailInfo'), true);" />
                        <StyleAction AnimationTarget="div_UploadFailInfo" Attribute="display" Value="block" />
                        <ScriptAction Script="div_UploadFailInfo.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_UploadFailInfo" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout1" Attribute="display" Value="none"/>
                        
                        <%-- Flash the text/border red and fade in the "close" button --%>
                        <Parallel AnimationTarget="div_UploadFailInfo" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_UploadFailInfo" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>
                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
         <cc1:AnimationExtender id="AnimationExtender1" runat="server" TargetControlID="lnk_Vendor">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_lnk_Vendor'), $get('flyout2'));" />
                        <StyleAction AnimationTarget="flyout2" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout2" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="70" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout2'), $get('div_Vendor'), true);" />
                        <StyleAction AnimationTarget="div_Vendor" Attribute="display" Value="block" />
                        <ScriptAction Script="div_Vendor.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_Vendor" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout2" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_Vendor" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_Vendor" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>
                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
         <cc1:AnimationExtender id="AnimationExtender2" runat="server" TargetControlID="btn_UTOrderInfo">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_UTOrderInfo'), $get('flyout3'));" />
                        <StyleAction AnimationTarget="flyout3" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout3" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout3'), $get('div_UTOrderInfo'), true);" />
                        <StyleAction AnimationTarget="div_UTOrderInfo" Attribute="display" Value="block" />
                        <ScriptAction Script="div_UTOrderInfo.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_UTOrderInfo" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout3" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_UTOrderInfo" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_UTOrderInfo" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>
                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
         <cc1:AnimationExtender id="AnimationExtender3" runat="server" TargetControlID="btn_BookingInfo">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_BookingInfo'), $get('flyout4'));" />
                        <StyleAction AnimationTarget="flyout4" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout4" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout4'), $get('div_BookingInfo'), true);" />
                        <StyleAction AnimationTarget="div_BookingInfo" Attribute="display" Value="block" />
                        <ScriptAction Script="div_BookingInfo.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_BookingInfo" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout4" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_BookingInfo" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_BookingInfo" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>
                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
         <cc1:AnimationExtender id="AnimationExtender4" runat="server" TargetControlID="btn_DocInfo">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_DocInfo'), $get('flyout5'));" />
                        <StyleAction AnimationTarget="flyout5" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout5" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout5'), $get('div_DocInfo'), true);" />
                        <StyleAction AnimationTarget="div_DocInfo" Attribute="display" Value="block" />
                        <ScriptAction Script="div_DocInfo.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_DocInfo" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout5" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_DocInfo" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_DocInfo" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>
                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
         <cc1:AnimationExtender id="AnimationExtender5" runat="server" TargetControlID="btn_MissingSunAccCode">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_MissingSunAccCode'), $get('flyout6'));" />
                        <StyleAction AnimationTarget="flyout6" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout6" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout6'), $get('div_MissingSunAccCode'), true);" />
                        <StyleAction AnimationTarget="div_MissingSunAccCode" Attribute="display" Value="block" />
                        <ScriptAction Script="div_MissingSunAccCode.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_MissingSunAccCode" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout6" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_MissingSunAccCode" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_MissingSunAccCode" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>

                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
         <cc1:AnimationExtender id="AnimationExtender6" runat="server" TargetControlID="btn_MissingPayAdvEMail">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_MissingPayAdvEMail'), $get('flyout7'));" />
                        <StyleAction AnimationTarget="flyout7" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout7" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout7'), $get('div_MissingPayAdvEMail'), true);" />
                        <StyleAction AnimationTarget="div_MissingPayAdvEMail" Attribute="display" Value="block" />
                        <ScriptAction Script="div_MissingPayAdvEMail.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_MissingPayAdvEMail" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout7" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_MissingPayAdvEMail" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_MissingPayAdvEMail" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>

                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
         <cc1:AnimationExtender id="AnimationExtender7" runat="server" TargetControlID="btn_EziBuyOSPaymentList">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_EziBuyOSPaymentList'), $get('flyout8'));" />
                        <StyleAction AnimationTarget="flyout8" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout8" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout8'), $get('div_EziBuyOSPaymentList'), true);" />
                        <StyleAction AnimationTarget="div_EziBuyOSPaymentList" Attribute="display" Value="block" />
                        <ScriptAction Script="div_EziBuyOSPaymentList.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_EziBuyOSPaymentList" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout8" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_EziBuyOSPaymentList" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_EziBuyOSPaymentList" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>

                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
         <cc1:AnimationExtender id="AnimationExtender8" runat="server" TargetControlID="btn_ResubmitInfo">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_ResubmitInfo'), $get('flyout8'));" />
                        <StyleAction AnimationTarget="flyout8" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout8" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout8'), $get('div_ResubmitInfo'), true);" />
                        <StyleAction AnimationTarget="div_ResubmitInfo" Attribute="display" Value="block" />
                        <ScriptAction Script="div_ResubmitInfo.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_ResubmitInfo" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout8" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_ResubmitInfo" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_ResubmitInfo" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>
                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
         <cc1:AnimationExtender id="AnimationExtender9" runat="server" TargetControlID="btn_DocOffshoreInfo">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_DocOffshoreInfo'), $get('flyout9'));" />
                        <StyleAction AnimationTarget="flyout9" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout9" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout9'), $get('div_DocOffshoreInfo'), true);" />
                        <StyleAction AnimationTarget="div_DocOffshoreInfo" Attribute="display" Value="block" />
                        <ScriptAction Script="div_DocOffshoreInfo.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_DocOffshoreInfo" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout9" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_DocOffshoreInfo" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_DocOffshoreInfo" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>
                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
        <cc1:AnimationExtender id="AnimationExtender10" runat="server" TargetControlID="btn_OSNTInvDeptApvInfo">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_OSNTInvDeptApvInfo'), $get('flyout10'));" />
                        <StyleAction AnimationTarget="flyout10" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout10" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout10'), $get('div_OSNTInvDeptApv'), true);" />
                        <StyleAction AnimationTarget="div_OSNTInvDeptApv" Attribute="display" Value="block" />
                        <ScriptAction Script="div_OSNTInvDeptApv.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_OSNTInvDeptApv" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout10" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_OSNTInvDeptApv" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_OSNTInvDeptApv" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>
                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
        <cc1:AnimationExtender id="AnimationExtender11" runat="server" TargetControlID="btn_OSNTInvAccLvl1ApvInfo">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_OSNTInvAccLvl1ApvInfo'), $get('flyout11'));" />
                        <StyleAction AnimationTarget="flyout11" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout11" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout11'), $get('div_OSNTInvAccLvl1Apv'), true);" />
                        <StyleAction AnimationTarget="div_OSNTInvAccLvl1Apv" Attribute="display" Value="block" />
                        <ScriptAction Script="div_OSNTInvAccLvl1Apv.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_OSNTInvAccLvl1Apv" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout11" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_OSNTInvAccLvl1Apv" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_OSNTInvAccLvl1Apv" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>
                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
        <cc1:AnimationExtender id="AnimationExtender12" runat="server" TargetControlID="btn_OSNTInvAccLvl2ApvInfo">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_OSNTInvAccLvl2ApvInfo'), $get('flyout12'));" />
                        <StyleAction AnimationTarget="flyout12" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout12" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout12'), $get('div_OSNTInvAccLvl2Apv'), true);" />
                        <StyleAction AnimationTarget="div_OSNTInvAccLvl2Apv" Attribute="display" Value="block" />
                        <ScriptAction Script="div_OSNTInvAccLvl2Apv.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_OSNTInvAccLvl2Apv" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout12" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_OSNTInvAccLvl2Apv" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_OSNTInvAccLvl2Apv" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>
                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
        <cc1:AnimationExtender id="AnimationExtender13" runat="server" TargetControlID="btn_NTInvAccApvInfo">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_NTInvAccApvInfo'), $get('flyout13'));" />
                        <StyleAction AnimationTarget="flyout13" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout13" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout13'), $get('div_NTInvAccApv'), true);" />
                        <StyleAction AnimationTarget="div_NTInvAccApv" Attribute="display" Value="block" />
                        <ScriptAction Script="div_NTInvAccApv.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_NTInvAccApv" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout13" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_NTInvAccApv" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_NTInvAccApv" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>
                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
        <cc1:AnimationExtender id="AnimationExtender14" runat="server" TargetControlID="btn_NewNTVendorApprovalInfo">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_NewNTVendorApprovalInfo'), $get('flyout14'));" />
                        <StyleAction AnimationTarget="flyout14" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout14" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout14'), $get('div_NewNTVendorApproval'), true);" />
                        <StyleAction AnimationTarget="div_NewNTVendorApproval" Attribute="display" Value="block" />
                        <ScriptAction Script="div_NewNTVendorApproval.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_NewNTVendorApproval" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout14" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_NewNTVendorApproval" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_NewNTVendorApproval" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>
                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
        <cc1:AnimationExtender id="AnimationExtender15" runat="server" TargetControlID="btn_NTVendorAmendmentInfo">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_NTVendorAmendmentInfo'), $get('flyout15'));" />
                        <StyleAction AnimationTarget="flyout15" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout15" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout15'), $get('div_NTVendorAmendment'), true);" />
                        <StyleAction AnimationTarget="div_NTVendorAmendment" Attribute="display" Value="block" />
                        <ScriptAction Script="div_NTVendorAmendment.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_NTVendorAmendment" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout15" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_NTVendorAmendment" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_NTVendorAmendment" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>
                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>
         <cc1:AnimationExtender id="AnimationExtender16" runat="server" TargetControlID="btn_MissingEpicorSupplierId">
            <Animations>                
                <OnClick>
                    <Sequence>
                        <EnableAction Enabled="false" />
                        
                       <ScriptAction Script="Cover($get('ctl00_ContentPlaceHolder1_btn_MissingEpicorSupplierId'), $get('flyout16'));" />
                        <StyleAction AnimationTarget="flyout16" Attribute="display" Value="block"/>
                        
                        <Parallel AnimationTarget="flyout16" Duration=".3" Fps="25">
                            <Move Horizontal="-10" Vertical="10" />
                            <Resize Width="260" Height="140" />
                            <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                        </Parallel>
                        
                        <ScriptAction Script="Cover($get('flyout16'), $get('div_MissingEpicorSupplierId'), true);" />
                        <StyleAction AnimationTarget="div_MissingEpicorSupplierId" Attribute="display" Value="block" />
                        <ScriptAction Script="div_MissingEpicorSupplierId.style.visibility='visible';" />
                        <FadeIn AnimationTarget="div_MissingEpicorSupplierId" Duration=".2"/>
                        <StyleAction AnimationTarget="flyout16" Attribute="display" Value="none"/>
                        
                         <Parallel AnimationTarget="div_MissingEpicorSupplierId" Duration=".5">
                            <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                            <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                        </Parallel>
                        <Parallel AnimationTarget="div_MissingEpicorSupplierId" Duration=".5">
                            <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                            <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                        </Parallel>

                        <EnableAction Enabled="true" />
                    </Sequence>
                </OnClick>
            </Animations>
        </cc1:AnimationExtender>

<br />
<asp:Panel ID="pn_Debug" runat='server' style='width:800px;'  Visible='false'>
<label style='background-color:Yellow;font-size:large;width:150px;'> DEBUG MESSAGE </label><br />
</asp:Panel> 
<br />

</asp:Content>
