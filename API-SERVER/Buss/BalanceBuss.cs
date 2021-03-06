﻿using API_SERVER.Common;
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
            return balanceDao.getPartner(userId);
        }
        /// <summary>
        /// 获取累计收益
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetTotalProfit(object param, string userId)
        {
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.getTotalProfit(userId);
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
#if DEBUG
            userId = searchBalanceParam.userId;
#endif
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(userId);
            BalanceDao balanceDao = new BalanceDao();
            if (userType == "9")//供应代理
            {
                return balanceDao.getEstimateBySupplierAgent(searchBalanceParam, userId);
            }
            else if (userType == "10")//采购代理
            {
                return balanceDao.getEstimateByPurchaseAgent(searchBalanceParam, userId);
            }
            else if (userType == "2")//采购商
            {
                return balanceDao.getEstimateByPurchase(searchBalanceParam, userId);
            }
            else if (userType == "3")//采购代理
            {
                return balanceDao.getEstimateByAgent(searchBalanceParam, userId);
            }
            else
            {
                return balanceDao.getEstimateBySupplierAgent(searchBalanceParam, userId);
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
#if DEBUG
            userId = searchBalanceParam.userId;
#endif
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.getSettle(searchBalanceParam, userId);
        }
        /// <summary>
        /// 获取结算收益-预估收益
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetSettleInfo(object param, string userId)
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
            searchBalanceParam.BalanceDate = null;
#if DEBUG
            userId = searchBalanceParam.userId;
#endif
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(userId);
            BalanceDao balanceDao = new BalanceDao();
            if (userType == "9")//供应代理
            {
                return balanceDao.getEstimateBySupplierAgent(searchBalanceParam, userId);
            }
            else if (userType == "10")//采购代理
            {
                return balanceDao.getEstimateByPurchaseAgent(searchBalanceParam, userId);
            }
            else if (userType == "2")//采购代理
            {
                return balanceDao.getEstimateByPurchase(searchBalanceParam, userId);
            }
            else if (userType == "3")//采购代理
            {
                return balanceDao.getEstimateByAgent(searchBalanceParam, userId);
            }
            else
            {
                return balanceDao.getEstimateBySupplierAgent(searchBalanceParam, userId);
            }

        }
        #endregion

        /// <summary>
        /// 获取代销-货款结算
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetPayment(object param, string userId)
        {
            PaymentParam paymentParam = JsonConvert.DeserializeObject<PaymentParam>(param.ToString());
            if (paymentParam.pageSize == 0)
            {
                paymentParam.pageSize = 10;
            }
            if (paymentParam.current == 0)
            {
                paymentParam.current = 1;
            }
            BalanceDao balanceDao = new BalanceDao();
            
            return balanceDao.GetPayment(paymentParam, userId);

        }

        /// <summary>
        /// 获取代销-货款结算明细
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetPaymentDetailed(object param, string userId)
        {
            PaymentDetailedParam paymentParam = JsonConvert.DeserializeObject<PaymentDetailedParam>(param.ToString());
            if (paymentParam.pageSize == 0)
            {
                paymentParam.pageSize = 10;
            }
            if (paymentParam.current == 0)
            {
                paymentParam.current = 1;
            }
            BalanceDao balanceDao = new BalanceDao();
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(userId);
            if (userType == "1")//供应商 
            {
                return balanceDao.GetPaymentDetailedGY(paymentParam, userId);
            }
            else if (userType == "2")//采购商
            {
                return balanceDao.GetPaymentDetailed(paymentParam, userId);
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {
                return balanceDao.GetPaymentDetailed(paymentParam, userId);
            }
            else
            {
                MsgResult msg = new MsgResult();
                msg.msg = "用户权限错误";
                return msg;
            }
           

        }

        /// <summary>
        /// 获取代销-货款结算其他明细
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetPaymentOtherDetailed(object param, string userId)
        {
            PaymentDetailedParam paymentDetailedParam = JsonConvert.DeserializeObject<PaymentDetailedParam>(param.ToString());
            if (paymentDetailedParam.pageSize == 0)
            {
                paymentDetailedParam.pageSize = 10;
            }
            if (paymentDetailedParam.current == 0)
            {
                paymentDetailedParam.current = 1;
            }
            BalanceDao balanceDao = new BalanceDao();


            return balanceDao.GetPaymentOtherDetailed(paymentDetailedParam, userId);

        }

        /// <summary>
        /// 获取代销-货款结算打印
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetPaymentPrinting(object param, string userId)
        {
            PaymentDetailedParam paymentDetailedParam = JsonConvert.DeserializeObject<PaymentDetailedParam>(param.ToString());
            if (paymentDetailedParam.pageSize == 0)
            {
                paymentDetailedParam.pageSize = 10;
            }
            if (paymentDetailedParam.current == 0)
            {
                paymentDetailedParam.current = 1;
            }
            BalanceDao balanceDao = new BalanceDao();


            return balanceDao.GetPaymentPrinting(paymentDetailedParam, userId);

        }

        /// <summary>
        /// 代销结账
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_SettleAccounts(object param, string userId)
        {
            SettleAccountsParam settleAccountsParam = JsonConvert.DeserializeObject<SettleAccountsParam>(param.ToString());
            if (settleAccountsParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (settleAccountsParam.dateTo == null || settleAccountsParam.dateTo == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            if (settleAccountsParam.userCode == null || settleAccountsParam.userCode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            BalanceDao balanceDao = new BalanceDao();

            return balanceDao.settleAccounts(settleAccountsParam, userId);

        }

        /// <summary>
        /// 获取货款结算-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_PurchasePayment(object param, string userId)
        {
            PaymentParam paymentParam = JsonConvert.DeserializeObject<PaymentParam>(param.ToString());
            if (paymentParam.pageSize == 0)
            {
                paymentParam.pageSize = 10;
            }
            if (paymentParam.current == 0)
            {
                paymentParam.current = 1;
            }
            BalanceDao balanceDao = new BalanceDao();


            return balanceDao.PurchasePayment(paymentParam, userId);

        }

        /// <summary>
        /// 完成手动调账-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_FinishReconciliation(object param, string userId)
        {
            PaymentDetailedParam pdp = JsonConvert.DeserializeObject<PaymentDetailedParam>(param.ToString());
            if (pdp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (pdp.accountCode == null || pdp.accountCode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }

            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.FinishReconciliation(pdp, userId);
        }


        /// <summary>
        /// 获取货款结算-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_ManualChangeAccount(object param, string userId)
        {
            ManualChangeAccountParam pdp = JsonConvert.DeserializeObject<ManualChangeAccountParam>(param.ToString());
            if (pdp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (pdp.pageSize == 0)
            {
                pdp.pageSize = 10;
            }
            if (pdp.current == 0)
            {
                pdp.current = 1;
            }

            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.ManualChangeAccount(pdp, userId);
        }


        /// <summary>
        /// 财务管理-创建手动调账-调整事项下拉接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_AdjustmentMatters(object param, string userId)
        {           
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.AdjustmentMatters(userId);
        }


        /// <summary>
        /// 财务管理-创建手动调账-获取客商信息接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_CustomersInformation(object param, string userId)
        {
            CustomersInformationParam cip = JsonConvert.DeserializeObject<CustomersInformationParam>(param.ToString());
            if (cip==null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.CustomersInformation(cip,userId);
        }


        /// <summary>
        /// 财务管理-创建手动调账接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_CreateAccount(object param, string userId)
        {
            CreateAccountParam cip = JsonConvert.DeserializeObject<CreateAccountParam>(param.ToString());
            if (cip == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (cip.date==null || cip.date== "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            if (cip.price == null || cip.price == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            if (cip.adjustType == null || cip.adjustType == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            if (cip.userCode == null || cip.userCode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            if (cip.detail == null || cip.detail == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.CreateAccount(cip, userId);
        }


        /// <summary>
        /// 货款结算确认付款接口-代销
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_SurePayMent(object param, string userId)
        {
            PaymentDetailedParam pdp = JsonConvert.DeserializeObject<PaymentDetailedParam>(param.ToString());
            if (pdp==null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (pdp.accountCode == null || pdp.accountCode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.SurePayMent(pdp, userId);
        }

        /// <summary>
        /// 结算管理-供货结算接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_SupplySettlement(object param, string userId)
        {
            SupplySettlementParam supplySettlementParam = JsonConvert.DeserializeObject<SupplySettlementParam>(param.ToString());
            if (supplySettlementParam==null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (supplySettlementParam.current == 0)
            {
                supplySettlementParam.current = 1;
            }
            if (supplySettlementParam.pageSize == 0)
            {
                supplySettlementParam.pageSize = 10;
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.SupplySettlement(supplySettlementParam, userId);
        }



        /// <summary>
        /// 结算管理-供货结算明细接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_SupplySettlementDetails(object param, string userId)
        {
            SupplySettlementDetailsPrarm ssdp = JsonConvert.DeserializeObject<SupplySettlementDetailsPrarm>(param.ToString());
            if (ssdp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (ssdp.accountCode==null || ssdp.accountCode=="")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            if (ssdp.model == null || ssdp.model == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.SupplySettlementDetails(ssdp, userId);
        }

        /// <summary>
        /// 结算管理-供货结算确认付款接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_SupplySettlementSubmit(object param, string userId)
        {
            SupplySettlementDetailsPrarm ssdp = JsonConvert.DeserializeObject<SupplySettlementDetailsPrarm>(param.ToString());
            if (ssdp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (ssdp.accountCode == null || ssdp.accountCode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.SupplySettlementSubmit(ssdp, userId);
        }

        /// <summary>
        /// 结算管理-新采购结算接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_NewPurchaseSettlement(object param, string userId)
        {
            SupplySettlementParam supplySettlementParam = JsonConvert.DeserializeObject<SupplySettlementParam>(param.ToString());
            if (supplySettlementParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (supplySettlementParam.current == 0)
            {
                supplySettlementParam.current = 1;
            }
            if (supplySettlementParam.pageSize == 0)
            {
                supplySettlementParam.pageSize = 10;
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.NewPurchaseSettlement(supplySettlementParam, userId);
        }

        /// <summary>
        /// 结算管理-新采购结算明细接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_NewPurchaseSettlementDetails(object param, string userId)
        {
            SupplySettlementDetailsPrarm ssdp = JsonConvert.DeserializeObject<SupplySettlementDetailsPrarm>(param.ToString());
            if (ssdp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (ssdp.accountCode == null || ssdp.accountCode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            if (ssdp.model == null || ssdp.model == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.NewPurchaseSettlementDetails(ssdp, userId);
        }


        /// <summary>
        /// 结算管理-新采购结算确认付款接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_NewPurchaseSettlementSubmit(object param, string userId)
        {
            SupplySettlementDetailsPrarm ssdp = JsonConvert.DeserializeObject<SupplySettlementDetailsPrarm>(param.ToString());
            if (ssdp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (ssdp.accountCode == null || ssdp.accountCode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            if (ssdp.status == null || ssdp.status == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            if (ssdp.model == null || ssdp.model == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InvalidParam");
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.NewPurchaseSettlementSubmit(ssdp, userId);
        }

        /// <summary>
        /// 结算管理-采购结算接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_PurchaseSettlement(object param, string userId)
        {
            SupplySettlementParam supplySettlementParam = JsonConvert.DeserializeObject<SupplySettlementParam>(param.ToString());
            if (supplySettlementParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (supplySettlementParam.current == 0)
            {
                supplySettlementParam.current = 1;
            }
            if (supplySettlementParam.pageSize == 0)
            {
                supplySettlementParam.pageSize = 10;
            }
            BalanceDao balanceDao = new BalanceDao();
            return balanceDao.PurchaseSettlement(supplySettlementParam, userId);
        }


    }

    public class SupplySettlementDetailsPrarm
    {
        public string accountCode;//结算编号
        public string model;//模式
        public string status;//状态
    }

    public class SupplySettlementParamItem
    {
        public int settled;//已结算
        public int settling;//待结算
    }

    public class NewPurchaseSettlementItem
    {
        public int reconciliationing;//待对账
        public int receivabling;//待收款
        public int settled;//已结算
        public int settling;//待结算
    }

    public class SupplySettlementParam
    {
        public string model;//模式
        public string status;//类型
        public string accountCode;//结算编号
        public string userCode;//用户名
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class CreateAccountParam
    {
        public string date;//年份
        public string price;//钱数
        public string adjustType;//调整类型
        public string userCode;//用户码
        public string detail;//调整原因
    }


    public class CustomersInformationParam
    {
        public string customersCode;//客商编码
        public string userName;//客商名
    }

    public class CustomersInformationItem
    {
        public string customersCode;//客商编码       
        public string userCode;//用户编码       
    }

    public class AdjustmentMattersItem
    {
        public string adjustCode;//事项编码
        public string adjustName;//事项名
    }

    public class ManualChangeAccountParam
    {
        public string[] date;//日期区间
        public int current;//多少页
        public int pageSize;//页面显示多少个商品

    }


    public class SearchBalanceParam
    {
        public string userId;
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
        public double totalProfit = 0;// 收益
    }
    public class BalanceTotalItemNew
    {
        public double total = 0;//总单数
        public double totalSales = 0;//总销售额
        public double totalProfit = 0;// 收益
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
        public double profit;// 收益
        public string distribution;//订单来源：bbc：分销商账号，普通订单：采购商账号
        public string payType;//支付类型：1线上，2线下
    }

    public class BalanceItemNew
    {
        public string keyId;//序号
        public string id;//id
        public string merchantOrderId;//订单号
        public string tradeTime;//订单时间
        public double tradeAmount;//订单销售额 
        public string waybillTime;//结算时间（发货时间）
        public double profit;// 收益
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
    public class ProfitTotalItem
    {
        public string totalEstimate = "";//预估收益
        public string totalUnpaid = "";//在库收益
        public string totalProfit = "";//已结算收益
    }

    public class PaymentParam
    {
        public string[] date;//日期区间
        public string status;//结算状态
        public string accountCode;//结算单号
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }
    public class PaymentItem
    {
        public string keyId;//序号
        public string company;//公司名
        public string userCode;//用户code
        public string date; //账期
        public string status;//结算状态
        public double purchasemoney;//采购金额
        public double refundmoney;//退款金额
        public double othermoney;//其他金额
        public double paymoney;//付
        public string accountCode;//结算单号
        public string flag;//对账状态
    }


    public class PaymentDetailedParam
    {
        public string accountCode;//结算单号
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }
    public class PaymentDetailedItem
    {
        public string keyId;//序号
        public string orderId;//订单号
        public string orderType;//订单类型
        public string barCode;//商品条码
        public string slt;//图片
        public string goodsName;//商品名
        public string brand;//商品品牌
        public double skuUnitPrice;//销售单价
        public double purchasePrice;//供货价
        public int  quantity;//销售数量
        public double money;//金额
        public string accountType;//1正数，2负数
        public string tradeTime;//销售日期
    }

   
    public class PaymentOtherDetailedItem
    {
        public string keyId;//序号
        public string year;//年份
        public string month;//月份
        public double price;//金额
        public string detail;//调整事由
        public string adjustName;//调整项目
        public string customersCode;//客商编码
        public string userName;//客商名
    }

    public class PaymentPrintingItem
    {
        public string accountCode;//结算单号
        public string date; //账期
        public string money;//收据金额
        public string contractCode;//合同编号
        public string today;//打印日期
        public string name;//打印人
        
    }
    public class PaymentPrintingList
    {
        public string keyId;//序号
        public string type;//类别
        public string explain;//说明
        public double price;//金额
    }
    public class PaymentPrinting
    {
        public object item;
        public List<PaymentPrintingList> list = new List<PaymentPrintingList>();
        
    }
    public class SettleAccountsParam
    {
        public string dateTo;
        public string userCode;
    }
}






