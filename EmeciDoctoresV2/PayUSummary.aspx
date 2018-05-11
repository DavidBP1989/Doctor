<%@ Page Language="C#" AutoEventWireup="true"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <p>Gracias Por Pagar Su Membresia</p>
        <p>Número de Operacion:<%= Request.QueryString["Operacion"].ToString()  %></p>
    </div>
    </form>
</body>
</html>
