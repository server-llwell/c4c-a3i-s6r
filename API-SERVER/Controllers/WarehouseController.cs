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
    [Produces("application / json")]
    [Route(Global.ROUTE_PX+ "/[controller]/[action]")]
    public class WarehouseController :Controller
    {
        /// <summary>
        /// 获取收货订单-代销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CollectGoods")]
        public ActionResult CollectGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }
        /// <summary>
        /// 获取收货订单详情-代销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CollectGoodsList")]
        public ActionResult CollectGoodsList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 获取收货确认or退货确认-代销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ConfirmGoods")]
        public ActionResult ConfirmGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 获取收货确认or退货确认-代销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ExportSendGoods")]
        public ActionResult ExportSendGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 我要发货-运营-导入
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("OperationDeliveryImport")]
        public ActionResult OperationDeliveryImport([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 我要发货-运营-采购商下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeliveryPurchasersList")]
        public ActionResult DeliveryPurchasersList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }


        /// <summary>
        /// 发货列表-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeliverOrderList")]
        public ActionResult DeliverOrderList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 发货列表撤回-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeliverOrderListWithdraw")]
        public ActionResult DeliverOrderListWithdraw([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 发货列表删除-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeliverOrderListDelete")]
        public ActionResult DeliverOrderListDelete([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 发货商品列表-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeliverGoodsList")]
        public ActionResult DeliverGoodsList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 查看发货单-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeliverOrderDetails")]
        public ActionResult DeliverOrderDetails([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 发货数量修改or安全数量-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeliverGoodsNum")]
        public ActionResult DeliverGoodsNum([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 发货商品删除-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeliverGoodsDelete")]
        public ActionResult DeliverGoodsDelete([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 选择发货商品列表-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ChooseDeliverGoods")]
        public ActionResult ChooseDeliverGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 选中发货商品-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ChooseGoods")]
        public ActionResult ChooseGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 发货单提交接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeliverOrderSubmission")]
        public ActionResult DeliverOrderSubmission([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 发货单保存接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeliverOrderConserve")]
        public ActionResult DeliverOrderConserve([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 库存管理-平台库存-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("PlatformInventory")]
        public ActionResult PlatformInventory([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }

        /// <summary>
        /// 平台库存-导入入库商品-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("OnloadWarehousingGoods")]
        public ActionResult OnloadWarehousingGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.WarehouseApi, param));
        }
    }
}
