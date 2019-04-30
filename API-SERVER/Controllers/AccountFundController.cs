using API_SERVER.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Controllers
{
    [Produces("application/json")]
    [Route(Global.ROUTE_PX + "/[controller]/[action]")]
    public class AccountFundController : Controller
    {
        /// <summary>
        /// 获取用户余额
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetRetailMoney")]
        public ActionResult GetRetailMoney([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.AccountFundApi, param));
        }

        /// <summary>
        /// 用户充值
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("RetailRecharge")]
        public ActionResult RetailRecharge([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.AccountFundApi, param));
        }
    }
}
