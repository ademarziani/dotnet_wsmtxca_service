using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using WSMTXCA_SRV.Entidades;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using System.Globalization;

namespace WSMTXCA_SRV.Util
{
    class LayoutPDF
    {
        private static Document doc;
        private static PdfWriter writer;
        private static BaseFont _helvetica;
        private static Comprobante compImp;
        private static int topePagina = 20;

        public static void generarPDF(Comprobante comprobante)
        {            
            string nombrePdf = Path.ChangeExtension(comprobante.nombreArchivo, ".pdf");
            doc = new Document(PageSize.A4);
            writer = PdfWriter.GetInstance(doc, new FileStream(getDirectorio() + nombrePdf, FileMode.Create));

            _helvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            compImp = comprobante;
            int paginas = cantPaginas();

            doc.AddTitle(comprobante.nrocomp.ToString());
            doc.AddCreator("Desarrollado por Andres Demarziani");
            doc.Open();

            try
            {
                for (short i = 1; i <= 2; i++)
                {
                    if (i > 1)
                        doc.NewPage();

                    for (int p = 1; p <= paginas; p++)
                    {
                        if (p > 1)
                        {
                            doc.NewPage();
                        }

                        encabezado(i, p, paginas);
                        detalle(p);

                        if (p == paginas)
                        {
                            pie();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
            finally
            {
                doc.Close();
                writer.Close();
            }
        }

        private static void encabezado(short copia, int pagina, int totPaginas)
        {
            float inicio = doc.PageSize.Height;
            float posserie = doc.PageSize.Width / 2;
            float left1 = 25;
            float left2 = 400;
            int tamfon1 = 9;
            float saltolinea = 13f;
            string nroComp = compImp.pv.ToString().PadLeft(5, '0') + "-" + compImp.nrocomp.ToString().PadLeft(8, '0');
            Dictionary<short, string> copias = new Dictionary<short, string>();           
            Dictionary<short, string> tipoDocumentacion = new Dictionary<short, string>();

            copias.Add(1, "ORIGINAL");
            copias.Add(2, "DUPLICADO");
            copias.Add(3, "TRIPLICADO");

            tipoDocumentacion.Add(80, "CUIT");
            tipoDocumentacion.Add(96, "DNI");

            inicio -= 20f;
            imprimeTexto($"Página {pagina} de {totPaginas}", 8, Element.ALIGN_RIGHT, doc.PageSize.Width-20f, inicio);
            imprimeTexto($"{copias[copia]}", 8, Element.ALIGN_RIGHT, doc.PageSize.Width - 20f, inicio - saltolinea);

            inicio -= 20f;
            imprimeTexto(compImp.serie, 24, Element.ALIGN_CENTER, posserie, inicio);

            imprimeTexto(compImp.descComp, 12, Element.ALIGN_LEFT, left2, inicio + 5f);
            imprimeTexto($"Nro {nroComp}", 12, Element.ALIGN_LEFT, left2, inicio - 12f);
            imprimeTexto($"Fecha: {compImp.fecha.ToString("dd/MM/yyyy")}", 12, Element.ALIGN_LEFT, left2, inicio - 29f);

            inicio -= 20f;
            imprimeTexto($"Cod. {compImp.tipocomp.ToString().PadLeft(3,'0')}", tamfon1, Element.ALIGN_CENTER, posserie, inicio);

            inicio -= 10f;

            Image logo = Image.GetInstance(Variables.DIRTXT+"\\img\\logo.jpg");
            logo.ScaleAbsoluteHeight(Variables.LOGO_HEIGHT);
            logo.ScaleAbsoluteWidth(Variables.LOGO_WIDTH);
            logo.SetAbsolutePosition(70f, inicio);

            doc.Add(logo);

            inicio -= 20f;
            imprimeTexto(Variables.EMPRESA, tamfon1, Element.ALIGN_LEFT, left1, inicio);
            imprimeTexto(Variables.CONDFIS, tamfon1, Element.ALIGN_LEFT, left2, inicio);

            inicio -= 11f;
            imprimeTexto(Variables.DIRECCION, tamfon1, Element.ALIGN_LEFT, left1, inicio);
            imprimeTexto("Inicio de Actividades: " + Variables.INIACTIV, tamfon1, Element.ALIGN_LEFT, left2, inicio);

            inicio -= 11f;
            imprimeTexto("E-Mail: " + Variables.EMAIL, tamfon1, Element.ALIGN_LEFT, left1, inicio);
            imprimeTexto("CUIT: " + Variables.CUIT.ToString(), tamfon1, Element.ALIGN_LEFT, left2, inicio);

            inicio -= 11f;
            imprimeTexto(Variables.MUNICIPIO, tamfon1, Element.ALIGN_LEFT, left1, inicio);
            imprimeTexto(Variables.NROIIBB, tamfon1, Element.ALIGN_LEFT, left2, inicio);

            inicio -= 30f;
            imprimeTexto($"Sr. (s): {compImp.sujeto.codigo} - {compImp.sujeto.razon} | {tipoDocumentacion[compImp.sujeto.coddoc]}: {compImp.sujeto.nrodoc}", tamfon1, Element.ALIGN_LEFT, left1, inicio);
            imprimeTexto($"Cond. IVA: {compImp.sujeto.condIVA}", tamfon1, Element.ALIGN_LEFT, left2, inicio);

            inicio -= saltolinea;
            imprimeTexto($"Dirección: {compImp.sujeto.direccion}", tamfon1, Element.ALIGN_LEFT, left1, inicio);            

            inicio -= saltolinea;
            imprimeTexto($"Dir. Entrega: {compImp.sujeto.direntrega}", tamfon1, Element.ALIGN_LEFT, left1, inicio);

            inicio -= saltolinea;
            imprimeTexto($"Localidad: {compImp.sujeto.localidad}", tamfon1, Element.ALIGN_LEFT, left1, inicio);
            imprimeTexto($"Provincia: {compImp.sujeto.provincia}", tamfon1, Element.ALIGN_LEFT, left2, inicio);

            inicio -= saltolinea;
            imprimeTexto($"Telefono: {compImp.sujeto.telefono}", tamfon1, Element.ALIGN_LEFT, left1, inicio);
            imprimeTexto($"Vendedor: {compImp.vendedor}", tamfon1, Element.ALIGN_LEFT, left2, inicio);

            inicio -= saltolinea;
            imprimeTexto($"Cond. Venta: {compImp.condPago}", tamfon1, Element.ALIGN_LEFT, left1, inicio);
            imprimeTexto($"Transporte: {compImp.transporte}", tamfon1, Element.ALIGN_LEFT, left2, inicio);
        }

        private static void detalle(int pagina)
        {
            int index = 0;
            float inicio = doc.PageSize.Height;
            float[] widths = new float[] {  160f, /* 01-Descripcion*/
                                            038f, /* 02-Bultos*/
                                            019f, /* 03-(UM)*/
                                            042f, /* 04-Cantidad*/
                                            019f, /* 05-(UM)*/
                                            048f, /* 06-Precio*/
                                            048f, /* 07-Bonif*/
                                            038f, /* 08-(Porc.)*/
                                            048f, /* 09-Promo*/
                                            038f, /* 10-(Porc. Promo)*/
                                            050f, /* 11-IVA*/
                                            036f, /* 12-(Porc. IVA)*/
                                            050f  /* 13-Total*/  };

            PdfPTable table = new PdfPTable(widths.Length);
            PdfContentByte pcb = writer.DirectContent;
            Dictionary<short, string> descIva = new Dictionary<short, string>();

            descIva.Add(3, "0 %");
            descIva.Add(4, "10,5 %");
            descIva.Add(5, "21 %");

            /*01*/ table.AddCell(cellEncabezado("Descripción", Element.ALIGN_LEFT));
            /*02*/ table.AddCell(cellEncabezado("Bultos", Element.ALIGN_RIGHT));
            /*03*/ table.AddCell(cellEncabezado(" ", Element.ALIGN_LEFT));
            /*04*/ table.AddCell(cellEncabezado("Cant.", Element.ALIGN_RIGHT));
            /*05*/ table.AddCell(cellEncabezado(" ", Element.ALIGN_LEFT));
            /*06*/ table.AddCell(cellEncabezado("Precio", Element.ALIGN_RIGHT));
            /*07*/ table.AddCell(cellEncabezado((compImp.impDescuento != 0 ? "Bonif.": " "), Element.ALIGN_RIGHT));
            /*08*/ table.AddCell(cellEncabezado(" ", Element.ALIGN_LEFT));
            /*09*/ table.AddCell(cellEncabezado((compImp.impDescuento != 0 ? "Promoción" : " "), Element.ALIGN_RIGHT));
            /*10*/ table.AddCell(cellEncabezado(" ", Element.ALIGN_LEFT));
            /*11*/ table.AddCell(cellEncabezado((Variables.DISC_IMPS || compImp.serie == "A" ? "IVA" : " "), Element.ALIGN_RIGHT));
            /*12*/ table.AddCell(cellEncabezado(" ", Element.ALIGN_LEFT));
            /*13*/ table.AddCell(cellEncabezado("Importe", Element.ALIGN_RIGHT));

            table.TotalWidth = 565f;
            table.SetWidths(widths);

            foreach (var item in compImp.detalles)
            {
                if (nroPagina(index) == pagina)
                {
                    table.AddCell(cellItems(item.prod.descrip));
                    table.AddCell(cellItems(item.bultos));
                    table.AddCell(cellItems(item.prod.unidadm2));
                    table.AddCell(cellItems(item.cantidad));
                    table.AddCell(cellItems(item.prod.unidadm1));
                    table.AddCell(cellItems(item.precioPdf));

                    // Bonificacion
                    if (item.porcBonif != 0)
                    {
                        table.AddCell(cellItems(item.bonif));
                        table.AddCell(cellItems($"{item.porcBonif.ToString("N")}%"));
                    }
                    else
                    {
                        table.AddCell(cellItems(" "));
                        table.AddCell(cellItems(" "));
                    }

                    // Promociones
                    if (item.porcPromo != 0)
                    {
                        table.AddCell(cellItems(item.promo));
                        table.AddCell(cellItems($"{item.porcPromo.ToString("N")}%"));
                    }
                    else
                    {
                        table.AddCell(cellItems(" "));
                        table.AddCell(cellItems(" "));
                    }

                    if (Variables.DISC_IMPS || compImp.serie == "A")
                    {
                        table.AddCell(cellItems(item.ivaPdf));
                        table.AddCell(cellItems(descIva[item.codIva]));
                    }
                    else
                    {
                        table.AddCell(cellItems(" "));
                        table.AddCell(cellItems(" "));
                    }
                    table.AddCell(cellItems(item.totalItem));
                }

                index++;
            }

            inicio -= 250f;

            table.WriteSelectedRows(0, -1, 15, inicio, pcb);
        }

        private static void pie()
        {
            float inicio = doc.PageSize.Height;
            float left1 = 20f;
            float left2 = 480f;
            float left3 = doc.PageSize.Width - 20f;
            float leftCae = 100f;
            Dictionary<short, string> descIva = new Dictionary<short, string>();

            descIva.Add(3, "IVA (0,00 %)");
            descIva.Add(4, "IVA (10,50 %)");
            descIva.Add(5, "IVA (21,00 %)");

            inicio -= 560f;

            if (compImp.tipocomp > 200)
            {

                if (compImp.tipocomp == 201 || compImp.tipocomp == 206)
                {
                    imprimeTexto($"{compImp.descComp.ToUpper()} DE CRÉDITO ELECTRÓNICA MiPyMEs (FCE)", 8, Element.ALIGN_LEFT, left1, inicio);

                    inicio -= 10f;
                    imprimeTexto($"Fecha de Vcto para el pago: {Convert.ToDateTime(compImp.vctoPago).ToString("dd/MM/yyyy")}", 8, Element.ALIGN_LEFT, left1, inicio);
                }
                else
                {
                    imprimeTexto($"{compImp.descComp.ToUpper()} ELECTRÓNICA MiPyMEs (FCE)", 8, Element.ALIGN_LEFT, left1, inicio);
                }

                foreach (var dato in compImp.datosAdicionales)
                {
                    if (dato.t == 21)
                    {
                        inicio -= 10f;
                        imprimeTexto($"CBU del Emisor: {dato.c1}", 8, Element.ALIGN_LEFT, left1, inicio);
                    }
                }

                inicio -= 30f;                
            }
            else if (compImp.sujeto.condIVA == "MONOTRIBUTO" && compImp.serie == "A")
            {
                imprimeTexto("El crédito fiscal discriminado en el presente comprobante, sólo podrá ser computado a efectos ", 8, Element.ALIGN_LEFT, left1, inicio);

                inicio -= 10f;
                imprimeTexto("del Régimen de Sostenimiento e Inclusión Fiscal para Pequeños Contribuyentes de la Ley Nº 27.618", 8, Element.ALIGN_LEFT, left1, inicio);

                inicio -= 40f;
            }
            else if (Variables.DISC_IMPS && compImp.sujeto.condIVA == "CONSUMIDOR FINAL" && compImp.serie == "B")
            {
                imprimeTexto("Régimen de Transparencia Fiscal al Consumidor Ley 27.743.", 8, Element.ALIGN_LEFT, left1, inicio);
                inicio -= 50f;
            }
            else
            {
                inicio -= 50f;
            }

            imprimeTexto("Recibí", 10, Element.ALIGN_LEFT, left1, inicio);

            inicio -= 15f;
            imprimeTexto("______________", 10, Element.ALIGN_CENTER, 100, inicio);
            imprimeTexto("______________", 10, Element.ALIGN_CENTER, 300, inicio);
            imprimeTexto("______________", 10, Element.ALIGN_CENTER, 500, inicio);

            inicio -= 15f;
            imprimeTexto("Firma", 10, Element.ALIGN_CENTER, 100, inicio);
            imprimeTexto("Aclaración", 10, Element.ALIGN_CENTER, 300, inicio);
            imprimeTexto("DNI", 10, Element.ALIGN_CENTER, 500, inicio);

            inicio -= 30f;
            imprimeTexto($"Observaciones: {compImp.observaciones}", 10, Element.ALIGN_LEFT, left1, inicio);

            if (compImp.pesoTotal > 0)
            { 
                inicio -= 15f;
                imprimeTexto($"Peso Total: {compImp.pesoTotal.ToString("N")} Kgs", 10, Element.ALIGN_LEFT, left1, inicio);
            }

            inicio -= 20f;
            if (compImp.impDescuento > 0)
            {
                imprimeTexto("(*) El valor total de descuento corresponde a la bonificación por No devolución, válido para", 8, Element.ALIGN_LEFT, left1, inicio);
                imprimeTexto("los productos de la familia 'Frescos'.", 8, Element.ALIGN_LEFT, left1, inicio - 10f);
            }

            inicio -= 30f;
            imprimeTexto("Para el caso de incumplimiento en el pago de la presente factura, ", 8, Element.ALIGN_LEFT, leftCae, inicio);            
            imprimeTexto("la parte de común acuerdo prorrogan la jurisdicción a los", 8, Element.ALIGN_LEFT, leftCae, inicio - 10f);            
            imprimeTexto("Tribunales Ordinarios de la Ciudad de Paraná provincia de Entre Ríos.", 8, Element.ALIGN_LEFT, leftCae, inicio - 20f);

            inicio = 62f;
            imprimeTexto($"C.A.E. N°: {compImp.cae}", 9, Element.ALIGN_LEFT, leftCae, inicio);

            inicio -= 15f;
            imprimeTexto($"Fecha Vcto. C.A.E: {compImp.vctoCae.ToString("dd/MM/yyyy")}", 9, Element.ALIGN_LEFT, leftCae, inicio);
            imprimeTexto("TOTAL", 13, Element.ALIGN_RIGHT, left2, inicio);
            imprimeTexto(compImp.impTotal.ToString("N"), 13, Element.ALIGN_RIGHT, left3, inicio);

            if (Variables.DISC_IMPS || compImp.serie == "A")
            {
                inicio += 45f;
                foreach (var iva in compImp.subtotalesIVAs)
                {
                    imprimeTexto(descIva[iva.codIva], 11, Element.ALIGN_RIGHT, left2, inicio);
                    imprimeTexto(iva.importe.ToString("N"), 11, Element.ALIGN_RIGHT, left3, inicio);

                    inicio += 15f;
                }

                inicio += 15f;
                foreach (var imp in compImp.otrosTributos)
                {
                    imprimeTexto(imp.descripcion, 11, Element.ALIGN_RIGHT, left2, inicio);
                    imprimeTexto(imp.imp.ToString("N"), 11, Element.ALIGN_RIGHT, left3, inicio);

                    inicio += 15f;
                }

                if (compImp.impDescuento > 0)
                {
                    inicio += 15f;
                    imprimeTexto("DESCUENTO", 11, Element.ALIGN_RIGHT, left2, inicio);
                    imprimeTexto(compImp.impDescuento.ToString("N"), 11, Element.ALIGN_RIGHT, left3, inicio);
                }

                inicio += 15f;
                imprimeTexto("SUBTOTAL", 11, Element.ALIGN_RIGHT, left2, inicio);
                imprimeTexto((compImp.impSubTotal + compImp.impDescuento).ToString("N"), 11, Element.ALIGN_RIGHT, left3, inicio);
            }

            codigoDeBarra();
        }

        private static void codigoDeBarra()
        {
            string jsonQr;

            jsonQr = "{\"ver\":1" +
                ",\"fecha\":\""+compImp.fecha.ToString("yyyy-MM-dd")+"\"" +
                ",\"cuit\":"+Variables.CUIT+
                ",\"ptoVta\":"+compImp.pv+
                ",\"tipoCmp\":"+compImp.tipocomp+
                ",\"nroCmp\":"+compImp.nrocomp+
                ",\"importe\":"+compImp.impTotal.ToString(CultureInfo.InvariantCulture)+
                ",\"moneda\":\""+compImp.codMoneda+"\""+
                ",\"ctz\":"+compImp.cotizMoneda.ToString(CultureInfo.InvariantCulture)+ ""+
                ",\"tipoDocRec\":"+compImp.sujeto.coddoc+
                ",\"nroDocRec\":"+compImp.sujeto.nrodoc+
                ",\"tipoCodAut\":\"E\""+
                ",\"codAut\":"+compImp.cae.ToString()+"}";

            byte[] stringqr= System.Text.Encoding.UTF8.GetBytes(jsonQr);
            string link = "https://www.afip.gob.ar/fe/qr/?p=";
            BarcodeQRCode codeqr = new BarcodeQRCode(link+Convert.ToBase64String(stringqr), 0, 0,null);
            //BarcodeQRCode codeqr = new BarcodeQRCode(jsonQr, 100, 100, null);
            Image imgBarQR = codeqr.GetImage();
            imgBarQR.SetAbsolutePosition(20f, 40f);
            doc.Add(imgBarQR);

            /*
            BarcodeInter25 code25 = new BarcodeInter25();
            code25.ChecksumText = false;
            code25.Code = compImp.calculoDigitoVerificador();
            code25.BarHeight = 35;
            PdfContentByte cb = writer.DirectContent;
            Image imgBarCode = code25.CreateImageWithBarcode(cb, null, null);
            imgBarCode.SetAbsolutePosition(20f, 30f);

            doc.Add(imgBarCode);
            */
        }

        private static void imprimeTexto(string texto, int tamfont, int alineamiento, float x, float y)
        {
            PdfContentByte cb = writer.DirectContent;

            // we tell the ContentByte we're ready to draw text
            cb.BeginText();
            cb.SetFontAndSize(_helvetica, tamfont);
            // we draw some text on a certain position
            cb.ShowTextAligned(alineamiento, texto, x, y, 0);

            // we tell the contentByte, we've finished drawing text
            cb.EndText();
        }

        private static PdfPCell cellEncabezado(string titCampo, int align)
        {
            Phrase frase;
            Font helvB = new Font(_helvetica, 8, Font.BOLD);

            frase = new Phrase(titCampo, helvB);
            return creaCelda(frase, align, 0.75f);          
        }

        private static PdfPCell cellItems(decimal valor)
        {
            Phrase frase;
            Font helv = new Font(_helvetica, 7.5f);

            frase = new Phrase(valor.ToString(), helv);
            return creaCelda(frase, Element.ALIGN_RIGHT);
        }

        private static PdfPCell cellItems(string valor)
        {
            Phrase frase;
            Font helv = new Font(_helvetica, 7.5f);

            frase = new Phrase(valor, helv);
            return creaCelda(frase, Element.ALIGN_LEFT);
        }

        private static PdfPCell cellItems(string valor, int align)
        {
            Phrase frase;
            Font helv = new Font(_helvetica, 7.5f);

            frase = new Phrase(valor, helv);
            return creaCelda(frase, align);
        }

        private static PdfPCell creaCelda(Phrase frase, int align)
        {
            PdfPCell celda;            

            celda = new PdfPCell(frase);
            celda.Border = 0;
            celda.HorizontalAlignment = align;
            celda.VerticalAlignment = Element.ALIGN_CENTER;

            return celda;
        }

        private static PdfPCell creaCelda(Phrase frase, int align, float border)
        {
            PdfPCell celda = creaCelda(frase,align);
            celda.BorderWidthBottom = border;

            return celda;
        }

        private static string getDirectorio()
        {
            string nuevoPath = Variables.DIRPDF;
            string dia = DateTime.Now.Day.ToString().PadLeft(2, '0');
            string mes = DateTime.Now.Month.ToString().PadLeft(2, '0');
            string anio = DateTime.Now.Year.ToString();

            nuevoPath += anio + "\\";

            if (!Directory.Exists(nuevoPath))
            {
                Directory.CreateDirectory(nuevoPath);
            }

            nuevoPath += mes + "\\";

            if (!Directory.Exists(nuevoPath))
            {
                Directory.CreateDirectory(nuevoPath);
            }

            nuevoPath += dia + "\\";

            if (!Directory.Exists(nuevoPath))
            {
                Directory.CreateDirectory(nuevoPath);
            }

            return nuevoPath;
        }

        private static int cantPaginas()
        {
            return ((int) compImp.detalles.Count() / topePagina) + 1;
        }

        private static int nroPagina(int item)
        {
            return ((int) item / topePagina) + 1;
        }
    }
}
