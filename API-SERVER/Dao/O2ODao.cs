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
    public class O2ODao
    {
        private string path = System.Environment.CurrentDirectory;

        public O2ODao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        public PageResult getO2OList(O2OParam o2oParam)
        {
            PageResult O2OOrderResult = new PageResult();
            O2OOrderResult.pagination = new Page(o2oParam.current, o2oParam.pageSize);
            O2OOrderResult.list = new List<Object>();
            string st = "";
            if (o2oParam.date != null && o2oParam.date.Length ==2)
            {
                st = " and tradeTime BETWEEN '" + o2oParam.date[0] + "' AND DATE_ADD('" + o2oParam.date[1] + "',INTERVAL 1 DAY) ";
            }
            if (o2oParam.orderId != null && o2oParam.orderId != "")
            {
                st = "and merchantOrderId like '%" + o2oParam.orderId + "%' ";
            }
            if (o2oParam.status != null && o2oParam.status != "" && o2oParam.status != "全部")
            {
                st = "and status = '" + o2oParam.status + "' ";
            }
            if (o2oParam.wcode != null && o2oParam.wcode != "")
            {
                st = "and warehouseCode = '" + o2oParam.wcode + "' ";
            }
            if (o2oParam.shopId != null && o2oParam.shopId != "")
            {
                st = "and purchaserId = '" + o2oParam.shopId + "' ";
            }
            string sql = "SELECT id,status,merchantOrderId,tradeTime,waybillno,consigneeName FROM t_order_list t " +
                         " where apitype='XXC' " + st +
                         " ORDER BY status asc, id desc LIMIT " + (o2oParam.current - 1) * o2oParam.pageSize + "," + o2oParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql1 = "SELECT count(*) FROM t_order_list t " +
                         " where apitype='XXC' " + st;

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                O2OOrderResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    O2OOrder o2oOrder = new O2OOrder();
                    o2oOrder.id = dt.Rows[i]["id"].ToString();
                    o2oOrder.merchantOrderId = dt.Rows[i]["merchantOrderId"].ToString();
                    o2oOrder.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    o2oOrder.waybillno = dt.Rows[i]["waybillno"].ToString();
                    o2oOrder.consigneeName = dt.Rows[i]["consigneeName"].ToString();
                    o2oOrder.status = dt.Rows[i]["status"].ToString();
                    O2OOrderResult.list.Add(o2oOrder);
                }

            }
            return O2OOrderResult;
        }

        public O2OOrder getO2O(O2OParam o2oParam)
        {
            O2OOrder o2oOrder = new O2OOrder();
            string sql1 = "select id,status,merchantOrderId,tradeTime,waybillno,consigneeName FROM t_order_list " +
                          "where merchantOrderId  = '" + o2oParam.orderId + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_daigou_ticket").Tables[0];
            if (dt.Rows.Count > 0)
            {
                o2oOrder.id = dt.Rows[0]["id"].ToString();
                o2oOrder.merchantOrderId = dt.Rows[0]["merchantOrderId"].ToString();
                o2oOrder.tradeTime = dt.Rows[0]["tradeTime"].ToString();
                o2oOrder.waybillno = dt.Rows[0]["waybillno"].ToString();
                o2oOrder.consigneeName = dt.Rows[0]["consigneeName"].ToString();
                o2oOrder.status = dt.Rows[0]["status"].ToString();
                o2oOrder.o2oOrderGoods = new List<O2OOrderGoods>();
                string sql2 = "select * from t_order_goods where  merchantOrderId  = '" + o2oParam.orderId + "'";
                DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "t_daigou_ticket").Tables[0];
                if (dt2.Rows.Count > 0)
                {
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        O2OOrderGoods o2oOrderGoods = new O2OOrderGoods();
                        o2oOrderGoods.id = dt2.Rows[i]["id"].ToString();
                        o2oOrderGoods.barCode = dt2.Rows[i]["barCode"].ToString();
                        o2oOrderGoods.skuUnitPrice = dt2.Rows[i]["skuUnitPrice"].ToString();
                        o2oOrderGoods.skuBillName = dt2.Rows[i]["skuBillName"].ToString();
                        o2oOrderGoods.quantity = dt2.Rows[i]["quantity"].ToString();
                        o2oOrder.o2oOrderGoods.Add(o2oOrderGoods);
                    }
                }
            }
            return o2oOrder;
        }

        public bool UpdateStatus(TicketParam ticketParam)
        {
            string sql = "UPDATE t_daigou_ticket set remark ='"+ ticketParam.remark1 + "', status ='"+ ticketParam.status1 + "'" +
                "  WHERE ticketCode ='" + ticketParam.ticketCode + "'";
            return DatabaseOperationWeb.ExecuteDML(sql);
        }
    }
}
