using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace API_SERVER.Buss
{
    public class CustomsBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.CustomsApi;
        }

        public bool NeedCheckToken()
        {
            return false;
        }
        public object Do_GetOrderStatus(object param, string userId)
        {
            OrderStatusParam orderStatusParam = JsonConvert.DeserializeObject<OrderStatusParam>(param.ToString());

            CustomsDao customsDao = new CustomsDao();
            return customsDao.getOrderStatus(orderStatusParam);
        }
        public object Do_GetReportStatus(object param, string userId)
        {
            ReportStatusParam reportStatusParam = JsonConvert.DeserializeObject<ReportStatusParam>(param.ToString());

            CustomsDao customsDao = new CustomsDao();
            return customsDao.getReportStatus(reportStatusParam);
        }
        public object Do_GetWayBillNo(object param, string userId)
        {
            WayBillNoParam wayBillNoParam = JsonConvert.DeserializeObject<WayBillNoParam>(param.ToString());

            CustomsDao customsDao = new CustomsDao();
            return customsDao.getWayBillNo(wayBillNoParam);
        }
        public object Do_platDataOpen(object param, string userId)
        {
            PlatDataParam platDataParam = JsonConvert.DeserializeObject<PlatDataParam>(param.ToString());

            CustomsDao customsDao = new CustomsDao();
            return customsDao.getPlatData(platDataParam);
        }
    }
    public class PlatDataParam
    {
        public string orderNo;
        public string sessionID;
        public long serviceTime;
    }
    public class PlatReturnData
    {
        public string code;
        public string message;
        public long serviceTime;
    }

    public class OrderStatusParam
    {
        public string orderCode;//单据编码
        public string orderType;//单据类型
        public string status;//单据状态,OMS_REJECT：拒单,OMS_ACCEPT：接单
        public string statusReasonCode;//状态原因码,10：企业信息未备案,11：商品信息未备案,12：商品库存不足,13：预付税金不足,199：其它原因
        public string statusReason;//状态原因
        public string operateRemark;//操作备注
        public string @operator;//操作人
        public string operateTime;//操作时间yyyy-mm-dd HH:mi:ss
    }
    public class ReportStatusParam
    {
        public string reportId;//进境单号
        public string orderNo;//订单ID
        public string applyTime;//申报时间yyyy-mm-dd HH:mi:ss
        public int status;//放行状态：
                             //海关状态	
                             //21：已报海关
                             //22：海关单证放行
                             //23：报海关失败
                             //24：海关查验/转人工/挂起
                             //25：海关单证审核不通过
                             //26：海关已接受申报，待货物运抵后处理
                             //41：海关货物查扣
                             //42：海关货物放行
                             //国检状态	
                             //11：已报国检
                             //12：国检放行
                             //13：国检审核驳回
                             //14：国检抽检
                             //15：国检抽检未过
                             //16：国检审核失败
        public string wayBillNo;//运单号（国内快递单号）
        public string logisticsName;//物流企业名称（顺丰，圆通等）
        public string logisticsCode;//物流企业代码【顺丰：0000017】;【圆通：0000025】；【百世：1000000239】
        public string notes;//如失败，则放失败信息
        public int type;//关检类型,1.国检；2海关
        public string ratifyDate;//清单放行时间yyyy-mm-dd HH:mi:ss

    }
    public class WayBillNoParam
    {
        public string enOrderCode;//订单号
        public string wayBillNo;//运单号
        public string logisticsCode;//物流企业编码
        public string logisticsname;//物流企业名称
    }
    public class OrderStatusReceipt
    {
        public string flag; //响应结果 字符串,SUCCESS：成功FAILURE：失败
        public string code;//响应码
        public string message;//响应信息
    }
    public class ReportStatusReceipt
    {
        public string orderId;//订单ID
        public int status;// 接收状态:
        public string notes;// 如失败，则放错误信息
    }
    public class WayBillNoReceipt
    {
        public int status;// 接收状态:
        public string notes;// 如失败，则放错误信息
    }
}






