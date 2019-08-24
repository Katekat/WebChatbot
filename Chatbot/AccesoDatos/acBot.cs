using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos
{
    public class acBot
    {
        private SqlCommand cmdComando;

        public DataSet SaveAnswer(int pCoduser, int pCodfuncionality, bool pAnswer)
        {
            try
            {
                SqlConnection cnnConexion = acConnection.ObtenerConexion();


                string strSentenciaSQL = "SpInsertRespuestaUsuario";
                strSentenciaSQL = string.Format(strSentenciaSQL);

                SqlCommand cmdComando = new SqlCommand(strSentenciaSQL, cnnConexion);
                cmdComando.CommandType = CommandType.StoredProcedure;
                cmdComando.Parameters.Add("@cod_usuario", SqlDbType.Int).Value = pCoduser;
                cmdComando.Parameters.Add("@cod_funcionalidad", SqlDbType.Int).Value = pCodfuncionality;
                cmdComando.Parameters.Add("@respuesta", SqlDbType.Bit).Value = pAnswer;

                SqlDataAdapter adpAdapter = new SqlDataAdapter(cmdComando);

                DataSet dsConsulta = new DataSet();

                adpAdapter.Fill(dsConsulta, "consulta");

                cnnConexion.Close();

                return dsConsulta;
            }
            catch (Exception ex)
            {
                throw new Exception("Error, al insertar respuesta de usuario " + ex.Message);
            }

        }
    }
}
