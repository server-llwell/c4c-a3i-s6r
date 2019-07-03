using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Net;
using System;
using System.Text;
using System.Security.Cryptography;

namespace API_SERVER.Buss
{
    public class WebApiBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.WebApiApi;
        }
        public bool NeedCheckToken()
        {
            return false ;
        }

        public object Do_giveWaybillList(object param,string userId)
        {
            GiveWaybillListParam wparam = JsonConvert.DeserializeObject<GiveWaybillListParam>(param.ToString());
            if (wparam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (wparam.userCode == null || wparam.userCode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (wparam.dateFrom == null || wparam.dateFrom == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (wparam.dateTo == null || wparam.dateTo == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (wparam.sign == null || wparam.sign == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            //判断MD5加密签名
            WebApiDao webApiDao = new WebApiDao();
            string securityKey = webApiDao.getSecurityKey(wparam.userCode);
            if (securityKey=="")
            {
                throw new ApiException(CodeMessage.SecurityKeyNull, "SecurityKeyNull");
            }
            string signText = "userCode="+ wparam.userCode + "&dateFrom=" + wparam.dateFrom + "&dateTo=" + wparam.dateTo + "&securityKey=" + securityKey;
            string sign = MD5Manager.MD5Encrypt32(signText).ToLower();
            if (sign!= wparam.sign )
            {
                throw new ApiException(CodeMessage.SignError, "SignError");
            }
            return webApiDao.getWaybillList(wparam);
        }

        public object Do_sendOrderList(object param, string userId)
        {
            SendOrderListParam wparam = JsonConvert.DeserializeObject<SendOrderListParam>(param.ToString());
            if (wparam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (wparam.userCode == null || wparam.userCode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (wparam.dateFrom == null || wparam.dateFrom == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (wparam.dateTo == null || wparam.dateTo == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (wparam.sign == null || wparam.sign == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (wparam.orderList == null || wparam.orderList.Count == 0)
            {
                throw new ApiException(CodeMessage.OrderNull, "OrderNull");
            }
            //判断MD5加密签名
            WebApiDao webApiDao = new WebApiDao();
            string securityKey = webApiDao.getSecurityKey(wparam.userCode);
            if (securityKey == "")
            {
                throw new ApiException(CodeMessage.SecurityKeyNull, "SecurityKeyNull");
            }
            string signText = "userCode=" + wparam.userCode + "&dateFrom=" + wparam.dateFrom + "&dateTo=" + wparam.dateTo + "&securityKey=" + securityKey;
            string sign = MD5Manager.MD5Encrypt32(signText).ToLower();
            if (sign != wparam.sign)
            {
                throw new ApiException(CodeMessage.SignError, "SignError");
            }
            return webApiDao.sendOrderList(wparam);
        }

        public object Do_getGoodsList(object param, string userId)
        {
            GetGoodsListParam wparam = JsonConvert.DeserializeObject<GetGoodsListParam>(param.ToString());
            if (wparam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (wparam.userCode == null || wparam.userCode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (wparam.date == null || wparam.date == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (wparam.goodsInfo == null)
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (wparam.sign == null || wparam.sign == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            //判断MD5加密签名
            WebApiDao webApiDao = new WebApiDao();
            string securityKey = webApiDao.getSecurityKey(wparam.userCode);
            if (securityKey == "")
            {
                throw new ApiException(CodeMessage.SecurityKeyNull, "SecurityKeyNull");
            }
            string goodsInfo = "";
            if (wparam.goodsInfo.Length>0)
            {
                foreach (var st in wparam.goodsInfo)
                {
                    goodsInfo += "," + st;
                }
                goodsInfo = goodsInfo.Substring(1);
            }
            string signText = "userCode=" + wparam.userCode + "&date=" + wparam.date + "&goodsInfo=" + goodsInfo + "&securityKey=" + securityKey;
            string sign = MD5Manager.MD5Encrypt32(signText).ToLower();
            if (sign != wparam.sign)
            {
                throw new ApiException(CodeMessage.SignError, "SignError");
            }
            //wparam.userCode = "bbcagent@llwell.net";
            return webApiDao.getGoodsList(wparam);
        }
    }
    public class GiveWaybillListParam
    {
        public string userCode;
        public string dateFrom;
        public string dateTo;
        public string sign;
    }
    public class WebApiGiveWaybillList
    {
        public string userCode;
        public string dateFrom;
        public string dateTo;
        public List<WebApiGiveWaybill> waybillList;
    }
    public class WebApiGiveWaybill
    {
        public string merchantOrderId;
        public string waybillTime;
        public string waybillNo;
        public string expressName;
        public string status;
        public string remark;
    }

    public class SendOrderListParam
    {
        public string userCode;
        public string dateFrom;
        public string dateTo;
        public string sign;
        public List<OrderItem> orderList;
    }
    public class ReturnItem
    {
        public string code;
        public string message;
        public string serviceTime;
    }

    public class GetGoodsListParam
    {
        public string userCode;
        public string date;
        public string[] goodsInfo;
        public string sign;
    }
    public class WebApiGetGoodsList
    {
        public string userCode;
        public string date;
        public List<WebApiGetGoods> goodsList;
    }
    public class WebApiGetGoods
    {
        public string barcode;
        public string goodsName;
        public string brand;
        public string slt;
        public double pprice;
        public double rprice;
        public double stock;
    }
}
