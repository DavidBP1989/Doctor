
<%@ Page Language="C#" AutoEventWireup="true"  %>
<%@ Import  Namespace="System.Data" %>
<%@ Import  Namespace="System.Net" %>
<%@ Import  Namespace="System.IO" %>
<%@ Import  Namespace="System.Security.Cryptography" %>
<%@ Import  Namespace="System.Xml" %>

<script runat="Server">
    string json = "";
    string tx_value = "40";
    protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.Form["pago"]))
            { 
                string strjson="";
                StreamReader sr = new StreamReader(Server.MapPath("/payutest.txt"));
                strjson = sr.ReadToEnd();

                string resp = getRequest("https://api.payulatam.com/payments-api/4.0/service.cgi", replacejson(strjson)); 
                sr.Close();
                processPayment(resp);
            }
            if (!IsPostBack)
            {

                string ApiKey = "5ualu0dhdlfptqa9fthtr2h78k";
                string merchantId = "516707";
                string referenceCode = "03240-0001-0028";
                
                string currency = "MXN";

                signature = ApiKey + "~" + merchantId + "~" + referenceCode + "~" + tx_value + "~" + currency;

                using (MD5 md5Hash = MD5.Create())
                {

                    signature = GetMd5Hash(md5Hash, signature);
                }

                using (MD5 md5Hash = MD5.Create())
                {
                    Random random = new Random();

                    sessionid = GetMd5Hash(md5Hash, Session.SessionID + GetUnixTimestamp(DateTime.Now));
                    usuarioid = random.Next(0, 99999).ToString("0000#");

                }
            }
        
        }

        private void processPayment(string xml)
        {
            System.Xml.XmlDocument XD;
            XD = new XmlDocument();
            XD.LoadXml(xml);
            XmlNodeList nls =  XD.SelectNodes("//code");
            if (nls.Count > 0)
            {
                if (nls[0].InnerText == "SUCCESS")
                {//ok
                    nls = XD.SelectNodes("//responseCode");
                    if (nls.Count > 0)
                    {
                        if (nls[0].InnerText == "APPROVED") // aprovado
                        {
                            Session["xml"] = xml;
                            nls[0].ParentNode.SelectSingleNode("//authorizationCode");
                            Response.Redirect("PayUSummary.aspx?operacion=" + nls[0].ParentNode.SelectSingleNode("//authorizationCode").InnerText);
                        }
                        else //Rejected
                        {
                            lblerror.Visible = true;
                            lblerror.InnerText = "Error Tarjeta Declinada";
                        }                          
                    }
                }
                else // error API
                {
                    lblerror.Visible = true;
                    lblerror.InnerText = "Error Servicio no Disponible";
                } 
            
            }
                
        }
        
         private string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }

        string sessionid { get { return Session["sessionid"].ToString(); } set { Session["sessionid"] = value; } }
        string usuarioid { get { return Session["usuarioid"].ToString(); } set { Session["usuarioid"] = value; } }
        string signature{ get{ return Session["signature"].ToString();}set{ Session["signature"] = value;}}
       

        private string replacejson(string json)
    {
        string[] args={ "03240-0001-0028", "Tarjeta Emeci Pago", "x", "pagoemeci.aspx","1",
        "4242424242424242","123","12","2015","ADR TEST","VISA"};





        json = json.Replace("{REFERENCECODE}", Request.Form["emeci"])
            .Replace("{DESCRIPTION}", "Tarjeta Emeci Pago")
            .Replace("{TOTAL}", tx_value)
            .Replace("{NUMBER}", Request.Form["notarjeta"])
            .Replace("{SECURITYCODE}", Request.Form["codigo"])
            .Replace("{MONTHYEAR}", Request.Form["anio"].ToString() + "/" + Request.Form["mes"].ToString())
            .Replace("{NAME}", Request.Form["nombretarjeta"])
          
            .Replace("{SIGNATURE}", signature)
            .Replace("{DEVICESESSIONID}", Session.SessionID)
            .Replace("{PAYMENTMETHOD}", Request.Form["pago"]);
            
        return json;
    }

    private string getRequest(string url, string data)
    {
        HttpWebRequest request;

        string requestBody = data;
        string responseBody = string.Empty;
        byte[] byteData = UTF8Encoding.UTF8.GetBytes(requestBody.ToString());

        request = (HttpWebRequest)HttpWebRequest.Create(url);
        //request.Credentials = new NetworkCredential("tripadvisor", "Tr1padv2013");
        //request.PreAuthenticate = true;
        request.ContentType = "application/json";
        request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; BOIE9;ENUS)";
        request.Accept = "*/*";
        request.Method = "POST";

        ServicePointManager.ServerCertificateValidationCallback +=
          (sender, cert, chain, sslPolicyErrors) => true;
        
        
        if (data != "")
        {
            request.ContentLength = byteData.Length;

            // Write data 
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

        }
        // Get response 
        using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
        {
            // Get the response stream 
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                responseBody = reader.ReadToEnd();
            }
        }

        return (responseBody);
    }

    public readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

    // equivalent to PHP mktime :
    public int GetUnixTimestamp(DateTime dt)
    {
        TimeSpan span = dt - UnixEpoch;
        return (int)span.TotalSeconds;
    }

</script>
<form action ="PayU.aspx" method="post">
<table width="100%" border="0"  cellpadding="2" cellspacing="2">
    <tbody>
        <tr>
            <td>
                
                <img src="~/img/Img_AmericanExpress.jpg" border="0">
            </td>
           
            <td>
               
                <img src="~/img/Img_MasterCard.jpg" border="0">
            </td>
            <td>
               
                <img src="~/img/Img_Visa.jpg" border="0">
            </td>
            <td>
                <img src="~/img/oxxo.jpg" border="0">
            </td>
        </tr>
  
        <tr >
            <td>
                &nbsp;</td>
           
            <td colspan="3">
                <table>
                    <tr>
                        <td>
                           Número Emeci
                        </td>
                    </tr>
                       <tr>
                        <td>
                            <input type ="text" name="emeci" value="03240-0001-0028" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Número Tarjeta 
                        </td>
                    </tr>
                       <tr>
                        <td>
                            <input type ="text" name="notarjeta" value="4772912502882891" />
                        </td>
                    </tr>
                       <tr>
                        <td>
                            Nombre Tarjeta
                        </td>
                    </tr>
                       <tr>
                        <td>
                           <input type ="text" name="nombretarjeta" value="REJECTED" />
                        </td>
                    </tr>

                       <tr>
                        <td>
                           Expiera Tarjeta MM/YYYY
                        </td>
                    </tr>
                       <tr>
                        <td>
                           <input type ="text" name="mes" value="02" />/<input type ="text" name="anio" value="2016" />
                        </td>
                    </tr>
                       <tr>
                        <td>
                           Código de Seguridad
                        </td>
                    </tr>
                      <tr>
                        <td>
                           <input type ="text" name="codigo" value="123" />
                        </td>
                    </tr>

                </table>   
            
            </td>
        </tr>
  
        <tr align="center">
            <td>
                &nbsp;</td>
            <td colspan="2">
                <input type="submit" value="Pagar" />
                &nbsp;</td>
            <td><span id="lblerror" visible="false" runat="server"></span></td>
        </tr>
  
    </tbody>

    <p style="background:url(https://maf.pagosonline.net/ws/fp?id=<%= this.sessionid%><%= this.usuarioid %>"></p>

  <img src="https://maf.pagosonline.net/ws/fp/clear.png?id=<%= this.sessionid%><%= this.usuarioid %>">

  <script src="https://maf.pagosonline.net/ws/fp/check.js?id=<%= this.sessionid%><%= this.usuarioid %>"></script>

  <object type="application/x-shockwave-flash"

  data="https://maf.pagosonline.net/ws/fp/fp.swf?id=<%= this.sessionid%><%= this.usuarioid %>" width="1" height="1"

  id="thm_fp">

  <param name="movie" value="https://maf.pagosonline.net/ws/fp/fp.swf?id=<%= this.sessionid%><%= this.usuarioid %>" />

</object>

</table>
    </form>


