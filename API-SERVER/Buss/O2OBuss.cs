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
    public class O2OBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.O2OApi;
        }
        public bool NeedCheckToken()
        {
            return true;
        }

        public object Do_O2OOrderList(object param,string userId)
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
            OrderDao ordertDao = new OrderDao();
            //处理用户账号对应的查询条件
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(userId);
            if (userType == "1")//供应商 
            {
                return ordertDao.getOrderListOfSupplier(orderParam, "", false);
            }
            else if (userType == "2")//采购商
            {
                return ordertDao.getOrderListOfPurchasers(orderParam, "", false);
            }
            else if (userType == "3")//代理
            {
                return ordertDao.getOrderListOfSupplier(orderParam, "", false);
            }
            else if (userType == "4")//代理分销商
            {
                return ordertDao.getOrderListOfSupplier(orderParam, "", false);
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {
                return ordertDao.getOrderListOfOperator(orderParam, "", false);
            }
            else
            {
                MsgResult msg = new MsgResult();
                msg.msg = "用户权限错误";
                return msg;
            }
        }
        public object Do_O2OOrder(object param, string userId)
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
            OrderDao orderDao = new OrderDao();
            return orderDao.getOrderItem(orderParam, true);
        }
      
    }
}
