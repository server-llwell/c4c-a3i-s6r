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
    public class HomePageController :Controller
    {
        /// <summary>
        /// 首页接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AllMessage")]
        public ActionResult AllMessage([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.HomePageApi, param));
        }

        /// <summary>
        /// 国家馆接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CountrySalseList")]
        public ActionResult CountrySalseList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.HomePageApi, param));
        }

        /// <summary>
        /// 国家馆商品接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CountryGoods")]
        public ActionResult CountryGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.HomePageApi, param));
        }
    }
}
