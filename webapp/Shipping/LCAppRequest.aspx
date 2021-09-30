<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="LCAppRequest.aspx.cs" Inherits="com.next.isam.webapp.shipping.LCAppRequest" Title="L/C Application Request (Merchandiser)" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc3" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_shipping_lc_mer.gif" runat="server" id="imgHeaderText" alt="workplace"/>
-->
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Shipping">L/C Application Request (Merchandiser)</asp:Panel> 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">
        function isDate(DMY_Date)
        {
            var sDate;
            var sYear, sMonth, sDay;
            var nP1, nP2
            
            if (DMY_Date=="")
            {
                return true;
            }
            else
            {
            nP1 = DMY_Date.indexOf("/",0);
            if (nP1>=0)
            {
                nP2 =DMY_Date.indexOf("/",nP1+1);
                if (nP2>=0)
                    nP3 = DMY_Date.indexOf("/",nP2+1);
                else
                    nP3 = -1 ;   
            }        
            else
                nP2 = -1 ;
            
            if (nP1>=0 && nP2>=0 && nP3==-1)
            {
                sDay = DMY_Date.substr(0,nP1);
                sMonth = DMY_Date.substr(nP1+1, nP2-nP1-1);
                sYear = DMY_Date.substr(nP2+1, DMY_Date.length-nP2);
                //alert (sYear + "-" + sMonth + "-" + sDay);
                if (sMonth.valueOf()>=1 && sMonth.valueOf()<=12 && sDay.valueOf()>=1)
                    if (sMonth.valueOf()!=2)
                        if (sMonth.valueOf()==4 || sMonth.valueOf()==6 || sMonth.valueOf()==9 || sMonth.valueOf()==11)
                            return (sDay.valueOf()<=30);
                        else
                            return (sDay.valueOf()<=31);
                    else
                        if (sYear.valueOf()%4==0)
                            return (sDay.valueOf()<=29);
                        else
                            return (sDay.valueOf()<=28);    
            }
            return false;    
            }
        }

        function inputValidation()
        {
            var sFromAwhDate;
            var sToAwhDate;
            var sFromAppDate;
            var sToAppDate;      
            var sFromAppNo;
            var sToAppNo;      
            
                   
            
            // oFromDate=eval(document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateFrom_txt_AtWHDateFrom_textbox"));
            // oToDate=eval(document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateTo_txt_AtWHDateTo_textbox"));
            sFromAwhDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateFrom_txt_AtWHDateFrom_textbox").value);
            sToAwhDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateTo_txt_AtWHDateTo_textbox").value);

            sFromAppDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppDateFrom_txt_LCAppDateFrom_textbox").value)
            sToAppDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppDateTo_txt_LCAppDateTo_textbox").value)
            sFromAppNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppNoFrom").value)
            sToAppNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppNoTo").value)


            if ((sFromAwhDate=="" || sToAwhDate=="") && (sFromAppDate=="" || sToAppDate=="") && sFromAppNo=="" && sToAppNo=="")
            {
                alert ("Please input any of the followings:\n\r - At-Warehouse Date;\n\r - L/C Application Date;\n\r - L/C Application No.");
                return false;
            }
            else 
            if (!isDate(sFromAwhDate) || !isDate(sToAwhDate) || !isDate(sFromAppDate) || !isDate(sToAppDate))
            {
                // sDate = new Date(sFromDate);
                alert("Invalid Date Format.");
                return false;
            }
            else
            {   
                return true;
            }       
            return false;
        }

		
		function checkAppNo(obj)
		{
		    var input = "";
		    var val = 0;

		    input = obj.value.trim();
		    val = parseInt(input);
		    if (input != "")
		    {   if (isNaN(val))
		        {
		    	    alert("Invalid L/C Application No.");
		            return false;
                }
                else
                {
                    if (parseInt(input).toString() != input) 
                    {
                        alert("Invalid L/C Application No.");
                        return false;
                    }
		        }
		    }
		    return true;
		}

		function SubmitApplication(obj) {
		    obj.disabled = true;
		    document.all.ctl00_ContentPlaceHolder1_btn_Apply.click();
		    return true;
		}

        function WhoAmI(obj)
        {
            alert(obj.id );
            return true;
        }
    </script>

    <script type="text/vbscript">
        function inputValidation_vb
            sFromAwhDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateFrom_txt_AtWHDateFrom_textbox").value)
            sToAwhDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateTo_txt_AtWHDateTo_textbox").value)
            sFromAppDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppDateFrom_txt_LCAppDateFrom_textbox").value)
            sToAppDate = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppDateTo_txt_LCAppDateTo_textbox").value)
            sFromAppNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppNoFrom_txt_LCAppNoFrom_textbox").value)
            sToAppNo = (document.getElementById("ctl00_ContentPlaceHolder1_txt_LCAppNoTo_txt_LCAppNoTo_textbox").value)

            
            if (sFromAwhDate="" or sToAwhDate="" or sFromAppDate="" or sToAppDate="" or sFromAppNo="" or sToAppNo="") then
                'nButton = MsgBox ("Please input At Warehouse Date", vbOKOnly+vbExclamation, "Data Input Warning")
                'MessageBox.Show ("Please input At Warehouse Date", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                'alert("Please input At Warehouse Date")
                nButton = MsgBox ("Please input any of the followings" + vbcrlf + " - At Warehouse Date;" + vbcrlf + " - L/C Application Date;" + vbcrlf + " - L/C Application No.", vbOKOnly+vbExclamation, "Data Input Warning")
                inputValidation= false
            else
                if (not isDate(sFromAwhDate) or not isDate(sToAwhDate) or not isDate(sFromAppDate) or not isDate(sToAppDate) ) then
                    nButton = MsgBox("Invalid Date Format. Please input again.")
                    inputValidation = false
                else
                    inputValidation = true
                end if    
            end if  
        end function
    </script>
    
    <table width="800px" cellspacing="0" cellpadding="2" >
        <col width="120"/><col width="70"/><col width="80"/><col width="150"/><col width="250"/><col />
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="FieldLabel2_H2">&nbsp;<span>P.O. Scheduled<br />&nbsp;At-warehouse Date</span></td>
            <td colspan="4">
                <cc3:smartcalendar id="txt_AtWHDateFrom" runat="server" 
                    ToDateControl="txt_AtWHDateTo" RequiredFieldEnabled="False" 
                    RequiredFieldText="Please Input At Warehouse Date here" />
                &nbsp;&nbsp;to&nbsp;
			    <cc3:smartcalendar id="txt_AtWHDateTo" runat="server"  
                    FromDateControl="txt_AtWHDateFrom" RequiredFieldEnabled="False" 
                    RequiredFieldText="Please Input At Warehouse Date here" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">&nbsp;Supplier</td>
            <td colspan="4">
                <uc1:UclSmartSelection  ID="txt_VendorName" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">&nbsp;Item No</td>
            <td>
                <span><asp:TextBox ID="txt_ItemNo" runat="server" /></span>
            </td>
            <td class="FieldLabel2_H2">Country of Origin</td>
            <td>
                <cc3:SmartDropDownList ID='ddl_CO' runat='server'  />
            </td>
            <td></td>
        </tr>
        <tr>
            <td class="FieldLabel2">&nbsp;Office</td>
            <td>
                <cc3:SmartDropDownList ID='ddl_Office' runat='server' width="150" 
                    onselectedindexchanged="ddl_Office_SelectedIndexChanged" AutoPostBack="True" />
            </td>
            <td class="FieldLabel2">Product Team</td>
            <td>
                <cc3:SmartDropDownList ID='ddl_ProductTeam' runat='server' width="250"/>
            </td>
            <td></td>
        </tr>
        <tr>
            <td class="FieldLabel2">L/C Application Date</td>
            <td colspan="3">
                <cc3:smartcalendar id="txt_LCAppDateFrom"  runat="server" Width="120px" FromDateControl="txt_LCAppDateFrom"
				    ToDateControl="txt_LCAppDateTo"/>&nbsp;&nbsp;to&nbsp;
			    <cc3:smartcalendar id="txt_LCAppDateTo" runat="server" Width="120px"  FromDateControl="txt_LCAppDateFrom"
				    ToDateControl="txt_LCAppDateTo" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2" >L/C Application No.</td>
            <td colspan="3">
                <asp:TextBox ID ="txt_LCAppNoFrom" runat="server"  Skinid="DateTextBox" onchange='return(checkAppNo(this));' />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;to&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
		    	<asp:TextBox ID="txt_LCAppNoTo" runat="server" SkinID="DateTextBox" onchange='return(checkAppNo(this));' />
            </td>
        </tr>
        
        <tr id="tr_SumissionStatus" style="display:none;">
        <td class="FieldLabel2">L/C Application Status</td>
        <td>
            <cc3:SmartDropDownList ID='ddl_LCAppSubmitStatus' runat='server' width="140">
                <asp:ListItem Text="--All--" Value="-1" Selected="True" />
                <asp:ListItem Text="Outstanding" Value="0" />
                <asp:ListItem Text="Submitted / Completed" Value="1" />
            </cc3:SmartDropDownList> 
        </td>
        </tr>

        <tr id="tr1" style="display:none;">
        <td class="FieldLabel2">Outstanding Shipment Only</td>
        <td>
            <asp:CheckBox runat='server' ID='ckb_OutstandingShipmentOnly'  />
        </td>
        </tr>

        <tr><td>&nbsp;</td></tr>
    </table>
    <table cellspacing="10">
        <tr>
            <td colspan="1">
                <asp:Button ID="btn_Search" runat="server" Text="Search" 
                    onclick="btn_Search_Click" OnClientClick='return inputValidation();' />
            </td>
            <td >
                <asp:Button ID='btn_PrintScreen' runat='server' text='Print' onClientClick='window.print();return false' />
            </td>
            <td>
                <asp:Button ID="btn_Submit" runat="server" Text="Submit" onClientClick='SubmitApplication(this);' visible="false"/>&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_Apply" runat="server" Text="Apply" onclick='btn_Apply_Click' style='display:none;' /></td>
            <td>&nbsp;</td>
        </tr>
    </table>
    <asp:Label ID="lbl_RowCount" runat="server" style="color:#ff9900; font-weight :bolder;" text="" /><br /><br />
    &nbsp;&nbsp;
    <asp:LinkButton ID="btn_SelectAll" runat="server" Text="Select All" OnClientClick="CheckAll('ckb_LC');" visible="false"/>&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="btn_DeselectAll" runat="server" Text="Deselect All" OnClientClick="UncheckAll('ckb_LC');" visible="false"/>

    <asp:Panel ID="pnl_SearchResult" runat="server" Visible="false">
        <asp:UpdatePanel ID="up_LC" runat="server" >
        <ContentTemplate>
        <asp:GridView ID="gv_LC" runat="server" AutoGenerateColumns="false" onrowdatabound="gv_LC_RowDataBound">
                <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:CheckBox ID="ckb_LC" runat="server" Enabled='false' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Contract No.">
                    <ItemTemplate>
                        <asp:Label ID="lbl_ContractNo" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Dly No.">
                    <ItemTemplate>
                        <asp:Label ID="lbl_DlyNo" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Supplier">
                    <ItemTemplate>
                        <asp:Label ID="lbl_Supplier" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Item No.">
                    <ItemTemplate>
                        <asp:Label ID="lbl_ItemNo" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="P.O At-warehouse Date">
                    <ItemTemplate>
                        <asp:Label ID="lbl_POAwhDate" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField  HeaderText="Packing Method">
                    <ItemTemplate>
                        <asp:Label ID="lbl_PackMethod" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Country of Origin">
                    <ItemTemplate>
                        <asp:Label ID="lbl_CO" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Shipment Method">
                    <ItemTemplate>
                        <asp:Label ID="lbl_ShipMethod" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Purchase Term">
                    <ItemTemplate>
                        <asp:Label ID="lbl_PurchaseTerm" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Purchsae Location">
                    <ItemTemplate>
                        <asp:Label ID="lbl_PurchaseLocation" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Port of Loading">
                    <ItemTemplate>
                        <asp:Label ID="lbl_PortOfLoad" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Destination">
                    <ItemTemplate>
                        <asp:Label ID="lbl_Destination" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="PO Qty" >
                    <ItemTemplate>
                        <asp:Label ID="lbl_TotalPOQuantity" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="PO Amount">
                    <ItemTemplate>
                        <asp:Label ID="lbl_ShipmentTotalPOAmount" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Advising Bank">
                    <ItemTemplate>
                        <asp:Label ID="lbl_AdvisingBank" runat="server" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddl_AdvisingBank" runat="server" />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Shipment Status" >
                    <ItemTemplate>
                        <asp:Label ID='lbl_ShipmentWorkflowStatus' runat="server"/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField headerstyle-borderstyle='none' ItemStyle-BorderStyle='none'>
                </asp:TemplateField>
                <asp:TemplateField  HeaderText="LC Application No" >
                    <ItemTemplate >
                        <asp:Label ID="lbl_LCApplicationNo" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="LC Application Amount">
                    <ItemTemplate>
                        <asp:Label ID="lbl_LCApplicationTotalPOAmount" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="L/C Application Status">
                    <ItemTemplate>
                        <asp:Label ID='lbl_LCApplicationWorkflowStatus' runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="L/C No." >
                    <ItemTemplate>
                        <asp:Label ID='lbl_LCNo' runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="btn_Edit" runat="server" Text="Edit" CommandName="Edit" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton ID="btn_Save" runat="server" Text="Save" CommandName="Update" />
                        <asp:LinkButton ID="btn_Cancel" runat="server" Text="Cancel" CommandName="Cancel"  />
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>
        <EmptyDataTemplate>
            <span style="font-family: Arial; font-size: medium; font-weight: bold; background-color: #FFFFCC;">No record found.</span>
        </EmptyDataTemplate>
            
            
        </asp:GridView>
        </ContentTemplate>
        </asp:UpdatePanel>
        <br />
    </asp:Panel>
</asp:Content>
