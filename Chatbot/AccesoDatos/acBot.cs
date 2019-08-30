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

                /// var filterList = result.Where(l => l.teams.Any(t => t.players.Any(p => p.Age == 20)));
                //    var filterList = result.Where(l => l.Word.Equals(true)).(y=> y.ToString==true));
                //    var q = result.GroupBy(x => x.Name)
                //.Select(x => new {
                //    Count = x.Count(),
                //    Name = x.Key,
                //    ID = x.First().ID
                //})
                //.OrderByDescending(x => x.Count);
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
                                                                            //o se muestra la que sea mayor a 80% y si no hay ninguna mayor a 50 sino que varian de 
                                                                            // 35 a 50 se muestran 3 opciones 

                        });
                    }
                    cant = 0;

                    //item.Word
                }
                //var itemsWithActiveNames = result.Where(x => x.Word.Where(y=>y.Equals(true))/ x.Word.Count())
                //                .ToList();

                //result.Where(l => l.Word).Select(x => x.NameLeage).ToList();
                //foreach (var item in words)
                //{
                //    //if (item.Length <= 3)
                //    //{
                //    var result = dsFunctionalities.Tables[0].AsEnumerable().ToList();

                //        //.Select(x => x.Field<string>("Descripcion").ToUpper().Split(' ').Select(y => y.Equals(item))).Select(x => x).ToList();
                //    //}
                //    //else
                //    //{
                //    //    var result = from b in dsFunctionalities.Tables[0].AsEnumerable()
                //    //                 where b.Field<string>("Descripcion").ToUpper().Contains(item) //Obtener los registros que tenga la palabra que se busca
                //    //                 select b;
                //    //    if (result.Count() > 0)
                //    //    {
                //    //        foreach (DataRow row in result) // Si la palabra esta  se guarda la palabra y el id de la funcionalidad donde se ubico la palabra
                //    //        {
                //    //            list.Add(new mFunctionality()
                //    //            {
                //    //                FunctionalityID = Convert.ToInt32(row["IdFuncionalidad"]),
                //    //                CategoriaID = Convert.ToInt16(row["CodCategoria"]),
                //    //                Description = row["Descripcion"].ToString(),
                //    //                URL = row["Url"].ToString(),
                //    //                Coincidencia = item.ToString()
                //    //            });
                //    //        }
                //    //    }
                //    //}
                //}

                //var Grouped = list.GroupBy(n => n.FunctionalityID).
                //     Select(group =>
                //         new
                //         {
                //             word = group.Key,
                //             Count = group.Count()
                //         });

                ////Luego se debe contar cuantas veces se encuentra el id y palabras 

            }
            catch (Exception)
            {

                throw;
            }
            return list;

        }
    }
}
