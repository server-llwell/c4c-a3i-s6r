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
    public class GoodsController : Controller
    {
        [HttpPost]
        [ActionName("GetBrand")]
        public ActionResult GetBrand([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
        [HttpPost]
        [ActionName("GetWareHouse")]
        public ActionResult GetWareHouse([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
    }
}