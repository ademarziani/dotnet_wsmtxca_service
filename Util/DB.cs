using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Xml;
using System.IO;
using System.ServiceModel;

namespace WSMTXCA_SRV.Util
{
    class DB
    {
        private static OleDbConnection conexion;

        private static void ConectarBD()
        {
            if (conexion == null)
            {
                conexion = new OleDbConnection(Variables.CADENACONEXION);
            }
        }

        public static void EjecutarSql(string sql)
        {
            ConectarBD();

            OleDbCommand comando = new OleDbCommand(sql, conexion);

            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();
                conexion.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexion.Close();
            }
        }

    }
}
