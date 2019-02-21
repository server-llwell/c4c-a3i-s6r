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
    public class DashboardController : Controller
    {
        #region 旧工作台
        /// <summary>
        /// 工作台-供应商
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetWorkBenchS")]
        public ActionResult GetWorkBenchS([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.DashboardApi, param));
        }
        /// <summary>
        /// 工作台-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetWorkBenchO")]
        public ActionResult GetWorkBenchO([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.DashboardApi, param));
        }
        #endregion

        #region 新工作台
        /// <summary>
        /// 工作台-供应商
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetNewWorkBenchS")]
        public ActionResult GetNewWorkBenchS([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.DashboardApi, param));
        }

        #endregion
    }
}