using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            O2OParam o2oParam = JsonConvert.DeserializeObject<O2OParam>(param.ToString());
            if (o2oParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (o2oParam.pageSize == 0)
            {
                o2oParam.pageSize = 10;
            }
            if (o2oParam.current == 0)
            {
                o2oParam.current = 1;
            }
            O2ODao ticketDao = new O2ODao();
            return ticketDao.getO2OList(o2oParam);
        }
        public object Do_O2OOrder(object param)
        {
            O2OParam o2oParam = JsonConvert.DeserializeObject<O2OParam>(param.ToString());
            if (o2oParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (o2oParam.orderId == null|| o2oParam.orderId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            O2ODao ticketDao = new O2ODao();
            return ticketDao.getO2O(o2oParam);
        }
        public object Do_UpdateStatus(object param)
        {
            TicketParam ticketParam = JsonConvert.DeserializeObject<TicketParam>(param.ToString());
            if (ticketParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (ticketParam.ticketCode == null || ticketParam.ticketCode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (ticketParam.status1 == null || ticketParam.status1 == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (ticketParam.remark1 == null)
            {
                ticketParam.remark1 = "";
            }
            MsgResult msg = new MsgResult();
            TicketDao ticketDao = new TicketDao();
            if(ticketDao.UpdateStatus(ticketParam))
            {
                msg.type = "1";
                msg.msg = "操作成功";
            }
            else
            {
                msg.msg = "操作失败";
            }
            return msg;
        }
    }
    
    public class O2OParam
    {
        public string[] date;//日期区间
        public string status;//状态
        public string orderId;//订单号
        public string wcode;//仓库编号
        public string shopId;//店铺
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }
    
    public class O2OOrder
    {
        public string id;
        public string status;//状态
        public string merchantOrderId;//订单号
        public string tradeTime;//订单时间
        public string waybillno;//运单号
        public string consigneeName;//收货人
        public List<O2OOrderGoods> o2oOrderGoods;//商品列表
    }
    public class O2OOrderGoods
    {
        public string id;
        public string barCode;//条码
        public string skuUnitPrice;//单价
        public string skuBillName;//名称
        public string quantity;//数量
    }
}
