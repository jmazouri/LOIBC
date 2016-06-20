using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LOIBC.WebInterface.Controllers
{
    public class ApiController : Controller
    {
        private LOIBCBot _bot;

        public ApiController(LOIBCBot bot)
        {
            _bot = bot;
        }

        [HttpGet]
        public IEnumerable<string> Data()
        {
            return _bot.SpamHeuristics.Select(d => d.GetType().Name);
        }
    }
}
