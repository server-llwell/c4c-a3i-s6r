using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;

namespace API_SERVER.Buss
{
    public class UserBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.UserApi;
        }

        public bool NeedCheckToken()
        {
            return true;
        }
        /// <summary>
        /// 获取运营客户接口
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetOperateCustomer(object param, string userId)
        {
            OperateCustomerParam ocParam = JsonConvert.DeserializeObject<OperateCustomerParam>(param.ToString());
            if (ocParam.pageSize == 0)
            {
                ocParam.pageSize = 10;
            }
            if (ocParam.current == 0)
            {
                ocParam.current = 1;
            }
            UserDao UserDao = new UserDao();
            return UserDao.GetOperateCustomer(ocParam, userId);
        }

        #region 原java版b2b

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_validate(object param, string userId)
        {
            LoginEntityParam loginEntity = JsonConvert.DeserializeObject<LoginEntityParam>(param.ToString());
            if (loginEntity == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (loginEntity.userName == null || loginEntity.userName == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (loginEntity.password == null || loginEntity.password == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            UserDao UserDao = new UserDao();

            LoginItem loginItem = UserDao.Validate(loginEntity.userName, MD5Manager.MD5Encrypt32(loginEntity.password));
            if (loginItem != null)
            {
                string token = MD5Manager.createToken(loginEntity.userName);
                using (var client = ConnectionMultiplexer.Connect(Global.REDIS))
                {
                    var db = client.GetDatabase(0);
                    db.StringSet(loginEntity.userName, "\'"+token+"\'");
                    loginItem.token.token = token;
                    return loginItem;
                }
            }
            else
            {
                throw new ApiException(CodeMessage.AccountOrPasswordIsIncorrect, "AccountOrPasswordIsIncorrect");
            }
        }
        /// <summary>
        /// 用户信息及头像
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_currentUser(object param, string userId)
        {
            UserDao UserDao = new UserDao();

            CurrentUserItem currentUserItem = UserDao.CurrentUser(userId);
            return currentUserItem;
        }
        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_menu(object param, string userId)
        {
            UserDao UserDao = new UserDao();

            List<Menu> menuList = UserDao.menu();
            return menuList;
        }
        /// <summary>
        /// 用户消息列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_messagelist(object param, string userId)
        {
            UserDao UserDao = new UserDao();

            List<MessageEntity> messageList = UserDao.messagelist(userId);
            return messageList;
        }

        /// <summary>
        /// 未读消息数
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_messagecount(object param, string userId)
        {
            UserDao UserDao = new UserDao();
            return UserDao.messagecount(userId);
        }
        /// <summary>
        /// 清空消息
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_messageempty(object param, string userId)
        {
            MessageRequestParam messageRequestParam = JsonConvert.DeserializeObject<MessageRequestParam>(param.ToString());
            if (messageRequestParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            UserDao UserDao = new UserDao();
            return UserDao.messageempty(userId, messageRequestParam.type);
        }
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_registersubmit(object param, string userId)
        {
            RegisterParam registerParam = JsonConvert.DeserializeObject<RegisterParam>(param.ToString());
            if (registerParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            string mail = registerParam.mail;
            string code = registerParam.captcha;
            string pwd = registerParam.password;
            string type = registerParam.type;
            string ofAgent = registerParam.ofAgent;
            string verifycode = "2";
            string flag = "1";
            string avatar = "";
            MsgResult msg = new MsgResult();
            if (mail == null || code == null || pwd == null || type == null || mail == "" || code == ""
                    || pwd == "" || type == "" || (type != "1" && type != "2" && type != "3" && type != "4"))
            {
                msg.msg = "非法请求，参数有误.";
                return msg;
            }
            if (!MD5Manager.checkEmail(mail) && !MD5Manager.checkMobileNumber(mail))
            {
                msg.msg = "邮件或手机号格式不正确.";
                return msg;
            }
            if (pwd.Length < 6)
            {
                msg.msg = "密码格式有误.";
                return msg;
            }
            UserDao userDao = new UserDao();
            if (userDao.getUserId(mail) != null)
            {
                msg.msg = "账号已存在.";
                return msg;
            }
            string key = mail + "_code";
            using (var client = ConnectionMultiplexer.Connect(Global.REDIS))
            {
                try
                {
                    var db = client.GetDatabase(0);
                    var tokenRedis = db.StringGet(key);
                    string tokenRedisStr = tokenRedis.ToString().Substring(1, tokenRedis.ToString().Length - 2);
                    if (code == tokenRedisStr)
                    {
                        pwd = MD5Manager.MD5Encrypt32(pwd);

                        if (type == "1")
                        {
                            avatar = "http://ecc-product.oss-cn-beijing.aliyuncs.com/upload/head_s.png";
                        }
                        else
                        {
                            avatar = "http://ecc-product.oss-cn-beijing.aliyuncs.com/upload/head_p.png";
                        }
                        if (type == "4")
                        {
                            if (ofAgent == null || ofAgent == "")
                            {
                                string defaultAgent = userDao.getDefaultAgent();
                                if (defaultAgent != null)
                                {
                                    ofAgent = defaultAgent;
                                }
                            }
                            else
                            {
                                string userType = userDao.getUserType(ofAgent);
                                if (userType != null && userType == "3")
                                {
                                    ofAgent = userType;
                                }
                                else
                                {
                                    string defaultAgent = userDao.getDefaultAgent();
                                    if (defaultAgent != null)
                                    {
                                        ofAgent = defaultAgent;
                                    }
                                }
                            }
                        }

                        if (userDao.insertUser(mail, pwd, type, ofAgent, verifycode, flag, avatar))
                        {
                            msg.msg = "注册失败，请检查格式";
                            return msg;
                        }


                        int role_id = 5;

                        if (type == "2" || type == "3" || type == "4")
                        {
                            role_id = 6;
                        }

                        if (userDao.insertUserRole(mail, role_id))
                        {
                            msg.msg = "注册成功，请重新登录，完善资料.";
                            msg.type = "1";
                            return msg;
                        }

                        msg.msg = "注册失败";
                        return msg;
                    }
                    else
                    {
                        msg.msg = "验证码不正确.";
                        return msg;
                    }
                }
                catch (Exception ex)
                {
                    msg.msg = "无效的验证码.";
                    return msg;
                }
            }
        }
        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_registerrename(object param, string userId)
        {
            RegisterParam registerParam = JsonConvert.DeserializeObject<RegisterParam>(param.ToString());
            if (registerParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            string mail = registerParam.mail;
            string code = registerParam.captcha;
            string pwd = registerParam.password;

            MsgResult msg = new MsgResult();
            if (mail == null || code == null || pwd == null || mail == "" || code == "" || pwd == "")
            {
                msg.msg = "非法请求，参数有误.";
                return msg;
            }
            if (!MD5Manager.checkEmail(mail) && !MD5Manager.checkMobileNumber(mail))
            {
                msg.msg = "邮件或手机号格式不正确.";
                return msg;
            }
            if (pwd.Length < 6)
            {
                msg.msg = "密码格式有误.";
                return msg;
            }
            UserDao userDao = new UserDao();
            if (userDao.getUserId(mail) == null)
            {
                msg.msg = "账号不存在.";
                return msg;
            }
            string key = mail + "_code";
            using (var client = ConnectionMultiplexer.Connect(Global.REDIS))
            {
                try
                {
                    var db = client.GetDatabase(0);
                    var tokenRedis = db.StringGet(key);
                    string tokenRedisStr = tokenRedis.ToString().Substring(1, tokenRedis.ToString().Length - 2);
                    if (code == tokenRedisStr)
                    {
                        pwd = MD5Manager.MD5Encrypt32(pwd);

                        if (userDao.updateUserPwd(mail, pwd))
                        {
                            msg.msg = "修改密码成功.";
                            msg.type = "1";
                            return msg;
                        }

                        msg.msg = "修改密码失败";
                        return msg;
                    }
                    else
                    {
                        msg.msg = "验证码不正确.";
                        return msg;
                    }
                }
                catch (Exception ex)
                {
                    msg.msg = "无效的验证码.";
                    return msg;
                }
            }
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_registerrenameNew(object param, string userId)
        {
            RegisterParam registerParam = JsonConvert.DeserializeObject<RegisterParam>(param.ToString());
            if (registerParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            string mail = registerParam.mail;
            //string code = registerParam.captcha;
            string pwd = registerParam.password;
            string oldpwd = registerParam.oldpwd;

            MsgResult msg = new MsgResult();
            if (mail == null || pwd == null || oldpwd == null || mail == ""   || pwd == "" || oldpwd == "")
            {
                msg.msg = "非法请求，参数有误.";
                return msg;
            }
            //if (!MD5Manager.checkEmail(mail) && !MD5Manager.checkMobileNumber(mail))
            //{
            //    msg.msg = "邮件或手机号格式不正确.";
            //    return msg;
            //}
            if (pwd.Length < 6)
            {
                msg.msg = "密码格式有误.";
                return msg;
            }
            UserDao userDao = new UserDao();
            if (userDao.Validate(mail, MD5Manager.MD5Encrypt32(oldpwd)) == null)
            {
                msg.msg = "原密码错误！";
                return msg;
            }
            pwd = MD5Manager.MD5Encrypt32(pwd);

            if (userDao.updateUserPwd(mail, pwd))
            {
                msg.msg = "修改密码成功.";
                msg.type = "1";
                return msg;
            }
            else
            {
                msg.msg = "修改密码失败";
                return msg;
            }

            
        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_registercode(object param, string userId)
        {
            RegisterParam registerParam = JsonConvert.DeserializeObject<RegisterParam>(param.ToString());
            if (registerParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (registerParam.mail == null || registerParam.mail == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            string mail = registerParam.mail;
            MsgResult msg = new MsgResult();
            if (mail == null || mail == "" )
            {
                msg.msg = "非法的调用,账号为空.";
                return msg;
            }
            UserDao userDao = new UserDao();
            if (userDao.getUserId(mail) != null)
            {
                msg.msg = "账号已存在.";
                return msg;
            }
            else
            {
                if (MD5Manager.checkEmail(mail))
                {
                    using (var client = ConnectionMultiplexer.Connect(Global.REDIS))
                    {
                        string key = mail + "_code_temp";
                        try
                        {
                            var db = client.GetDatabase(0);
                            if (db.KeyExists(key))
                            {
                                msg.msg = "60";
                                msg.type = "-1";
                                return msg;
                            }
                            Random r = new Random();
                            string code = r.Next(100000,999999).ToString();
                            db.StringSet(key, code, new TimeSpan(0, 1, 0));
                            db.StringSet(mail + "_code", code, new TimeSpan(1, 0, 0));
                            SMSEMAILHandle s = new SMSEMAILHandle();
                            s.MailSend(mail, mail, "流连优选", "【验证码】", mail + ",您好！欢迎加入流连优选，您正在验证账号，验证码：" + code + "，有效期为1小时。");
                        }
                        catch (Exception ex)
                        {
                            msg.msg = "无效的验证码.";
                            return msg;
                        }
                    }
                }
                else if (MD5Manager.checkMobileNumber(mail))
                {
                    using (var client = ConnectionMultiplexer.Connect(Global.REDIS))
                    {
                        string key = mail + "_code_temp";
                        try
                        {
                            var db = client.GetDatabase(0);
                            if (db.KeyExists(key))
                            {
                                msg.msg = "60";
                                msg.type = "-1";
                                return msg;
                            }
                            Random r = new Random();
                            string code = r.Next(100000, 999999).ToString();
                            db.StringSet(key, code, new TimeSpan(0, 1, 0));
                            db.StringSet(mail + "_code", code, new TimeSpan(1, 0, 0));
                            SMSEMAILHandle s = new SMSEMAILHandle();
                            s.smsSend(mail, code);
                        }
                        catch (Exception ex)
                        {
                            msg.msg = "无效的验证码.";
                            return msg;
                        }
                    }
                }
                else
                {
                    msg.msg = "账号格式不正确.";
                    return msg;
                }
            }
            msg.msg = "验证码已发送";
            msg.type = "1";
            return msg;
        }
        /// <summary>
        /// 获取修改密码验证码
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_registerrenamecode(object param, string userId)
        {
            RegisterParam registerParam = JsonConvert.DeserializeObject<RegisterParam>(param.ToString());
            if (registerParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (registerParam.mail == null || registerParam.mail == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            string mail = registerParam.mail;
            MsgResult msg = new MsgResult();
            if (mail == null || mail == "")
            {
                msg.msg = "非法的调用,账号为空.";
                return msg;
            }
            UserDao userDao = new UserDao();
            if (userDao.getUserId(mail) == null)
            {
                msg.msg = "账号不存在.";
                return msg;
            }
            else
            {
                if (MD5Manager.checkEmail(mail))
                {
                    using (var client = ConnectionMultiplexer.Connect(Global.REDIS))
                    {
                        string key = mail + "_code_temp";
                        try
                        {
                            var db = client.GetDatabase(0);
                            if (db.KeyExists(key))
                            {
                                msg.msg = "60";
                                msg.type = "-1";
                                return msg;
                            }
                            Random r = new Random();
                            string code = r.Next(100000, 999999).ToString();
                            db.StringSet(key, code, new TimeSpan(0, 1, 0));
                            db.StringSet(mail + "_code", code, new TimeSpan(1, 0, 0));
                            SMSEMAILHandle s = new SMSEMAILHandle();
                            s.MailSend(mail, mail, "流连优选", "【验证码】", mail + ",您好！欢迎加入流连优选，您正在验证账号，验证码：" + code + "，有效期为1小时。");
                        }
                        catch (Exception ex)
                        {
                            msg.msg = "无效的验证码.";
                            return msg;
                        }
                    }
                }
                else if (MD5Manager.checkMobileNumber(mail))
                {
                    using (var client = ConnectionMultiplexer.Connect(Global.REDIS))
                    {
                        string key = mail + "_code_temp";
                        try
                        {
                            var db = client.GetDatabase(0);
                            if (db.KeyExists(key))
                            {
                                msg.msg = "60";
                                msg.type = "-1";
                                return msg;
                            }
                            Random r = new Random();
                            string code = r.Next(100000, 999999).ToString();
                            db.StringSet(key, code, new TimeSpan(0, 1, 0));
                            db.StringSet(mail + "_code", code, new TimeSpan(1, 0, 0));
                            SMSEMAILHandle s = new SMSEMAILHandle();
                            string content = s.smsSendBackContent(mail, code);
                            if (content== "发送失败")
                            {
                                msg.msg = "发送失败";
                                return msg;
                            }
                            else
                            {
                                msg.msg = content;
                                msg.type = "1";
                                return msg;
                            }
                        }
                        catch (Exception ex)
                        {
                            msg.msg = "无效的验证码.";
                            return msg;
                        }
                    }
                }
                else
                {
                    msg.msg = "账号格式不正确.";
                    return msg;
                }
            }
            //msg.msg = "验证码已发送";
            //msg.type = "1";
            return msg;
        }

        /// <summary>
        /// 获取注册用户状态
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_registerstatus(object param, string userId)
        {
            UserDao UserDao = new UserDao();
            return UserDao.registerstatus(userId);
        }
        /// <summary>
        /// 审核账号
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_registercheck(object param, string userId)
        {
            RegisterCheckParam registerCheckParam = JsonConvert.DeserializeObject<RegisterCheckParam>(param.ToString());
            if (registerCheckParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            int role_id = 2;
            string verifycode = "4";

            UserDao userDao = new UserDao();
            if (registerCheckParam.check == "1")
            {
                if (registerCheckParam.usertype =="2" )
                {
                    role_id = 3;
                }
                if (registerCheckParam.usertype =="3")
                {
                    role_id = 8;
                }
                if (registerCheckParam.usertype =="4")
                {
                    role_id = 9;
                }

                userDao.updatetUserRoleRegister(registerCheckParam.userid, role_id.ToString());
                registerCheckParam.failmark = "";
                SMSEMAILHandle s = new SMSEMAILHandle();
                if (MD5Manager.checkEmail(registerCheckParam.usercode))
                {
                    s.MailSend(registerCheckParam.usercode, registerCheckParam.usercode, "流连优选", "【审核通过】",
                        registerCheckParam.usercode + ",恭喜您，您的账号已通过审核！更多操作，请登录 http://console.llwell.net/#/user/login 流连优选后台查看");
                }
                else if (MD5Manager.checkMobileNumber(registerCheckParam.usercode))
                {
                    s.sendRegisterSuccess(registerCheckParam.usercode);
                }
            }
            else
            {
                SMSEMAILHandle s = new SMSEMAILHandle();
                verifycode = "-1";
                if (MD5Manager.checkEmail(registerCheckParam.usercode))
                {
                    s.MailSend(registerCheckParam.usercode, registerCheckParam.usercode, "流连优选", "【审核未通过】",
                        registerCheckParam.usercode + ",很遗憾，您的账号未通过审核！原因为："+ registerCheckParam.failmark + "，您可以登录 http://console.llwell.net/#/user/login 流连优选后台重新提交审核资料，感谢您对流连优选的信任和支持。");
                }
                else if (MD5Manager.checkMobileNumber(registerCheckParam.usercode))
                {
                    s.sendRegisterSuccess(registerCheckParam.usercode);
                }

            }
            userDao.updatetUserStatusById(verifycode, registerCheckParam.userid, registerCheckParam.failmark);
            MsgResult msg = new MsgResult();
            msg.msg = "审核成功";
            msg.type = "1";
            return msg;
        }

        /// <summary>
        /// 获取审批用户列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_memberpagelist(object param, string userId)
        {
            UserParam userParam = JsonConvert.DeserializeObject<UserParam>(param.ToString());
            if (userParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (userParam.pageSize == 0)
            {
                userParam.pageSize = 10;
            }
            if (userParam.current == 0)
            {
                userParam.current = 1;
            }
            UserDao UserDao = new UserDao();

            return UserDao.getPageUserForCheck(userParam);
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_memberinfolist(object param, string userId)
        {
            UserParam userParam = JsonConvert.DeserializeObject<UserParam>(param.ToString());
            if (userParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (userParam.pageSize == 0)
            {
                userParam.pageSize = 10;
            }
            if (userParam.current == 0)
            {
                userParam.current = 1;
            }
            UserDao UserDao = new UserDao();

            return UserDao.getUserList(userParam);
        }

        /// <summary>
        /// 获取用户详细信息
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_memberinfodetails(object param, string userId)
        {
            MemberParam memberParam = JsonConvert.DeserializeObject<MemberParam>(param.ToString());
            if (memberParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (memberParam.userid == null || memberParam.userid == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            UserDao UserDao = new UserDao();

            return UserDao.getUserDetails(memberParam.userid);
        }
        /// <summary>
        /// 修改账户状态
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_memberupdatestatus(object param, string userId)
        {
            MemberParam memberParam = JsonConvert.DeserializeObject<MemberParam>(param.ToString());
            if (memberParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (memberParam.userid == null || memberParam.userid == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            UserDao UserDao = new UserDao();

            return UserDao.updateUserStatusByUserId(memberParam.userid,memberParam.flag);
        }
        #endregion



    }

    public class OperateCustomerParam
    {
        public string search;//查询信息
        public int usertype;//用户权限
        public int pageSize; //页面显示多少个商品
        public int current;//多少页

    }
    public class OperateCustomerItem
    {
        public string id;//id
        public string keyid;//序号
        public string usercode;//用户账号
        public string username;//用户昵称
        public string company;//公司
        public string contact;//联系人
        public string tel;//电话
        public string email;//email
    }
    #region 原java版b2b --Param

    public class LoginEntityParam
    {
        public string userName;
        public string password;
        public string type;
    }
    public class MessageRequestParam
    {
        public string type;
    }
    public class RegisterParam
    {
        public string mail;
        public string captcha;
        public string password;
        public string oldpwd;
        public string type;
        public string ofAgent;
    }
    public class RegisterCheckParam
    {
        public string userid;
        public string usercode;
        public string check;
        public string failmark;
        public string usertype;
    }
    public class MemberParam
    {
        public string userid;
        public string flag;
    }
    public class UserParam
    {
        public int id;
        public string usercode;
        public string usertype;
        public string username;
        public string avatar;
        public string company;
        public string contact;
        public string website;
        public string tel;
        public string email;
        public string three;
        public string img1;
        public string img2;
        public string img3;
        public string createtime;
        public string verifycode;
        public string flag;
        public string lasttime;
        public int current;
        public int pageSize;
    }
    #endregion
    #region 原java版b2b --Item
    public class LoginItem
    {
        public bool status = true;
        public string currentAuthority = "guest";
        public Token token = new Token();
    }
    public class Token
    {
        public string userId = "";
        public string token = "";
    }
    public class CurrentUserItem
    {
        public string name;
        public string avatar;
        public string userid;
        public string notifyCount;
    }
    public class Menu
    {
        public string name;
        public string icon;
        public string path;
        public List<string> authority=new List<string>();
        public List<MenuChildren> children=new List<MenuChildren>();
    }
    public class MenuChildren
    {
        public string name;
        public string path;
        public List<string> authority=new List<string>();
    }
    public class MessageEntity
    {
        public string id;
        public string avatar;
        public string title;
        public string datetime;
        public string type;
    }
    public class MessageCountEntity
    {
        public int notifyCount;
    }
    public class RegisterStepOne
    {
        public int id;
        public string mail;
        public string avatar;
        public string password;
        public string type;
        public string ofAgent;
        public string verifycode = "2";
        public string flag = "1";
    }
    public class UserStatus
    {
        public string usertype;
        public string company;
        public string contact;
        public string email;
        public string website;
        public string tel;
        public string img1;
        public string img2;
        public string img3;
        public string three;
        public string verifycode;
    }
    #endregion
}
