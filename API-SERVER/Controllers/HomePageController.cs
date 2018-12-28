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
        /// 顶部所有分类接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AllMessage")]
        public ActionResult AllMessage([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.HomePageApi, param));
        }
    }
}
