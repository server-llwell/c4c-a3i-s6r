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
        /// <summary>
        /// 获取渠道商类型下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetPlatform")]
        public ActionResult GetPlatform([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.DistributorApi, param));
        }
        /// <summary>
        /// 获取渠道商费用列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DistributorList")]
        public ActionResult DistributorList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.DistributorApi, param));
        }
        /// <summary>
        /// 修改渠道商费用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UpdateDistributor")]
        public ActionResult UpdateDistributor([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.DistributorApi, param));
        }
        /// <summary>
        /// 获取渠道商商品列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DGoodsList")]
        public ActionResult DGoodsList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.DistributorApi, param));
        }
        /// <summary>
        /// 修改渠道商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UpdateDGoods")]
        public ActionResult UpdateDGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.DistributorApi, param));
        }

    }
}