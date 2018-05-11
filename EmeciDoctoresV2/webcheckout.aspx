<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Import  Namespace="System.Data" %>
<%@ Import  Namespace="System.Net" %>
<%@ Import  Namespace="System.IO" %>
<%@ Import  Namespace="System.Security.Cryptography" %>
<%@ Import  Namespace="System.Xml" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="Scripts/jquery-1.7.1.min.js"></script>
    <script src="js/jquery.maskedinput.js"></script>
    
</head>

    
<script runat="Server">
    public string signature = "";
    public string tx_value = "250";
    public string ApiKey = "5ualu0dhdlfptqa9fthtr2h78k";
    public string merchantId = "516707";
    public string referenceCode = DateTime.Now.ToString("ddMMMyyyymmss");
    public string accountId = "518172";
    string currency = "MXN";


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

    private void getSignature()
    {
        signature = ApiKey + "~" + merchantId + "~" + referenceCode + "~" + tx_value + "~" + currency;
        using (MD5 md5Hash = MD5.Create())
        {

            signature = GetMd5Hash(md5Hash, signature);
        }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {

        getSignature();
        
    }

    protected void ddlPrecio_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlPrecio.SelectedIndex == 1) tx_value = "2000";

        getSignature();
    }
</script>

<body>
    
    <div>
     <form id="form2" runat="server"> 
        <asp:DropDownList ID="ddlPrecio" runat="server" OnSelectedIndexChanged="ddlPrecio_SelectedIndexChanged" AutoPostBack="true">
            <asp:ListItem Value="250" Text="Anual - 250.00"></asp:ListItem>
            <asp:ListItem value="2000" Text="Unico - 2,000.00"></asp:ListItem>
        </asp:DropDownList>
    </form>
   <form id="form" method="post"  target="ipayu"  action="https://gateway.payulatam.com/ppp-web-gateway/">
  <input name="merchantId"    type="hidden"  value="<%= merchantId %>">
  <input name="accountId"     type="hidden"  value="<%= accountId %>">
       <asp:RequiredFieldValidator ID="rfvdesc" runat="server" ControlToValidate="description" ErrorMessage="*" Text="Falta Descripción"></asp:RequiredFieldValidator>
  <input  id="description" name="description" data-mask="99999-9999-9999"  type="text"    >
  <input name="referenceCode" type="hidden"  value="<%= referenceCode %>">
  <input name="amount"        type="hidden"  value="<%= tx_value %>">   
  <input name="tax"           type="hidden"  value="0"  >
  <input name="taxReturnBase" type="hidden"  value="0" >
  <input name="currency"      type="hidden"  value="<%=currency %>">
  <input name="signature"     type="hidden"  value="<%=signature %>" >
  <input name="test"          type="hidden"  value="0" >
  <input name="buyerEmail"    type="hidden"  value="info@emeci.com">
  <input name="lng"    type="hidden"  value="esp">         
  <input name="Submit" onclick="document.getElementById('ipayu').style.display = 'block'; document.getElementById('form').style.display = 'none'; return true;" type="submit"  value="Enviar" >
</form>
        <iframe id="ipayu" name="ipayu" style="  clear:both; border-style: none; width:820px; height:880px; list-style-type: none; table-layout: auto; overflow: hidden;">

        </iframe>
    </div>
  <script>
      $(document).ready(function () {
          
          $('#description').mask('99999-9999-9999');
      });
         </script>

</body>
</html>
