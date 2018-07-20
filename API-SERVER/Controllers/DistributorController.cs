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
    public class DistributorController : Controller
    {
        [HttpPost]
        [ActionName("GetPlatform")]
        public ActionResult GetPlatform([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.DistributorApi, param));
        }
        [HttpPost]
        [ActionName("DistributorList")]
        public ActionResult DistributorList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.DistributorApi, param));
        }
        [HttpPost]
        [ActionName("UpdateDistributor")]
        public ActionResult UpdateDistributor([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.DistributorApi, param));
        }
    }
}