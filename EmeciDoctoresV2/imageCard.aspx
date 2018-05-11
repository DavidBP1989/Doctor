<%@ Page Language="C#" AutoEventWireup="true"  %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace= "System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import  Namespace="System.Data" %>
<script runat ="server">

    private void Page_Load(object sender, System.EventArgs e)
    {


        System.Data.SqlClient.SqlConnection conn =
                new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings["connection"]);
        System.Data.SqlClient.SqlDataAdapter sqlda = new System.Data.SqlClient.SqlDataAdapter("select * from  datostarjeta where notarjeta = '"
            + Request.QueryString["notarjeta"].ToString() + "' order by substring(coordenada,1,1),cast( substring(coordenada,2,2) as int) ", conn);
        System.Data.DataSet ds = new DataSet();
        try
        {

            sqlda.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {


            }
            else return;
        }
        catch
        { return; }
        
        
        
        //Load the Image to be written on.
        Bitmap bitMapImage = new System.Drawing.Bitmap(Server.MapPath("img.jpg"));
        Graphics graphicImage = Graphics.FromImage(bitMapImage);

        //Smooth graphics is nice.
        graphicImage.SmoothingMode = SmoothingMode.AntiAlias;

        //I am drawing a oval around my text.
       // graphicImage.DrawArc(new Pen(Color.Red, 3), 90, 235, 150, 50, 0, 360);

        //Write your text.
        graphicImage.DrawString(Request.QueryString["notarjeta"].ToString(), new Font("Arial", 20, FontStyle.Bold), SystemBrushes.WindowText, new Point(50, 175));

       
        int cont = 0; int shor = 30; int salv = 17;
        int px = 545; int py = 41;
        for (int i = 1; i <= 10; i++)
        {
            
            py = 41;
            for (int j = 0; j <= 9; j++)
            {


                graphicImage.DrawString(ds.Tables[0].Rows[cont]["Dato"].ToString(), 
                    new Font("Arial", 9, FontStyle.Bold), SystemBrushes.WindowText, new Point(px, py));

                cont += 1;
                py = py + salv;
            }
            px = px + shor;
            
        }
        
        
        
        
        //graphicImage.DrawString("000", new Font("Arial", 9, FontStyle.Bold), SystemBrushes.WindowText, new Point(542, 41));
        //graphicImage.DrawString("999", new Font("Arial", 9, FontStyle.Bold), SystemBrushes.WindowText, new Point(572, 41));

        //graphicImage.DrawString("999", new Font("Arial", 9, FontStyle.Bold), SystemBrushes.WindowText, new Point(572, 58));
        
        //Set the content type
        Response.ContentType = "image/jpeg";

        //Save the new image to the response output stream.
        bitMapImage.Save(Response.OutputStream, ImageFormat.Jpeg);

        //Clean house.
        graphicImage.Dispose();
        bitMapImage.Dispose();
    }
</script>