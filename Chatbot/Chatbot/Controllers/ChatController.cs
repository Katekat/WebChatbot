using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using AccesoDatos;
using Models;

namespace Chatbot.Controllers
{
    public class ChatController : Controller
    {
        AccesoDatos.acBot acBot = new acBot();
        // GET: Chat
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult ChatBot()
        {
            try
            {
                ViewBag.Categories = GetCategories();
                return PartialView("_ChatBot", new mRequestChat() { User = new mUser() { UserID = 1, Name = "Luis" } });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public ActionResult GetResponse(mRequestChat requestChat)
        {
            List<mFunctionality> Lrequest = new List<mFunctionality>();
            try
            {
                ViewData["UserID"] = requestChat.User.UserID;

                string pQuestion = RemoveDiacritics(requestChat.Request);
                pQuestion = CleanInput(pQuestion);
                Lrequest = acBot.Getanswer(pQuestion, Convert.ToInt16(requestChat.FunctionalityID));
                mResponseChat responseChat = new mResponseChat();

                if (Lrequest.Count == 0)
                {
                    responseChat.TypeResponse = false;
                    responseChat.MessageResponse = " No encontré resultados para tu consulta. Prueba ingresando otra consulta " +
                        "o seleccionando otra categoría. Si no, puedes enviar un mensaje a nuestros asesores, que te responderán dentro de las próximas 24 hs";
                    ViewBag.Categories = GetCategories();
                }
                else
                {
                    responseChat.TypeResponse = true;
                    responseChat.MessageResponse = "Encontré estos resultados para tu consulta";

                    responseChat.Functionality = Lrequest.OrderByDescending(x => x.Coincidencia).Take(3).ToList();// máximo 3 respuesa al usuario

                }
                ViewData["msgBot"] = Convert.ToInt32(ViewData["msgBot"]) + 1;
                ViewBag.Categories = GetCategories();
                return PartialView("_ResponseMessage", responseChat);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult SetEffectivenessMeasurement(long pUserID, long pFunctionalityID, bool pLike, long pCategoriaID)
        {
            try
            {
                if (acBot.SaveAnswer(pUserID, pFunctionalityID, pLike, pCategoriaID))
                {
                    return Json(new { isSatisfactorio = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { isSatisfactorio = false, message = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { isSatisfactorio = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        ///  pasar la cadena por aqui antes de enviarla al acceso a datos
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String RemoveDiacritics(String s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        public static string CleanInput(string strIn)
        {
            // Reemplaza caracteres invalidos de la cadena
            try
            {
                return Regex.Replace(strIn, @"[^0-9A-Za-z' ']", "", RegexOptions.None);
            }

            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }

        private List<mCategoty> GetCategories()
        {
            return acBot.Getcategories();
        }


    }
}