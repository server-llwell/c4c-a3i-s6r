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
        public bool NeedCheckToken()
        {
            return true;
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
        /// <summary>
        /// 获取我的客户
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_GetClient(object param, string userId)
        {
            ClientSeachParam clientSeachParam = JsonConvert.DeserializeObject<ClientSeachParam>(param.ToString());
            if (clientSeachParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (clientSeachParam.pageSize == 0)
            {
                clientSeachParam.pageSize = 10;
            }
            if (clientSeachParam.current == 0)
            {
                clientSeachParam.current = 1;
            }
            clientSeachParam.userId = userId;
            SalesDao salesDao = new SalesDao();
            return salesDao.getClient(clientSeachParam);
        }
        /// <summary>
        /// 获取销售商品-代销
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_GetGoods(object param, string userId)
        {
            SalesGoods salesGoods = JsonConvert.DeserializeObject<SalesGoods>(param.ToString());
            if (salesGoods == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (salesGoods.current==0 )
            {
                salesGoods.current = 1;
            }
            if(salesGoods.pageSize ==0)
            {
                salesGoods.pageSize = 10;
            }
            
            SalesDao salesDao = new SalesDao();
            return salesDao.getGoods(salesGoods, userId);
        }


        /// <summary>
        /// 获取门店销售订单-运营
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_ShopSalseOrders(object param, string userId)
        {
            ShopSalseOrdersParam salesGoods = JsonConvert.DeserializeObject<ShopSalseOrdersParam>(param.ToString());
            if (salesGoods == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (salesGoods.current == 0)
            {
                salesGoods.current = 1;
            }
            if (salesGoods.pageSize == 0)
            {
                salesGoods.pageSize = 10;
            }

            SalesDao salesDao = new SalesDao();
            return salesDao.ShopSalseOrders(salesGoods, userId);
        }


        /// <summary>
        /// 获取门店销售订单详情-运营
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_ShopSalseOrdersDetails(object param, string userId)
        {
            ShopSalseOrdersDetailsParam salesGoods = JsonConvert.DeserializeObject<ShopSalseOrdersDetailsParam>(param.ToString());
            if (salesGoods == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (salesGoods.orderId == null || salesGoods.orderId=="")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (salesGoods.current == 0)
            {
                salesGoods.current = 1;
            }
            if (salesGoods.pageSize == 0)
            {
                salesGoods.pageSize = 10;
            }

            SalesDao salesDao = new SalesDao();
            return salesDao.ShopSalseOrdersDetails(salesGoods, userId);
        }


    }

    public class ShopSalseOrdersDetailsParam
    {
        public string orderId;//订单号
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class ShopSalseOrdersDetailsItem
    {
        public string keyId;//序号
        public string tradeTime;//销售日期
        public string supplierName;//供货商名
        public string model;//规格
        public string barcode;//商品条码
        public string goodsName;//商品名
        public string country;//原产地
        public string num;//销售数量
        public string brand;//生产商
        public string rprice;//零售
        public string pprice;//平台供货
        public string inprice;//平台采购价
        public string platformPrice;//服务费
    }

    public class ShopSalseOrdersList
    {
        public string keyId;//序号
        public string orderId;//订单号
        public string tradeTime;//销售日期
        public string purchaseName;//采购商名
        public string rprice;//零售
        public string pprice;//平台供货
        public string inprice;//平台采购价
        public string platformPrice;//服务费

    }

    public class ShopSalseOrdersItem
    {    
        public string rprice;//零售
        public string pprice;//平台供货
        public string inprice;//平台采购价
        public string platformPrice;//服务费

    }

    public class ShopSalseOrdersParam
    {
        public string[] date;//日期区间
        public string  purchaseName;//采购商名
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
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
        public string salesNum;//销量
        public string salesPrice;//销售额
        public string supplyPrice;//销售单价
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class ClientSeachParam
    {
        public string userId;
        public string userName;
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
        public string platformId;//销售类型
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
        public string platformType;//平台渠道or销售类型
        public string purchaserName;//销售商
        public double brokerage;//佣金
        public string distribution;//bbc分销商
        public string salseTime;//销售时间
        public string supplyPrice;//销售单价
        public string wname;//仓库
    }
    public class ClientItem
    {
        public string keyId;
        public string id;
        public string createDate;
        public string userName;
        public string agentCost;
    }

    public class SalesGoods
    {
        public string[] date;//日期区间
        public string select;//查询条件
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }
    public class SalesOrderItem
    {
        public string keyId;//序号 
        public string orderId;//订单号
        public int num;//商品数量
        public double orderMoney;//订单金额
        public string payTime;//结账时间
        public double receivable;//应收金额
        public string payType;//支付方式
        public double paymoney;//支付金额
        public string discountName;//优惠名称
        public double discountMoney;//优惠金额
        public List<SalesGoodsItem> list = new List<SalesGoodsItem>();       
    }

    public class SalesGoodsItem
    {
        public string keyId;//序号 
        public string goodsName;//商品名称
        public double goodsPrice;//商品金额
        public int quantity;//商品数量      
    }

    public class SalesTotal
    {
        public int totalnum;//商品总数
        public double totalOrderMoney;//订单金额
        public double totalDiscountMoney;//优惠金额
        public double totalReceivable;//应收金额
        public double totalSupplyMoney;//供货金额
    }
}
