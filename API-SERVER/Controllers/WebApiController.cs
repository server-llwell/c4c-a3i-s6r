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
    public class WebApiController : Controller
    {
        [HttpPost]
        [ActionName("giveWaybillList")]
        public ActionResult giveWaybillList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WebApiApi, param));
        }
        [HttpPost]
        [ActionName("sendOrderList")]
        public ActionResult sendOrderList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WebApiApi, param));
        }
        [HttpPost]
        [ActionName("getGoodsList")]
        public ActionResult getGoodsList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WebApiApi, param));
        }
    }

}