using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WSMTXCA_SRV.Util;
using WSMTXCA_SRV.App;

namespace WSMTXCA_SRV
{
    public partial class Servicio : ServiceBase
    {
        Timer timer = new Timer();
        public static bool semaforo = true;

        public Servicio()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.guardarLog($"{DateTime.Now} - Servicio Iniciado");

            timer.Elapsed += new ElapsedEventHandler(ProcesoServicio);
            timer.Interval = Variables.SEGUNDOS * 1000; //number in milisecinds  
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            Log.guardarLog($"{DateTime.Now} - Servicio Parado");
        }

        private void ProcesoServicio(object source, ElapsedEventArgs e)
        {
            if (semaforo) {
                try { 
                    semaforo = false;

                    CargaComprobante proceso = new CargaComprobante();

                    // Procesa facturas a transmitir a servicio AFIP
                    proceso.autorizarComprobantes(false);

                    // Solo procesa reimpresiones
                    proceso.autorizarComprobantes(true);

                    semaforo = true;
                }
                catch (Exception error)
                {
                    Log.guardarLog($"{DateTime.Now} - Error en generacion de comprobante: {error.StackTrace}");
                }
                finally
                {
                    semaforo = true;
                }
            }
        }
    }
}
