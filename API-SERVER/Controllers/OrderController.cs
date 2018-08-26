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
    public class OrderController : Controller
    {
        #region 查询
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetOrderList")]
        public ActionResult GetOrderList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
        /// <summary>
        /// 获取单个订单信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetOrder")]
        public ActionResult GetOrder([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
        /// <summary>
        /// 获取快递下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetExpress")]
        public ActionResult GetExpress([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
        /// <summary>
        /// 确认发货
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SingleWaybill")]
        public ActionResult SingleWaybill([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
        /// <summary>
        /// 海外已出库
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Overseas")]
        public ActionResult Overseas([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }

        #endregion

        #region 上传、导出
        /// <summary>
        /// 上传订单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UploadOrder")]
        public ActionResult UploadOrder([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
        /// <summary>
        /// 上传分销商订单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UploadOrderOfDistribution")]
        public ActionResult UploadOrderOfDistribution([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
        /// <summary>
        /// 导出订单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ExportOrder")]
        public ActionResult ExportOrder([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
        /// <summary>
        /// 上传运单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UploadWaybill")]
        public ActionResult UploadWaybill([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
        /// <summary>
        /// 导出运单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ExportWaybill")]
        public ActionResult ExportWaybill([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
        /// <summary>
        /// 导出查询出来的订单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ExportSelectOrder")]
        public ActionResult ExportSelectOrder([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
        #endregion
    }

}