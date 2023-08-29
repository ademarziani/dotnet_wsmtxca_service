using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using WSMTXCA_SRV.Entidades;
using WSMTXCA_SRV.Util;

namespace WSMTXCA_SRV.App
{
    class CargaComprobante
    {
        public void autorizarComprobantes(bool reimp)
        {
            DirectoryInfo directory = new DirectoryInfo(Variables.DIRTXT);
            FileInfo[] files = directory.GetFiles("*."+(reimp ? "reimp" : "json"));
            StreamReader objReader;
            Comprobante objetoComp;
            String json = "";
            int cantArchivos = files.Count();
            int nroReg = 0;

            string timestampArchivo = DateTime.Now.ToString("yyyyMMddhhmmss");
            string arcAnterior;
            string arcNuevo;
            string archivo;

            if (cantArchivos > 0)
            {
                ServicioAFIP.iniciar();

                foreach (FileInfo file in files)
                {
                    // Paso de JSON a String
                    try
                    {
                        // Leo el archivo y guardo todo el contenido en la variable 'json'
                        arcAnterior = file.FullName;
                        objReader = new StreamReader(arcAnterior);
                        json = objReader.ReadToEnd();
                        objReader.Close();

                        // Una vez leido el contenido, cambio la extension y lo muevo a bkp
                        arcNuevo = Path.ChangeExtension(arcAnterior, ".tmp");
                        System.IO.File.Move(arcAnterior, arcNuevo);

                        archivo = Path.ChangeExtension(file.Name, ".tmp");
                        System.IO.File.Move(Variables.DIRTXT + archivo, Variables.DIRBKP + archivo + "_" + timestampArchivo);

                        objetoComp = JsonConvert.DeserializeObject<Comprobante>(json);
                        objetoComp.nombreArchivo = file.Name;
                        objetoComp.setTipoComprobante();
                        nroReg = objetoComp.nroRegistro;

                        if (reimp)
                        {
                            if (Variables.IMPPDF && objetoComp.cae != 0)
                            {
                                LayoutPDF.generarPDF(objetoComp);
                            }
                        }
                        else
                        {
                            ServicioAFIP.autorizar(ref objetoComp);
                            actualizaDatosDB(objetoComp);

                            if (Variables.IMPPDF && objetoComp.resultado != "R" && objetoComp.cae != 0)
                            {
                                LayoutPDF.generarPDF(objetoComp);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (nroReg != 0)
                        {
                            actualizaDatosDB(e.Message, nroReg);
                        }

                        Log.guardarLog(file.Name + Variables.SEP + e.Message, Variables.DIRERR + DateTime.Now.ToString("yyyyMMdd").Trim() + ".log");

                        throw e;
                    }
                }
            }
        }

        private void actualizaDatosDB(Comprobante objetoComp)
        {
            if (Variables.ACTBD)
            {
                string sp = "EXEC dbo.WSMTXCA_UPDATE_CAE " +
                    $"@REGISTRO = '{objetoComp.nroRegistro}', " +
                    $"@TIPOCOMP = '{objetoComp.tipocomp}', " +
                    $"@SERIE = '{objetoComp.serie}', " +
                    $"@NROCOMP = '{objetoComp.nrocomp}', " +
                    $"@RESULTADO = '{objetoComp.resultado}', " +
                    $"@MOTIVO = '{objetoComp.motivoError}', " +
                    $"@CAE = '{objetoComp.cae}', " +
                    $"@VCTOCAE = '{objetoComp.vctoCae.ToString("yyyyMMdd")}' ";

                DB.EjecutarSql(sp);
            }
        }

        private void actualizaDatosDB(string error, int reg)
        {
            if (Variables.ACTBD)
            {
                string sp = "EXEC dbo.WSMTXCA_UPDATE_CAE " +
                $"@REGISTRO = '{reg}', " +
                $"@TIPOCOMP = '0', " +
                $"@SERIE = '', " +
                $"@NROCOMP = '', " +
                $"@RESULTADO = 'R', " +
                $"@MOTIVO = '{error}', " +
                $"@CAE = '', " +
                $"@VCTOCAE = '' ";

                DB.EjecutarSql(sp);
            }
        }
    }
}
