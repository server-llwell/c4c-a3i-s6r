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
    public class CustomsController : Controller
    {
        /// <summary>
        /// 获取代理所属分销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetOrderStatus")]
        public ActionResult GetOrderStatus([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.CustomsApi, param));
        }
        /// <summary>
        /// 修改代理所属分销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetReportStatus")]
        public ActionResult GetReportStatus([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.CustomsApi, param));
        }
        /// <summary>
        /// 删除代理所属分销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetWayBillNo")]
        public ActionResult GetWayBillNo([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.CustomsApi, param));
        }
        /// <summary>
        /// 企业接收海关发起的支付相关实时数据获取请求
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("platDataOpen")]
        public ActionResult platDataOpen([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.CustomsApi, param));
        }
    }
}