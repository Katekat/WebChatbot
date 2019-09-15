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
                cmdComando.Parameters.Add("@id_funcionalidad", SqlDbType.Int).Value = pFunctionalityID;
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
                cmdComando.Parameters.Add("@idCategoria", SqlDbType.Int).Value = pCodfunctionalities;
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
            string[] words = pQuestion.Trim().ToUpper().Split(' ');  //Separar la cadena 
            cantword = words.Length; // Para mantener la cantidad original de palabras ingresadas

            var plurilizacion = (from item in words
                                 where item.Length > 4 && Convert.ToChar(item.Substring(item.Length - 1)) != 'S'  //Evaluo si ya no viene una palabra en plural
                                 select new
                                 {
                                     item = item + "S"
                                 }).ToList();


            foreach (var item in plurilizacion)
            {
                pQuestion = pQuestion + " " + item.item.ToString(); //Se le agrega a la cadena original las otras palabras en plural a buscar
            }

            words = pQuestion.Trim().ToUpper().Split(' '); // Arreglo con todas las palabras a buscar
            List<mFunctionality> list = new List<mFunctionality>();

            try
            {
                var result = dsFunctionalities.Tables[0].AsEnumerable().Select(x => new
                {
                    FunctionalityID = x.Field<int>("IdFuncionalidad"),
                    Descripcion = x.Field<string>("Descripcion"),
                    CategoriaID = x.Field<int>("idCategoria"),
                    URL = x.Field<string>("Url"),
                    //Coincidencias = new
                    //{
                    Word = x.Field<string>("Descripcion").ToUpper().Trim().Split(' ').Select(y => words.Contains(y))
                    //}
                }).ToList();

                foreach (var item in result)
                {
                    // cantword = (item.Word.Count());
                    cant = item.Word.Count(x => x.Equals(true));
                    decimal probabilidad = ((cant / cantword) * 100);
                    if (probabilidad >= 35)
                    {
                        list.Add(new mFunctionality()
                        {
                            FunctionalityID = item.FunctionalityID,
                            CategoriaID = item.CategoriaID,
                            Description = item.Descripcion,
                            URL = item.URL,
                            Coincidencia = probabilidad.ToString("N2") // para luego mostrar la coincidencia mayor en la view si no existe una de 100% 
                                                                       //se deben mostrar al menos 3

                        });
                    }
                    cant = 0;
                    if (probabilidad == 100)
                        break;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return list;

        }
    }
}
