<%@ Control Language="C#" AutoEventWireup="true"  %>
<%@ Import  Namespace="System.Data" %>

<script runat="Server">
    int idpac=0;
    protected void Page_Load(object sender, EventArgs e)
        {
            DataSet   dsPac = new DataSet();

            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(
                    
                "select * from paciente where idpaciente =" + HttpContext.Current.Session["idpaciente"].ToString(),
                System.Configuration.ConfigurationManager.AppSettings["connection"]);
            
            da.Fill(dsPac);

            if (dsPac.Tables[0].Rows[0].IsNull("acepto") || !(Boolean)dsPac.Tables[0].Rows[0]["acepto"])
            {
                idpac = (int)this.Session["idpaciente"];
               
            }
            else dialog.Visible = false;
        }

</script>


  <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css">
  <script src="//code.jquery.com/jquery-1.10.2.js"></script>
  <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>

    
<script type='text/javascript'>    //<![CDATA[ 
    var showdiag=<%=idpac %>;
    if (showdiag>=1)
    {
        $(window).load(function () {
            $('#<%= dialog.ClientID %>').dialog({
                close: function () {

                },
                buttons: {
                    Acepto: function () {
                        // Mandar la peticion en ajax para que actualice el dato
                        $.get("Acepto.aspx?idpaciente=<%= idpac %>","" );
                        $(this).dialog('close');
                    }

                },
                width: 800,
                title: "Aviso de Contrato"
            })
        });
    }
    
            
    
</script>



<div id="dialog" runat="server" style="text-align:left" >
    <p>
    Contrato de Membresía que celebran por una parte EMECI, S.C., en adelante "EMECI" y por la otra el titular de este contrato, identificado por el número de membresía indicado en la carátula del mismo, a quien para estos efectos se le denominará "EL USUARIO", al tenor de las siguientes declaraciones y cláusulas:
</p>
    <p>
I. DECLARACIONES:
1.	Declara EMECI, ser una Sociedad Civil Constituida de conformidad con la escritura número 36592 expedida por el Notario Público número  11 de la ciudad de La Paz, Baja California Sur, México, licenciado Jorge L. Álvarez Gámez con fecha 25 de Septiembre del 2007 e inscrita en el Registro Público de la Propiedad y del Comercio con el número 24 con fecha 23 de Octubre del 2007, y cuenta con el Registro Federal de Contribuyentes EME070925L5A.
2.	Que tiene su domicilio principal ubicado en Calle República # 1240 Colonia Colina de la Cruz, en la ciudad de La Paz, estado de Baja California Sur, México
3.	Que su principal objeto social consiste en "La combinación de recursos, esfuerzos, y conocimientos para brindar el servicio de preparación y acceso vía Internet a expedientes médicos clínicos".
4. Que tiene celebrado un contrato para el procesamiento y almacenamiento de datos con la empresa "Clinical Medical Data Processing, Inc." y que esta empresa dispone en los Estados Unidos de Norteamérica y en España de la infraestructura y tecnología necesaria para brindar a los miembros de EMECI el servicio de acceso a sus Expedientes Médicos Clínicos desde cualquier parte del mundo por medio de Internet con seguridad de tipo bancario.
5. Declara el "EL USUARIO" estar interesado en recibir los servicios de "EMECI" referente al mantenimiento de un expediente médico clínico con el pago de la membresía y la utilización de los servicios vía Internet desde cualquier parte del mundo, y acepta los términos de este instrumento.
II. CLÁUSULAS:
PRIMERA: "EMECI" vende a favor de "EL USUARIO" quién compra la membresía cuyo número se menciona en la carátula de este contrato.
SEGUNDA: "EL USUARIO" se obliga a pagar a EMECI. por concepto de mantenimiento de la membresía materia de este contrato, la cantidad de $250.00 (Doscientos cincuenta pesos 00/100 M.N.) anuales.
TERCERA: Este contrato será obligatorio para el "EL USUARIO" y EMECI desde el momento que el "EL USUARIO" pague a "EMECI" el costo de la membresía y haga uso de los servicios que la misma ampara.
CUARTA: El presente contrato dejará de surtir efectos y será dado por terminado sin necesidad de declaración judicial ni aviso previo, en caso de que el "EL USUARIO" omita el pago de su cuota de mantenimiento.
QUINTA: El presente contrato tendrá una vigencia de un año y se renovará automáticamente por el mismo periodo, en el momento en que el usuario haga el pago de su cuota de mantenimiento.
SEXTA: "EMECI” en ningún caso y bajo ninguna circunstancia será responsable del estado de salud del "EL USUARIO" o dependientes económicos o cualquier persona que se vea afectada de manera directa o indirecta por el uso de su Expediente Médico Clínico.
“EL USUARIO” reconoce con el uso del servicio, que este contrato es únicamente para efectos de obtener servicios de mantenimiento de un Expediente Médico Clínico y la asesoría necesaria para su llenado y actualización, por medio de correo electrónico. "EL USUARIO" de este contrato libera a "EMECI”, sus socios, funcionarios, empleados y promotores de cualquier responsabilidad o daño que pueda sufrir éste, sus dependientes económicos o cualquier persona relacionada, a consecuencia de negligencia médica o cualquier otra afectación o daño causado por los prestadores directos de los servicios de salud. Asimismo, dado que el Expediente Médico Electrónico Clínico Internacional que se me otorga  “EL USUARIO” es un documento de mi propiedad, sin coerción alguna, acepto no utilizarlo como instrumento legal.
SÉPTIMA: "EMECI” garantiza, con las limitaciones normales, el buen funcionamiento del programa y almacenamiento de datos, efectuando los debidos respaldos diarios en equipos ubicados en diferentes partes del mundo, de modo tal que siempre sea posible su recuperación en caso necesario.
OCTAVA: "EMECI” otorga las siguientes garantías de servicio:
1.- Los afiliados gozarán de los servicios que brinda "EMECI, desde el primer día de contratación y pago total de su membresía con sólo activar su tarjeta, no tendrán restricción limitación ni exclusión por estado de salud ni por edad.
2.- Garantía de servicio.: Si el servicio de mantenimiento del Expediente Médico Clínico, no se efectúa de manera satisfactoria para el usuario, EMECI le reembolsará la cuota de mantenimiento del periodo (año) en cuestión, con la sola solicitud por escrito del Usuario explicando sus causas.
NOVENA: El USUARIO será responsable del buen uso de su tarjeta "EMECI”. En caso de robo o extravió deberá notificarlo inmediatamente a "EMECI", para efecto de cancelación y la reposición correspondiente (toda reposición tendrá un costo)
DECIMA: La membresía podrá ser cancelada a petición del "EL USUARIO", únicamente durante el primer mes de iniciación de la vigencia siempre y cuando exista causa justificada imputable a "EMECI". En el entendido que la vigencia es anual.
DÉCIMA PRIMERA: Como primera instancia en caso de controversia se le dará atribución a la Procuraduría Federal del Consumidor y en caso de no resolverse mediante esta vía, la interpretación y cumplimiento de este contrato, las partes se someten a la jurisdicción y competencia de los tribunales del fuero común de la Ciudad de La Paz. Baja California Sur, renunciando expresamente al fuero que por razón de sus domicilios presentes o futuros o por cualquier concepto pudieran corresponderles.
</p>


    <h3>AVISO DE PRIVACIDAD</h3>
    <p> 
    EMECI S.C., con Registro Federal de Contribuyentes EME070925L5A, con domicilio principal en Calle República # 1240 Colonia Colina de la Cruz, en la ciudad de La Paz y estado de Baja California Sur, México, hace constar que su principal objetivo social consiste en “La combinación de recursos, esfuerzos y conocimientos para brindar el servicio de preparación y accesos vía Internet a Expedientes Médicos Clínicos”.
     </p>
    <p>
        Su información personal será utilizada para proveer los servicios y productos que ha solicitado, para informarle sobre cambios en los mismos y evaluar la calidad del servicio que le brindamos. Para las finalidades antes mencionadas requerimos obtener, de la manera más completa posible, sus datos personales generales, teléfonos de contacto, así como su correo electrónico, estos dos últimos vigentes y actualizados, datos que la compañía EMECI S.C. se compromete a proteger por considerarlos como sensibles según la Ley Federal de Protección de Datos Personales en Posesión de los Particulares.
      </p>
    <p>Usted será responsable del resto del llenado de la información en su expediente médico, el cual está compuesto de texto, imágenes y opciones múltiples, por medio del acceso al sistema que EMECI S.C. le ha proporcionado, asimismo EMECI  S.C. solo será responsable de otorgarle los medios necesarios para el resguardo y conservación de los datos de su expediente, sin que la compañía tenga acceso a esta información codificada. 
     </p>
    <p>Usted tiene derecho, en cualquier momento, de acceder, rectificar y cancelar sus datos personales, así como de oponerse al manejo de los mismos o revocar el consentimiento que para tal fin nos haya otorgado, a través de los procedimientos que hemos implementado. Para conocer dichos procedimientos, los requisitos y plazos, se puede poner en contacto con nuestro Departamento de Información del Expediente Médico Electrónico al correo info@emeci.com o visitar nuestra página de Internet www.emeci.com 
     </p>
    <p>EMECI S.C. posee la infraestructura y tecnología necesaria para brindarle a los miembros de EMECI el servicio de acceso a sus Expedientes Médicos Clínicos desde cualquier parte del mundo por medio de Internet con seguridad de tipo bancario, con un certificado de seguridad (Secure Certificate Authority - SSL) para el almacenamiento y procesamiento de su información.
   </p>
      <p>Si usted no manifiesta su oposición para que sus datos personales sean almacenados y procesados por esta empresa, se entenderá que ha otorgado su consentimiento y en caso contrario, le pedimos envíe un correo a info@emeci.com para expresar su oposición o en su caso, solicitar la atención personalizada de un asesor para cualquier aclaración.
    </p><p>Si usted desea dejar de recibir mensajes promocionales de nuestra parte puede solicitarlo a través del correo electrónico soporte@emeci.com
   </p><p>Cualquier modificación a este aviso de privacidad podrá consultarla directamente en su expediente médico que EMECI S.C. le ha proporcionado o será enviada al correo electrónico que nos proporcionó. 
</p><p>Fecha de la última actualización 01 de Enero del 2015
    <p>
   

</div>