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
    public class AgentController : Controller
    {
        /// <summary>
        /// 获取代理所属分销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetDistributionList")]
        public ActionResult GetDistributionList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.AgentApi, param));
        }
        ///// <summary>
        ///// 添加代理所属分销
        ///// </summary>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[ActionName("AddDistribution")]
        //public ActionResult AddDistribution([FromBody]object param)
        //{
        //    return Json(Global.BUSS.BussResults(this, ApiType.AgentApi, param));
        //}
        /// <summary>
        /// 修改代理所属分销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UpdateDistribution")]
        public ActionResult UpdateDistribution([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.AgentApi, param));
        }
        /// <summary>
        /// 删除代理所属分销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeleteDistribution")]
        public ActionResult DeleteDistribution([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.AgentApi, param));
        }
        /// <summary>
        /// 代理二维码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetAgentQRCode")]
        public ActionResult GetAgentQRCode([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.AgentApi, param));
        }
    }
}