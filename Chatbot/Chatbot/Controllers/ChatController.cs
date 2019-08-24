using Chatbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chatbot.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult ChatBot()
        {
            try
            {
                List<Functionality> functionalitys = new List<Functionality>();
                functionalitys.Add(new Functionality() { FunctionalityID = 1, Description = "FAQ" });
                functionalitys.Add(new Functionality() { FunctionalityID = 2, Description = "Entrenamiento" });
                functionalitys.Add(new Functionality() { FunctionalityID = 3, Description = "Guías paso a paso" });
                functionalitys.Add(new Functionality() { FunctionalityID = 4, Description = "Consultas" });
                functionalitys.Add(new Functionality() { FunctionalityID = 5, Description = "Gestión de usuarios" });
                functionalitys.Add(new Functionality() { FunctionalityID = 6, Description = "Otros" });
                ViewBag.Funtionalitys = functionalitys;
                return PartialView("_ChatBot", new RequestChat() { User = new User() { UserID = 1, Name = "Luis" } });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public JsonResult GetResponse(RequestChat requestChat)
        {
            try
            {
                return Json(new { User = requestChat.User.UserID, Response = requestChat.Request }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.DenyGet);
            }
        }
    }
}