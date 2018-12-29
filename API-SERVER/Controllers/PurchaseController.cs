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
    public class PurchaseController : Controller
    {
        /// <summary>
        /// 导入询价商品
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("OnLoadGoodsList")]
        public ActionResult OnLoadGoodsList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 询价、询价中、待提交状态的商品分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Goodspagination")]
        public ActionResult Goodspagination([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 询价商品删除接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GoodsDelete")]
        public ActionResult GoodsDelete([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 询价表保存接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("InquiryPreservation")]
        public ActionResult InquiryPreservation([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 询价表提交接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("InquirySubmission")]
        public ActionResult InquirySubmission([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 询价列表接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("InquiryList")]
        public ActionResult InquiryList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 询价列表查看接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("InquiryListDetailed")]
        public ActionResult InquiryListDetailed([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 表单删除接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("InquiryListDelete")]
        public ActionResult InquiryListDelete([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 已报价状态的商品分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("InquiryPagesn")]
        public ActionResult InquiryPagesn([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 已报价商品详情接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GoodsDetails")]
        public ActionResult GoodsDetails([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 已报价商品详情确定接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GoodsDetailsDetermine")]
        public ActionResult GoodsDetailsDetermine([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 已报价商品确定接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GoodsDetermine")]
        public ActionResult GoodsDetermine([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 已报价商品删除接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("InquiryGoodsDelete")]
        public ActionResult InquiryGoodsDelete([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 已报价表取消接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("OfferCancel")]
        public ActionResult OfferCancel([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 已报价表提交接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("OfferSub")]
        public ActionResult OfferSub([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 报价中、已报价（二次）、已完成状态的商品分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("OtherInquiryPagesn")]
        public ActionResult OtherInquiryPagesn([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 报价中、已报价（二次）、已完成状态的商品详情接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("OtherGoodsDetails")]
        public ActionResult OtherGoodsDetails([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 导入询价商品
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SubOnLoadGoodsList")]
        public ActionResult SubOnLoadGoodsList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 已报价（二次）提交接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("OfferedSub")]
        public ActionResult OfferedSub([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 采购列表接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("PurchaseList")]
        public ActionResult PurchaseList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }

        /// <summary>
        /// 查看采购接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("PurchaseDetails")]
        public ActionResult PurchaseDetails([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }
    }
}
