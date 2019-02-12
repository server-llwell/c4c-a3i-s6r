using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace API_SERVER.Buss
{
    public class ApiBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.ApiApi;
        }

        public bool NeedCheckToken()
        {
            return false;
        }
        /// <summary>
        /// 订单接收接口
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_ImportOrder(object param, string userId)
        {
            ImportOrderResult importOrderResult = new ImportOrderResult();
            ImportOrderParam importOrderParam = JsonConvert.DeserializeObject<ImportOrderParam>(param.ToString());
            if (importOrderParam == null)
            {
                importOrderResult.message += "参数为空";
                return importOrderParam;
            }
            if (importOrderParam.userCode == null || importOrderParam.userCode == "")
            {
                importOrderResult.message += "userCode为空";
                return importOrderParam;
            }
            if (importOrderParam.OrderList == null || importOrderParam.OrderList.Count ==0)
            {
                importOrderResult.message += "订单为空";
                return importOrderParam;
            }

            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(importOrderParam.userCode);
            if (userType == null || (userType != "2"))
            {
                importOrderResult.message += "用户权限错误";
                return importOrderParam;
            }

            ApiDao apiDao = new ApiDao();
            return apiDao.importOrder(importOrderParam, param.ToString());
        }
        /// <summary>
        /// 商品信息获取接口
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetGoodsList(object param, string userId)
        {
            GoodsListResult goodsListResult = new GoodsListResult();
            ImportOrderParam importOrderParam = JsonConvert.DeserializeObject<ImportOrderParam>(param.ToString());
            if (importOrderParam == null)
            {
                goodsListResult.message += "参数为空";
                return importOrderParam;
            }
            if (importOrderParam.userCode == null || importOrderParam.userCode == "")
            {
                goodsListResult.message += "userCode为空";
                return importOrderParam;
            }

            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(importOrderParam.userCode);
            if (userType == null || (userType != "2"))
            {
                goodsListResult.message += "用户权限错误";
                return goodsListResult;
            }

            ApiDao apiDao = new ApiDao();
            return apiDao.getGoodsList(importOrderParam);
        }

        public object Do_BindingWXAPP(object param, string userId)
        {
            MsgResult msgResult = new MsgResult();
            WXAPPParam wXAPPParam = JsonConvert.DeserializeObject<WXAPPParam>(param.ToString());
            if (wXAPPParam == null)
            {
                msgResult.msg += "参数为空";
                return msgResult;
            }
            if (wXAPPParam.appId == null || wXAPPParam.appId == "")
            {
                msgResult.msg += "appId 为空";
                return msgResult;
            }
            if (wXAPPParam.openId == null || wXAPPParam.openId == "")
            {
                msgResult.msg += "openId 为空";
                return msgResult;
            }
            if (wXAPPParam.pagentCode == null || wXAPPParam.pagentCode == "")
            {
                msgResult.msg += "pagentCode 为空";
                return msgResult;
            }
            ApiDao apiDao = new ApiDao();
            return apiDao.bindingWXAPP(wXAPPParam);
        }

        public object Do_BindingWXB2B(object param, string userId)
        {
            MsgResult msgResult = new MsgResult();
            WXAPPParam wXAPPParam = JsonConvert.DeserializeObject<WXAPPParam>(param.ToString());
            if (wXAPPParam == null)
            {
                msgResult.msg += "参数为空";
                return msgResult;
            }
            if (wXAPPParam.appId == null || wXAPPParam.appId == "")
            {
                msgResult.msg += "appId 为空";
                return msgResult;
            }
            if (wXAPPParam.openId == null || wXAPPParam.openId == "")
            {
                msgResult.msg += "openId 为空";
                return msgResult;
            }
            ApiDao apiDao = new ApiDao();
            return apiDao.bindingWXB2B(wXAPPParam);
        }

        public object Do_BindingInvite(object param, string userId)
        {
            MsgResult msgResult = new MsgResult();
            WXAPPParam wXAPPParam = JsonConvert.DeserializeObject<WXAPPParam>(param.ToString());
            if (wXAPPParam == null)
            {
                msgResult.msg += "参数为空";
                return msgResult;
            }
            if (wXAPPParam.appId == null || wXAPPParam.appId == "")
            {
                msgResult.msg += "appId 为空";
                return msgResult;
            }
            if (wXAPPParam.openId == null || wXAPPParam.openId == "")
            {
                msgResult.msg += "openId 为空";
                return msgResult;
            }
            ApiDao apiDao = new ApiDao();
            return apiDao.bindingInvite(wXAPPParam);
        }

        public object Do_GetTypeByOpenId(object param, string userId)
        {
            MsgResult msgResult = new MsgResult();
            WXAPPParam wXAPPParam = JsonConvert.DeserializeObject<WXAPPParam>(param.ToString());
            if (wXAPPParam == null)
            {
                msgResult.msg += "参数为空";
                return msgResult;
            }
            if (wXAPPParam.openId == null || wXAPPParam.openId == "")
            {
                msgResult.msg += "openId 为空";
                return msgResult;
            }
            ApiDao apiDao = new ApiDao();
            return apiDao.getTypeByOpenId(wXAPPParam);
        }

        public object Do_GetProfitByOpenId(object param, string userId)
        {
            MsgResult msgResult = new MsgResult();
            WXAPPParam wXAPPParam = JsonConvert.DeserializeObject<WXAPPParam>(param.ToString());
            if (wXAPPParam == null)
            {
                msgResult.msg += "参数为空";
                return msgResult;
            }
            if (wXAPPParam.openId == null || wXAPPParam.openId == "")
            {
                msgResult.msg += "openId 为空";
                return msgResult;
            }
            ApiDao apiDao = new ApiDao();
            return apiDao.getProfitByOpenId(wXAPPParam);
        }
        public object Do_GetQrcode(object param, string userId)
        {
            MsgResult msgResult = new MsgResult();
            WXAPPParam wXAPPParam = JsonConvert.DeserializeObject<WXAPPParam>(param.ToString());
            if (wXAPPParam == null)
            {
                msgResult.msg += "参数为空";
                return msgResult;
            }
            if (wXAPPParam.appId == null || wXAPPParam.appId == "")
            {
                msgResult.msg += "appId 为空";
                return msgResult;
            }
            if (wXAPPParam.openId == null || wXAPPParam.openId == "")
            {
                msgResult.msg += "openId 为空";
                return msgResult;
            }
            ApiDao apiDao = new ApiDao();
            return apiDao.getQrcode(wXAPPParam);
        }
        public object Do_GetAgetnQrcode(object param, string userId)
        {
            MsgResult msgResult = new MsgResult();
            WXAPPParam wXAPPParam = JsonConvert.DeserializeObject<WXAPPParam>(param.ToString());
            if (wXAPPParam == null)
            {
                msgResult.msg += "参数为空";
                return msgResult;
            }
            if (wXAPPParam.appId == null || wXAPPParam.appId == "")
            {
                msgResult.msg += "appId 为空";
                return msgResult;
            }
            if (wXAPPParam.openId == null || wXAPPParam.openId == "")
            {
                msgResult.msg += "openId 为空";
                return msgResult;
            }
            ApiDao apiDao = new ApiDao();
            return apiDao.getAgetnQrcode(wXAPPParam);
        }
        public object Do_AddBankCode(object param, string userId)
        {
            MsgResult msgResult = new MsgResult();
            BankParam bankParam = JsonConvert.DeserializeObject<BankParam>(param.ToString());
            if (bankParam == null)
            {
                msgResult.msg += "参数为空";
                return msgResult;
            }
            if (bankParam.openId == null || bankParam.openId == "")
            {
                msgResult.msg += "openId 为空";
                return msgResult;
            }
            if (bankParam.bank == null || bankParam.bank == "")
            {
                msgResult.msg += "bank 为空";
                return msgResult;
            }
            if (bankParam.bankName == null || bankParam.bankName == "")
            {
                msgResult.msg += "bankName 为空";
                return msgResult;
            }
            if (bankParam.bankTel == null || bankParam.bankTel == "")
            {
                msgResult.msg += "bankTel 为空";
                return msgResult;
            }
            if (bankParam.bankCardCode == null || bankParam.bankCardCode == "")
            {
                msgResult.msg += "bankCardCode 为空";
                return msgResult;
            }
            if (bankParam.bankOperator == null || bankParam.bankOperator == "")
            {
                msgResult.msg += "bankOperator 为空";
                return msgResult;
            }
            ApiDao apiDao = new ApiDao();
            return apiDao.addBankCode(bankParam);
        }
        public object Do_GetBankCode(object param, string userId)
        {
            MsgResult msgResult = new MsgResult();
            BankParam bankParam = JsonConvert.DeserializeObject<BankParam>(param.ToString());
            if (bankParam == null)
            {
                msgResult.msg += "参数为空";
                return msgResult;
            }
            if (bankParam.openId == null || bankParam.openId == "")
            {
                msgResult.msg += "openId 为空";
                return msgResult;
            }
            ApiDao apiDao = new ApiDao();
            return apiDao.getBankCode(bankParam.openId);
        }
    }
    public class WXAPPParam
    {
        public string appId;
        public string openId;
        public string pagentCode;
    }
    public class ImportOrderParam
    {
        public string userCode;//
        public List<OrderItem> OrderList;
    }

    public class BankParam
    {
        public string openId;
        public string bank;//银行
        public string bankName;//开户支行
        public string bankTel;//电话
        public string bankCardCode;//卡号
        public string bankOperator;//开户人
    }
    public class BankItem
    {
        public string type="0";
        public BankParam bankParam;
    }
    public class ImportOrderResult
    {
        public string code="0";
        public string message="";
        public List<ImportOrderResultItem> content;
    }
    public class ImportOrderResultItem
    {
        public string merchantOrderId;
        public string code;
        public string message;
    }

    public class GoodsListResult
    {
        public string code = "0";
        public string message = "";
        public List<GoodsListResultItem> goodsList;
    }
    public class GoodsListResultItem
    {
        public string barcode;//商品条码
        public string brand;//商品品牌
        public string goodsName;//商品名称
        public string catelog1;//商品分类1
        public string catelog2;//商品分类2
        public string slt;//商品缩略图
        public string[] thumb;//商品主图
        public string[] content;//商品详情图
        public string country;//原产国
        public string model;//规格型号
        public string sendType;//提货方式
        public double price;//价格
        public string stock;//库存
        public string businessType;//商品业务类型（普通、海外、完税、保税）
        public string customClearType;//报关模式（BC，个物）
        public string taxType;//税费设置（默认、自定义）
        public double taxRate;//税率
        public double weight;//重量
    }

    public class ProfitData
    {
        public bool success = true;
        public string errorMessage="";
        public ProfitItem data;
    }
    public class ProfitItem
    {
        public double accountMoney = 0;//账户余额
        public double monthProfit = 0;//本月预估
        public double lastMonthProfit = 0;//上月结算
        public List<MonthGoodsProfit> monthGoodsProfitList = new List<MonthGoodsProfit>();//收益明细
        public List<AccountInfo> accountInfoList = new List<AccountInfo>();//结算记录
    }
    public class MonthGoodsProfit
    {
        public string month;//月份
        public string monthTotal;//月合计
        public List<GoodsProfit> goodsProfitList=new List<GoodsProfit>();
    }
    public class GoodsProfit
    {
        public string goodsName;//商品名
        public string profit;//收益
        public string tradeTime;//订单时间
        public string accountTime;//结算时间
        public string tradeAmount;//订单金额
    }
    public class AccountInfo
    {
        public string merchantOrderId;//订单编号
        public string accountTime;//结算时间
        public string profit;//收益
    }
}






