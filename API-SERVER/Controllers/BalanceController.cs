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
    public class BalanceController : Controller
    {
        /// <summary>
        /// 获取运营的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetBalanceListByOperator")]
        public ActionResult GetBalanceListByOperator([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取供应商的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetBalanceListBySupplier")]
        public ActionResult GetBalanceListBySupplier([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取采购商的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetBalanceListByPurchase")]
        public ActionResult GetBalanceListByPurchase([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取代理的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetBalanceListByAgent")]
        public ActionResult GetBalanceListByAgent([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取分销的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetBalanceListByDistribution")]
        public ActionResult GetBalanceListByDistribution([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
    }
}