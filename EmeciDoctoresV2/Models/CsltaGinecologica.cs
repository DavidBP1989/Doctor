
public class CsltaGinecologica
{
    public int edad { get; set; }
    public int peso { get; set; }
    public int talla { get; set; }
    public int masa { get; set; }
    public int edadMenarca { get; set; }
    public int temperatura { get; set; }
    public int pres_sistolica { get; set; }
    public int pres_diastolica { get; set; }
    public string fechaMestruacion { get; set; }
    public int gestas { get; set; }
    public int paragestas { get; set; }
    public int cesareas { get; set; }
    public int abortos { get; set; }
    public int recienNacios { get; set; }
    public int mortinatos { get; set; }
    public int edadSexual { get; set; }
    public int menacma { get; set; }
    public string ocupacion { get; set; }
    public string escolaridad { get; set; }
    public int grupoRH { get; set; }
    public int estadoCivil { get; set; }
    public string religion { get; set; }


    public bool oligomenorrea { get; set; } = false;
    public bool proimenorrea { get; set; } = false;
    public bool hipermenorrea { get; set; } = false;
    public bool dismenorrea { get; set; } = false;
    public bool dispareunia { get; set; } = false;
    public bool leucorrea { get; set; } = false;
    public bool lactorrea { get; set; } = false;
    public bool amenorrea { get; set; } = false;
    public bool metrorragia { get; set; } = false;
    public bool otros { get; set; } = false;



    public int pareja { get; set; }
    public string motivo { get; set; }
    public string exploracion { get; set; }

    public CsltaGinecologica()
    {

    }
}