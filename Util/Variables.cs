using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace WSMTXCA_SRV.Util
{
    class Variables
    {
        public readonly static Int64 CUIT;
        public readonly static string DIRRAIZ;
        public readonly static string DIRLOG;
        public readonly static string LOGDEARCHIVOS;
        public readonly static string DIRTXT;
        public readonly static string DIRBKP;
        public readonly static string DIRERR;
        public readonly static string DIROK;
        public readonly static string DIRPDF;
        public readonly static string DIRTICKET;
        public readonly static string URL;
        public readonly static string SEP;
        public readonly static string ENTER;
        public readonly static bool IMPPDF;
        public readonly static bool ACTBD;
        public readonly static Int64 SEGUNDOS;
        public readonly static string CADENACONEXION;

        public readonly static string EMPRESA;
        public readonly static string CONDFIS;
        public readonly static string EMAIL;
        public readonly static string DIRECCION;
        public readonly static string INIACTIV;
        public readonly static string MUNICIPIO;
        public readonly static string NROIIBB;
        public readonly static bool ULTCOMP;
        public readonly static int LOGO_WIDTH;
        public readonly static int LOGO_HEIGHT;

        static Variables()
        {
            bool esHomo;

            XmlDocument config = new XmlDocument();
            XmlNode xmlNodo;

            DIRRAIZ = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            config.Load(DIRRAIZ + "\\configuracion.xml");

            xmlNodo = config.GetElementsByTagName("configuracion")[0];

            SEP = "||";
            ENTER = System.Environment.NewLine;
            LOGDEARCHIVOS = DateTime.Now.ToString("yyyyMMdd").Trim() + ".log";
            DIRLOG = DIRRAIZ + "\\log.log";
            DIRTXT = xmlNodo["dirtxt"].InnerText;
            DIRBKP = xmlNodo["dirbkp"].InnerText;
            DIRERR = xmlNodo["direrr"].InnerText;
            DIROK = xmlNodo["dirok"].InnerText;
            DIRPDF = xmlNodo["dirpdf"].InnerText;
            IMPPDF = xmlNodo["imprimePdf"].InnerText == "1";
            ACTBD = xmlNodo["actualizaBD"].InnerText == "1";
            CUIT = Int64.Parse(xmlNodo["cuit"].InnerText);
            SEGUNDOS = Int64.Parse(xmlNodo["segundos"].InnerText);
            esHomo = xmlNodo["homologacion"].InnerText == "1";
            
            EMPRESA = xmlNodo["empresa"].InnerText;
            CONDFIS = xmlNodo["condfiscal"].InnerText;
            EMAIL = xmlNodo["email"].InnerText;
            DIRECCION = xmlNodo["direccion"].InnerText;
            INIACTIV = xmlNodo["inicioactividad"].InnerText;
            MUNICIPIO = xmlNodo["municipio"].InnerText;
            NROIIBB = xmlNodo["nroiibb"].InnerText;
            ULTCOMP = xmlNodo["ultcomp"].InnerText == "1";
            LOGO_WIDTH = int.Parse(xmlNodo["widthLogo"].InnerText);
            LOGO_HEIGHT = int.Parse(xmlNodo["heightLogo"].InnerText);

            if (esHomo)
            {
                DIRTICKET = xmlNodo["dirticketHomo"].InnerText;
                URL = "https://fwshomo.afip.gov.ar/wsmtxca/services/MTXCAService";
                CADENACONEXION = xmlNodo["conexionHomo"].InnerText;
            }
            else
            {
                DIRTICKET = xmlNodo["dirticketProd"].InnerText;
                URL = "https://serviciosjava.afip.gob.ar/wsmtxca/services/MTXCAService";
                CADENACONEXION = xmlNodo["conexionProd"].InnerText;
            }
        }
    }
}
