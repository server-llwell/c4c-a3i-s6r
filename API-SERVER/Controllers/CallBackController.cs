using API_SERVER.Buss;
using API_SERVER.Common;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace API_SERVER.Controllers
{
    [Produces("text/xml")]
    [Route(Global.ROUTE_PX + "/[controller]/[action]")]
    public class CallBackController : Controller
    {
        /// <summary>
        /// 支付操作类API回调地址
        /// </summary>
        /// <param name="paymentApi"></param>
        /// <returns></returns>
        [HttpPost]
        public XmlResult PaymentCallBack()
        {
            ResponseHandler resHandler = new ResponseHandler(HttpContext);

            CallBackBuss paymentCallBackBuss = new CallBackBuss();
            string result = paymentCallBackBuss.PaymentCallBack(resHandler);
            return this.Xml(result);

        }

    }
}
