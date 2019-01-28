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

        /// <summary>
        /// 获取合同列表-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ContractList")]
        public ActionResult ContractList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.AgreementApi, param));
        }

        /// <summary>
        /// 获取合同详情-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ContractDetails")]
        public ActionResult ContractDetails([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.AgreementApi, param));
        }

        /// <summary>
        /// 创建合同上传图片-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ContractUploadImg")]
        public ActionResult ContractUploadImg([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.AgreementApi, param));
        }

        /// <summary>
        /// 创建合同-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CreateContract")]
        public ActionResult CreateContract([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.AgreementApi, param));
        }

        /// <summary>
        /// 合同-搜索客户名接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SelectUserName")]
        public ActionResult SelectUserName([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.AgreementApi, param));
        }
    }
}
