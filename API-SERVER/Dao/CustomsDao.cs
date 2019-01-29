using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Dao
{
    public class CustomsDao
    {
        public CustomsDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        public OrderStatusReceipt getOrderStatus(OrderStatusParam orderStatusParam)
        {
            OrderStatusReceipt orderStatusReceipt = new OrderStatusReceipt();
            orderStatusReceipt.flag = "SUCCESS";
            orderStatusReceipt.code = "1";
            orderStatusReceipt.message = "成功";
            string sql = "insert into t_log_orderstatus(orderCode,orderType,status,statusReasonCode,statusReason,operateRemark,operator,operateTime) " +
                "values('" + orderStatusParam.orderCode + "',"
                       + "'" + orderStatusParam.orderType + "',"
                       + "'" + orderStatusParam.status + "',"
                       + "'" + orderStatusParam.statusReasonCode + "',"
                       + "'" + orderStatusParam.statusReason + "',"
                       + "'" + orderStatusParam.operateRemark + "',"
                       + "'" + orderStatusParam.@operator + "',"
                       + "'" + orderStatusParam.operateTime + "')";
            DatabaseOperationWeb.ExecuteDML(sql);
            return orderStatusReceipt;
        }
        public ReportStatusReceipt getReportStatus(ReportStatusParam reportStatusParam)
        {
            ReportStatusReceipt reportStatusReceipt = new ReportStatusReceipt();
            reportStatusReceipt.orderId = reportStatusParam.orderNo;
            reportStatusReceipt.status = 1;
            reportStatusReceipt.notes = "";
            string sql = "insert into t_log_reportstatus(reportId,orderNo,applyTime,status,wayBillNo,logisticsName,logisticsCode,notes,type,ratifyDate) " +
                "values('" + reportStatusParam.reportId + "',"
                       + "'" + reportStatusParam.orderNo + "',"
                       + "'" + reportStatusParam.applyTime + "',"
                       + "'" + reportStatusParam.status + "',"
                       + "'" + reportStatusParam.wayBillNo + "',"
                       + "'" + reportStatusParam.logisticsName + "',"
                       + "'" + reportStatusParam.logisticsCode + "',"
                       + "'" + reportStatusParam.notes + "',"
                       + "'" + reportStatusParam.type + "',"
                       + "'" + reportStatusParam.ratifyDate + "')";
            DatabaseOperationWeb.ExecuteDML(sql);
            return reportStatusReceipt;
        }
        public WayBillNoReceipt getWayBillNo(WayBillNoParam wayBillNoParam)
        {
            WayBillNoReceipt wayBillNoReceipt = new WayBillNoReceipt();
            wayBillNoReceipt.status = 1;
            wayBillNoReceipt.notes = "";
            string sql = "insert into t_log_customs_waybillno(enOrderCode,wayBillNo,logisticsCode,logisticsname) " +
                "values('" + wayBillNoParam.enOrderCode + "',"
                       + "'" + wayBillNoParam.wayBillNo + "',"
                       + "'" + wayBillNoParam.logisticsCode + "',"
                       + "'" + wayBillNoParam.logisticsname + "')";
            DatabaseOperationWeb.ExecuteDML(sql);
            return wayBillNoReceipt;
        }
        public PlatReturnData getPlatData(PlatDataParam platDataParam)
        {
            PlatReturnData platReturnData = new PlatReturnData();
            string insql = "insert into t_log_customs_plat(orderNo,sessionID,serviceTime,createTime) " +
                "values('"+ platDataParam.orderNo + "','"+ platDataParam.sessionID + "','"+ platDataParam.serviceTime + "',now())";
            DatabaseOperationWeb.ExecuteDML(insql);
            string sql = "select * from t_order_list " +
                "where merchantOrderId = '"+ platDataParam.orderNo + "' and sessionId = '"+ platDataParam.sessionID + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];
            if (dt.Rows.Count>0)
            {
                platReturnData.code = "10000";
                platReturnData.message = "";
                platReturnData.serviceTime = GetTimeStamp();
            }
            else
            {
                platReturnData.code = "10001";
                platReturnData.message = "没找到对应订单";
                platReturnData.serviceTime = GetTimeStamp();
            }
            return platReturnData;
        }
        /// <summary>
        /// 生成时间戳 
        /// </summary>
        /// <returns>当前时间减去 1970-01-01 00.00.00 得到的毫秒数</returns>
        public long GetTimeStamp()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            DateTime nowTime = DateTime.Now;
            long unixTime = (long)System.Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);
            return unixTime;
        }
    }
}
