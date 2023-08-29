using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSMTXCA_SRV.Entidades
{
    class ComprobanteAsociado
    {
        public short codigo { get; private set; }
        public int pv { get; private set; }
        public int nrocomp { get; private set; }
        public long cuit { get; private set; }

        public ComprobanteAsociado(short codigo, int pv, int nrocomp, long cuit)
        {
            this.codigo = codigo;
            this.pv = pv;
            this.nrocomp = nrocomp;
            this.cuit = cuit;
        }
    }
}
