using _1Fix.Common.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _1Fix.Shop.Controllers
{
    [TerminalAuthorization(true)]
    public class IdentificationController : Controller
    {
        //
        // GET: /Identification/

        public ActionResult Index()
        {
            return View();
        }

    }
}
