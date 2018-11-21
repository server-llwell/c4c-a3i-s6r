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
    [Produces("application / json")]
    [Route(Global.ROUTE_PX+ "/[controller]/[action]")]
    public class WarehouseController :Controller
    {
        /// <summary>
        /// 获取收货订单-代销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
    [ActionName("CollectGoods")]
    public ActionResult CollectGoods([FromBody]object param)
    {
        return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
    }
        /// <summary>
        /// 获取收货订单详情-代销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CollectGoodsList")]
        public ActionResult CollectGoodsList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 获取收货确认or退货确认-代销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ConfirmGoods")]
        public ActionResult ConfirmGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

       
    }
}
