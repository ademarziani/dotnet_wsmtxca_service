using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSMTXCA_SRV.Entidades
{
    class Sujeto
    {
        public string codigo { get; set; }
        public string tienda { get; set; }
        public string razon { get; set; }
        public short coddoc { get; set; }
        public long nrodoc { get; set; }
        public string direccion { get; set; }
        public string direntrega { get; set; }
        public string localidad { get; set; }
        public string telefono { get; set; }
        public string provincia { get; set; }
        public string condIVA { get; set; }        
    }
}
