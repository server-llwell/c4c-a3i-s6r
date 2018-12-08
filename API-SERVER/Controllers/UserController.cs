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
    public class UserController :Controller
    {
        /// <summary>
        /// 获取运营客户
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetOperateCustomer")]
        public ActionResult GetOperateCustomer([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("validate")]
        public ActionResult Validate([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
        /// <summary>
        /// 用户信息及头像
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("currentUser")]
        public ActionResult CurrentUser([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("menu")]
        public ActionResult Menu([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }

        /// <summary>
        /// 消息列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("message/list")]
        public ActionResult Messagelist([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
        /// <summary>
        /// 未读消息数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("message/count")]
        public ActionResult Messagecount([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
        /// <summary>
        /// 清空消息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("message/empty")]
        public ActionResult Messageempty([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("register/submit")]
        public ActionResult Registersubmit([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("register/rename")]
        public ActionResult Registerrename([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("register/renameNew")]
        public ActionResult RegisterrenameNew([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("register/code")]
        public ActionResult Registercode([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
        /// <summary>
        /// 获取修改密码验证码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("register/renamecode")]
        public ActionResult Registerrenamecode([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
        /// <summary>
        /// 获取注册用户状态
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("register/status")]
        public ActionResult Registerstatus([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
        /// <summary>
        /// 审核账号
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("register/check")]
        public ActionResult Registercheck([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }


        /// <summary>
        /// 获取审批用户列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("member/pagelist")]
        public ActionResult Memberpagelist([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("member/info/list")]
        public ActionResult Memberinfolist([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }

        /// <summary>
        /// 获取用户详细信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("member/info/details")]
        public ActionResult Memberinfodetails([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }

        /// <summary>
        /// 修改账户状态
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("member/update/status")]
        public ActionResult Memberupdatestatus([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UserApi, param));
        }
    }
}
