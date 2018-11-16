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
    public class UserController :Controller
    {
        /// <summary>
        /// 获取运营客户
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetOperateCustomer")]
        public ActionResult GetOperateCustomer([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
    }
}
