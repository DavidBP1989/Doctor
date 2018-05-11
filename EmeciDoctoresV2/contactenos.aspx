<%@ Page Language="C#" AutoEventWireup="true"  %>
<%@ Import Namespace="System.Net.Mail" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.IO" %>

<!DOCTYPE html>
<script runat="server">

    protected void btnMensaje_Click(object sender, EventArgs e)
    {
        if (this.Page.IsValid && IsGoogleCaptchaValid() )
        {
            MailMessage eM = new MailMessage(); 
        eM.Priority = MailPriority.Normal;
        eM.IsBodyHtml = false;
        eM.Subject = "Contacto EMECI";
        eM.To.Add("info@emeci.com");
        eM.From = new MailAddress( "info@emeci.com");
       
        eM.Body = new string('*', 100)  + "\r\n";
        eM.Body += "Nombre:" + txtnombre.Value + "\r\n";
        eM.Body += "Correo:" + txtcorreo.Value + "\r\n";
        if(chkMedico.Checked)
            eM.Body += "Médico Especialista:SI" + "\r\n";
        else
            eM.Body += "Paciente:SI" + "\r\n";
        
        eM.Body += "Télefono:" + txtTelefono.Value + "\r\n";
        eM.Body += "Comentario:" + txtcomentario.Value + "\r\n";
        eM.Body += new string('*', 100);
        Boolean bnd=false;
        try
        {
            
            SmtpClient client = new SmtpClient();
            client.Host = ConfigurationManager.AppSettings["Host"];
            client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            string m = ConfigurationManager.AppSettings["MailFrom"];
            string p = ConfigurationManager.AppSettings["passwordMail"];
            client.Credentials = new System.Net.NetworkCredential(m, p);
            
            
            client.Send(eM);
            
         
            bnd = true;
        }
        catch( Exception ex)
        {
            Response.Write(ex.Message);
        }
        if( bnd )
            Response.Redirect("exito.aspx");
        
        }
        
    }
    
    
    private Boolean IsGoogleCaptchaValid()
    {
        
             string userIP = Request.UserHostAddress;

            string privateKey = ConfigurationManager.AppSettings["ReCaptcha.PrivateKey"];

            string postData = string.Format("?secret={0}&remoteip={1}&response={2}",
                                         privateKey,
                                         userIP,
                                         Request.Form["g-recaptcha-response"]);

          

            // Create web request
            WebRequest request = WebRequest.Create("https://www.google.com/recaptcha/api/siteverify" + postData);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
           
            Stream dataStream = request.GetRequestStream();
            //dataStream.Write(postDataAsBytes, 0, postDataAsBytes.Length);
            dataStream.Close();

            // Get the response.
            WebResponse response = request.GetResponse();

            using (dataStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    string responseFromServer = reader.ReadToEnd();

                    if (!(responseFromServer.IndexOf( "success\": true")>0))
                        return false;
                    else return true;
                }
            }
        
        
        /////////////////////////////////////////////////////////
    //    try
    //    {
    //    string  recaptchaResponse  = Request.Form["g-recaptcha-response"];
    //    if( ! String.IsNullOrEmpty(recaptchaResponse) )
    //    {
    //        Net.WebRequest request  = Net.WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6Lc9UgITAAAAAGBf9TH0MrrTpbkQjUfPTB7wtNWQ&response=" + recaptchaResponse);
    //        request.Method = "POST";
    //        request.ContentType = "application/json; charset=utf-8";
    //       string  postData  = "";

    //        //'get a reference to the request-stream, and write the postData to it
    //        using IO.Stream s  = request.GetRequestStream()
    //        {
    //            using  new IO.StreamWriter(s) sw
    //            {  
    //                sw.Write(postData);
    //            }
    //        }
    //        //''get response-stream, and use a streamReader to read the content
    //        using IO.Stream s  = request.GetResponse().GetResponseStream()
    //        {
    //            using new IO.StreamReader(s) sr
    //            {  
    //                //'decode jsonData with javascript serializer
    //                var jsonData = sr.ReadToEnd()
    //                if (jsonData = "{" + "\r\n" + "  "\"success\"": true" + "\r\n" + "}" )
    //                    return true;
                    
    //            }
    //        }
    //    }
    //    }
    //Catch ex As Exception
    //    'Dont show the error
    //End Try
    //return false;
    }
    
</script>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src='https://www.google.com/recaptcha/api.js'></script>
</head>
<body>
   <form id="Form1" method="post" runat="server">
				<table id="tableMainContent" width="100%" border="0" cellspacing="0" cellpadding="0">
					<tr>
						<td colSpan="2">							
								<table id="frmContacto" width="75%" border="0" align="center" cellpadding="4" cellspacing="0">
									<tr>
										<td><label>Nombre y Apellido:</label></td>
										<td><input name="txtnombre" type="text" id="txtnombre" size="45" runat="server">
											<BR>
											<asp:RequiredFieldValidator id="tfvNombre" runat="server" ErrorMessage="Se Requiere Nombre" ControlToValidate="txtnombre"
												Display="Dynamic"></asp:RequiredFieldValidator></td>
									</tr>
									<tr>
										<td>&nbsp;</td>
										<td><p>
												<input type="checkbox" name="chkMedico" id="chkMedico" runat="server"> Soy 
												Médico Especialista</p>
										</td>
									</tr>
									<tr>
										<td><label>No. Telefonico para Contácto:<br>
      (Incluir clave lada)</label></td>
										<td><input name="textfield2" type="text" id="txtTelefono" size="40" runat="server">
											<BR>
											<asp:RequiredFieldValidator id="rfvTelefono" runat="server" ErrorMessage="Se Requiere No Telefonico" ControlToValidate="txtTelefono"
												Display="Dynamic"></asp:RequiredFieldValidator></td>
									</tr>
									<tr>
										<td><label>Correo Electronico:</label></td>
										<td><input name="textfield3" type="text" id="txtcorreo" size="50" runat="server">
											<BR>
											<asp:RequiredFieldValidator id="rfvCorreo" runat="server" ErrorMessage="Se Requiere Correo" ControlToValidate="txtcorreo"
												Display="Dynamic"></asp:RequiredFieldValidator>
											<asp:RegularExpressionValidator id="revCorreo" runat="server" ErrorMessage="Correo Invalido" ControlToValidate="txtcorreo"
												Display="Dynamic" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator></td>
									</tr>
									<tr>
										<td><label>Comentarios:</label></td>
										<td><textarea name="textarea" id="txtcomentario" cols="45" rows="5" runat="server"></textarea><BR>
											<asp:RequiredFieldValidator id="rfvComentarios" runat="server" ErrorMessage="Se Requiere Comentario" ControlToValidate="txtcomentario"
												Display="Dynamic"></asp:RequiredFieldValidator></td>
									</tr>
									<tr>
										<td> </td>
										<td><div class="g-recaptcha" data-sitekey="6Lc9UgITAAAAACALBQtsf4BB11I2jRddQCBgcj2G"></div> 
											<asp:Button id="btnMensaje" Text="Enviar Mensaje" runat="server" OnClick="btnMensaje_Click"></asp:Button></td>
									</tr>
								</table>
								<!-- Mensaje de Envio de Informacion -->
								<div style="DISPLAY:none">
									<p>Gracias por ponerse en contacto con EMECI, uno de nuestros asesores pronto se 
										pondra en contacto con usted para dar seguimiento a su mensaje.
									</p>
									<div>
										<!-- Termina mensaje -->
									</div>
								</div>
							</div>
						</td>
					</tr>
					<tr>
						<td class="mainTableLeftSide">
						</td>
					</tr>
				</table>
			</form>
</body>
</html>
