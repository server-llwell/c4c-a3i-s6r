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
    public class ScanController : Controller
    {
        [HttpPost]
        [ActionName("SCAN")]
        public ActionResult SCAN([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ScanApi, param));
        }
        [HttpPost]
        [ActionName("SCANGOODSURL")]
        public ActionResult SCANGOODSURL([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ScanApi, param));
        }
    }

}