using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_SERVER.Common;
using Microsoft.AspNetCore.Mvc;

namespace API_SERVER.Controllers
{
    [Produces("application / json") ]
    [Route(Global.ROUTE_PX+"/[controller]/[action]")]
    public class AgreementController : Controller
    {
        /// <summary>
        /// 获取合同信息-代销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ContractInformation")]
        public ActionResult ContractInformation([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.AgreementApi, param));
        }
    }
}
