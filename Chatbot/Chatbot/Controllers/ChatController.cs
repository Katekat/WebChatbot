using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
                ViewData["Like"] = false;
                ViewData["Dislike"] = false;

                string pQuestion = RemoveDiacritics(requestChat.Request);
                Lrequest = acBot.Getanswer(pQuestion, Convert.ToInt16(requestChat.FunctionalityID));
                mResponseChat responseChat = new mResponseChat();

                if (Lrequest.Count == 0)
                {
                    responseChat.TypeResponse = false;
                    responseChat.MessageResponse = " No encontré resultados para tu consulta. Prueba ingresando otra consulta " +
                        "o seleccionando otra funcionalidad. Si no, puedes enviar un mensaje a nuestros asesores, que te responderán dentro de las próximas 24 hs";
                    ViewBag.Categories = GetCategories();
                }
                else
                {
                    responseChat.TypeResponse = true;
                    responseChat.MessageResponse = "Encontré estos resultados para tu consulta";

                    var m = from word in Lrequest
                            where Convert.ToDecimal(word.Coincidencia) == 100
                            select word;
                    if (m.Count() >= 1)
                    {
                        responseChat.Functionality = m.ToList();
                    }
                    else
                    {
                        responseChat.Functionality = Lrequest.OrderByDescending(x => x.Coincidencia).Take(3).ToList();// maximo 3 respuesa al usuario

                    }

                }
                return PartialView("_ResponseMessage", responseChat);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetEffectivenessMeasurement(long pUserID, long pFunctionalityID, bool pLike)
        {
            try
            {

                bool resp = acBot.SaveAnswer(pUserID, pFunctionalityID, pLike);
                if (resp)
                {

                    return Json(new { isSatisfactorio = true, pUserID, pFunctionalityID, pLike }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { isSatisfactorio = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.DenyGet);
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

        private List<mCategoty> GetCategories()
        {
            return acBot.Getcategories();
        }


    }
}