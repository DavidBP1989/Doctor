<%@ Page Language="VB"   %>
<%@ Import Namespace="EmeciCommon" %>
<%@ Import Namespace="EmeciFacade" %>
<%@ Import Namespace="System.Web.Mail" %>
<%@ Import Namespace="System.Data" %>
<script runat="server" >



    Private Function Valida()
        Return Request.QueryString("titulo") <> "" _
         AndAlso Request.QueryString("APELL_PATE") <> "" _
         AndAlso Request.QueryString("APELL_MATE") <> "" _
         AndAlso Request.QueryString("NOMBRES") <> "" _
         AndAlso Request.QueryString("EMECI") <> "" _
         AndAlso Request.QueryString("LOCALIDAD") <> "" _
         AndAlso Request.QueryString("ENTIDAD") <> "" _
         AndAlso Request.QueryString("email") <> "" _
         AndAlso Request.QueryString("CLAVE") <> ""

    End Function

    Private Sub GuardarDoctor()
        Dim ds As New DSRegistro
        Dim dr As DSRegistro.RegistroRow
        Dim drM As DSRegistro.MedicoRow
        Dim dsC As dsCiudades
        Dim dsmed As DataSet
        Dim rp As Boolean
        Dim sguid As String
        With New clsFARegistro
            dsmed = .GetMedicoByEmeci(txtEmeci.Text.Trim)
        End With
        lblExito.Text = "error"
        If Not dsmed Is Nothing AndAlso dsmed.Tables(0).Rows.Count > 0 Then

            dr = ds.Registro.NewRegistroRow




            dr.IdPais = "MX"
            dr.IdUsuario = "Administrador"




            dr.Status = "V"
            dr.Tipo = "M"
            dr.Clave = txtClave.Text

            dr.idRegistro = dsmed.Tables(0).Rows(0)("idRegistro")
            ds.Registro.AddRegistroRow(dr)
            drM = ds.Medico.NewMedicoRow
            'drM.IdEspecialidad = 32
            ' drM.PediatraPlus = False
            'drM.NoTarjeta = txtNoSSA.Text
            drM.Idmedico = dsmed.Tables(0).Rows(0)("idmedico")
            ''''' medico no tenia guid ''''


            ds.Medico.AddMedicoRow(drM)

            With New clsFARegistro
                rp = .GuardarRegistro(ds, True)
            End With


        End If

        If rp Then
            lblExito.Text = "Actualizo Correctamente..!"
        End If
        '  EnviaEmail(Request.QueryString("EMECI"), Request.QueryString("NOMBRES") & " " & Request.QueryString("APELL_PATE") & " " & Request.QueryString("APELL_MATE"), _
        '  Request.QueryString("email"), sguid)
        '  End If
    End Sub

    Private Sub EnviaEmail(ByVal emeci As String, ByVal nombre As String, ByVal email As String, ByVal guid As String)
        Dim eM As New MailMessage
        eM.Priority = MailPriority.Normal
        eM.BodyFormat = MailFormat.Text
        eM.Subject = "Registro Médico EMECI"
        eM.To = email
        eM.From = "soporte@emeci.com"
        eM.Bcc = "adrian@softwebmedia.com"
        eM.Body = "Bienvenido a Emeci.com, su petición ha sido procesada satisfactoriamente" & vbCrLf
        eM.Body &= New String("*", 100) & vbCrLf
        eM.Body &= "Nombre:" & nombre & vbCrLf
        eM.Body &= "Correo:" & email & vbCrLf
        eM.Body &= "Acceso a Expediente:" & guid & vbCrLf
        eM.Body &= New String("*", 100)
        Dim bnd As Boolean
        Try
            SmtpMail.SmtpServer = "localhost"
            SmtpMail.Send(eM)
            bnd = True
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)
        GuardarDoctor()
    End Sub
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<HTML>
	<HEAD>
		<title>Actualizar Pediatras</title>
		<LINK href="../../emeci.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<div id="MainWrapper">
			<form id="Form1" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px"
				method="post" runat="server">
				<table id="tableMainContent" cellSpacing="0" cellPadding="5" width="100%" border="0">
					<tr>
						<td class="mainTableLeftSide" vAlign="top">
						</td>
						<td class="mainTableRightSide">
							<TABLE id="Table1" width="100%" border="0">
								

								
								<TR>
									<TD vAlign="top" height="21">
										<DIV align="right"><LABEL>Emeci:</LABEL>
										</DIV>
									</TD>
									<TD vAlign="top" height="21">
										<asp:textbox id="txtEmeci" runat="server"></asp:textbox></TD>
								</TR>
								
								<TR>
									<TD vAlign="top">
										<DIV align="right"><LABEL>Clave:</LABEL>
										</DIV>
									</TD>
									<TD vAlign="top">
										<asp:TextBox id="txtClave" TextMode="Password" runat="server"></asp:TextBox></TD>
								</TR>
                                <tr>
                                    <td >

                                    </td>
                                    <td >
                                         <asp:Button runat="server" ID="btnGuardar" OnClick="btnGuardar_Click" Text="Guardar" />
                            <asp:Label ID="lblExito" Text="" runat="server"></asp:Label>
                                    </td>
                                </tr>
								
							</TABLE>
                           
						</td>
					</tr>
				</table>
			</form>
			<table id="tableFooter">
				<tr>
					<td>
						<div>
							EMECI - Expediente Médico Electrónico Clínico Internacional<br>
							Version 0.1
						</div>
					</td>
				</tr>
			</table>
		</div>
	</body>
</HTML>
