using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace API_SERVER.Buss
{
    public class SalesBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.SalesApi;
        }

        public object Do_GetPurchase(object param, string userId)
        {
            SalesDao salesDao = new SalesDao();
            return salesDao.getPurchase();
        }

        public object Do_GetDistribution(object param, string userId)
        {
            SalesDao salesDao = new SalesDao();
            return salesDao.getDistribution();
        }
        /// <summary>
        /// 获取订单列表-运营
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_GetSalesListByOperator(object param, string userId)
        {
            SalesSeachParam salesSeachParam = JsonConvert.DeserializeObject<SalesSeachParam>(param.ToString());
            if (salesSeachParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (salesSeachParam.pageSize == 0)
            {
                salesSeachParam.pageSize = 10;
            }
            if (salesSeachParam.current == 0)
            {
                salesSeachParam.current = 1;
            }
            salesSeachParam.userCode = userId;
            SalesDao salesDao = new SalesDao();
            return salesDao.getSalesListByOperator(salesSeachParam);
        }
        /// <summary>
        /// 获取订单列表-供应商
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_GetSalesListBySupplier(object param, string userId)
        {
            SalesSeachParam salesSeachParam = JsonConvert.DeserializeObject<SalesSeachParam>(param.ToString());
            if (salesSeachParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (salesSeachParam.pageSize == 0)
            {
                salesSeachParam.pageSize = 10;
            }
            if (salesSeachParam.current == 0)
            {
                salesSeachParam.current = 1;
            }
            userId = "gongying";
            salesSeachParam.userCode = userId;
            SalesDao salesDao = new SalesDao();
            return salesDao.getSalesListBySupplier(salesSeachParam);
        }
        /// <summary>
        /// 获取订单列表-代理
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_GetSalesListByAgent(object param, string userId)
        {
            SalesSeachParam salesSeachParam = JsonConvert.DeserializeObject<SalesSeachParam>(param.ToString());
            if (salesSeachParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (salesSeachParam.pageSize == 0)
            {
                salesSeachParam.pageSize = 10;
            }
            if (salesSeachParam.current == 0)
            {
                salesSeachParam.current = 1;
            }
            salesSeachParam.userCode = userId;
            SalesDao salesDao = new SalesDao();
            return salesDao.getSalesListByAgent(salesSeachParam);
        }
    }
    
    public class SalesSeachParam
    {
        public string[] date;//日期区间
        public string userCode;//用户账号
        public string barcode;//条码
        public string goodsName;//商品名
        public string brand;//品牌
        public string purchaseCode;//渠道商code
        public string platformId;//平台渠道
        public string distributionCode;//分销商code
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class PurchaseItem
    {
        public string purchaseCode;//渠道商code
        public string purchaseName;//渠道商名
    }
    public class DistributionItem
    {
        public string distributionCode;//分销商code
        public string distributionName;//分销商名
    }

    public class SalesListItem
    {
        public int salesNumTotal=0;//总销量
        public double salesPriceTotal = 0;//总销售额
        public double costTotal = 0;//总成本
        public double grossProfitTotal = 0;//总毛利
        public double brokerageTotal = 0;//总佣金
    }

    public class SalesItem
    {
        public int id;
        public string barcode;//商品条码
        public string goodsName;//商品名称
        public string slt;//商品图片
        public string[] category;//商品分类信息
        public string brand;//品牌
        public int salesNum;//销量
        public double salesPrice;//销售额
        public double cost;//成本
        public double grossProfit;//毛利
        public string platformType;//平台渠道
        public string purchaserName;//销售商
        public double brokerage;//佣金
        public string distribution;//bbc分销商
    }
}
