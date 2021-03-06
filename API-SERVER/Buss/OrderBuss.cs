﻿using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using System.Data;

namespace API_SERVER.Buss
{
    public class OrderBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.OrderApi;
        }
        public bool NeedCheckToken()
        {
            return true;
        }
        #region 查询
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_GetOrderList(object param, string userId)
        {
            OrderParam orderParam = JsonConvert.DeserializeObject<OrderParam>(param.ToString());
            if (orderParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (orderParam.pageSize == 0)
            {
                orderParam.pageSize = 10;
            }
            if (orderParam.current == 0)
            {
                orderParam.current = 1;
            }                       
            orderParam.userId = userId;
            OrderDao ordertDao = new OrderDao();
            //处理用户账号对应的查询条件
            UserDao userDao = new UserDao(); 
            string userType = userDao.getUserType(orderParam.userId); 
            if (userType == "1")//供应商 
            {
                return ordertDao.getOrderListOfSupplier(orderParam, "1", false);
            }
            else if (userType == "2")//采购商
            {
                return ordertDao.getOrderListOfPurchasers(orderParam, "1", false);
            }
            else if (userType == "3")//代理
            {
                return ordertDao.getOrderListOfAgent(orderParam, "1", false);
            }
            else if (userType == "4")//代理分销商
            {
                return ordertDao.getOrderListOfDealer(orderParam, "1", false);
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {
                return ordertDao.getOrderListOfOperator(orderParam, "1", false);
            }
            else if (userType == "12")//零售
            {
                return ordertDao.getOrderListOfRetail(orderParam, "1", false, userId);
            }
            else
            {
                MsgResult msg = new MsgResult();
                msg.msg = "用户权限错误";
                return msg;
            }

        }

        /// <summary>
        /// 零售订单支付
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_PayOrder(object param, string userId)
        {
            UserDao userDao = new UserDao(); 
            string userType = userDao.getUserType(userId);
            PayOrderParam payOrderParam = JsonConvert.DeserializeObject<PayOrderParam>(param.ToString()); 
            if (payOrderParam==null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (payOrderParam.parentOrderId == null || payOrderParam.parentOrderId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (userType!="12")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "账号权限不足，请联系客服");
            }
            OrderDao ordertDao = new OrderDao();
            return ordertDao.PayOrder(payOrderParam,userId);
        }

        /// <summary>
        /// 零售订单退货申请
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_ReGoodsApply(object param, string userId)
        {
            PayOrderParam getRetailMoneyParam = JsonConvert.DeserializeObject<PayOrderParam>(param.ToString());
            if (getRetailMoneyParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (getRetailMoneyParam.parentOrderId == null || getRetailMoneyParam.parentOrderId == "")
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (getRetailMoneyParam.refundRemark == null || getRetailMoneyParam.refundRemark == "")
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            OrderDao ordertDao = new OrderDao();

            return ordertDao.ReGoodsApply(getRetailMoneyParam, userId);
        }

        /// <summary>
        /// 零售订单同意退货接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_AgreeReGoods(object param, string userId)
        {
            PayOrderParam getRetailMoneyParam = JsonConvert.DeserializeObject<PayOrderParam>(param.ToString());
            if (getRetailMoneyParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (getRetailMoneyParam.parentOrderId == null || getRetailMoneyParam.parentOrderId == "")
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            OrderDao orderDao = new OrderDao();
            return orderDao.AgreeReGoods(getRetailMoneyParam, userId);
        }
     

        /// <summary>
        /// 零售订单退货成功接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_MakeSureReGoods(object param, string userId)
        {
            PayOrderParam getRetailMoneyParam = JsonConvert.DeserializeObject<PayOrderParam>(param.ToString());
            if (getRetailMoneyParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }                       
            if (getRetailMoneyParam.parentOrderId == null || getRetailMoneyParam.parentOrderId == "")
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            OrderDao ordertDao = new OrderDao();
            return ordertDao.MakeSureReGoods(getRetailMoneyParam, userId);
        }

        /// <summary>
        /// 零售订单填写退货运单号接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_ReGoodsFundId(object param, string userId)
        {
            MakeSureReGoodsParam getRetailMoneyParam = JsonConvert.DeserializeObject<MakeSureReGoodsParam>(param.ToString());
            if (getRetailMoneyParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (getRetailMoneyParam.refundExpressId == null || getRetailMoneyParam.refundExpressId == "")
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (getRetailMoneyParam.refundId == null || getRetailMoneyParam.refundId == "")
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            if (getRetailMoneyParam.parentOrderId == null || getRetailMoneyParam.parentOrderId == "")
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            OrderDao ordertDao = new OrderDao();
            return ordertDao.ReGoodsFundId(getRetailMoneyParam, userId);
        }

        /// <summary>
        /// 零售订单退货双方信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_ReGoodsMessage(object param, string userId)
        {
            PayOrderParam getRetailMoneyParam = JsonConvert.DeserializeObject<PayOrderParam>(param.ToString());
            if (getRetailMoneyParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (getRetailMoneyParam.parentOrderId == null || getRetailMoneyParam.parentOrderId == "")
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            OrderDao ordertDao = new OrderDao();
            return ordertDao.ReGoodsMessage(getRetailMoneyParam, userId);
        }

        /// <summary>
        /// 零售订单退货双方信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_ReGoodsFundIdMessage(object param, string userId)
        {
            PayOrderParam getRetailMoneyParam = JsonConvert.DeserializeObject<PayOrderParam>(param.ToString());
            if (getRetailMoneyParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (getRetailMoneyParam.parentOrderId == null || getRetailMoneyParam.parentOrderId == "")
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            OrderDao ordertDao = new OrderDao();
            return ordertDao.ReGoodsFundIdMessage(getRetailMoneyParam, userId);
        }

        /// <summary>
        /// 获取单个订单信息
        /// </summary>
        /// <param name="param">包含订单编号</param>
        /// <returns></returns>
        public object Do_GetOrder(object param, string userId)
        {
            OrderParam orderParam = JsonConvert.DeserializeObject<OrderParam>(param.ToString());
            if (orderParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (orderParam.orderId == null || orderParam.orderId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
#if !DEBUG
                orderParam.userId = userId;
#endif
            OrderDao orderDao = new OrderDao();
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(orderParam.userId);
            if (userType == "2" || userType == "3")//供应商 
            {
                return orderDao.getOrderItemByParent(orderParam, false);
            }
            else
            {
                return orderDao.getOrderItem(orderParam, false);
            }
        }
        
        /// <summary>
        /// 获取快递下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetExpress(object param, string userId)
        {
            OrderDao orderDao = new OrderDao();
            return orderDao.getExpress();
        }

        /// <summary>
        /// 获取发货信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetConsigneeMsg(object param, string userId)
        {
            GetConsigneeMsgParam gcmp = JsonConvert.DeserializeObject<GetConsigneeMsgParam>(param.ToString());
            if (gcmp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (gcmp.merchantOrderId == null || gcmp.merchantOrderId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            OrderDao orderDao = new OrderDao();
            return orderDao.GetConsigneeMsg(gcmp, userId);
        }


        /// <summary>
        /// 确认发货
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_SingleWaybill(object param, string userId)
        {
            SingleWaybillParam singleWaybillParam = JsonConvert.DeserializeObject<SingleWaybillParam>(param.ToString());

#if !DEBUG
                singleWaybillParam.userId = userId;
#endif
            if (singleWaybillParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (singleWaybillParam.userId == null || singleWaybillParam.userId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (singleWaybillParam.orderId == null || singleWaybillParam.orderId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (singleWaybillParam.waybillno == null || singleWaybillParam.waybillno == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (singleWaybillParam.expressId == null || singleWaybillParam.expressId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            OrderDao orderDao = new OrderDao();
            return orderDao.singleWaybill(singleWaybillParam);
        }
        /// <summary>
        /// 海外已出库
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_Overseas(object param, string userId)
        {
            SingleWaybillParam singleWaybillParam = JsonConvert.DeserializeObject<SingleWaybillParam>(param.ToString());
#if !DEBUG
                singleWaybillParam.userId = userId;
#endif
            if (singleWaybillParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (singleWaybillParam.userId == null || singleWaybillParam.userId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (singleWaybillParam.orderId == null || singleWaybillParam.orderId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            OrderDao orderDao = new OrderDao();
            return orderDao.Overseas(singleWaybillParam);
        }
        /// <summary>
        /// 获取订单页二维码图片
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetOrderPageQRCode(object param, string userId)
        {
            UserDao userDao = new UserDao();
            orderUrl orderUrl = new orderUrl();
            orderUrl.url = userDao.getOrderPageQRCode(userId);
            return orderUrl;
        }

        /// <summary>
        /// 获取清关信息
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetCustomsState(object param, string userId)
        {
            CustomsStateParam customsStateParam = JsonConvert.DeserializeObject<CustomsStateParam>(param.ToString());
            if (customsStateParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            OrderDao orderDao = new OrderDao();
            var list = orderDao.getCustomsState(customsStateParam.orderId);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].key = i;
            }

            return list;

        }
        #endregion

        #region 上传、导出
        /// <summary>
        /// 导出订单
        /// </summary>
        /// <param name="param">包含用户code，仓库编号</param>
        /// <returns></returns>
        public object Do_ExportOrder(object param, string userId)
        {
            OrderParam orderParam = JsonConvert.DeserializeObject<OrderParam>(param.ToString());
            if (orderParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (orderParam.wid == null || orderParam.wid == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            OrderDao ordertDao = new OrderDao();
#if !DEBUG
                orderParam.userId = userId;
#endif
            return ordertDao.exportOrder(orderParam);
        }
        /// <summary>
        /// 上传直营订单
        /// </summary>
        /// <param name="param">包含用户code，上传文件名</param>
        /// <returns></returns>
        public object Do_UploadOrder(object param, string userId)
        {
            FileUploadParam uploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (uploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (uploadParam.fileTemp == null || uploadParam.fileTemp == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            OrderDao orderDao = new OrderDao();
#if !DEBUG
                uploadParam.userId = userId;
#endif
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(uploadParam.userId);
            if (userType == "12")//零售
            {
                return orderDao.UploadOrderLS(uploadParam);
            }
            else
            {
                return orderDao.UploadOrder(uploadParam);
            }
            
        }
        /// <summary>
        /// 上传代销订单
        /// </summary>
        /// <param name="param">包含用户code，上传文件名</param>
        /// <returns></returns>
        public object Do_UploadOrderDX(object param, string userId)
        {
            FileUploadParam uploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (uploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (uploadParam.fileTemp == null || uploadParam.fileTemp == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            OrderDao orderDao = new OrderDao();
            UserDao userDao = new UserDao();
#if !DEBUG
                uploadParam.userId = userId;
#endif
            if (userDao.getUserOrderType(uploadParam.userId) == "JW")
            {
                return orderDao.UploadOrderDXJW(uploadParam);
            }
            else if (userDao.getUserOrderType(uploadParam.userId) == "WXC")
            {
                return orderDao.UploadOrderDXSY(uploadParam);
            }
            else
            {
                return orderDao.UploadOrderDX(uploadParam);
            }

        }
        /// <summary>
        /// 上传分销商订单
        /// </summary>
        /// <param name="param">包含用户code，上传文件名</param>
        /// <returns></returns>
        public object Do_UploadOrderOfDistribution(object param, string userId)
        {
            FileUploadParam uploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (uploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (uploadParam.fileTemp == null || uploadParam.fileTemp == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            OrderDao orderDao = new OrderDao();
            uploadParam.userId = userId;
            return orderDao.UploadOrderOfDistribution(uploadParam);
        }

        /// <summary>
        /// 导出运单
        /// </summary>
        /// <param name="param">包含用户code，仓库编号</param>
        /// <returns></returns>
        public object Do_ExportWaybill(object param, string userId)
        {
            OrderParam orderParam = JsonConvert.DeserializeObject<OrderParam>(param.ToString());
            if (orderParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            OrderDao orderDao = new OrderDao();
#if !DEBUG
                orderParam.userId = userId;
#endif
            return orderDao.exportWaybill(orderParam);
        }
        /// <summary>
        /// 上传运单
        /// </summary>
        /// <param name="param">包含用户code，上传文件名</param>
        /// <returns></returns>
        public object Do_UploadWaybill(object param, string userId)
        {
            FileUploadParam uploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (uploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (uploadParam.fileTemp == null || uploadParam.fileTemp == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            OrderDao ordertDao = new OrderDao();
            uploadParam.userId = userId;

            return ordertDao.UploadWaybill(uploadParam);
        }

        /// <summary>
        /// 导出查询出来的订单
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_ExportSelectOrder(object param, string userId)
        {
            OrderParam orderParam = JsonConvert.DeserializeObject<OrderParam>(param.ToString());
            if (orderParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
#if !DEBUG
                orderParam.userId = userId;
#endif
            OrderDao ordertDao = new OrderDao();
            return ordertDao.exportSelectOrder(orderParam);
        }

        #region 搁置财务部分
        ///// <summary>
        ///// 获取销售日报表
        ///// </summary>
        ///// <param name="param">查询条件</param>
        ///// <returns></returns>
        //public object Do_GetSalesFrom(object param, string userId)
        //{
        //    GetSalesFromParam orderParam = JsonConvert.DeserializeObject<GetSalesFromParam>(param.ToString());
        //    if (orderParam == null)
        //    {
        //        throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
        //    }
        //    if (orderParam.pageSize == 0)
        //    {
        //        orderParam.pageSize = 10;
        //    }
        //    if (orderParam.current == 0)
        //    {
        //        orderParam.current = 1;
        //    }
        //    OrderDao ordertDao = new OrderDao();
        //    return ordertDao.GetSalesFrom(orderParam, userId);
        //}
        #endregion
        #endregion
    }

    public class MakeSureReGoodsParam
    {
        public string parentOrderId;//订单号
        public string refundId;//退运单号
        public string refundExpressId;//快递公司号
        public string expressName;//快递公司名
        public int type = 0;//0不显示，1显示
    }

    public class PayOrderParam
    {
        public string parentOrderId;//订单号
        public string refundRemark;//退单备注 
    }

    public class ReGoodsMessageItem
    {
        public string purchaserCode;//采购商名
        public string purchaserTel;//采购商电话
        public string customerCode;//供应商名
        public string customerTel;//供应商电话
        public string refundRemark;//退货理由
    }

    public class GetSalesFromParam
    {
        public string[] date;//时间段
        public string customerCode;//供应商
        public string purchaserCode;//渠道商
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class GetSalesFromItem
    {
        public string tradeTime;//销售日期
        public string customerName;//供货商名称
        public string platformType;//订单类型
        public string merchantOrderId;//订单号
        public string purchaserName;//采购商名称
        public string waybillPrice;//运费
        public string platformPrice;//服务费
        public string payType;//支付方式
        public string tradeAmount;//销售收入
        public string noTaxPrice;//不含税收入
        public string waybillBelong;//运费承担方
    }

    public class GetConsigneeMsgParam
    {
        public string merchantOrderId;//订单号
    }

    public class GetConsigneeMsgItem
    {
        public string consigneeName;//发货人
        public string consigneeMobile;//发货人电话
        public string consigneeAdr;//发货人地址
    }

    public class CustomsStateParam
    {
        public string orderId;
    }

    public class OrderUploadItem
    {
        public List<string> fileName;
    }

    public class CustomsStateItem
    {
        public int key;
        public string applyTime;
        public string orderNo;
        public string wayBillNo;
        public string logisticsName;
        public string notes;
        public string ratifyDate;
    }

    public class OrderParam
    {
        public string[] date;//日期区间
        public string userId;//用户名
        public string status;//状态
        public string orderId;//订单号
        public string platformId;//平台渠道id
        public string supplier;//供应商信息
        public string waybillno;//运单号
        public string wid;//仓库id
        public string wcode;//仓库编号
        public string shopId;//店铺
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class SingleWaybillParam
    {
        public string userId;//用户名
        public string orderId;//订单号
        public string waybillno;//运单号
        public string expressId;//快递公司id
        public string inputFreight;//快递费
    }

    public class ExpressItem
    {
        public string expressId;
        public string expressName;//快递公司
    }

    public class OrderTotalItem
    {
        public double total = 0;//总条数
        public double totalSales = 0;//总销量
        public double totalTradeAmount = 0;//总金额
        public double totalPurchase = 0;//总渠道利润
        public double totalAgent = 0;//总代理佣金
        public double totalDealer = 0;//总分销佣金
        public double totalDistribution = 0;//总分销商数量
        public double accountBalance;//账户余额
    }

    public class OrderItem
    {
        public string keyId;//序号
        public string id;
        public string status;//状态
        public string ifSend;//是否有发货按钮0没有1有
        public string ifAgree;//是否有同意退货按钮0没有1有
        public string ifFinish;//是否有完成退货按钮0没有1有
        public string warehouseId;//仓库id
        public string warehouseCode;//仓库code
        public string warehouseName;//仓库名
        public string parentOrderId;//父订单号
        public string merchantOrderId;//订单号
        public string tradeTime;//订单时间
        public string expressName;//快递公司
        public string waybillno;//运单号
        public string purchase;//渠道商
        public string purchaseId;//渠道商id
        public string supplier;//供应商
        public string supplierAgentCode;//供应代理usercode
        public string purchaseAgentCode;//采购代理usercode
        public string consigneeCode;//收货人的账号
        public string consigneeName;//收货人
        public string tradeAmount;//订单总金额
        public string idNumber;//身份证号
        public string consigneeMobile;//收货人电话
        public string addrCountry;//国家
        public string addrProvince;//省份
        public string addrCity;//城市
        public string addrDistrict;//县区
        public string addrDetail;//详细地址
        public double freight;//运费
        public string platformId;//平台渠道id
        public double sales;//销量
        public double purchaseTotal;//渠道利润
        public double agentTotal;//代理利润
        public double dealerTotal;//分销利润
        public string distribution;//分销商
        public string consignorName;//发货人
        public string consignorMobile;//发货人电话
        public string consignorAddr;//发货人地址
        public string payType;//支付类型
        public string payNo;//支付单号
        public string payTime;//支付单生成时间
        public string derateName;//优惠名称
        public double derate;//优惠金额
        

        public List<OrderGoodsItem> OrderGoods;//商品列表
    }
    public class OrderGoodsItem
    {
        public string id;
        public string slt;//商品图片
        public string barCode;//条码
        public double skuUnitPrice;//销售单价
        public double totalPrice;//销售总价
        public string skuBillName;//名称
        public double quantity;//数量
        public double purchasePrice;//供应价
        public string suppliercode;//供应商code
        public double supplyPrice;//进价
        public double tax;//税
        public double waybillPrice;//运费
        public double platformPrice;//平台提点
        public double supplierAgentPrice;//供货代理提点
        public string supplierAgentCode;//供应代理usercode
        public double purchaseAgentPrice;//采购代理提点
        public string purchaseAgentCode;//采购代理usercode
        public double profitPlatform;//平台利润
        public double profitAgent;//代理利润
        public double profitDealer;//分销利润
        public double profitOther1;//其他利润1
        public string other1Name;//其他1名称
        public double profitOther2;//其他利润2
        public string other2Name;//其他2名称
        public double profitOther3;//其他利润3
        public string other3Name;//其他3名称
        public double purchaseP;//渠道利润
        public DataRow dr;
    }
    public class OrderGoodsOtherItem
    {
        public double profitPlatform;//平台利润
        public double profitAgent;//代理利润
        public double profitDealer;//分销利润
        public double profitOther1;//其他利润1
        public string other1Name;//其他1名称
        public double profitOther2;//其他利润2
        public string other2Name;//其他2名称
        public double profitOther3;//其他利润3
        public string other3Name;//其他3名称
        public double taxation;//税率
        public double taxation2;//提档税率
        public string taxation2type;//提档类别
        public double taxation2line;//提档线
        public double freight;//运费
    }
    public class orderUrl
    {
        public string url;
    }
}
