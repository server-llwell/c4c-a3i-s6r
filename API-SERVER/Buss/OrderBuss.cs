using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;

namespace API_SERVER.Buss
{
    public class OrderBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.OrderApi;
        }
        #region 查询
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_GetOrderList(object param,string userId)
        {
            OrderParam orderParam = JsonConvert.DeserializeObject<OrderParam>(param.ToString());
            if (orderParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (orderParam.userId == null || orderParam.userId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
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
            return ordertDao.getOrderList(orderParam,"",false);
        }
        /// <summary>
        /// 获取单个订单信息
        /// </summary>
        /// <param name="param">包含订单编号</param>
        /// <returns></returns>
        public object Do_GetOrder(object param,string userId)
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
            return orderDao.getOrderItem(orderParam, false);
        }
        /// <summary>
        /// 获取快递下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetExpress(object param,string userId)
        {
            OrderDao orderDao = new OrderDao();
            return orderDao.getExpress();
        }
        /// <summary>
        /// 确认发货
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_SingleWaybill(object param,string userId)
        {
            SingleWaybillParam singleWaybillParam = JsonConvert.DeserializeObject<SingleWaybillParam>(param.ToString());
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
        public object Do_Overseas(object param,string userId)
        {
            SingleWaybillParam singleWaybillParam = JsonConvert.DeserializeObject<SingleWaybillParam>(param.ToString());
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
        #endregion

        #region 上传、导出
        /// <summary>
        /// 导出订单
        /// </summary>
        /// <param name="param">包含用户code，仓库编号</param>
        /// <returns></returns>
        public object Do_ExportOrder(object param,string userId)
        {
            OrderParam orderParam = JsonConvert.DeserializeObject<OrderParam>(param.ToString());
            if (orderParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (orderParam.userId == null || orderParam.userId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (orderParam.wid == null || orderParam.wid == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            OrderDao ordertDao = new OrderDao();

            return ordertDao.exportOrder(orderParam);
        }
        /// <summary>
        /// 上传订单
        /// </summary>
        /// <param name="param">包含用户code，上传文件名</param>
        /// <returns></returns>
        public object Do_UploadOrder(object param,string userId)
        {
            FileUploadParam uploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (uploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (uploadParam.userId == null || uploadParam.userId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (uploadParam.fileTemp == null || uploadParam.fileTemp == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            OrderDao orderDao = new OrderDao();

            return orderDao.UploadOrder(uploadParam);
        }

        /// <summary>
        /// 导出运单
        /// </summary>
        /// <param name="param">包含用户code，仓库编号</param>
        /// <returns></returns>
        public object Do_ExportWaybill(object param,string userId)
        {
            OrderParam orderParam = JsonConvert.DeserializeObject<OrderParam>(param.ToString());
            if (orderParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (orderParam.userId == null || orderParam.userId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (orderParam.wid == null || orderParam.wid == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            OrderDao orderDao = new OrderDao();

            return orderDao.exportWaybill(orderParam);
        }
        /// <summary>
        /// 上传运单
        /// </summary>
        /// <param name="param">包含用户code，上传文件名</param>
        /// <returns></returns>
        public object Do_UploadWaybill(object param,string userId)
        {
            FileUploadParam uploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (uploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (uploadParam.userId == null || uploadParam.userId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (uploadParam.byte64 == null || uploadParam.byte64 == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            OrderDao ordertDao = new OrderDao();

            return ordertDao.UploadWaybill(uploadParam);
        }
        #endregion
    }

    public class OrderUploadItem
    {
        public List<string> fileName;
    }

    public class OrderParam
    {
        public string[] date;//日期区间
        public string userId;//用户名
        public string status;//状态
        public string orderId;//订单号
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
    }

    public class ExpressItem
    {
        public string expressId;
        public string expressName;//快递公司
    }

    public class OrderItem
    {
        public string id;
        public string status;//状态
        public string ifSend;//是否有发货按钮0没有1有
        public string warehouseId;//仓库id
        public string warehouseCode;//仓库code
        public string parentOrderId;//父订单号
        public string merchantOrderId;//订单号
        public string tradeTime;//订单时间
        public string expressName;//快递公司
        public string waybillno;//运单号
        public string purchase;//分销渠道
        public string supplier;//供应商
        public string consigneeName;//收货人
        public string tradeAmount;//订单总金额
        public string idNumber;//身份证号
        public string consigneeMobile;//收货人电话
        public string addrCountry;//国家
        public string addrProvince;//省份
        public string addrCity;//城市
        public string addrDistrict;//县区
        public string addrDetail;//详细地址
        public List<OrderGoodsItem> OrderGoods;//商品列表
    }
    public class OrderGoodsItem
    {
        public string id;
        public string slt;//商品图片
        public string barCode;//条码
        public string skuUnitPrice;//销售单价
        public string totalPrice;//销售总价
        public string skuBillName;//名称
        public string quantity;//数量
        public string purchasePrice;//供应价
        public string suppliercode;//供应商code
        public string supplyPrice;//进价
        public string waybillPrice;//运费
        public string tax;//税
        public string profitPlatform;//平台利润
        public string profitAgent;//代理利润
        public string profitDealer;//分销利润
        public string profitOther1;//其他利润1
        public string other1Name;//其他1名称
        public string profitOther2;//其他利润2
        public string other2Name;//其他2名称
        public string profitOther3;//其他利润3
        public string other3Name;//其他3名称
public string taxation;//
        public string taxation2;//
        public string taxation2type;//
        public string taxation2line;//
        public string freight;//

    }
}
