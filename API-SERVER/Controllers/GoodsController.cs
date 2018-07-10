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
    public class GoodsController : Controller
    {
        #region 商品管理
        /// <summary>
        /// 获取品牌下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetBrand")]
        public ActionResult GetBrand([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
        /// <summary>
        /// 获取仓库下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetWarehouse")]
        public ActionResult GetWarehouse([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetGoodsList")]
        public ActionResult GetGoodsList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
        /// <summary>
        /// 获取单个商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetGoods")]
        public ActionResult GetGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
        /// <summary>
        /// 修改商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UpdateGoods")]
        public ActionResult UpdateGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
        #endregion

        #region 商品库存上传
        /// <summary>
        /// 商品库存上传列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UploadList")]
        public ActionResult UploadList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
        /// <summary>
        /// 上传商品库存信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UploadWarehouseGoods")]
        public ActionResult UploadWarehouseGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
        /// <summary>
        /// 上传商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UploadGoods")]
        public ActionResult UploadGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
        /// <summary>
        /// 上传商品图片zip
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UploadGoodsZip")]
        public ActionResult UploadGoodsZip([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
        #endregion

        #region 仓库列表
        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetWarehouseList")]
        public ActionResult GetWarehouseList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
        /// <summary>
        /// 新增仓库信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AddWarehouse")]
        public ActionResult AddWarehouse([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
        /// <summary>
        /// 修改仓库信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UpdateWarehouse")]
        public ActionResult UpdateWarehouse([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.GoodsApi, param));
        }
        #endregion
    }
}