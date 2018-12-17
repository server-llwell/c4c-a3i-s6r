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
        /// 已报价、报价中、已报价（二次）、已完成状态的商品分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("InquiryPagesn")]
        public ActionResult InquiryPagesn([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.PurchaseApi, param));
        }
    }
}
