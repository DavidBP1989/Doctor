using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace EmeciDoctoresV2.Models
{
    public class Log
    {
        public Log()
        {

        }

        public static void write(string message) 
        {
            try
            {
                string pathlog = System.Configuration.ConfigurationManager.AppSettings["RutaLog"].ToString();
                try
                {
                    if (!System.IO.Directory.Exists(pathlog))
                    {
                        System.IO.Directory.CreateDirectory(pathlog);
                    }
                }
                catch (Exception ex)
                {
                }

                string filename = string.Format("Log_{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));

                System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("{0}{1}", pathlog, filename), true);
                sw.WriteLine(String.Format("{0}", DateTime.Now.ToString()));

                sw.WriteLine(String.Format("Message:"));
                sw.WriteLine(String.Format("{0}", message));
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
            }
        }
    }
}