<%@ Register TagPrefix="uc1" TagName="UclLoginOffice" Src="../common/UclLoginOffice.ascx" %>
<%@ Page language="c#" Codebehind="Login.aspx.cs" AutoEventWireup="True" Inherits="com.next.isam.webapp.main.Login" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>ISAM - Integrated Shipping and Account Management System</title>
		<script language="javascript">
			if (parent && parent.location!=window.location){				
				parent.location=window.location;
			}
		</script>
		<link href="../includes/stylesheet.css" type="text/css" rel="stylesheet">
		<link href="../includes/LoginStyle.css" type="text/css" rel="stylesheet">
		<script src="../common/Revamp.js" type="text/javascript">
		</script>

	</HEAD>
	<!--
	onload="MM_preloadImages('../images/login/btn_login_b.jpg')"
	-->
	<body onload='javascript:MM_preloadImages("../images/login/btn_login_b.jpg");document.Form1.txtLoginName.focus();' 
	style="background-image:../images/login/btn_login_b.jpg;">
		<form id="Form1" method="post" runat="server">
			<table width="0" border="0" cellpadding="0" cellspacing="0" align="center">
			  <tr>
                <td><img src="../images/login/top.jpg" width="986" height="104" /></td>
              </tr>
                <tr>
                    <td height="120" align="center" valign="top" style="background-repeat: no-repeat">
                        <table  border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="712" height="362" valign="top" background="../images/Login/left.jpg" align="center">
                                    <table id="tb_ApplicationPanel" width="699" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td width="40px" height="25">&nbsp;
                                            <div style="width:40px;"></div>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td valign="top">
                                                <span class="style_t_yellow">Integrated Shipping and Account Management System (ISAM)</span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="60">&nbsp;</td>
                                            <td valign="top">
                                                <span class="style_white_12" style="line-height: 21px;">
                                                    We design, source, buy and merchandise <img src='..\images\login\next.jpg' /> branded products from overseas offices in Europe, South Asia and the Far East. Our global presence currently spans 17 cities worldwide 
                                                    <img src="../images/Login/next.jpg" width="35" height="10"  style="display:none;"/>
                                                </span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div style="width:20px;">&nbsp;</div>
                                            </td>
                                            <td>
                                                <table width="0" border="0" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <img src="../images/Login/company.jpg" width="308" height="243" />
                                                        </td>
                                                        <td width="20px" >
                                                            <div style="width:20px;">&nbsp;</div>
                                                        </td>
                                                        <td valign="top">
                                                            <table id="tb_LoginPanel" width="0" border="0" cellspacing="0" cellpadding="0" >
                                                                <tr>
                                                                <td>&nbsp;</td>
                                                                </tr>
                                                                <tr>
                                                                    <td width="85" height="30" class="style_white_12">
                                                                        User Name
                                                                    </td>
                                                                    <td align="left">
															            <input name="Login" id="txtLoginName" type="text"  runat="server"
															                class="style_white_12" style="background-color: #909598;width: 200px; border: 1px;">
															        </td>
															        <td >
																        <asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter your login name."
																	        ControlToValidate="txtLoginName" Display="Dynamic">*</asp:RequiredFieldValidator>
																        <asp:CustomValidator id="cvalLogin" OnServerValidate="cvalLogin_ServerValidate" runat="server" Display="Dynamic" 
																            ErrorMessage="Please enter a valid User Name and Password.">*</asp:CustomValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td height="30" class="style_white_12">
                                                                        Password
                                                                    </td>
                                                                    <td align="left">
                                                                        <input name="Password" id="txtPassword" type="password" runat="server" 
                                                                            class="style_white_12" style="background-color:#909598;width:200px;border:1px">
                                                                    </td>
                                                                    <td >
															            <asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" 
															                ErrorMessage="Please enter your password." ControlToValidate="txtPassword" Display="Dynamic">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td height="30" class="style_white_12">
                                                                        Office
                                                                    </td>
                                                                    <td align="left">
                                                                        <label>
                                                                            <uc1:UclLoginOffice id="uclLoginOffice" runat="server" 
                                                                                class="style_white_12" style="background-color: #909598;width:200px; border:1px" listwidth="135px"></uc1:UclLoginOffice>
                                                                        </label>
                                                                    </td>
                                                                    <td></td>
                                                                </tr>
                                                                <tr>
                                                                    <td height="30">
                                                                        <a href="#" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('btn_Login','','../images/Login/btn_login_b.jpg',1)" style=" height:21px;"> 
                                                                            <asp:ImageButton id="btn_Login" runat="server" name="Imagelogin"  onclick="btn_Login_Click"
                                                                                width="73" height="21" border="0" ImageUrl="../images/Login/btn_login.jpg" >
                                                                            </asp:ImageButton>
                                                                        </a>
                                                                    </td>
                                                                    <td>&nbsp;</td>
                                                                    <td></td>
                                                                </tr>
                                                            </table>
                                                                    <asp:Label id="lblLoginStatus" runat="server" ForeColor="Red" Width="300px" height="60px" style="OVERFLOW: auto"></asp:Label>
                                                                    <div><asp:ValidationSummary id="ValidationSummary1" runat="server" Width="328px" ></asp:ValidationSummary></div>

                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td width="274" align="left" valign="top" background="../images/Login/right.jpg">
                                    <table id="tb_OurMission" width="0" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td width="15" height="25">&nbsp;</td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td><img src="../images/Login/our_mission.gif" width="123" height="18" /></td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td valign="top">
                                                <table width="227" border="0" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td width="12" height="30" align="left" valign="top">
                                                            <img src="../images/Login/pt.gif" />
                                                        </td>
                                                        <td align="left" valign="top" class="style_white_12">
                                                            Exciting
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td height="30" align="left" valign="top">
                                                            <img src="../images/Login/pt.gif" />
                                                        </td>
                                                        <td align="left" valign="top">
                                                            <span class="style_white_12">Beautifully Designed </span>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td height="43" align="left" valign="top">
                                                            <img src="../images/Login/pt.gif" />
                                                        </td>
                                                        <td align="left" valign="top">
                                                            <span class="style_white_12">Excellent Quality Clothing and Homeware </span>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td height="30" align="left" valign="top">
                                                            <img src="../images/Login/pt.gif" />
                                                        </td>
                                                        <td align="left" valign="top">
                                                            <span class="style_white_12">Presented in collections that reflect the aspirations and
                                                                means of our customers</span>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table id="tb_PicturePanel" width="0" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td><img src="../images/Login/top3a.jpg" width="986" height="74" /></td>
                            </tr>
                            <tr>
                                <td height="125" align="left" valign="bottom" background="../images/Login/top3b.jpg" >
                                    <table width="0" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td width="50" valign="bottom"></td>
                                            <td valign="bottom"><img src="../images/Login/01.jpg" width="107" height="125" /></td>
                                            <td width="39" valign="bottom"></td>
                                            <td valign="bottom"><img src="../images/Login/02.jpg" width="113" height="121" /></td>
                                            <td width="26" valign="bottom"></td>
                                            <td valign="bottom"><img src="../images/Login/03.jpg" width="102" height="100" /></td>
                                            <td width="18" valign="bottom"></td>
                                            <td valign="bottom"><img src="../images/Login/04.jpg" width="86" height="86" /></td>
                                            <td width="41" valign="bottom"></td>
                                            <td valign="bottom"><img src="../images/Login/05.jpg" width="90" height="118" /></td>
                                            <td width="26" valign="bottom"></td>
                                            <td valign="bottom"><img src="../images/Login/06.jpg" width="97" height="125" /></td>
                                            <td width="35" valign="bottom">
                                            </td><td valign="bottom"><img src="../images/Login/07.jpg" width="111" height="119" /></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <img src="../images/Login/top3c.jpg" width="986" height="16" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right" bgcolor="#1A1A1A">
                        <table id="tb_Footer" width="0" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="100" align="center" class="style_white_12">
                                    <A href="http://ns-s13/nslhd/default.aspx" target="helpDesk" title="Hotline: (852) 2376 5433">Help Desk</A>
                                </td>
                                <td width="20" align="center" class="style_white_12" style="display:none;">
                                    |
                                </td>
                                <td width="50" align="center" class="style_white_12" style="display:none;">
                                    <a href="support.php">Support</a>
                                </td>
                                <td width="30" align="center">
                                    &nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                
                
                
                
                <tr  style="display:none;">
					<td align="center" valign="middle" height="450">
						<table id="tb_OldLoginPanel_Table_01" width="625" height="303" border="0" cellpadding="0" cellspacing="0" style="border-color:green;">
							<tr>
								<td colspan="2" bgcolor="black" >
									<!--<img src="../images/login_01.jpg" width="358" height="24" alt="">-->
									<!--<img src="../images/login_01_2_final.jpg" width="358" height="24" alt="">-->
									<img src="../images/login_01_3.jpg" width="625" height="24" alt="" >
								</td>
									
								<!--
								<td>
									<img src="../images/login_02.jpg" width="268" height="24" alt=""></td>
									-->
							</tr>
							<tr>
								<td >
									<img src="../images/login_03.jpg" width="360" height="100%" alt="">
								</td>
								<td height="262" width="270" valign="top" bgcolor="white">
									<table width="100%" cellpadding="5" cellspacing="5" >
										<tr>
											<td>
												<font color="#a4bc66">Integrated Shipping and Account Management System (ISAM)&nbsp;</font> <br />An integrated logistics and finance management solution for shipping, accounts, data exchange and reporting analysis.
												<br>
												<hr color="#a4bc66" width="100%">
												<table cellspacing="2" cellpadding="2" border="0">
													<tr>
														<td>User Name</td>
														<td></td>
													</tr>
													<tr>
														<td>Password</td>
														<td></td>
													</tr>
													<tr>
														<td>Office</td>
														<td>
															</td>
													</tr>
												</table>
												
												<br>
												
												<br>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td colspan="2" height="17"><font color="#9c9d5d"><SPAN style="FONT-SIZE: 8pt; FONT-FAMILY: 'Times New Roman'; mso-fareast-font-family: PMingLiU; mso-ansi-language: EN-US; mso-fareast-language: ZH-HK; mso-bidi-language: AR-SA">Integrated Shipping and Account Management System – Next Sourcing 2009.</SPAN></font></td>
							</tr>
						</table>
						<br>
						<div><asp:ValidationSummary id="ValidationSummary0" runat="server" Width="328px"></asp:ValidationSummary></div>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
