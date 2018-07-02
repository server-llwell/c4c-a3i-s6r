using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace API_SERVER.Buss
{
    public class O2OBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.O2OApi;
        }
        
        public object Do_O2OOrderList(object param)
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
            return ordertDao.getOrderList(orderParam,"XXC");
        }
        public object Do_O2OOrder(object param)
        {
            OrderParam orderParam = JsonConvert.DeserializeObject<OrderParam>(param.ToString());
            if (orderParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (orderParam.orderId == null|| orderParam.orderId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            OrderDao orderDao = new OrderDao();
            return orderDao.getOrderItem(orderParam);
        }
        public object Do_ExportOrder(object param)
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
            MsgResult msg = new MsgResult();

            return msg;
        }
        public object Do_UploadOrder(object param)
        {
            UploadParam uploadParam = JsonConvert.DeserializeObject<UploadParam>(param.ToString());
            if (uploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            MsgResult msg = new MsgResult();

            return msg;
        }
    }
    public class UploadParam
    {
        public FileStream context; 
    }

    public class OrderParam
    {
        public string[] date;//日期区间
        public string status;//状态
        public string orderId;//订单号
        public string wcode;//仓库编号
        public string shopId;//店铺
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    
    public class OrderItem
    {
        public string id;
        public string status;//状态
        public string merchantOrderId;//订单号
        public string tradeTime;//订单时间
        public string waybillno;//运单号
        public string consigneeName;//收货人
        public string tradeAmount;//订单总金额
        public string idNumber;//身份证号
        public string consigneeMobile;//收货人电话
        public string addrProvince;//省份
        public string addrCity;//城市
        public string addrDistrict;//县区
        public string addrDetail;//详细地址
        public List<OrderGoodsItem> OrderGoods;//商品列表
    }
    public class OrderGoodsItem
    {
        public string id;
        public string barCode;//条码
        public string skuUnitPrice;//单价
        public string skuBillName;//名称
        public string quantity;//数量
    }
}
