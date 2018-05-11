<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ucltrMenu.ascx.vb" Inherits="EMECIWeb.ucltrMenu" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script src="core.js" type="text/javascript"></script>
<style> body, html { margin:0; padding:0 }
	#HeadContainer { background-color:#FFFFFF; border-bottom: 1px solid #9fe0ff; }
	#HeadExp { width:860px; background-color:#FFFFFF; font-family: Arial, Helvetica, sans-serif; font-size:12px; }
	#logoEmeciHeader { float: left }
	#logout { float: right; text-align:right; }
	#Menu { clear:both; width:120%; margin-left:-9%; }
	#NombrePaciente { font-size:18px; margin-top:70px }
</style>
<div id="HeadContainer">
	<div id="HeadExp">
		<div id="logoEmeciHeader">
			<img src="/imagenes/logoEmeci183x112.jpg">
		</div>
		<div id="logout">
			<div id="NombrePaciente" runat="server">Nombre del Paciente</div>
			<a ID="lnkCambiar" href="CambiarPassword.aspx">Cambiar Contraseña</a> &nbsp;&nbsp;&nbsp;<a ID="lnkSalir" href="logoutexp.aspx">Salir de Expediente</a>
			<!-- 
      <a href=""> </a>
      -->
		</div>
		<div id="Menu">
			<a href="Historial_Clinico.aspx"><img src="images/_resumen.jpg" border="0" alt=""></a>
			<a href="Generales.aspx"><img src="images/_generales.jpg" border="0"></a> <a href="expediente_historialconsultas.aspx">
				<img src="images/_consultas.jpg" border="0"></a> <a href="expediente_nacimiento.aspx">
				<img src="images/_nacimiento.jpg" border="0"></a> <a href="AntecedentesPatologicos.aspx">
				<img src="images/_patologicos.jpg" border="0"></a> <a href="AntecedentesHeredofamiliares.aspx">
				<img src="images/_heredofamiliares.jpg" border="0"></a> <a href="expediente_vacunas.aspx">
				<img src="images/_vacunas.jpg" border="0"></a> <a href="estudios.aspx"><img src="images/_paraclinicos.jpg" border="0"></a>
			<!--
			<a href="expediente_somatometria.aspx"><img src="images/_somatometria.jpg" border="0"></a>
			 -->
			<a href="ImagenesEstudios.aspx"><img src="images/_Estudios.jpg" border="0"></a>
            <a href="Ginecologia.aspx"><img src="images/_Ginecologia.jpg" /></a>
			<a href="OtrasConsultas.aspx"><img src="images/_otrasconsultas.jpg" border="0"></a>
		</div>
	</div>
</div>
