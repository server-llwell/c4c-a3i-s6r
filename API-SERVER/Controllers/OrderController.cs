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
        ///零售订单支付
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("PayOrder")]
        public ActionResult PayOrder([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }

        /// <summary>
        /// 零售订单退货申请
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ReGoodsApply")]
        public ActionResult ReGoodsApply([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }
      

        /// <summary>
        /// 零售订单退货成功接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("MakeSureReGoods")]
        public ActionResult MakeSureReGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }

        /// <summary>
        /// 零售订单同意退货接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AgreeReGoods")]
        public ActionResult AgreeReGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }

        /// <summary>
        /// 零售订单填写退货运单号接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ReGoodsFundId")]
        public ActionResult ReGoodsFundId([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }

        /// <summary>
        /// 零售订单退货双方信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ReGoodsMessage")]
        public ActionResult ReGoodsMessage([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }

        /// <summary>
        /// 零售订单退货运单号信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ReGoodsFundIdMessage")]
        public ActionResult ReGoodsFundIdMessage([FromBody]object param)
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
        /// 获取发货信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetConsigneeMsg")]
        public ActionResult GetConsigneeMsg([FromBody]object param)
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
        /// <summary>
        /// 获取订单页二维码图片
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetOrderPageQRCode")]
        public ActionResult GetOrderPageQRCode([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }

        /// <summary>
        /// 获取清关信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetCustomsState")]
        public ActionResult GetCustomsState([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        }

        #region 搁置财务部分
        /// <summary>
        /// 获取销售日报表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        //[HttpPost]
        //[ActionName("GetSalesFrom")]
        //public ActionResult GetSalesFrom([FromBody]object param)
        //{
        //    return Json(Global.BUSS.BussResults(this, ApiType.OrderApi, param));
        //}
        #endregion


        #endregion

        #region 上传、导出
        /// <summary>
        /// 上传直邮订单
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
        /// 上传代销订单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UploadOrderDX")]
        public ActionResult UploadOrderDX([FromBody]object param)
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