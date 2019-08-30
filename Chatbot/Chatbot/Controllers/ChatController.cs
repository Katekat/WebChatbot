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
                ViewBag.Funtionalitys = GetFunctionalits();
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

                Lrequest = acBot.Getanswer(requestChat.Request, Convert.ToInt16(requestChat.FunctionalityID));
                mResponseChat responseChat = new mResponseChat();

                if (Lrequest.Count == 0)
                {
                    responseChat.TypeResponse = false;
                    responseChat.MessageResponse = " No encontré resultados para tu consulta. Prueba ingresando otra consulta " +
                        "o seleccionando otra funcionalidad. Si no, puedes enviar un mensaje a nuestros asesores, que te responderán dentro de las próximas 24 hs";
                    ViewBag.Funtionalitys = GetFunctionalits();
                }
                else
                {
                    responseChat.TypeResponse = true;
                    responseChat.MessageResponse = "Encontré estos resultados para tu consulta";
                    responseChat.Functionality = Lrequest;
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
        public JsonResult SetEffectivenessMeasurement(mEffectivenessMeasurement effectiveness)
        {
            try
            {
                if (!effectiveness.Like)
                {
                    return Json(GetFunctionalits(), JsonRequestBehavior.AllowGet);
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
        /// Por probar pasar la cadena por aqui antes de enviarla al acceso a datos
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

        private List<mFunctionality> GetFunctionalits()
        {
            List<mFunctionality> functionalitys = new List<mFunctionality>();
            functionalitys.Add(new mFunctionality() { FunctionalityID = 1, Name = "FAQ", Description = "", URL = string.Empty });
            functionalitys.Add(new mFunctionality() { FunctionalityID = 2, Name = "Entrenamiento", Description = "", URL = string.Empty });
            functionalitys.Add(new mFunctionality() { FunctionalityID = 3, Name = "Guías paso a paso", Description = "", URL = string.Empty });
            functionalitys.Add(new mFunctionality() { FunctionalityID = 4, Name = "Consultas", Description = "", URL = string.Empty });
            functionalitys.Add(new mFunctionality() { FunctionalityID = 5, Name = "Gestión de usuarios", Description = "", URL = string.Empty });
            functionalitys.Add(new mFunctionality() { FunctionalityID = 6, Name = "Otros", Description = "", URL = string.Empty });
            return functionalitys;
        }



    }
}