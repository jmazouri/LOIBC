using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LOIBC.SpamHeuristics;
using Microsoft.AspNetCore.Mvc;

namespace LOIBC.WebInterface.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
