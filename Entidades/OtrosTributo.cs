using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSMTXCA_SRV.Entidades
{
    class OtrosTributo
    {
        public short codigo { get; private set; }
        public String descripcion { get; private set; }
        public decimal baseImp { get; private set; }
        public decimal imp { get; private set; }

        public OtrosTributo(short codigo, String descripcion, decimal baseImp, decimal imp)
        {
            this.codigo = codigo;
            this.descripcion = descripcion;
            this.baseImp = baseImp;
            this.imp = imp;
        }
    }
}
