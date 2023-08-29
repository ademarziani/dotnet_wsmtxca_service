using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSMTXCA_SRV.Util;

namespace WSMTXCA_SRV.Entidades
{
    class Comprobante
    {
        public string nombreArchivo;
        public short tipocomp;
        public string descComp;
        public string serie;
        public int pv;
        public long nrocomp;
        public DateTime fecha;
        public Sujeto sujeto;
        public string vendedor;
        public string transporte;
        public string condPago;
        public String codMoneda;
        public decimal cotizMoneda;
        public String observaciones;
        public short codConcepto;
        public int nroRegistro;
        public string desdeServicio;
        public string hastaServicio;
        public string vctoPago;
        public long cae;
        public DateTime vctoCae;
        public string resultado;
        public string motivoError;

        public decimal impGravado;
        public decimal impNoGravado;
        public decimal impExento;
        public decimal impSubTotal;
        public decimal impDescuento;
        public decimal impOtroTributos;
        public decimal impTotal;
        public decimal pesoTotal;

        public List<Detalle> detalles;
        public List<OtrosTributo> otrosTributos;
        public List<SubTotIVA> subtotalesIVAs;
        public List<DatoAdicional> datosAdicionales;
        public List<ComprobanteAsociado> comprobantesAsociados;

        public Comprobante()
        {
            detalles = new List<Detalle>();
            otrosTributos = new List<OtrosTributo>();
            subtotalesIVAs = new List<SubTotIVA>();
            datosAdicionales = new List<DatoAdicional>();
            comprobantesAsociados = new List<ComprobanteAsociado>();
        }

        public void setItem(Detalle det)
        {
            detalles.Add(det);
        }

        public void setOtroTributo(OtrosTributo ot)
        {
            otrosTributos.Add(ot);
        }

        public void setSubTotalIVA(SubTotIVA subIVA)
        {
            subtotalesIVAs.Add(subIVA);
        }

        public void setDatoAdicional(DatoAdicional datosAdic)
        {
            datosAdicionales.Add(datosAdic);
        }

        public void setComprobanteAsociado(ComprobanteAsociado compAsoc)
        {
            comprobantesAsociados.Add(compAsoc);
        }

        public MTXCA.ItemType[] getItems()
        {
            List<MTXCA.ItemType> auxItems = new List<MTXCA.ItemType>();
            bool informaIVA = ( this.tipocomp == 1 ||
                                this.tipocomp == 2 || 
                                this.tipocomp == 3 ||
                                this.tipocomp == 201 ||
                                this.tipocomp == 202 ||
                                this.tipocomp == 203 );

            foreach (var IT in this.detalles)
            {
                auxItems.Add(new MTXCA.ItemType()
                {
                    unidadesMtx = IT.unidadesMX,
                    unidadesMtxSpecified = true,
                    codigoMtx = IT.prod.codmx,
                    codigo = IT.prod.codigo,
                    descripcion = IT.prod.descrip,
                    cantidad = IT.cantidad,
                    cantidadSpecified = true,
                    codigoUnidadMedida = IT.prod.codum,
                    precioUnitario = IT.precio,
                    precioUnitarioSpecified = true,
                    importeBonificacion = IT.bonif,
                    codigoCondicionIVA = IT.codIva,
                    importeIVA = IT.importeIva,
                    importeIVASpecified = informaIVA,
                    importeItem = IT.totalItem
                });
            }

            return auxItems.ToArray();
        }

        public MTXCA.SubtotalIVAType[] getSubIVAs()
        {
            List<MTXCA.SubtotalIVAType> auxSubIVAs = new List<MTXCA.SubtotalIVAType>();

            foreach (var SUB in this.subtotalesIVAs)
            {
                auxSubIVAs.Add(new MTXCA.SubtotalIVAType()
                {
                    codigo = SUB.codIva,                    
                    importe = SUB.importe
                });
            }

            return auxSubIVAs.ToArray();
        }

        public MTXCA.OtroTributoType[] getOTributos()
        {
            List<MTXCA.OtroTributoType> auxOT = new List<MTXCA.OtroTributoType>();
            
            foreach(var OT in this.otrosTributos)
            {
                auxOT.Add(new MTXCA.OtroTributoType() {
                    codigo = OT.codigo,
                    descripcion = OT.descripcion,
                    baseImponible = OT.baseImp,
                    importe = OT.imp
                });
            }

            return auxOT.ToArray();
        }

        public MTXCA.DatoAdicionalType[] getDatosAdicionales()
        {
            List<MTXCA.DatoAdicionalType> auxDatoAdic = new List<MTXCA.DatoAdicionalType>();

            foreach (var DADIC in this.datosAdicionales)
            {
                auxDatoAdic.Add(new MTXCA.DatoAdicionalType()
                {
                    t = DADIC.t,
                    c1 = DADIC.c1,
                    c2 = DADIC.c2,
                    c3 = DADIC.c3,
                    c4 = DADIC.c4,
                    c5 = DADIC.c5,
                    c6 = DADIC.c6,
                });
            }

            return auxDatoAdic.ToArray();
        }

        public MTXCA.ComprobanteAsociadoType[] getComprobantesAsociados()
        {
            List<MTXCA.ComprobanteAsociadoType> auxCompAsoc = new List<MTXCA.ComprobanteAsociadoType>();

            foreach (var COMPASOC in this.comprobantesAsociados)
            {
                auxCompAsoc.Add(new MTXCA.ComprobanteAsociadoType()
                {
                    codigoTipoComprobante = COMPASOC.codigo,
                    numeroPuntoVenta = COMPASOC.pv,
                    numeroComprobante = COMPASOC.nrocomp,
                    cuit = COMPASOC.cuit,
                    cuitSpecified = this.tipocomp == 202 || this.tipocomp == 203 || this.tipocomp == 207 || this.tipocomp == 208
                });
            }

            return auxCompAsoc.ToArray();
        }

        public void setTipoComprobante()
        {
            Dictionary<short, string[]> tipoComprobante = new Dictionary<short, string[]>();

            tipoComprobante.Add(1, new string[] { "A", "Factura" });
            tipoComprobante.Add(2, new string[] { "A", "Nota de débito" });
            tipoComprobante.Add(3, new string[] { "A", "Nota de crédito" });
            tipoComprobante.Add(6, new string[] { "B", "Factura" });
            tipoComprobante.Add(7, new string[] { "B", "Nota de débito" });
            tipoComprobante.Add(8, new string[] { "B", "Nota de crédito" });
            tipoComprobante.Add(201, new string[] { "A", "Factura" });
            tipoComprobante.Add(202, new string[] { "A", "Nota de débito" });
            tipoComprobante.Add(203, new string[] { "A", "Nota de crédito" });
            tipoComprobante.Add(206, new string[] { "B", "Factura" });
            tipoComprobante.Add(207, new string[] { "B", "Nota de débito" });
            tipoComprobante.Add(208, new string[] { "B", "Nota de crédito" });

            this.serie = tipoComprobante[this.tipocomp][0];
            this.descComp = tipoComprobante[this.tipocomp][1];
        }

        public string calculoDigitoVerificador()
        {
            string cod;

            if (this.cae == 0)
            {
                cod = "9999999999999999999999999999999999999999";
            }
            else
            {
                int impares;
                int pares;
                int total;
                int digito;

                cod = Variables.CUIT +
                      this.tipocomp.ToString().PadLeft(3, '0') +
                      this.pv.ToString().PadLeft(5, '0') +
                      this.cae +
                      this.vctoCae.ToString("yyyyMMdd");
                //
                // Ahora analizo la cadena de caracteres:
                // Tengo que sumar todos los caracteres impares y los pares
                pares = 0;
                impares = 0;

                for (int i = 1; i <= cod.Length; i++)
                {
                    //
                    // If I Mod 2 = 0 Then
                    if (i % 2 == 0)
                    {
                        // es par
                        // Pares = Pares + CLng(Mid(Cod, I, 1))
                        pares += Int16.Parse(cod.Substring(i - 1, 1));
                    }
                    else
                    {
                        // es impar
                        // Impares = Impares + CLng(Mid(Cod, I, 1))
                        impares += Int16.Parse(cod.Substring(i - 1, 1));
                    }
                }
                //
                impares = 3 * impares;
                total = pares + impares;
                digito = 10 - (total % 10);

                if (digito == 10)
                {
                    digito = 0;
                }

                cod = cod + digito;
            }

            return cod;
        }

    }
}
