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
    public class ApiController : Controller
    {
        /// <summary>
        /// 上传订单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ImportOrder")]
        public ActionResult ImportOrder([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ApiApi, param));
        }
        /// <summary>
        /// 获取商品
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetGoodsList")]
        public ActionResult GetGoodsList([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ApiApi, param));
        }
        /// <summary>
        /// 根据小程序传来的数据，绑定openId，appId和pagentCode
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("BindingWXAPP")]
        public ActionResult BindingWXAPP([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ApiApi, param));
        }
        /// <summary>
        /// 根据小程序传来的数据，新建分销商账号
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("BindingWXB2B")]
        public ActionResult BindingWXB2B([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ApiApi, param));
        }
        /// <summary>
        /// 根据小程序传来的openid,确认账号类型，1代理，2分销，3买家
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetTypeByOpenId")]
        public ActionResult GetTypeByOpenId([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ApiApi, param));
        }
        /// <summary>
        /// 获取分销商的收益
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetProfitByOpenId")]
        public ActionResult GetProfitByOpenId([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ApiApi, param));
        }
        /// <summary>
        /// 获取建立下线的二维码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetQrcode")]
        public ActionResult GetQrcode([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ApiApi, param));
        }
        /// <summary>
        /// 获取分销推广二维码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetAgetnQrcode")]
        public ActionResult GetAgetnQrcode([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ApiApi, param));
        }
        /// <summary>
        /// 添加银行卡信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AddBankCode")]
        public ActionResult AddBankCode([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ApiApi, param));
        }
        /// <summary>
        /// 获取银行卡信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetBankCode")]
        public ActionResult GetBankCode([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.ApiApi, param));
        }
    }
}