using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_SERVER.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace API_SERVER.Controllers
{
    [Produces("application/json")]
    [Route(Global.ROUTE_PX + "/[controller]/[action]")]
    public class TicketController : Controller
    {
        [HttpPost]
        [ActionName("Test/Abc")]
        public ActionResult Test([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.TicketApi, param));
        }
    }
}