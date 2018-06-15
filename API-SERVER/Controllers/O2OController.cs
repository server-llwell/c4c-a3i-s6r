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
    public class O2OController : Controller
    {
        [HttpPost]
        [ActionName("O2OOrderList")]
        public ActionResult TicketList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.O2OApi, param));
        }
        [HttpPost]
        [ActionName("O2OOrder")]
        public ActionResult Ticket([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.O2OApi, param));
        }
        [HttpPost]
        [ActionName("UpdateStatus")]
        public ActionResult UpdateStatus([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.O2OApi, param));
        }
    }
}