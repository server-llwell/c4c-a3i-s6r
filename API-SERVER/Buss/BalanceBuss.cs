using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace API_SERVER.Buss
{
    public class BalanceBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.BalanceApi;
        }

        public bool NeedCheckToken()
        {
            return true;
        }
        #region 旧结算表
        /// <summary>
        /// 获取运营的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetBalanceListByOperator(object param, string userId)
        {
            SearchBalanceParam searchBalanceParam = JsonConvert.DeserializeObject<SearchBalanceParam>(param.ToString());
            if (searchBalanceParam.pageSize == 0)
            {
                searchBalanceParam.pageSize = 10;
            }
            if (searchBalanceParam.current == 0)
            {
                searchBalanceParam.current = 1;
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.getBalanceListByOperator(searchBalanceParam, userId);
        }
        /// <summary>
        /// 获取供应商的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetBalanceListBySupplier(object param, string userId)
        {
            SearchBalanceParam searchBalanceParam = JsonConvert.DeserializeObject<SearchBalanceParam>(param.ToString());
            if (searchBalanceParam.pageSize == 0)
            {
                searchBalanceParam.pageSize = 10;
            }
            if (searchBalanceParam.current == 0)
            {
                searchBalanceParam.current = 1;
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.getBalanceListBySupplier(searchBalanceParam, userId);
        }
        /// <summary>
        /// 获取采购商的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetBalanceListByPurchase(object param, string userId)
        {
            SearchBalanceParam searchBalanceParam = JsonConvert.DeserializeObject<SearchBalanceParam>(param.ToString());
            if (searchBalanceParam.pageSize == 0)
            {
                searchBalanceParam.pageSize = 10;
            }
            if (searchBalanceParam.current == 0)
            {
                searchBalanceParam.current = 1;
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.getBalanceListByPurchase(searchBalanceParam, userId);
        }
        /// <summary>
        /// 获取代理的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetBalanceListByAgent(object param, string userId)
        {
            SearchBalanceParam searchBalanceParam = JsonConvert.DeserializeObject<SearchBalanceParam>(param.ToString());
            if (searchBalanceParam.pageSize == 0)
            {
                searchBalanceParam.pageSize = 10;
            }
            if (searchBalanceParam.current == 0)
            {
                searchBalanceParam.current = 1;
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.getBalanceListByAgent(searchBalanceParam, userId);
        }
        /// <summary>
        /// 获取分销的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetBalanceListByDistribution(object param, string userId)
        {
            SearchBalanceParam searchBalanceParam = JsonConvert.DeserializeObject<SearchBalanceParam>(param.ToString());
            if (searchBalanceParam.pageSize == 0)
            {
                searchBalanceParam.pageSize = 10;
            }
            if (searchBalanceParam.current == 0)
            {
                searchBalanceParam.current = 1;
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.getBalanceListByDistribution(searchBalanceParam, userId);
        }

        #endregion

        #region 新结算表
        /// <summary>
        /// 获取合作方
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetPartner(object param, string userId)
        {
            BalanceDao balanceDao = new BalanceDao();
            userId = "bbcagent@llwell.net";
            return balanceDao.getPartner(userId);
        }
        /// <summary>
        /// 获取结算收益-预估收益
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetEstimate(object param, string userId)
        {
            SearchBalanceParam searchBalanceParam = JsonConvert.DeserializeObject<SearchBalanceParam>(param.ToString());
            if (searchBalanceParam.pageSize == 0)
            {
                searchBalanceParam.pageSize = 10;
            }
            if (searchBalanceParam.current == 0)
            {
                searchBalanceParam.current = 1;
            }
            userId = "gongying";
            BalanceDao balanceDao = new BalanceDao();
            if (searchBalanceParam.userType == "9")//供应代理
            {
                return balanceDao.getEstimateBySupplierAgent(searchBalanceParam, userId);
            }
            else if (searchBalanceParam.userType == "10")//采购代理
            {
                return balanceDao.getBalanceListByOperator(searchBalanceParam, userId);
            }
            else
            {
                return balanceDao.getBalanceListByOperator(searchBalanceParam, userId);
            }

        }
        /// <summary>
        /// 获取结算收益-已结算收益
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetSettle(object param, string userId)
        {
            SearchBalanceParam searchBalanceParam = JsonConvert.DeserializeObject<SearchBalanceParam>(param.ToString());
            if (searchBalanceParam.pageSize == 0)
            {
                searchBalanceParam.pageSize = 10;
            }
            if (searchBalanceParam.current == 0)
            {
                searchBalanceParam.current = 1;
            }
            userId = "gongying";
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.getSettle(searchBalanceParam, userId);
        }
        #endregion
    }
    public class SearchBalanceParam
    {
        public string[] OrderDate;//订单日期区间
        public string[] BalanceDate;//结算日期区间-发货时间
        public string merchantOrderId;//订单id
        public string purchaseCode;//销售商code
        public string accountCode;//结账id
        public string platformId;//平台渠道
        public string supplierId;//供应商id
        public string userType;//用户类型
        public string payType;//支付类型：1线上，2线下
        public string accountStatus;//结算状态:0在途，1已打款
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class BalanceTotalItem
    {
        public double total = 0;//总单数
        public double totalSales = 0;//总销售额
        public double totalSupplier = 0;//供货总结算额-供货商的结算金额
        public double totalPurchase = 0;//佣金总结算额-采购代理分销的结算金额
        public double totalPlatform = 0;// 平台总提点额
        public double totalSupplierAgent = 0;// 供货代理收益
        public double totalPurchaseAgent = 0;// 采购代理收益
    }
    public class BalanceItem
    {
        public string keyId;//序号
        public string id;//id
        public string merchantOrderId;//订单号
        public string tradeTime;//订单时间
        public double tradeAmount;//订单销售额 
        public string waybillTime;//结算时间（发货时间）
        public double supplier;//供货结算额 -供货的结算额
        public double purchase;//佣金结算额-采购代理分销的结算额
        public double platform;//平台提点额
        public double supplierAgent;// 供货代理收益
        public double purchaseAgent;// 采购代理收益
        public string distribution;//订单来源：bbc：分销商账号，普通订单：采购商账号
        public string payType;//支付类型：1线上，2线下
    }
    public class AccountItem
    {
        public string keyId;//序号
        public string id;//id
        public string accountCode;//结算编号
        //public string dateFrom;//结算时间从
        //public string dateTo;//结算时间到
        public string createTime;//结算操作时间
        public string usercode;//结算账户
        public string price;//结算总金额
        public string status;//结算状态:0在途，1已打款
        //public string inputOperator;//操作账户

    }
}






