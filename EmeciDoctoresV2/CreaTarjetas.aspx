<%@ Page Language="C#" AutoEventWireup="true"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
   
<script runat="Server">
   
    protected void Button1_Click(object sender, EventArgs e)
    {
        string notarjeta="";
        
        int cont = 0;
        string[] arrABC = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
        string letra;
        Random random;
        //genero valores de tarjeta
       
        random = new Random();
        
        System.Data.SqlClient.SqlConnection conn =
            new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["EmeciConnection"].ConnectionString);
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
        string strSql = "insert into datostarjeta(notarjeta,coordenada,dato) values('{0}','{1}','{2}')";
        conn.Open();
        cmd.Connection = conn;

        for (int cnt = 0; cnt < int.Parse(txtiter.Text); cnt++)
        {

            notarjeta = txt.Text + '-' + ( int.Parse(txt2.Text) + cnt).ToString("000#");    
            
            for (int i = 1; i <= 10; i++)
            {
                cont += 1;
                for (int j = 0; j <= 9; j++)
                {
                    letra = arrABC[j];
                    cmd.CommandText = string.Format(strSql, notarjeta, string.Concat(letra, i), random.Next(0, 999).ToString("00#"));
                    cmd.ExecuteNonQuery();
                }
            }
        }
        conn.Close();
    }
</script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        tarjeta:<asp:TextBox ID="txt" runat="server" Text=""></asp:TextBox>-<asp:TextBox ID="txt2" runat="server" Text=""></asp:TextBox>
        iter:<asp:TextBox ID="txtiter" runat="server" Text=""></asp:TextBox>
         <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Crear Tarjetas" />
    </div>
    </form>
</body>
</html>
