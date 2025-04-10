using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using WSMTXCA_SRV.Entidades;
using System.Globalization;
using System.IO;
using Org.BouncyCastle.Asn1.Cmp;

namespace WSMTXCA_SRV.Util
{
    class ServicioAFIP
    {
        public static MTXCA.MTXCAServicePortTypeClient service;
        
        public static MTXCA.CodigoDescripcionType[] arrayObservaciones;
        public static MTXCA.CodigoDescripcionType[] arrayErrores;
        public static MTXCA.CodigoDescripcionType evento;

        public static MTXCA.ComprobanteCAEResponseType comprobanteResponse;
        public static MTXCA.ComprobanteType compExistResponse;
        public static MTXCA.ResultadoSimpleType resultadoComprobante;

        public static void iniciar()
        {
            if (service == null)
            {
                service = new MTXCA.MTXCAServicePortTypeClient();
                service.Endpoint.Address = new EndpointAddress(Variables.URL);
                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidarCertificado);
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            }        
        }

        public static Boolean ValidarCertificado(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static void autorizar(ref Comprobante comprobante)
        {
            long ultimoComprobante;
            string textoOk;
            string textoError;
            string motivoError;
            MTXCA.AuthRequestType auth;
            MTXCA.ComprobanteType compEnvio;
            MTXCA.ItemType[] items = comprobante.getItems();
            MTXCA.OtroTributoType[] otroTributos = comprobante.getOTributos();
            MTXCA.SubtotalIVAType[] subIVAs = comprobante.getSubIVAs();
            MTXCA.DatoAdicionalType[] datosAdic = comprobante.getDatosAdicionales();            
            MTXCA.ComprobanteAsociadoType[] compAsociados = comprobante.getComprobantesAsociados();

            auth = getAuth();

            compExistResponse = service.consultarComprobante(auth,
                    new MTXCA.ConsultaComprobanteRequestType()
                    {
                        codigoTipoComprobante = comprobante.tipocomp,
                        numeroPuntoVenta = comprobante.pv,
                        numeroComprobante = comprobante.nrocomp
                    }, out arrayObservaciones, out arrayErrores, out evento);

            if (arrayErrores == null || arrayErrores.Length == 0)
            {
                textoOk = comprobante.nombreArchivo + Variables.SEP +
                comprobante.nroRegistro.ToString() + Variables.SEP +
                compExistResponse.codigoTipoComprobante.ToString() + Variables.SEP +
                compExistResponse.numeroPuntoVenta.ToString() + Variables.SEP +
                compExistResponse.numeroComprobante.ToString() + Variables.SEP +
                compExistResponse.codigoAutorizacion.ToString() + Variables.SEP +
                compExistResponse.fechaVencimiento.ToString("yyyyMMdd");

                comprobante.cae = compExistResponse.codigoAutorizacion;
                comprobante.vctoCae = compExistResponse.fechaVencimiento;
                comprobante.resultado = "A";

                Log.guardarLog(textoOk, Variables.DIROK + DateTime.Now.ToString("yyyyMMdd").Trim() + ".log");
            }
            else
            {
                ultimoComprobante = service.consultarUltimoComprobanteAutorizado(auth,
                    new MTXCA.ConsultaUltimoComprobanteAutorizadoRequestType()
                    {
                        codigoTipoComprobante = comprobante.tipocomp,
                        numeroPuntoVenta = comprobante.pv
                    }, out arrayErrores, out evento) + 1;

                if (Variables.ULTCOMP || comprobante.nrocomp == ultimoComprobante)
                {
                    compEnvio = new MTXCA.ComprobanteType()
                    {
                        codigoTipoComprobante = comprobante.tipocomp,
                        numeroPuntoVenta = comprobante.pv,
                        numeroComprobante = comprobante.nrocomp,
                        fechaEmision = comprobante.fecha,
                        fechaEmisionSpecified = true,
                        fechaVencimientoPago = Convert.ToDateTime(comprobante.vctoPago),
                        fechaVencimientoPagoSpecified = comprobante.codConcepto == 2 || comprobante.tipocomp == 201 || comprobante.tipocomp == 206,
                        codigoTipoDocumento = comprobante.sujeto.coddoc,
                        codigoTipoDocumentoSpecified = true,
                        numeroDocumento = comprobante.sujeto.nrodoc,
                        numeroDocumentoSpecified = true,
                        importeGravado = comprobante.impGravado,
                        importeGravadoSpecified = true,
                        importeNoGravado = comprobante.impNoGravado,
                        importeNoGravadoSpecified = true,
                        importeExento = comprobante.impExento,
                        importeExentoSpecified = true,
                        importeSubtotal = comprobante.impSubTotal,
                        importeOtrosTributos = comprobante.impOtroTributos,
                        importeOtrosTributosSpecified = comprobante.impOtroTributos != 0,
                        importeTotal = comprobante.impTotal,
                        codigoMoneda = comprobante.codMoneda,
                        cotizacionMoneda = comprobante.cotizMoneda,
                        cotizacionMonedaSpecified = comprobante.codMoneda == "PES" || comprobante.cancelaMismaMoneda == "N",
                        cancelaEnMismaMonedaExtranjeraSpecified = (comprobante.codMoneda != "PES" && comprobante.cancelaMismaMoneda == "S"),
                        cancelaEnMismaMonedaExtranjera = (comprobante.cancelaMismaMoneda == "S" ? MTXCA.SiNoSimpleType.S : MTXCA.SiNoSimpleType.N),
                        observaciones = comprobante.observaciones,
                        codigoConcepto = comprobante.codConcepto,
                        fechaServicioDesde = Convert.ToDateTime(comprobante.desdeServicio),
                        fechaServicioDesdeSpecified = comprobante.codConcepto == 2,
                        fechaServicioHasta = Convert.ToDateTime(comprobante.hastaServicio),
                        fechaServicioHastaSpecified = comprobante.codConcepto == 2,
                        condicionIVAReceptor = comprobante.condicionIVAReceptor,
                    };

                    if (items.Count() > 0)
                    {
                        compEnvio.arrayItems = items;
                    }

                    if (otroTributos.Count() > 0)
                    {
                        compEnvio.arrayOtrosTributos = otroTributos;
                    }

                    if (subIVAs.Count() > 0)
                    {
                        compEnvio.arraySubtotalesIVA = subIVAs;
                    }

                    if (datosAdic.Count() > 0)
                    {
                        compEnvio.arrayDatosAdicionales = datosAdic;
                    }

                    if (compAsociados.Count() > 0)
                    {
                        compEnvio.arrayComprobantesAsociados = compAsociados;
                    }

                    if (comprobante.periodosAsociados != null)
                    {
                        compEnvio.periodoComprobantesAsociados = new MTXCA.PeriodoComprobantesAsociadosType()
                        {
                            fechaDesde = comprobante.periodosAsociados.desde,
                            fechaHasta = comprobante.periodosAsociados.hasta
                        };
                    }

                    resultadoComprobante = service.autorizarComprobante(auth, compEnvio, out comprobanteResponse, out arrayObservaciones, out arrayErrores, out evento);

                    if (resultadoComprobante == MTXCA.ResultadoSimpleType.A || resultadoComprobante == MTXCA.ResultadoSimpleType.O)
                    {
                        textoOk = comprobante.nombreArchivo + Variables.SEP +
                                comprobante.nroRegistro.ToString() + Variables.SEP +
                                comprobanteResponse.codigoTipoComprobante.ToString() + Variables.SEP +
                                comprobanteResponse.numeroPuntoVenta.ToString() + Variables.SEP +
                                comprobanteResponse.numeroComprobante.ToString() + Variables.SEP +
                                comprobanteResponse.CAE.ToString() + Variables.SEP +
                                comprobanteResponse.fechaVencimientoCAE.ToString("yyyyMMdd");

                        comprobante.cae = comprobanteResponse.CAE;
                        comprobante.vctoCae = comprobanteResponse.fechaVencimientoCAE;
                        comprobante.resultado = resultadoComprobante.ToString();

                        Log.guardarLog(textoOk, Variables.DIROK + DateTime.Now.ToString("yyyyMMdd").Trim() + ".log");
                    }
                    else
                    {
                        motivoError = "";
                        textoError = comprobante.nombreArchivo + Variables.SEP +
                                    comprobante.nroRegistro.ToString() + Variables.SEP +
                                    comprobante.tipocomp.ToString() + Variables.SEP +
                                    comprobante.pv.ToString() + Variables.SEP +
                                    comprobante.nrocomp.ToString() + Variables.SEP;

                        foreach (var error in arrayErrores)
                        {
                            motivoError += error.codigo + "-" + error.descripcion + ";";
                            textoError += error.codigo + "-" + error.descripcion + ";";
                        }

                        motivoError = motivoError.Replace("'", "\"");

                        comprobante.resultado = resultadoComprobante.ToString();
                        comprobante.motivoError = motivoError;

                        Log.guardarLog(textoError, Variables.DIRERR + DateTime.Now.ToString("yyyyMMdd").Trim() + ".log");
                    }
                }
                else
                {
                    motivoError = "00-" + $"Para el tipo de comprobante {comprobante.tipocomp} y punto de venta {comprobante.pv} el numero deberá ser el {ultimoComprobante}";

                    textoError = comprobante.nombreArchivo + Variables.SEP +
                    comprobante.nroRegistro.ToString() + Variables.SEP +
                    comprobante.tipocomp.ToString() + Variables.SEP +
                    comprobante.pv.ToString() + Variables.SEP +
                    comprobante.nrocomp.ToString() + Variables.SEP +
                    motivoError;

                    comprobante.resultado = "R";
                    comprobante.motivoError = motivoError;

                    Log.guardarLog(textoError, Variables.DIRERR + DateTime.Now.ToString("yyyyMMdd").Trim() + ".log");
                }
            }
        }

        private static MTXCA.AuthRequestType getAuth()
        {
            string tokenTicket = "";
            string signTicket = "";

            var objReader = new StreamReader(Variables.DIRTICKET);
            
            while (objReader.Peek() >= 0)
            {
                objReader.ReadLine();
                tokenTicket = objReader.ReadLine();
                objReader.ReadLine();
                signTicket = objReader.ReadLine();
            }

            objReader.Close();

            var auth = new MTXCA.AuthRequestType()
            {
                cuitRepresentada = Variables.CUIT,
                token = tokenTicket,
                sign = signTicket
            };            

            return auth;
        }
    }
}
