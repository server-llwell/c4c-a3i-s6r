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
    public class DashboardController : Controller
    {
        /// <summary>
        /// 获取渠道商类型下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetWorkBenchS")]
        public ActionResult GetWorkBenchS([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.DashboardApi, param));
        }
        /// <summary>
        /// 获取渠道商费用列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetWorkBenchO")]
        public ActionResult GetWorkBenchO([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.DashboardApi, param));
        }
    }
}