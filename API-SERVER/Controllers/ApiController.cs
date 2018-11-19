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
    public class ApiController : Controller
    {
        /// <summary>
        /// 上传订单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ImportOrder")]
        public ActionResult ImportOrder([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ApiApi, param));
        }
        /// <summary>
        /// 获取商品
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetGoodsList")]
        public ActionResult GetGoodsList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ApiApi, param));
        }
        /// <summary>
        /// 根据小程序传来的数据，绑定openId，appId和pagentCode
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("BindingWXAPP")]
        public ActionResult BindingWXAPP([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ApiApi, param));
        }
    }
}