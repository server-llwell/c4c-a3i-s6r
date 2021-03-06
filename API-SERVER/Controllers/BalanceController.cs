﻿using System;
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
    public class BalanceController : Controller
    {
        #region 旧结算表
        /// <summary>
        /// 获取运营的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetBalanceListByOperator")]
        public ActionResult GetBalanceListByOperator([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取供应商的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetBalanceListBySupplier")]
        public ActionResult GetBalanceListBySupplier([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取采购商的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetBalanceListByPurchase")]
        public ActionResult GetBalanceListByPurchase([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取代理的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetBalanceListByAgent")]
        public ActionResult GetBalanceListByAgent([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取分销的结算列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetBalanceListByDistribution")]
        public ActionResult GetBalanceListByDistribution([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        #endregion

        #region 新结算表
        /// <summary>
        /// 获取合作方
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetPartner")]
        public ActionResult GetPartner([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取结算收益-预估收益
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetEstimate")]
        public ActionResult GetEstimate([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取结算收益-已结算收益
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetSettle")]
        public ActionResult GetSettle([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取结算收益-已结算收益
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetSettleInfo")]
        public ActionResult GetSettleInfo([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
        /// <summary>
        /// 获取结算收益-累计收益
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetTotalProfit")]
        public ActionResult GetTotalProfit([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        #endregion

        /// <summary>
        /// 获取代销-货款结算
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetPayment")]
        public ActionResult GetPayment([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        /// <summary>
        /// 获取代销、运营-货款结算明细
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetPaymentDetailed")]
        public ActionResult GetPaymentDetailed([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        /// <summary>
        /// 获取代销、运营-货款结算其他明细
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetPaymentOtherDetailed")]
        public ActionResult GetPaymentOtherDetailed([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        /// <summary>
        /// 获取代销、运营-货款结算打印
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("GetPaymentPrinting")]
        public ActionResult GetPaymentPrinting([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        /// <summary>
        /// 代销结账
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SettleAccounts")]
        public ActionResult SettleAccounts([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        /// <summary>
        /// 获取货款结算-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("PurchasePayment")]
        public ActionResult PurchasePayment([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        /// <summary>
        /// 货款结算-完成对账接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("FinishReconciliation")]
        public ActionResult FinishReconciliation([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }


        /// <summary>
        /// 财务管理-手动调账查看、分页接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ManualChangeAccount")]
        public ActionResult ManualChangeAccount([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }


        /// <summary>
        /// 财务管理-创建手动调账-调整事项下拉接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AdjustmentMatters")]
        public ActionResult AdjustmentMatters([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }


        /// <summary>
        /// 财务管理-创建手动调账-获取客商信息接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CustomersInformation")]
        public ActionResult CustomersInformation([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        /// <summary>
        /// 财务管理-创建手动调账接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CreateAccount")]
        public ActionResult CreateAccount([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }


        /// <summary>
        /// 货款结算确认付款接口-代销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SurePayMent")]
        public ActionResult SurePayMent([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }


        /// <summary>
        /// 结算管理-供货结算接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SupplySettlement")]
        public ActionResult SupplySettlement([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        /// <summary>
        /// 结算管理-供货结算明细接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SupplySettlementDetails")]
        public ActionResult SupplySettlementDetails([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        /// <summary>
        /// 结算管理-供货结算确认付款接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SupplySettlementSubmit")]
        public ActionResult SupplySettlementSubmit([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        /// <summary>
        /// 结算管理-新采购结算接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("NewPurchaseSettlement")]
        public ActionResult NewPurchaseSettlement([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        /// <summary>
        /// 结算管理-新采购结算明细接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("NewPurchaseSettlementDetails")]
        public ActionResult NewPurchaseSettlementDetails([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        /// <summary>
        /// 结算管理-新采购结算确认付款接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("NewPurchaseSettlementSubmit")]
        public ActionResult NewPurchaseSettlementSubmit([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }

        /// <summary>
        /// 结算管理-采购结算接口-财务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("PurchaseSettlement")]
        public ActionResult PurchaseSettlement([FromBody]object param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.BalanceApi, param));
        }
    }
}