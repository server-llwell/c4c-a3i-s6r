using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_SERVER.Buss;
using API_SERVER.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace API_SERVER.Controllers
{
    [Produces("application/x-www-form-urlencoded")]
    [Route(Global.ROUTE_PX + "/[controller]/[action]")]
    public class CustomsApiController : Controller
    {
        /// <summary>
        /// 企业接收海关发起的支付相关实时数据获取请求
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("platDataOpen")]
        public ActionResult platDataOpen(Dictionary<string, string> param)
        {
            Console.WriteLine(param);
            return Json(Global.BUSS.BussResults(this, ApiType.CustomsApiApi, param));
        }
    }
}