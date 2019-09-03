using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace AccesoDatos
{
    public class acBot
    {
        private SqlCommand cmdComando;

        public bool SaveAnswer(long pUserID, long pFunctionalityID, bool pLike)
        {
            try
            {
                SqlConnection cnnConexion = acConnection.ObtenerConexion();

                string strSentenciaSQL = "SpInsertRespuestaUsuario";
                strSentenciaSQL = string.Format(strSentenciaSQL);

                SqlCommand cmdComando = new SqlCommand(strSentenciaSQL, cnnConexion);
                cmdComando.CommandType = CommandType.StoredProcedure;
                cmdComando.Parameters.Add("@cod_usuario", SqlDbType.Int).Value = pUserID;
                cmdComando.Parameters.Add("@cod_funcionalidad", SqlDbType.Int).Value = pFunctionalityID;
                cmdComando.Parameters.Add("@respuesta", SqlDbType.Bit).Value = pLike;


                SqlDataAdapter adpAdapter = new SqlDataAdapter(cmdComando);

                DataSet dsConsulta = new DataSet();

                adpAdapter.Fill(dsConsulta, "consulta");
                cnnConexion.Close();
                return Insertion(dsConsulta);
            }
            catch (Exception ex)
            {
                throw new Exception("Error, al insertar respuesta de usuario " + ex.Message);
            }

        }

        public bool Insertion(DataSet pData)
        {
            int cod = 0;

            if (pData.Tables[0].Rows.Count != 0)
            {

                cod = Convert.ToInt16(pData.Tables[0].Rows[0]["Column1"].ToString());
                if (cod == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }

        public DataSet Getfunctionalities(int pCodfunctionalities)
        {
            SqlConnection cnnConexion = acConnection.ObtenerConexion();
            DataSet dsConsulta = new DataSet();

            try
            {
                string strSentenciaSQL = "SpSelectDescripcionFuncionalidad";
                strSentenciaSQL = string.Format(strSentenciaSQL);

                SqlCommand cmdComando = new SqlCommand(strSentenciaSQL, cnnConexion);
                cmdComando.CommandType = CommandType.StoredProcedure;
                cmdComando.Parameters.Add("@Cod_Categoria", SqlDbType.Int).Value = pCodfunctionalities;
                SqlDataAdapter adpAdapter = new SqlDataAdapter(cmdComando);

                adpAdapter.Fill(dsConsulta, "consulta");

                cnnConexion.Close();
            }
            catch (Exception ex)
            {

                throw new Exception("Error, al obtener respuesta de usuario " + ex.Message);
            }


            return dsConsulta;
        }

        public List<mCategoty> Getcategories()
        {
            SqlConnection cnnConexion = acConnection.ObtenerConexion();
            DataSet dsConsulta = new DataSet();
            try
            {
                string strSentenciaSQL = "SpSelectCategorias";
                strSentenciaSQL = string.Format(strSentenciaSQL);

                SqlCommand cmdComando = new SqlCommand(strSentenciaSQL, cnnConexion);
                cmdComando.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adpAdapter = new SqlDataAdapter(cmdComando);

                adpAdapter.Fill(dsConsulta, "consulta");

                cnnConexion.Close();

                return dsConsulta.Tables["consulta"].AsEnumerable().Select(x => new mCategoty
                {
                    CategoryID = x.Field<int>("IdCategoria"),
                    Name = x.Field<string>("Nombre")
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error, al obtener las categorias " + ex.Message);
            }
        }

        public List<mFunctionality> Getanswer(string pQuestion, int pCodFunctionality)
        {
            decimal cant = 0;
            decimal cantword = 0;
            DataSet dsFunctionalities = new DataSet();
            //Obtener funcionalidades por cod de categoria 
            dsFunctionalities = Getfunctionalities(pCodFunctionality);
            string[] words = pQuestion.ToUpper().Split(' ');  //Separar la cadena 

            List<mFunctionality> list = new List<mFunctionality>();

            try
            {
                var result = dsFunctionalities.Tables[0].AsEnumerable().Select(x => new
                {
                    FunctionalityID = x.Field<int>("IdFuncionalidad"),
                    Descripcion = x.Field<string>("Descripcion"),
                    CategoriaID = x.Field<int>("CodCategoria"),
                    URL = x.Field<string>("Url"),
                    //Coincidencias = new
                    //{
                    Word = x.Field<string>("Descripcion").ToUpper().Trim().Split(' ').Select(y => words.Contains(y))
                    //}
                }).ToList();

               
                foreach (var item in result)
                {
                    foreach (var word in item.Word)
                    {
                        if (item.Word.Count() >= 7)
                        {
                            cantword = 7;
                        }
                        else
                        {
                            cantword = item.Word.Count();
                        }
                        if (word.Equals(true))
                        {
                            cant = cant + 1;
                        }

                    }

                    if (((cant / cantword) * 100) >= 35)
                    {
                        list.Add(new mFunctionality()
                        {
                            FunctionalityID = item.FunctionalityID,
                            CategoriaID = item.CategoriaID,
                            Description = item.Descripcion,
                            URL = item.URL,
                            Coincidencia = (cant / cantword).ToString("N2") // para luego mostrar la coincidencia mayor en la view si no existe una de 100% 
                                                                            //se deben mostrar al menos 3
                                                                            //o se muestra la que sea igual a 100% y si no hay ninguna mayor a 100 sino que varian de 
                                                                            // 35 en adelante muestran 3 opciones 

                        });
                    }
                    cant = 0;

                 }
               
            }
            catch (Exception)
            {

                throw;
            }
            return list;

        }
    }
}
