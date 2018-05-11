<%@ Page Language="vb" AutoEventWireup="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<HTML>
	<HEAD>
		<title>EMECI - Expediente Medico Electronico Clinico Internacional</title>
		<meta content="text/html; charset=iso-8859-1" http-equiv="Content-Type">
		<LINK rel="stylesheet" type="text/css" href="../emeci.css">
			<LINK rel="stylesheet" type="text/css" href="../emeciwebsite.css">
				<script>
			function sumTarjeta()
			{var st=0;
			 var lst=document.getElementById("lstCard");
			 var txt=document.getElementById("txtsum");
			 txt.value = lst.length * 250;
			  txt.value += ".00"
			}
			function ValidaTarjeta(texto)
			{
			 var txtn=texto.split("-");
			  if (txtn.length!=3)
				return false;
			  if (txtn[0].length!=5)
				return false;
			  if (txtn[1].length!=4)
				return false;
			  if (txtn[2].length!=4)
				return false;
			  
			  if (isNaN(txtn[0]))
				return false;
			  if (isNaN(txtn[1]))
				return false;
			  if (isNaN(txtn[2]))
				return false;
			     
			}
			function addCard(texto)
			{   
			   if ( ValidaTarjeta(texto)==false) return;
			   
			    var e=document.getElementById("lstCard");
				var optionObject = new Option(texto,texto);
				var optionRank = e.options.length;
				e.options[optionRank]=optionObject;	
				sumTarjeta();
				document.getElementById("txtCard").value="";
				var txtC = document.getElementById("txtCards");
				if (txtC.value =="")
				{ txtC.value=texto;
				}
				else
				{ txtC.value+="|" + texto;
				}
			}
			
			function Desabilita()
			{  
			   var chk;			   
			   chk = document.getElementById('chkAcepta')			   
			   if (chk.checked==true)
			     document.getElementsByName('submit')[0].disabled=false;
			   else
			     document.getElementsByName('submit')[0].disabled=true;			   
			}
			
			function esvalido()
			{ 
				if (document.getElementById('os0').value=="")
					{alert("Falta Número de Tarjeta Emeci");return false}
				if (document.getElementById('os1').value=="")
					{alert("Falta Nombre del Paciente"); return false;}
					
				 if ( document.getElementById('chkAcepta').checked==false)
					{alert("Debe estar de acuerdo con las condiciones de uso");
					  return false;
					}
				return true;
			}

			
				</script>
	</HEAD>
	<body style="BACKGROUND-COLOR: #fff">
		<div id="bodyContent">
			<!--#INCLUDE VIRTUAL = "/includes/emeciHeader.inc"-->
			
			<div id="MenuPrincipal">
				<img src="../imagenes/menuActivoPacientes.jpg" border="0" usemap="#Map" />
				<map name="Map" id="Map">		
					<area shape="rect" coords="7,1,85,33" href="/default.aspx" alt="Inicio" />
					<area shape="rect" coords="87,1,243,34" href="/informacion_medicos.aspx" alt="Información para Médicos" />
					<area shape="rect" coords="249,0,420,35" href="/informacion_pacientes.aspx" alt="Información para Pacientes" />
					<area shape="rect" coords="425,-1,584,34" href="/directorio_pediatras.aspx" alt="Directorio de Especialistas" />
					<area shape="rect" coords="590,0,683,33" href="/contactenos.aspx" alt="Contáctenos" />
				</map>	
			</div>
			<div id="informationBlock">
				<h2>Pago por Mantenimiento Anual de Expediente Médico Clínico</h2>
				<h3>ESQUEMA PARA CUBRIR CUOTA DE MANTENIMIENTO ANUAL EMECI, S.C.</h3>
				<P>
				¡Ahora tener Activo su Expediente Médico es más Sencillo!
				</p>
				<p>
					A efectos de agilizar los pagos de las cuotas de mantenimiento de su Expediente EMECI, usted podrá optar por escoger cualquier de los siguientes medios:
				</p>
              
                <form method="post" action="https://gateway.payulatam.com/ppp-web-gateway/pb.zul">
								  <input type="image" border="0" alt="" src="dineromail.png" onClick="this.form.urlOrigen.value = window.location.href;"/>
								  <input name="merchantId" type="hidden" value="516707"/>
								  <input name="accountId" type="hidden" value="518172"/>
								  <input name="description" type="hidden" value="TARJETA EMECI"/>
								  <input name="referenceCode" type="hidden" value="EMECI"/>
								  <input name="amount" type="hidden" value="250.00"/>
								  <input name="tax" type="hidden" value="0.00"/>
								  <input name="taxReturnBase" type="hidden" value="0"/>
								<input name="shipmentValue" value="0.00" type="hidden"/>
								  <input name="currency" type="hidden" value="MXN"/>
								  <input name="lng" type="hidden" value="es"/>
								  <input name="sourceUrl" id="urlOrigen" value="" type="hidden"/>
								  <input name="buttonType" value="SIMPLE" type="hidden"/>
								  <input name="signature" value="d3e119b3be53e29df374e8b8d57630bb8c15b0bdd43c9dcad7f676ab60008e6b" type="hidden"/>
				</form>


				<p>
Solo de un Clic en el Botón “PayU”,  la cual es la forma más cómoda y segura de realizar su pago.
<p>
<p>
Consideramos que es un mecanismo bastante sencillo, sin embargo, de tener dudas, con gusto el departamento de Soporte está en la mayor de las disposiciones para auxiliarlo, ya sea vía correo electrónico a soporte@emeci.com, vía Facebook o Twitter.
</p>
<p>
Agradecemos su atención y deseamos servirle para la conservación de la salud de sus hijos.
</p>
				
	<!--		
				
				<P>
					A efectos de agilizar los pagos de las cuotas de mantenimiento de su expediente 
					clínico, hemos aperturado una cuenta en la compañia PAYU <a href="http://www.payulatam.com/mexico/">
						www.payulatam.com/mexico</a>, por medio de la cual, se da la opción cubrir la cuota 
					EMECI usando tarjetas de crédito o débito o bien directamente por cuenta en 
					PAYU de tenerse.
				</P>
				<p>
					Consideramos que es un mecanismo bastante sencillo, sin embargo, de tener 
					dudas, con gusto el departamento de Soporte esta en la mayor de las 
					disposiciones para auxiliarlo, ya sea vía correo electrónico a <a href="mailto:soporte@emeci.com">
						soporte@emeci.com</a> 
				</p>
				<p>
					La cuota que marca el botón de pago, es <STRONG>250.00 pesos</STRONG> que sería por mantenimiento anual de cada 
					expediente.
					
					
				</p>
				
				<p>
				    La Cuenta para deposito bancario es del Banco Bajio No. 105785400201 y CLABE INTERBANCARIA 030040900001909052
					Favor de enviar copia de su ficha de depósito con él número de tarjeta EMECI y 
					nombre a <a href="mailto:pagos@emeci.com">pagos@emeci.com</a>
				</p>
				-->
				<!--
				<p>
					Video explicativo de apertura de una cuenta en PayPal <a href="http://www.doring.com/CuentaPayPal800/CuentaPayPal800.html">
						Aquí</a>
				</p>
				<p>
					Video explicativo de pago mediante PayPal <a href="http://www.doring.com/PagoConPayPal/pagoConPayPal.html" name="http://www.doring.com/pagoConTarjeta/pagoConTarjeta.html">
						Aquí</a>
				</p>
				<P>Video de Pago Directo con Tarjeta de Crédito <A href="http://www.doring.com/pagoConTarjeta/pagoConTarjeta.html">
						Aquí</A>
				</P>
				-->
				
				<!--
				<p>Agradecemos su atención y deseamos servirle para la conservación de la salud de 
					sus hijos.
				</p>
				-->
				<table id="frmContacto" border="0" cellSpacing="0" cellPadding="4" width="75%" align="center">
					<TBODY>
						<TR>
							<TD></TD>
							<TD>
								<!--
								<form method="post" onsubmit="return esvalido();" action="https://www.paypal.com/cgi-bin/webscr">
									<input value="_s-xclick" type="hidden" name="cmd">
									<table>
										<tr>
											<td><input value="Tarjeta Emeci" type="hidden" name="on0">Tarjeta Emeci</td>
										</tr>
										<tr>
											<td><input cols="30" size="30" maxLength="60" name="os0" id="os0">
										<tr>
											<td><input value="Nombre Paciente" type="hidden" name="on1">Nombre Paciente</td>
										</tr>
										<tr>
											<td><input cols="40" size="40" maxLength="60" name="os1" id="os1">
											</td>
										</tr>
									</table>
									<input value="-----BEGIN PKCS7-----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-----END PKCS7-----&#13;&#10;"
										type="hidden" name="encrypted"> <input border="0" alt="" src="https://www.paypal.com/es_ES/ES/i/btn/btn_paynowCC_LG.gif"
										type="image" name="submit"> <IMG border="0" alt="" src="https://www.paypal.com/es_XC/i/scr/pixel.gif" width="1" height="1">
								</form>
							
								<form action="https://www.paypal.com/cgi-bin/webscr" onsubmit="return esvalido();" method="post">
									<input type="hidden" name="cmd" value="_s-xclick"> <input type="hidden" name="hosted_button_id" value="2795843">
									<table>
										<tr>
											<td><input type="hidden" name="on0" value="Tarjeta EMECI">Tarjeta EMECI</td>
										</tr>
										<tr>
											<td><input type="text" cols="30" size="30" name="os0" maxlength="60">
										<tr>
											<td><input type="hidden" name="on1" value="Nombre Paciente">Nombre Paciente</td>
										</tr>
										<tr>
											<td><input type="text" cols="40" size="40" name="os1" maxlength="60">
										</tr>
									</table>
									<input type="image" src="https://www.paypal.com/es_XC/i/btn/btn_buynowCC_LG.gif" border="0"
										name="submit" alt=""> <img alt="" border="0" src="https://www.paypal.com/es_XC/i/scr/pixel.gif" width="1" height="1">
								</form>
								
								<form action="https://www.paypal.com/cgi-bin/webscr" method="post" target="_top">
									<input type="hidden" name="cmd" value="_xclick">
									<input type="hidden" name="business" value="info@emeci.com">
									<input type="hidden" name="lc" value="MX">
									<input type="hidden" name="item_name" value="Tarjeta Eneci">
									<input type="hidden" name="item_number" value="Mem">
									<input type="hidden" name="amount" value="250.00">
									<input type="hidden" name="currency_code" value="MXN">
									<input type="hidden" name="button_subtype" value="services">
									<input type="hidden" name="no_note" value="0">
									<input type="hidden" name="bn" value="PP-BuyNowBF:btn_buynowCC_LG.gif:NonHostedGuest">
									<table>
									<tr><td><input type="hidden" name="on0" value="Tarjeta EMECI">Tarjeta EMECI</td></tr><tr><td><input type="text" name="os0" maxlength="200"></td></tr>
									<tr><td><input type="hidden" name="on1" value="Nombre Paciente">Nombre Paciente</td></tr><tr><td><input type="text" name="os1" maxlength="200"></td></tr>
									</table>
									<input type="image" src="https://www.paypalobjects.com/es_XC/MX/i/btn/btn_buynowCC_LG.gif" border="0" name="submit" alt="PayPal, la forma más segura y rápida de pagar en línea.">
									<img alt="" border="0" src="https://www.paypalobjects.com/es_XC/i/scr/pixel.gif" width="1" height="1">
								</form> -->
								
								
								
								
							</TD>
							<TD><!-- <IMG alt="" src="..\imagenes\PAYPAL.GIF"> --> </TD>
						</TR>
						<TR>
							<TD></TD>
							<TD><asp:hyperlink id="hlnkVer" runat="server" Target="_blank" NavigateUrl="PolitUso.htm">Ver Politicas</asp:hyperlink><BR>
								<INPUT id="chkAcepta" onclick="Desabilita();" type="checkbox">&nbsp;Acepta 
								Terminos y Condiciones de Uso</TD>
							<TD></TD>
						</TR>
						<tr>
							<td>&nbsp;</td>
							<td></td>
							<TD></TD>
						</tr>
					</TBODY>
				</table>
			</div>
		</div>
	</body>
</HTML>
