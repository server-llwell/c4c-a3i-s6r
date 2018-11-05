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
        #region 旧结算表
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
        #endregion

        #region 新结算表
        /// <summary>
        /// 获取合作方
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetPartner")]
        public ActionResult GetPartner([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取结算收益-预估收益
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetEstimate")]
        public ActionResult GetEstimate([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取结算收益-已结算收益
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetSettle")]
        public ActionResult GetSettle([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取结算收益-已结算收益
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetSettleInfo")]
        public ActionResult GetSettleInfo([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取结算收益-累计收益
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetTotalProfit")]
        public ActionResult GetTotalProfit([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        #endregion
    }
}