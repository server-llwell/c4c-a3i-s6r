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
    public class OrderController : Controller
    {
        [HttpPost]
        [ActionName("GetOrderList")]
        public ActionResult TicketList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
        [HttpPost]
        [ActionName("GetOrder")]
        public ActionResult Ticket([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
        [HttpPost]
        [ActionName("UploadOrder")]
        public ActionResult UploadOrder([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
    }

}