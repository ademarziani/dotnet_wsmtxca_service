using System.IO;

namespace WSMTXCA_SRV.Util
{
    class Log
    {
        private static StreamWriter arclog;

        public static void crearLog(string archivoCompleto)
        {
            arclog = File.CreateText(archivoCompleto);
            arclog.Close();
        }

        public static void guardarLog(string logMessage, string archivoCompleto = "")
        {
            string auxArchivo;

            if (archivoCompleto == "")
                auxArchivo = Variables.DIRLOG;
            else
                auxArchivo = archivoCompleto;

            if (!File.Exists(auxArchivo))
            {
                crearLog(auxArchivo);
            }

            arclog = File.AppendText(auxArchivo);
            arclog.WriteLine(logMessage);
            arclog.Flush();
            arclog.Close();
        }
    }
}
