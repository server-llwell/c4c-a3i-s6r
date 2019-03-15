using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_SERVER.Common;
using Microsoft.AspNetCore.Mvc;

namespace API_SERVER.Controllers
{
    [Produces("application/json")]
    [Route(Global.ROUTE_PX + "/[controller]/[action]")]
    public class NewHomePageController : Controller
    {

        /// <summary>
        /// 总分类接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AllClassification")]
        public ActionResult AllClassification([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.NewHomePageApi, param));
        }

        /// <summary>
        /// 首页上半部接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("HomePage")]
        public ActionResult HomePage([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.NewHomePageApi, param));
        }

        /// <summary>
        /// 首页下半部接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("HomePageDownPart")]
        public ActionResult HomePageDownPart([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.NewHomePageApi, param));

        }
        /// <summary>
        /// 首页各馆换一批接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("HomePageChangeGoods")]
        public ActionResult HomePageChangeGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.NewHomePageApi, param));
        }

        /// <summary>
        /// 品类页接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CategoryGoods")]
        public ActionResult CategoryGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.NewHomePageApi, param));
        }

        /// <summary>
        /// 搜索页接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SelectGoods")]
        public ActionResult SelectGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.NewHomePageApi, param));
        }

        /// <summary>
        /// 国家馆接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CountryGoods")]
        public ActionResult CountryGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.NewHomePageApi, param));
        }

        /// <summary>
        /// 品牌页接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("BrandsGoods")]
        public ActionResult BrandsGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.NewHomePageApi, param));
        }

        /// <summary>
        /// 商品详情页接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GoodsDetails")]
        public ActionResult GoodsDetails([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.NewHomePageApi, param));
        }

        /// <summary>
        /// 添加取消收藏接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UserCollection")]
        public ActionResult UserCollection([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.NewHomePageApi, param));
        }

        /// <summary>
        /// 收藏商品接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UserCollectionGoods")]
        public ActionResult UserCollectionGoods([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.NewHomePageApi, param));
        }

        /// <summary>
        /// 关注品牌展示接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UserCollectionBrands")]
        public ActionResult UserCollectionBrands([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.NewHomePageApi, param));
        }
    }
}
