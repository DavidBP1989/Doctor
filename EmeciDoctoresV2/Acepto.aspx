<%@ Page Language="C#" AutoEventWireup="true"  %>

<%@ Import  Namespace="System.Data" %>


<script runat="Server">
    int idpac=0;
    protected void Page_Load(object sender, EventArgs e)
        {

            System.Data.SqlClient.SqlConnection conn = 
                    new System.Data.SqlClient.SqlConnection( System.Configuration.ConfigurationManager.AppSettings["connection"]);
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("update paciente set acepto=1 where idpaciente = " 
                + Request.QueryString["idpaciente"].ToString(),conn);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                    
            }
            catch
            { }

        }

</script>

