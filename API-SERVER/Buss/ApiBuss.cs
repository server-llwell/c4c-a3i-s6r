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
        /// 获取运营的结算列表
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
        /// 获取供应商的结算列表
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
    }
    public class ImportOrderParam
    {
        public string userCode;//
        public List<OrderItem> OrderList;
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
}






