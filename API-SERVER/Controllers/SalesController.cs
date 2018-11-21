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
    public class SalesController : Controller
    {
        /// <summary>
        /// 获取渠道商下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetPurchase")]
        public ActionResult GetPurchase([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.SalesApi, param));
        }
        /// <summary>
        /// 获取分销商下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetDistribution")]
        public ActionResult GetDistribution([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.SalesApi, param));
        }
        /// <summary>
        /// 获取销售统计-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetSalesListByOperator")]
        public ActionResult GetSalesListByOperator([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.SalesApi, param));
        }
        /// <summary>
        /// 获取销售统计-供应商
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetSalesListBySupplier")]
        public ActionResult GetSalesListBySupplier([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.SalesApi, param));
        }
        /// <summary>
        /// 获取销售统计-代理
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetSalesListByAgent")]
        public ActionResult GetSalesListByAgent([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.SalesApi, param));
        }
        /// <summary>
        /// 获取销售统计-代理
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetClient")]
        public ActionResult GetClient([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.SalesApi, param));
        }
        /// <summary>
        /// 获取销售商品-代销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetGoods")]
        public ActionResult GetGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.SalesApi, param));
        }

       

    }

}