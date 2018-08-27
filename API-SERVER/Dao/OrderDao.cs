using Aliyun.OSS;
using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Dao
{
    public class OrderDao
    {
        private string path = System.Environment.CurrentDirectory;

        public OrderDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }
        #region 查询

        /// <summary>
        /// 获取订单列表-运营
        /// </summary>
        /// <param name="orderParam">查询信息</param>
        /// <param name="apiType">订单的类别，空白就不区分类别</param>
        /// <param name="ifShowConsignee">是否显示收货人信息</param>
        /// <returns></returns>
        public PageResult getOrderListOfOperator(OrderParam orderParam, string apiType, bool ifShowConsignee)
        {
            PageResult OrderResult = new PageResult();
            OrderResult.pagination = new Page(orderParam.current, orderParam.pageSize);
            OrderResult.list = new List<Object>();
            OrderTotalItem orderTotalItem = new OrderTotalItem();
            string st = "";
            if (apiType != null && apiType != "")
            {
                st += " and t.apitype='" + apiType + "' ";
            }

            if (orderParam.date != null && orderParam.date.Length == 2)
            {

                st += " and t.tradeTime BETWEEN str_to_date('" + orderParam.date[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + orderParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (orderParam.orderId != null && orderParam.orderId.Trim() != "")
            {
                st += " and t.merchantOrderId like '%" + orderParam.orderId.Trim() + "%' ";
            }
            if (orderParam.status != null && orderParam.status.Trim() != "" && orderParam.status.Trim() != "全部")
            {
                st += " and t.status = '" + orderParam.status.Trim() + "' ";
            }
            if (orderParam.wcode != null && orderParam.wcode.Trim() != "")
            {
                st += " and t.warehouseCode = '" + orderParam.wcode.Trim() + "' ";
            }
            if (orderParam.wid != null && orderParam.wid.Trim() != "")
            {
                st += " and t.warehouseId = '" + orderParam.wid.Trim() + "' ";
            }
            if (orderParam.shopId != null && orderParam.shopId.Trim() != "")
            {
                st += " and t.purchaserCode = '" + orderParam.shopId.Trim() + "' ";
            }
            if (orderParam.waybillno != null && orderParam.waybillno.Trim() != "")
            {
                st += " and t.waybillno = '" + orderParam.waybillno.Trim() + "' ";
            }
            if (orderParam.platformId != null && orderParam.platformId.Trim() != "")
            {
                st += " and t.platformId = '" + orderParam.platformId.Trim() + "' ";
            }
            if (orderParam.supplier != null && orderParam.supplier.Trim() != "")
            {
                st += " and t.customerCode in (select usercode from t_user_list ul " +
                    "where ul.email like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.tel like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.usercode like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.username like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.company like '%" + orderParam.supplier.Trim() + "%') ";
            }
            string sql = "SELECT t.id,w.wname,status,(select username from t_user_list where usercode =customerCode) customerCode," +
                         "(select username from t_user_list where usercode =purchaserCode) purchaser,merchantOrderId," +
                         "tradeTime,e.expressName,waybillno,consigneeName,tradeAmount,s.statusName, " +
                         "(select sum(g.quantity) from t_order_goods g where g.merchantOrderId = t.merchantOrderId) sales " +
                         "FROM t_base_status s,t_order_list t left join t_base_express e on t.expressId = e.expressId  " +
                         " LEFT JOIN t_base_warehouse w on w.id= t.warehouseId " +
                         " where s.statusId=t.status " + st +
                         " ORDER BY id desc LIMIT " + (orderParam.current - 1) * orderParam.pageSize + "," + orderParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql1 = "SELECT count(*) FROM t_order_list t " +
                         " where 1=1 " + st;

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                OrderResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
                orderTotalItem.total = Convert.ToInt16(dt1.Rows[0][0]);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.keyId = Convert.ToString((orderParam.current - 1) * orderParam.pageSize + i + 1);
                    orderItem.id = dt.Rows[i]["id"].ToString();
                    orderItem.tradeAmount = dt.Rows[i]["tradeAmount"].ToString();
                    orderItem.merchantOrderId = dt.Rows[i]["merchantOrderId"].ToString();
                    orderItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    orderItem.expressName = dt.Rows[i]["expressName"].ToString();
                    orderItem.warehouseName = dt.Rows[i]["wname"].ToString();
                    orderItem.waybillno = dt.Rows[i]["waybillno"].ToString();
                    orderItem.purchase = dt.Rows[i]["purchaser"].ToString();
                    orderItem.supplier = dt.Rows[i]["customerCode"].ToString();
                    orderItem.status = dt.Rows[i]["statusName"].ToString();
                    orderItem.sales = Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    if (dt.Rows[i]["status"].ToString() == "3")
                    {
                        if (dt.Rows[i]["waybillno"].ToString() == "海外已出库")
                        {
                            orderItem.ifSend = "1";
                        }
                        else
                        {
                            orderItem.ifSend = "0";
                        }
                    }
                    else if (dt.Rows[i]["status"].ToString() == "2")
                    {
                        orderItem.ifSend = "1";
                    }
                    else if (dt.Rows[i]["status"].ToString() == "1")
                    {
                        orderItem.ifSend = "1";
                    }
                    else
                    {
                        orderItem.ifSend = "0";
                    }
                    if (ifShowConsignee)
                    {
                        orderItem.consigneeName = dt.Rows[i]["consigneeName"].ToString();
                    }

                    OrderResult.list.Add(orderItem);
                    orderTotalItem.totalSales += Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    orderTotalItem.totalTradeAmount += Convert.ToDouble(dt.Rows[i]["tradeAmount"].ToString());
                }
                orderTotalItem.totalSales = Math.Round(orderTotalItem.totalSales, 2);
                orderTotalItem.totalTradeAmount = Math.Round(orderTotalItem.totalTradeAmount, 2);

            }
            OrderResult.item = orderTotalItem;
            return OrderResult;
        }

        /// <summary>
        /// 获取订单列表-供应商
        /// </summary>
        /// <param name="orderParam">查询信息</param>
        /// <param name="apiType">订单的类别，空白就不区分类别</param>
        /// <param name="ifShowConsignee">是否显示收货人信息</param>
        /// <returns></returns>
        public PageResult getOrderListOfSupplier(OrderParam orderParam, string apiType, bool ifShowConsignee)
        {
            PageResult OrderResult = new PageResult();
            OrderResult.pagination = new Page(orderParam.current, orderParam.pageSize);
            OrderResult.list = new List<Object>();
            OrderTotalItem orderTotalItem = new OrderTotalItem();
            string st = " and t.customerCode='" + orderParam.userId + "' ";
            if (apiType != null && apiType != "")
            {
                st += " and t.apitype='" + apiType + "' ";
            }

            if (orderParam.date != null && orderParam.date.Length == 2)
            {

                st += " and t.tradeTime BETWEEN str_to_date('" + orderParam.date[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + orderParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (orderParam.orderId != null && orderParam.orderId.Trim() != "")
            {
                st += " and t.merchantOrderId like '%" + orderParam.orderId.Trim() + "%' ";
            }
            if (orderParam.status != null && orderParam.status.Trim() != "" && orderParam.status.Trim() != "全部")
            {
                st += " and t.status = '" + orderParam.status.Trim() + "' ";
            }
            if (orderParam.wcode != null && orderParam.wcode.Trim() != "")
            {
                st += " and t.warehouseCode = '" + orderParam.wcode.Trim() + "' ";
            }
            if (orderParam.wid != null && orderParam.wid.Trim() != "")
            {
                st += " and t.warehouseId = '" + orderParam.wid.Trim() + "' ";
            }
            if (orderParam.shopId != null && orderParam.shopId.Trim() != "")
            {
                st += " and t.purchaserCode = '" + orderParam.shopId.Trim() + "' ";
            }
            if (orderParam.waybillno != null && orderParam.waybillno.Trim() != "")
            {
                st += " and t.waybillno = '" + orderParam.waybillno.Trim() + "' ";
            }
            if (orderParam.platformId != null && orderParam.platformId.Trim() != "")
            {
                st += " and t.platformId = '" + orderParam.platformId.Trim() + "' ";
            }
            if (orderParam.supplier != null && orderParam.supplier.Trim() != "")
            {
                st += " and t.customerCode in (select usercode from t_user_list ul " +
                    "where ul.email like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.tel like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.usercode like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.username like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.company like '%" + orderParam.supplier.Trim() + "%') ";
            }
            string sql = "SELECT t.id,w.wname,status,(select username from t_user_list where usercode =customerCode) customerCode," +
                         "(select username from t_user_list where usercode =purchaserCode) purchaser,merchantOrderId," +
                         "tradeTime,e.expressName,waybillno,consigneeName,tradeAmount,s.statusName," +
                         "(select sum(g.quantity) from t_order_goods g where g.merchantOrderId = t.merchantOrderId) sales " +
                         "FROM t_base_status s,t_order_list t left join t_base_express e on t.expressId = e.expressId  " +
                         " LEFT JOIN t_base_warehouse w on w.id= t.warehouseId " +
                         " where s.statusId=t.status " + st +
                         " ORDER BY id desc LIMIT " + (orderParam.current - 1) * orderParam.pageSize + "," + orderParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql1 = "SELECT count(*) FROM t_order_list t " +
                         " where 1=1 " + st;

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                OrderResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
                orderTotalItem.total = Convert.ToInt16(dt1.Rows[0][0]);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.keyId = Convert.ToString((orderParam.current - 1) * orderParam.pageSize + i + 1);
                    orderItem.id = dt.Rows[i]["id"].ToString();
                    orderItem.tradeAmount = dt.Rows[i]["tradeAmount"].ToString();
                    orderItem.merchantOrderId = dt.Rows[i]["merchantOrderId"].ToString();
                    orderItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    orderItem.expressName = dt.Rows[i]["expressName"].ToString();
                    orderItem.warehouseName = dt.Rows[i]["wname"].ToString();
                    orderItem.waybillno = dt.Rows[i]["waybillno"].ToString();
                    orderItem.status = dt.Rows[i]["statusName"].ToString();
                    orderItem.sales = Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    if (dt.Rows[i]["status"].ToString() == "3")
                    {
                        if (dt.Rows[i]["waybillno"].ToString() == "海外已出库")
                        {
                            orderItem.ifSend = "1";
                        }
                        else
                        {
                            orderItem.ifSend = "0";
                        }
                    }
                    else if (dt.Rows[i]["status"].ToString() == "2")
                    {
                        orderItem.ifSend = "1";
                    }
                    else if (dt.Rows[i]["status"].ToString() == "1")
                    {
                        orderItem.ifSend = "1";
                    }
                    else
                    {
                        orderItem.ifSend = "0";
                    }
                    if (ifShowConsignee)
                    {
                        orderItem.consigneeName = dt.Rows[i]["consigneeName"].ToString();
                    }

                    OrderResult.list.Add(orderItem);

                    orderTotalItem.totalSales += Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    orderTotalItem.totalTradeAmount += Convert.ToDouble(dt.Rows[i]["tradeAmount"].ToString());
                }
                orderTotalItem.totalSales = Math.Round(orderTotalItem.totalSales, 2);
                orderTotalItem.totalTradeAmount = Math.Round(orderTotalItem.totalTradeAmount, 2);

            }
            OrderResult.item = orderTotalItem;
            return OrderResult;
        }


        /// <summary>
        /// 获取订单列表-采购商
        /// </summary>
        /// <param name="orderParam">查询信息</param>
        /// <param name="apiType">订单的类别，空白就不区分类别</param>
        /// <param name="ifShowConsignee">是否显示收货人信息</param>
        /// <returns></returns>
        public PageResult getOrderListOfPurchasers(OrderParam orderParam, string apiType, bool ifShowConsignee)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(orderParam.current, orderParam.pageSize);
            pageResult.list = new List<Object>();
            OrderTotalItem orderTotalItem = new OrderTotalItem();
            string st = " and t.purchaserCode='" + orderParam.userId + "' ";
            if (apiType != null && apiType != "")
            {
                st += " and t.apitype='" + apiType + "' ";
            }

            if (orderParam.date != null && orderParam.date.Length == 2)
            {

                st += " and t.tradeTime BETWEEN str_to_date('" + orderParam.date[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + orderParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (orderParam.orderId != null && orderParam.orderId.Trim() != "")
            {
                st += " and t.merchantOrderId like '%" + orderParam.orderId.Trim() + "%' ";
            }
            if (orderParam.status != null && orderParam.status.Trim() != "" && orderParam.status.Trim() != "全部")
            {
                st += " and t.status = '" + orderParam.status.Trim() + "' ";
            }
            if (orderParam.wcode != null && orderParam.wcode.Trim() != "")
            {
                st += " and t.warehouseCode = '" + orderParam.wcode.Trim() + "' ";
            }
            if (orderParam.wid != null && orderParam.wid.Trim() != "")
            {
                st += " and t.warehouseId = '" + orderParam.wid.Trim() + "' ";
            }
            if (orderParam.shopId != null && orderParam.shopId.Trim() != "")
            {
                st += " and t.purchaserCode = '" + orderParam.shopId.Trim() + "' ";
            }
            if (orderParam.waybillno != null && orderParam.waybillno.Trim() != "")
            {
                st += " and t.waybillno = '" + orderParam.waybillno.Trim() + "' ";
            }
            if (orderParam.platformId != null && orderParam.platformId.Trim() != "")
            {
                st += " and t.platformId = '" + orderParam.platformId.Trim() + "' ";
            }
            if (orderParam.supplier != null && orderParam.supplier.Trim() != "")
            {
                st += " and t.customerCode in (select usercode from t_user_list ul " +
                    "where ul.email like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.tel like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.usercode like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.username like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.company like '%" + orderParam.supplier.Trim() + "%') ";
            }
            string sql = "select t.id,t.tradeTime,t.merchantOrderId,t.tradeAmount,sum(g.quantity) sales,t.waybillno,s.statusName," +
                         "sum((g.skuUnitPrice-g.purchasePrice)*g.quantity) purchase,sum(g.profitAgent) agent ," +
                         "sum(g.profitDealer) dealer,t.distributionCode ,e.expressName,t.status " +
                         "from t_order_goods g,t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
                         "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
                         " group by t.merchantOrderId " +
                         " ORDER BY t.distributionCode,t.id desc LIMIT " + (orderParam.current - 1) * orderParam.pageSize + "," + orderParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql1 = "SELECT count(*) FROM t_order_list t " +
                         " where 1=1 " + st;

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
                orderTotalItem.total = Convert.ToInt16(dt1.Rows[0][0]);
                //string distribution = dt.Rows[0]["distributionCode"].ToString();
                //orderTotalItem.totalDistribution = 1;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.keyId = Convert.ToString((orderParam.current - 1) * orderParam.pageSize + i + 1);
                    orderItem.id = dt.Rows[i]["id"].ToString();
                    orderItem.tradeAmount = dt.Rows[i]["tradeAmount"].ToString();
                    orderItem.merchantOrderId = dt.Rows[i]["merchantOrderId"].ToString();
                    orderItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    orderItem.expressName = dt.Rows[i]["expressName"].ToString();
                    orderItem.waybillno = dt.Rows[i]["waybillno"].ToString();
                    orderItem.status = dt.Rows[i]["statusName"].ToString();
                    orderItem.sales = Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    orderItem.purchaseTotal = Convert.ToDouble(dt.Rows[i]["purchase"].ToString());
                    //orderItem.agentTotal = Convert.ToDouble(dt.Rows[i]["agent"].ToString());
                    //orderItem.dealerTotal = Convert.ToDouble(dt.Rows[i]["dealer"].ToString());
                    //orderItem.distribution = dt.Rows[i]["distributionCode"].ToString();

                    if (dt.Rows[i]["status"].ToString() == "3")
                    {
                        if (dt.Rows[i]["waybillno"].ToString() == "海外已出库")
                        {
                            orderItem.ifSend = "1";
                        }
                        else
                        {
                            orderItem.ifSend = "0";
                        }
                    }
                    else if (dt.Rows[i]["status"].ToString() == "2")
                    {
                        orderItem.ifSend = "1";
                    }
                    else if (dt.Rows[i]["status"].ToString() == "1")
                    {
                        orderItem.ifSend = "1";
                    }
                    else
                    {
                        orderItem.ifSend = "0";
                    }
                    if (ifShowConsignee)
                    {
                        orderItem.consigneeName = dt.Rows[i]["consigneeName"].ToString();
                    }
                    orderTotalItem.totalSales += Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    orderTotalItem.totalTradeAmount += Convert.ToDouble(dt.Rows[i]["tradeAmount"].ToString());
                    orderTotalItem.totalPurchase += Convert.ToDouble(dt.Rows[i]["purchase"].ToString());
                    //orderTotalItem.totalAgent += Convert.ToDouble(dt.Rows[i]["agent"].ToString());
                    //orderTotalItem.totalDealer += Convert.ToDouble(dt.Rows[i]["dealer"].ToString());
                    //if (dt.Rows[i]["distributionCode"].ToString() != distribution)
                    //{
                    //    orderTotalItem.totalDistribution += 1;
                    //    distribution = dt.Rows[i]["distributionCode"].ToString();
                    //}

                    pageResult.list.Add(orderItem);
                }
                orderTotalItem.totalSales = Math.Round(orderTotalItem.totalSales, 2);
                orderTotalItem.totalTradeAmount = Math.Round(orderTotalItem.totalTradeAmount, 2);
                orderTotalItem.totalPurchase = Math.Round(orderTotalItem.totalPurchase, 2);
                //orderTotalItem.totalAgent = Math.Round(orderTotalItem.totalAgent, 2);
                //orderTotalItem.totalDealer = Math.Round(orderTotalItem.totalDealer, 2);

            }
            pageResult.item = orderTotalItem;
            return pageResult;
        }

        /// <summary>
        /// 获取订单列表-代理
        /// </summary>
        /// <param name="orderParam">查询信息</param>
        /// <param name="apiType">订单的类别，空白就不区分类别</param>
        /// <param name="ifShowConsignee">是否显示收货人信息</param>
        /// <returns></returns>
        public PageResult getOrderListOfAgent(OrderParam orderParam, string apiType, bool ifShowConsignee)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(orderParam.current, orderParam.pageSize);
            pageResult.list = new List<Object>();
            OrderTotalItem orderTotalItem = new OrderTotalItem();
            string st = " and t.purchaserCode='" + orderParam.userId + "' ";
            if (apiType != null && apiType != "")
            {
                st += " and t.apitype='" + apiType + "' ";
            }

            if (orderParam.date != null && orderParam.date.Length == 2)
            {

                st += " and t.tradeTime BETWEEN str_to_date('" + orderParam.date[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + orderParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (orderParam.orderId != null && orderParam.orderId.Trim() != "")
            {
                st += " and t.merchantOrderId like '%" + orderParam.orderId.Trim() + "%' ";
            }
            if (orderParam.status != null && orderParam.status.Trim() != "" && orderParam.status.Trim() != "全部")
            {
                st += " and t.status = '" + orderParam.status.Trim() + "' ";
            }
            if (orderParam.wcode != null && orderParam.wcode.Trim() != "")
            {
                st += " and t.warehouseCode = '" + orderParam.wcode.Trim() + "' ";
            }
            if (orderParam.wid != null && orderParam.wid.Trim() != "")
            {
                st += " and t.warehouseId = '" + orderParam.wid.Trim() + "' ";
            }
            if (orderParam.shopId != null && orderParam.shopId.Trim() != "")
            {
                st += " and t.purchaserCode = '" + orderParam.shopId.Trim() + "' ";
            }
            if (orderParam.waybillno != null && orderParam.waybillno.Trim() != "")
            {
                st += " and t.waybillno = '" + orderParam.waybillno.Trim() + "' ";
            }
            if (orderParam.platformId != null && orderParam.platformId.Trim() != "")
            {
                st += " and t.platformId = '" + orderParam.platformId.Trim() + "' ";
            }
            if (orderParam.supplier != null && orderParam.supplier.Trim() != "")
            {
                st += " and t.customerCode in (select usercode from t_user_list ul " +
                    "where ul.email like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.tel like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.usercode like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.username like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.company like '%" + orderParam.supplier.Trim() + "%') ";
            }
            string sql = "select t.id,t.tradeTime,t.merchantOrderId,t.tradeAmount,sum(g.quantity) sales,t.waybillno,s.statusName," +
                         "sum((g.skuUnitPrice-g.purchasePrice)*g.quantity) purchase,sum(g.profitAgent) agent ," +
                         "sum(g.profitDealer) dealer,t.distributionCode ,e.expressName,t.status " +
                         "from t_order_goods g,t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
                         "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
                         " group by t.merchantOrderId " +
                         " ORDER BY t.distributionCode,t.id desc LIMIT " + (orderParam.current - 1) * orderParam.pageSize + "," + orderParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql1 = "SELECT count(*) FROM t_order_list t " +
                         " where 1=1 " + st;

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
                orderTotalItem.total = Convert.ToInt16(dt1.Rows[0][0]);
                string distribution = dt.Rows[0]["distributionCode"].ToString();
                orderTotalItem.totalDistribution = 1;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.keyId = Convert.ToString((orderParam.current - 1) * orderParam.pageSize + i + 1);
                    orderItem.id = dt.Rows[i]["id"].ToString();
                    orderItem.tradeAmount = dt.Rows[i]["tradeAmount"].ToString();
                    orderItem.merchantOrderId = dt.Rows[i]["merchantOrderId"].ToString();
                    orderItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    orderItem.expressName = dt.Rows[i]["expressName"].ToString();
                    orderItem.waybillno = dt.Rows[i]["waybillno"].ToString();
                    orderItem.status = dt.Rows[i]["statusName"].ToString();
                    orderItem.sales = Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    //orderItem.purchaseTotal = Convert.ToDouble(dt.Rows[i]["purchase"].ToString());
                    orderItem.agentTotal = Convert.ToDouble(dt.Rows[i]["agent"].ToString());
                    orderItem.dealerTotal = Convert.ToDouble(dt.Rows[i]["dealer"].ToString());
                    orderItem.distribution = dt.Rows[i]["distributionCode"].ToString();

                    if (dt.Rows[i]["status"].ToString() == "3")
                    {
                        if (dt.Rows[i]["waybillno"].ToString() == "海外已出库")
                        {
                            orderItem.ifSend = "1";
                        }
                        else
                        {
                            orderItem.ifSend = "0";
                        }
                    }
                    else if (dt.Rows[i]["status"].ToString() == "2")
                    {
                        orderItem.ifSend = "1";
                    }
                    else if (dt.Rows[i]["status"].ToString() == "1")
                    {
                        orderItem.ifSend = "1";
                    }
                    else
                    {
                        orderItem.ifSend = "0";
                    }
                    if (ifShowConsignee)
                    {
                        orderItem.consigneeName = dt.Rows[i]["consigneeName"].ToString();
                    }
                    orderTotalItem.totalSales += Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    orderTotalItem.totalTradeAmount += Convert.ToDouble(dt.Rows[i]["tradeAmount"].ToString());
                    //orderTotalItem.totalPurchase += Convert.ToDouble(dt.Rows[i]["purchase"].ToString());
                    orderTotalItem.totalAgent += Convert.ToDouble(dt.Rows[i]["agent"].ToString());
                    orderTotalItem.totalDealer += Convert.ToDouble(dt.Rows[i]["dealer"].ToString());
                    if (dt.Rows[i]["distributionCode"].ToString() != distribution)
                    {
                        orderTotalItem.totalDistribution += 1;
                        distribution = dt.Rows[i]["distributionCode"].ToString();
                    }

                    pageResult.list.Add(orderItem);
                }
                orderTotalItem.totalSales = Math.Round(orderTotalItem.totalSales, 2);
                orderTotalItem.totalTradeAmount = Math.Round(orderTotalItem.totalTradeAmount, 2);
                //orderTotalItem.totalPurchase = Math.Round(orderTotalItem.totalPurchase, 2);
                orderTotalItem.totalAgent = Math.Round(orderTotalItem.totalAgent, 2);
                orderTotalItem.totalDealer = Math.Round(orderTotalItem.totalDealer, 2);

            }
            pageResult.item = orderTotalItem;
            return pageResult;
        }

        /// <summary>
        /// 获取订单列表-分销
        /// </summary>
        /// <param name="orderParam">查询信息</param>
        /// <param name="apiType">订单的类别，空白就不区分类别</param>
        /// <param name="ifShowConsignee">是否显示收货人信息</param>
        /// <returns></returns>
        public PageResult getOrderListOfDealer(OrderParam orderParam, string apiType, bool ifShowConsignee)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(orderParam.current, orderParam.pageSize);
            pageResult.list = new List<Object>();
            OrderTotalItem orderTotalItem = new OrderTotalItem();
            string st = " and t.distributionCode='" + orderParam.userId + "' ";
            if (apiType != null && apiType != "")
            {
                st += " and t.apitype='" + apiType + "' ";
            }

            if (orderParam.date != null && orderParam.date.Length == 2)
            {

                st += " and t.tradeTime BETWEEN str_to_date('" + orderParam.date[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + orderParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (orderParam.orderId != null && orderParam.orderId.Trim() != "")
            {
                st += " and t.merchantOrderId like '%" + orderParam.orderId.Trim() + "%' ";
            }
            if (orderParam.status != null && orderParam.status.Trim() != "" && orderParam.status.Trim() != "全部")
            {
                st += " and t.status = '" + orderParam.status.Trim() + "' ";
            }
            if (orderParam.wcode != null && orderParam.wcode.Trim() != "")
            {
                st += " and t.warehouseCode = '" + orderParam.wcode.Trim() + "' ";
            }
            if (orderParam.wid != null && orderParam.wid.Trim() != "")
            {
                st += " and t.warehouseId = '" + orderParam.wid.Trim() + "' ";
            }
            if (orderParam.shopId != null && orderParam.shopId.Trim() != "")
            {
                st += " and t.purchaserCode = '" + orderParam.shopId.Trim() + "' ";
            }
            if (orderParam.waybillno != null && orderParam.waybillno.Trim() != "")
            {
                st += " and t.waybillno = '" + orderParam.waybillno.Trim() + "' ";
            }
            if (orderParam.platformId != null && orderParam.platformId.Trim() != "")
            {
                st += " and t.platformId = '" + orderParam.platformId.Trim() + "' ";
            }
            if (orderParam.supplier != null && orderParam.supplier.Trim() != "")
            {
                st += " and t.customerCode in (select usercode from t_user_list ul " +
                    "where ul.email like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.tel like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.usercode like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.username like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.company like '%" + orderParam.supplier.Trim() + "%') ";
            }
            string sql = "select t.id,t.tradeTime,t.merchantOrderId,t.tradeAmount,sum(g.quantity) sales,t.waybillno,s.statusName," +
                         "sum((g.skuUnitPrice-g.purchasePrice)*g.quantity) purchase,sum(g.profitAgent) agent ," +
                         "sum(g.profitDealer) dealer,t.distributionCode ,e.expressName,t.status " +
                         "from t_order_goods g,t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
                         "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
                         " group by t.merchantOrderId " +
                         " ORDER BY t.distributionCode,t.id desc LIMIT " + (orderParam.current - 1) * orderParam.pageSize + "," + orderParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql1 = "SELECT count(*) FROM t_order_list t " +
                         " where 1=1 " + st;

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
                orderTotalItem.total = Convert.ToInt16(dt1.Rows[0][0]);
                //string distribution = dt.Rows[0]["distributionCode"].ToString();
                //orderTotalItem.totalDistribution = 1;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.keyId = Convert.ToString((orderParam.current - 1) * orderParam.pageSize + i + 1);
                    orderItem.id = dt.Rows[i]["id"].ToString();
                    orderItem.tradeAmount = dt.Rows[i]["tradeAmount"].ToString();
                    orderItem.merchantOrderId = dt.Rows[i]["merchantOrderId"].ToString();
                    orderItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    orderItem.expressName = dt.Rows[i]["expressName"].ToString();
                    orderItem.waybillno = dt.Rows[i]["waybillno"].ToString();
                    orderItem.status = dt.Rows[i]["statusName"].ToString();
                    orderItem.sales = Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    //orderItem.purchaseTotal = Convert.ToDouble(dt.Rows[i]["purchase"].ToString());
                    //orderItem.agentTotal = Convert.ToDouble(dt.Rows[i]["agent"].ToString());
                    orderItem.dealerTotal = Convert.ToDouble(dt.Rows[i]["dealer"].ToString());
                    //orderItem.distribution = dt.Rows[i]["distributionCode"].ToString();

                    if (dt.Rows[i]["status"].ToString() == "3")
                    {
                        if (dt.Rows[i]["waybillno"].ToString() == "海外已出库")
                        {
                            orderItem.ifSend = "1";
                        }
                        else
                        {
                            orderItem.ifSend = "0";
                        }
                    }
                    else if (dt.Rows[i]["status"].ToString() == "2")
                    {
                        orderItem.ifSend = "1";
                    }
                    else if (dt.Rows[i]["status"].ToString() == "1")
                    {
                        orderItem.ifSend = "1";
                    }
                    else
                    {
                        orderItem.ifSend = "0";
                    }
                    if (ifShowConsignee)
                    {
                        orderItem.consigneeName = dt.Rows[i]["consigneeName"].ToString();
                    }
                    orderTotalItem.totalSales += Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    orderTotalItem.totalTradeAmount += Convert.ToDouble(dt.Rows[i]["tradeAmount"].ToString());
                    //orderTotalItem.totalPurchase += Convert.ToDouble(dt.Rows[i]["purchase"].ToString());
                    //orderTotalItem.totalAgent += Convert.ToDouble(dt.Rows[i]["agent"].ToString());
                    orderTotalItem.totalDealer += Convert.ToDouble(dt.Rows[i]["dealer"].ToString());
                    //if (dt.Rows[i]["distributionCode"].ToString() != distribution)
                    //{
                    //    orderTotalItem.totalDistribution += 1;
                    //    distribution = dt.Rows[i]["distributionCode"].ToString();
                    //}

                    pageResult.list.Add(orderItem);
                }
                orderTotalItem.totalSales = Math.Round(orderTotalItem.totalSales, 2);
                orderTotalItem.totalTradeAmount = Math.Round(orderTotalItem.totalTradeAmount, 2);
                //orderTotalItem.totalPurchase = Math.Round(orderTotalItem.totalPurchase, 2);
                //orderTotalItem.totalAgent = Math.Round(orderTotalItem.totalAgent, 2);
                orderTotalItem.totalDealer = Math.Round(orderTotalItem.totalDealer, 2);

            }
            pageResult.item = orderTotalItem;
            return pageResult;
        }



        /// <summary>
        /// 查询单个订单的详情
        /// </summary>
        /// <param name="orderParam">查询信息</param>
        /// <param name="ifShowConsignee">是否显示收货人信息</param>
        /// <returns></returns>
        public OrderItem getOrderItem(OrderParam orderParam, bool ifShowConsignee)
        {
            OrderItem orderItem = new OrderItem();
            string sql1 = "select id,merchantOrderId,tradeAmount,tradeTime,waybillno,idNumber," +
                "consigneeName,consigneeMobile,addrProvince,addrCity,addrDistrict,addrDetail  " +
                "FROM t_order_list where merchantOrderId  = '" + orderParam.orderId + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_daigou_ticket").Tables[0];
            if (dt.Rows.Count > 0)
            {
                orderItem.id = dt.Rows[0]["id"].ToString();
                orderItem.merchantOrderId = dt.Rows[0]["merchantOrderId"].ToString();
                orderItem.tradeAmount = dt.Rows[0]["tradeAmount"].ToString();
                orderItem.tradeTime = dt.Rows[0]["tradeTime"].ToString();
                orderItem.waybillno = dt.Rows[0]["waybillno"].ToString();
                // orderItem.status = dt.Rows[0]["status"].ToString();

                if (ifShowConsignee)
                {
                    orderItem.idNumber = dt.Rows[0]["idNumber"].ToString();
                    orderItem.consigneeName = dt.Rows[0]["consigneeName"].ToString();
                    orderItem.consigneeMobile = dt.Rows[0]["consigneeMobile"].ToString();
                    orderItem.addrProvince = dt.Rows[0]["addrProvince"].ToString();
                    orderItem.addrCity = dt.Rows[0]["addrCity"].ToString();
                    orderItem.addrDistrict = dt.Rows[0]["addrDistrict"].ToString();
                    orderItem.addrDetail = dt.Rows[0]["addrDetail"].ToString();
                }
                orderItem.sales = 0;
                orderItem.purchaseTotal = 0;
                orderItem.agentTotal = 0;
                orderItem.dealerTotal = 0;
                orderItem.OrderGoods = new List<OrderGoodsItem>();
                string sql2 = "select id,slt,barCode,IFNULL(skuUnitPrice,0) as skuUnitPrice,skuBillName,IFNULL(quantity,0) as quantity," +
                    "IFNULL(purchasePrice,0) as purchasePrice,IFNULL(profitAgent,0) as profitAgent,IFNULL(profitDealer,0) as profitDealer," +
                    "(IFNULL(skuUnitPrice,0)-IFNULL(purchasePrice,0))* IFNULL(quantity,0) as purchaseP " +
                    "from t_order_goods where  merchantOrderId  = '" + orderParam.orderId + "'";
                DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "t_daigou_ticket").Tables[0];
                if (dt2.Rows.Count > 0)
                {
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        OrderGoodsItem orderGoods = new OrderGoodsItem();
                        try
                        {
                            orderGoods.totalPrice = Convert.ToDouble(dt2.Rows[i]["skuUnitPrice"]) * Convert.ToDouble(dt2.Rows[i]["quantity"]);
                        }
                        catch (Exception)
                        {
                        }
                        orderGoods.id = dt2.Rows[i]["id"].ToString();
                        orderGoods.slt = dt2.Rows[i]["slt"].ToString();
                        orderGoods.barCode = dt2.Rows[i]["barCode"].ToString();
                        orderGoods.skuUnitPrice = Convert.ToDouble(dt2.Rows[i]["skuUnitPrice"]);
                        orderGoods.skuBillName = dt2.Rows[i]["skuBillName"].ToString();
                        orderGoods.quantity = Convert.ToDouble(dt2.Rows[i]["quantity"]);
                        orderGoods.purchasePrice = Convert.ToDouble(dt2.Rows[i]["purchasePrice"]);
                        orderGoods.purchaseP = Convert.ToDouble(dt2.Rows[i]["purchaseP"]);
                        orderGoods.profitAgent = Convert.ToDouble(dt2.Rows[i]["profitAgent"]);
                        orderGoods.profitDealer = Convert.ToDouble(dt2.Rows[i]["profitDealer"]);
                        orderItem.OrderGoods.Add(orderGoods);
                        orderItem.sales += Convert.ToDouble(dt2.Rows[i]["quantity"]);
                        orderItem.purchaseTotal += Convert.ToDouble(dt2.Rows[i]["purchaseP"]);
                        orderItem.agentTotal += Convert.ToDouble(dt2.Rows[i]["profitAgent"]);
                        orderItem.dealerTotal += Convert.ToDouble(dt2.Rows[i]["profitDealer"]);
                    }
                }
            }
            return orderItem;
        }

        public List<ExpressItem> getExpress()
        {
            List<ExpressItem> le = new List<ExpressItem>();
            string sql = "select * from t_base_express order by expressId asc";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ExpressItem expressItem = new ExpressItem();
                expressItem.expressId = dt.Rows[i]["expressId"].ToString();
                expressItem.expressName = dt.Rows[i]["expressName"].ToString();
                le.Add(expressItem);
            }
            return le;
        }

        public MsgResult singleWaybill(SingleWaybillParam singleWaybillParam)
        {
            MsgResult msg = new MsgResult();
            string st = "";
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(singleWaybillParam.userId);
            if (userType == "1")//供应商 
            {
                st = " and customerCode = '" + singleWaybillParam.userId + "' ";
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {

            }
            else
            {
                msg.msg = "用户权限错误";
                return msg;
            }
            string sql = "select status from t_order_list where merchantOrderId='" + singleWaybillParam.orderId + "' " + st;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0][0].ToString() == "1" || dt.Rows[0][0].ToString() == "2" || dt.Rows[0][0].ToString() == "3")
                {
                    string upsql = "update t_order_list set  status=3,expressId = '" + singleWaybillParam.expressId + "'," +
                           "waybillno= '" + singleWaybillParam.waybillno + "',waybilltime=now() " +
                           "where merchantOrderId='" + singleWaybillParam.orderId + "' " + st;
                    if (DatabaseOperationWeb.ExecuteDML(upsql))
                    {
                        msg.msg = "保存成功";
                        msg.type = "1";
                    }
                    else
                    {
                        msg.msg = "数据保存失败！";
                    }
                }
                else
                {
                    msg.msg = "订单状态不正确！";
                }
            }
            else
            {
                msg.msg = "没找到对应订单！";
            }

            return msg;
        }

        public MsgResult Overseas(SingleWaybillParam singleWaybillParam)
        {
            MsgResult msg = new MsgResult();
            string st = "";
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(singleWaybillParam.userId);
            if (userType == "1")//供应商 
            {
                st = " and customerCode = '" + singleWaybillParam.userId + "' ";
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {

            }
            else
            {
                msg.msg = "用户权限错误";
                return msg;
            }
            string sql = "select status from t_order_list where merchantOrderId='" + singleWaybillParam.orderId + "' " + st;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0][0].ToString() == "1" || dt.Rows[0][0].ToString() == "2" || dt.Rows[0][0].ToString() == "3")
                {
                    string upsql = "update t_order_list set status=3, waybillno= '海外已出库',waybilltime=now() " +
                           "where merchantOrderId='" + singleWaybillParam.orderId + "' " + st;
                    if (DatabaseOperationWeb.ExecuteDML(upsql))
                    {
                        msg.msg = "保存成功";
                        msg.type = "1";
                    }
                    else
                    {
                        msg.msg = "数据保存失败！";
                    }
                }
                else
                {
                    msg.msg = "订单状态不正确！";
                }
            }
            else
            {
                msg.msg = "没找到对应订单！";
            }
            return msg;
        }
        #endregion

        #region 上传、导出
        /// <summary>
        /// 导出订单
        /// </summary>
        /// <param name="orderParam"></param>
        /// <returns></returns>
        public MsgResult exportOrder(OrderParam orderParam)
        {
            MsgResult msg = new MsgResult();
            string fileName = "export_Order_" + orderParam.userId + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx";
            string st = "";
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(orderParam.userId);
            if (userType == "1")//供应商 
            {
                st = " and customerCode = '" + orderParam.userId + "' ";
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {

            }
            else
            {
                msg.msg = "用户权限错误";
                return msg;
            }
            string sql = "select t.consigneeName as '收货人',t.consigneeMobile as '收货人电话',t.idNumber as '身份证号', " +
                         "concat_ws('',t.addrCountry,t.addrProvince,t.addrCity,t.addrDistrict,t.addrDetail) as '地址'," +
                         "t.merchantOrderId as '订单号', g.barCode as '商品条码',g.goodsName as '商品名',g.quantity as '数量'," +
                         "g.supplyPrice as '供货价' " +
                         "from t_order_list t ,t_order_goods g " +
                         "where t.merchantOrderId = g.merchantOrderId and t.warehouseId ='" + orderParam.wid + "' " +
                         "and (t.status = 1 or t.status= 2 or (t.status= 3 and waybillno= '海外已出库' )) " + st;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_ticket").Tables[0];
            if (dt.Rows.Count > 0)
            {
                FileManager fm = new FileManager();
                if (fm.writeDataTableToExcel(dt, fileName))
                {
                    if (fm.updateFileToOSS(fileName, Global.OssDirOrder, fileName))
                    {
                        msg.type = "1";
                        msg.msg = Global.OssUrl + Global.OssDirOrder + fileName;
                    }
                    else
                    {
                        msg.msg = "生成失败！！";
                    }
                }
                else
                {
                    msg.msg = "生成失败！";
                }
            }
            else
            {
                msg.msg = "没有需要导出的订单！";
            }
            return msg;
        }
        /// <summary>
        /// 导出运单
        /// </summary>
        /// <param name="orderParam"></param>
        /// <returns></returns>
        public MsgResult exportWaybill(OrderParam orderParam)
        {
            MsgResult msg = new MsgResult();
            string fileName = "export_Waybill_" + orderParam.userId + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx";
            string st = "";
            //处理用户账号对应的查询条件
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(orderParam.userId);
            if (userType == "1")//供应商 
            {
                st += " and customerCode='" + orderParam.userId + "' ";
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {

            }
            else if (userType == "2"|| userType == "3")//采购和代理
            {
                st += " and purchaserCode='" + orderParam.userId + "' ";
            }
            else if (userType == "4")//分销商
            {
                st += " and distributionCode='" + orderParam.userId + "' ";
            }

            if (orderParam.date != null && orderParam.date.Length == 2)
            {
                st += " and tradeTime BETWEEN str_to_date('" + orderParam.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + orderParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (orderParam.orderId != null && orderParam.orderId != "")
            {
                st += " and merchantOrderId like '%" + orderParam.orderId + "%' ";
            }
            if (orderParam.status != null && orderParam.status != "" && orderParam.status != "全部")
            {
                st += " and status = '" + orderParam.status + "' ";
            }
            if (orderParam.wcode != null && orderParam.wcode != "")
            {
                st += " and warehouseCode = '" + orderParam.wcode + "' ";
            }
            if (orderParam.wid != null && orderParam.wid != "")
            {
                st += " and warehouseId = '" + orderParam.wid + "' ";
            }
            if (orderParam.shopId != null && orderParam.shopId != "")
            {
                st += " and purchaserCode = '" + orderParam.shopId + "' ";
            }
            string sql = "SELECT merchantOrderId as '订单号' ,e.expressName as '快递公司',waybillno as '运单号' " +
                         "FROM t_order_list t left join t_base_express e on t.expressId = e.expressId where 1=1 " + st;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                FileManager fm = new FileManager();
                if (fm.writeDataTableToExcel(dt, fileName))
                {
                    if (fm.updateFileToOSS(fileName, Global.OssDirOrder, fileName))
                    {
                        msg.type = "1";
                        msg.msg = Global.OssUrl + Global.OssDirOrder + fileName;
                    }
                    else
                    {
                        msg.msg = "生成失败！！";
                    }
                }
                else
                {
                    msg.msg = "生成失败！";
                }
            }
            else
            {
                msg.msg = "没有需要导出的运单！";
            }
            return msg;
        }
        /// <summary>
        /// 上传运单
        /// </summary>
        /// <param name="uploadParam"></param>
        /// <returns></returns>
        public MsgResult UploadWaybill(FileUploadParam uploadParam)
        {
            MsgResult msg = new MsgResult();
            string logCode = uploadParam.userId + DateTime.Now.ToString("yyyyMMddHHmmssff");
            string fileName = logCode + ".xlsx";
            FileManager fm = new FileManager();
            if (fm.fileCopy(uploadParam.fileTemp, fileName))
            {
                DataTable dt = fm.readExcelFileToDataTable(fileName);

                if (dt == null)
                {
                    msg.msg = "导入文档错误，请确认excel里的列是否正确，是否有相同名称的列。";
                    return msg;
                }
                if (dt.Rows.Count > 0)
                {
                    #region 校验导入文档的列
                    if (!dt.Columns.Contains("订单号"))
                    {
                        msg.msg = "缺少“订单号”列，";
                    }
                    if (!dt.Columns.Contains("快递公司"))
                    {
                        msg.msg += "缺少“快递公司”列，";
                    }
                    if (!dt.Columns.Contains("运单号"))
                    {
                        msg.msg += "缺少“运单号”列，";
                    }
                    if (msg.msg != null && msg.msg != "")
                    {
                        return msg;
                    }
                    #endregion
                    string st = "";
                    UserDao userDao = new UserDao();
                    string userType = userDao.getUserType(uploadParam.userId);
                    if (userType == "1")//供应商 
                    {
                        st = " and customerCode = '" + uploadParam.userId + "' ";
                    }
                    else if (userType == "0" || userType == "5")//管理员或客服
                    {

                    }
                    else
                    {
                        msg.msg = "用户权限错误";
                        return msg;
                    }
                    string error = "";
                    ArrayList al = new ArrayList();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string expressId = "";
                        string expressSql = "select expressId from t_base_express where expressName = '" + dt.Rows[i]["快递公司"].ToString() + "'";
                        DataTable expressDT = DatabaseOperationWeb.ExecuteSelectDS(expressSql, "table").Tables[0];
                        if (expressDT.Rows.Count > 0)
                        {
                            expressId = expressDT.Rows[0][0].ToString();
                        }

                        string sql1 = "select status from t_order_list where merchantOrderId='" + dt.Rows[i]["订单号"].ToString() + "' " + st;
                        DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                        if (dt1.Rows.Count > 0)
                        {
                            if (dt1.Rows[0][0].ToString() == "1" || dt1.Rows[0][0].ToString() == "2" || dt1.Rows[0][0].ToString() == "3")
                            {
                                string upsql = "update t_order_list set  status=3,expressId = '" + expressId + "'," +
                                       "waybillno= '" + dt.Rows[i]["运单号"].ToString() + "',waybilltime=now() " +
                                       "where merchantOrderId='" + dt.Rows[i]["订单号"].ToString() + "' " + st;
                                al.Add(upsql);
                            }
                            else
                            {
                                error += dt.Rows[i]["订单号"].ToString() + "状态不正确！\r\n";
                            }
                        }
                        else
                        {
                            error += dt.Rows[i]["订单号"].ToString() + "没找到对应订单！\r\n";
                        }
                    }
                    if (DatabaseOperationWeb.ExecuteDML(al))
                    {
                        msg.msg = "导入成功";
                        msg.type = "1";
                    }
                    else
                    {
                        msg.msg = "数据保存失败！";
                    }
                }
                else
                {
                    msg.msg = "文件读取失败！";
                }
            }
            else
            {
                msg.msg = "文件上传失败！";
            }
            return msg;
        }

        /// <summary>
        /// 上传订单
        /// </summary>
        /// <param name="uploadParam"></param>
        /// <returns></returns>
        public MsgResult UploadOrder(FileUploadParam uploadParam)
        {
            MsgResult msg = new MsgResult();
            string logCode = uploadParam.userId + "UploadOrder" + DateTime.Now.ToString("yyyyMMddHHmmssff");
            string fileName = logCode + ".xlsx";
            FileManager fm = new FileManager();
            if (fm.fileCopy(uploadParam.fileTemp, fileName))
            {
                DataTable dt = fm.readExcelFileToDataTable(fileName);
                if (dt == null)
                {
                    msg.msg = "导入文档错误，请确认excel里的列是否正确，是否有相同名称的列。";
                    return msg;
                }
                if (dt.Rows.Count > 0)
                {
                    #region 校验导入文档的列
                    if (!dt.Columns.Contains("订单号"))
                    {
                        msg.msg = "缺少“订单号”列，";
                    }
                    if (!dt.Columns.Contains("商品名称"))
                    {
                        msg.msg += "缺少“商品名称”列，";
                    }
                    if (!dt.Columns.Contains("商品数量"))
                    {
                        msg.msg += "缺少“商品数量”列，";
                    }
                    if (!dt.Columns.Contains("商品条码"))
                    {
                        msg.msg += "缺少“商品条码”列，";
                    }
                    if (!dt.Columns.Contains("商品申报单价"))
                    {
                        msg.msg += "缺少“商品申报单价”列，";
                    }
                    if (!dt.Columns.Contains("收货人国家"))
                    {
                        msg.msg += "缺少“收货人国家”列，";
                    }
                    if (!dt.Columns.Contains("收货人省"))
                    {
                        msg.msg += "缺少“收货人省”列，";
                    }
                    if (!dt.Columns.Contains("收货人市"))
                    {
                        msg.msg += "缺少“收货人市”列，";
                    }
                    if (!dt.Columns.Contains("收货人区"))
                    {
                        msg.msg += "缺少“收货人区”列，";
                    }
                    if (!dt.Columns.Contains("收货人地址"))
                    {
                        msg.msg += "缺少“收货人地址”列，";
                    }
                    if (!dt.Columns.Contains("创建时间"))
                    {
                        msg.msg += "缺少“创建时间”列，";
                    }
                    if (!dt.Columns.Contains("收货人电话"))
                    {
                        msg.msg += "缺少“收货人电话”列，";
                    }
                    if (!dt.Columns.Contains("收货人"))
                    {
                        msg.msg += "缺少“收货人”列，";
                    }
                    if (!dt.Columns.Contains("收件人身份证号"))
                    {
                        msg.msg += "缺少“收件人身份证号”列，";
                    }
                    if (!dt.Columns.Contains("发货人姓名"))
                    {
                        msg.msg += "缺少“发货人姓名”列，";
                    }
                    if (!dt.Columns.Contains("发货人电话"))
                    {
                        msg.msg += "缺少“”列，";
                    }
                    if (!dt.Columns.Contains("发货地址"))
                    {
                        msg.msg += "缺少“发货地址”列，";
                    }
                    if (msg.msg != null && msg.msg != "")
                    {
                        return msg;
                    }
                    #endregion
                    
                    //UserDao userDao = new UserDao();
                    //string userType = userDao.getUserType(uploadParam.userId);
                    //if (userType == "1")//采购商 
                    //{
                    //    //st = " and customerCode = '" + uploadParam.userId + "' ";
                    //}
                    //else if (userType == "0" || userType == "5")//管理员或客服
                    //{
                    //    msg.msg = "用户权限错误";
                    //    return msg;
                    //}
                    //else
                    //{
                    //    msg.msg = "用户权限错误";
                    //    return msg;
                    //}
                    #region 检查项并给导入list中
                    List<OrderItem> OrderItemList = new List<OrderItem>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string error = "";
                        //判断订单是否已经存在
                        string sqlno = "select id from t_order_list where merchantOrderId = '" + dt.Rows[i]["订单号"].ToString() + "' or  parentOrderId = '" + dt.Rows[i]["订单号"].ToString() + "'";
                        DataTable dtno = DatabaseOperationWeb.ExecuteSelectDS(sqlno, "TABLE").Tables[0];
                        if (dtno.Rows.Count > 0)
                        {
                            error += "序号为" + dt.Rows[i]["序号"].ToString() + "行订单已存在，请核对\r\n";
                        }
                        //判断条码是否已经存在
                        string sqltm = "select id,goodsName from t_goods_list where barcode = '" + dt.Rows[i]["商品条码"].ToString() + "'";
                        DataTable dttm = DatabaseOperationWeb.ExecuteSelectDS(sqltm, "TABLE").Tables[0];
                        if (dttm.Rows.Count == 0)
                        {
                            error += "序号为" + dt.Rows[i]["序号"].ToString() + "行商品条码不存在，请核对\r\n";
                        }
                        //判断商品数量,商品申报单价是否为数字
                        double d = 0;
                        if (!double.TryParse(dt.Rows[i]["商品数量"].ToString(), out d))
                        {
                            error += "序号为" + dt.Rows[i]["序号"].ToString() + "行商品数量填写错误，请核对\r\n";
                        }
                        if (!double.TryParse(dt.Rows[i]["商品申报单价"].ToString(), out d))
                        {
                            error += "序号为" + dt.Rows[i]["序号"].ToString() + "行商品申报单价填写错误，请核对\r\n";
                        }
                        //判断订单日期是否正确
                        DateTime dtime = DateTime.Now;
                        try
                        {
                            dtime = Convert.ToDateTime(dt.Rows[i]["创建时间"].ToString());
                        }
                        catch
                        {
                            error += "序号为" + dt.Rows[i]["序号"].ToString() + "行创建时间日期格式填写错误，请核对\r\n";
                        }
                        if (error != "")
                        {
                            msg.msg += error;
                            continue;
                        }
                        //判断地址是否正确
                        string sqlp = "select provinceid from t_base_provinces where province like '" + dt.Rows[i]["收货人省"].ToString() + "%'";
                        DataTable dtp = DatabaseOperationWeb.ExecuteSelectDS(sqlp, "TABLE").Tables[0];
                        if (dtp.Rows.Count > 0)
                        {
                            string provinceid = dtp.Rows[0][0].ToString();
                            string sqlc = "select cityid from t_base_cities  " +
                                "where city like '" + dt.Rows[i]["收货人市"].ToString() + "%' and provinceid=" + provinceid + "";
                            DataTable dtc = DatabaseOperationWeb.ExecuteSelectDS(sqlc, "TABLE").Tables[0];
                            if (dtc.Rows.Count > 0)
                            {
                                string cityid = dtc.Rows[0][0].ToString();
                                string sqla = "select id from t_base_areas " +
                                    "where area ='" + dt.Rows[i]["收货人区"].ToString() + "' and cityid=" + cityid + "";
                                DataTable dta = DatabaseOperationWeb.ExecuteSelectDS(sqla, "TABLE").Tables[0];
                                if (dta.Rows.Count == 0)
                                {
                                    error += "序号为" + dt.Rows[i]["序号"].ToString() + "行收货人区填写错误，请核对\r\n";
                                }
                            }
                            else
                            {
                                error += "序号为" + dt.Rows[i]["序号"].ToString() + "行收货人市填写错误，请核对\r\n";
                            }
                        }
                        else
                        {
                            error += "序号为" + dt.Rows[i]["序号"].ToString() + "行收货人省填写错误，请核对\r\n";
                        }
                        if (error != "")
                        {
                            msg.msg += error;
                            continue;
                        }
                        bool isNotFound = true;
                        for (int j = 0; j < OrderItemList.Count(); j++)
                        {
                            if (OrderItemList[j].merchantOrderId == dt.Rows[i]["订单号"].ToString())
                            {
                                OrderGoodsItem orderGoodsItem = new OrderGoodsItem();
                                orderGoodsItem.id = dt.Rows[i]["序号"].ToString();
                                orderGoodsItem.barCode = dt.Rows[i]["商品条码"].ToString();
                                orderGoodsItem.skuUnitPrice = Convert.ToDouble(dt.Rows[i]["商品申报单价"]);
                                orderGoodsItem.skuBillName = dt.Rows[i]["商品名称"].ToString();
                                orderGoodsItem.quantity = Convert.ToDouble(dt.Rows[i]["商品数量"]);
                                orderGoodsItem.totalPrice = orderGoodsItem.skuUnitPrice * orderGoodsItem.quantity;
                                OrderItemList[j].OrderGoods.Add(orderGoodsItem);
                                isNotFound = false;
                                break;
                            }
                        }
                        if (isNotFound)//没有对应订单
                        {
                            OrderItem orderItem = new OrderItem();
                            orderItem.merchantOrderId = dt.Rows[i]["订单号"].ToString();
                            orderItem.tradeTime = dtime.ToString("yyyy-MM-dd HH:mm:ss");
                            orderItem.consigneeName = dt.Rows[i]["收货人"].ToString();
                            orderItem.consigneeMobile = dt.Rows[i]["收货人电话"].ToString();
                            orderItem.idNumber = dt.Rows[i]["收件人身份证号"].ToString();
                            orderItem.addrCountry = dt.Rows[i]["收货人国家"].ToString();
                            orderItem.addrProvince = dt.Rows[i]["收货人省"].ToString();
                            orderItem.addrCity = dt.Rows[i]["收货人市"].ToString();
                            orderItem.addrDistrict = dt.Rows[i]["收货人区"].ToString();
                            orderItem.addrDetail = dt.Rows[i]["收货人地址"].ToString();
                            orderItem.consignorName= dt.Rows[i]["发货人姓名"].ToString();   
                            orderItem.consignorMobile = dt.Rows[i]["发货人电话"].ToString();
                            orderItem.consignorAddr = dt.Rows[i]["发货地址"].ToString();
                            orderItem.OrderGoods = new List<OrderGoodsItem>();
                            OrderGoodsItem orderGoodsItem = new OrderGoodsItem();
                            orderGoodsItem.id = dt.Rows[i]["序号"].ToString();
                            orderGoodsItem.barCode = dt.Rows[i]["商品条码"].ToString();
                            orderGoodsItem.skuUnitPrice = Convert.ToDouble(dt.Rows[i]["商品申报单价"]);
                            orderGoodsItem.skuBillName = dt.Rows[i]["商品名称"].ToString();
                            orderGoodsItem.quantity = Convert.ToDouble(dt.Rows[i]["商品数量"]);
                            orderGoodsItem.totalPrice = orderGoodsItem.skuUnitPrice * orderGoodsItem.quantity;
                            orderItem.OrderGoods.Add(orderGoodsItem);
                            OrderItemList.Add(orderItem);
                        }
                    }
                    #endregion

                    #region 处理因仓库分单
                    List<OrderItem> newOrderItemList = new List<OrderItem>();
                    foreach (var orderItem in OrderItemList)
                    {
                        Dictionary<int, List<OrderGoodsItem>> myDictionary = new Dictionary<int, List<OrderGoodsItem>>();
                        foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
                        {
                            string wsql = "select d.platformId,u.id as userId,u.platformCost,u.platformCostType,u.priceType," +
                                          "d.pprice,d.profitPlatform,d.profitAgent,d.profitDealer,d.profitOther1," +
                                          "d.profitOther1Name,d.profitOther2,d.profitOther2Name,d.profitOther3," +
                                          "d.profitOther3Name,w.id as goodsWarehouseId,w.wid,w.wcode,w.goodsnum,w.inprice,bw.taxation,bw.taxation2," +
                                          "bw.taxation2type,bw.taxation2line,bw.freight,w.suppliercode,g.NW,g.slt " +
                                          "from t_goods_distributor_price d ,t_goods_warehouse w,t_base_warehouse bw," +
                                          "t_goods_list g,t_user_list u   " +
                                          "where u.usercode = d.usercode and g.barcode = d.barcode and w.wid = bw.id " +
                                          "and d.barcode = w.barcode and w.supplierid = d.supplierid and d.wid = bw.id " +
                                          "and d.usercode = '" + uploadParam.userId + "' " +
                                          "and d.barcode = '" + orderGoodsItem.barCode + "' and w.goodsnum >=" + orderGoodsItem.quantity +
                                          " order by w.goodsnum asc";
                            DataTable wdt = DatabaseOperationWeb.ExecuteSelectDS(wsql, "TABLE").Tables[0];
                            int wid = 0;
                            if (wdt.Rows.Count == 1)
                            {
                                wid = Convert.ToInt16(wdt.Rows[0]["wid"]);
                                orderGoodsItem.dr = wdt.Rows[0];
                            }
                            else if (wdt.Rows.Count > 1)
                            {
                                wid = Convert.ToInt16(wdt.Rows[0]["wid"]);
                                orderGoodsItem.dr = wdt.Rows[0];
                                for (int i = 0; i < wdt.Rows.Count; i++)
                                {
                                    if (myDictionary.ContainsKey(Convert.ToInt16(wdt.Rows[i]["wid"])))
                                    {
                                        wid = Convert.ToInt16(wdt.Rows[i]["wid"]);
                                        orderGoodsItem.dr = wdt.Rows[i];
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                msg.msg += "序号为" + orderGoodsItem.id + "行没找到对应默认供货信息，请核对\r\n";
                                continue;
                            }
                            if (!myDictionary.ContainsKey(wid))
                            {
                                myDictionary.Add(wid, new List<OrderGoodsItem>());
                            }
                            myDictionary[wid].Add(orderGoodsItem);
                        }
                        if (myDictionary.Count() > 1)
                        {
                            int num = 0;
                            foreach (var kvp in myDictionary)
                            {
                                if (num == 0)
                                {
                                    orderItem.parentOrderId = orderItem.merchantOrderId;
                                    orderItem.merchantOrderId += kvp.Key;
                                    orderItem.warehouseId = kvp.Key.ToString();
                                    orderItem.OrderGoods = new List<OrderGoodsItem>();
                                    double tradeAmount = 0;
                                    foreach (var item in kvp.Value)
                                    {
                                        tradeAmount += Convert.ToDouble(item.skuUnitPrice) * Convert.ToDouble(item.quantity);
                                        orderItem.OrderGoods.Add(item);
                                    }
                                    orderItem.tradeAmount = tradeAmount.ToString();
                                    newOrderItemList.Add(orderItem);
                                }
                                else
                                {
                                    OrderItem orderItemNew = new OrderItem();
                                    orderItemNew.parentOrderId = orderItem.parentOrderId;
                                    orderItemNew.merchantOrderId = orderItem.parentOrderId + kvp.Key;
                                    orderItemNew.tradeTime = orderItem.tradeTime;
                                    orderItemNew.consigneeName = orderItem.consigneeName;
                                    orderItemNew.consigneeMobile = orderItem.consigneeMobile;
                                    orderItemNew.idNumber = orderItem.idNumber;
                                    orderItemNew.addrCountry = orderItem.addrCountry;
                                    orderItemNew.addrProvince = orderItem.addrProvince;
                                    orderItemNew.addrCity = orderItem.addrCity;
                                    orderItemNew.addrDistrict = orderItem.addrDistrict;
                                    orderItemNew.addrDetail = orderItem.addrDetail;
                                    orderItemNew.consignorName = orderItem.consignorName;
                                    orderItemNew.consignorMobile = orderItem.consignorMobile;
                                    orderItemNew.consignorAddr = orderItem.consignorAddr;
                                    orderItemNew.OrderGoods = new List<OrderGoodsItem>();
                                    double tradeAmount = 0;
                                    foreach (var item in kvp.Value)
                                    {
                                        tradeAmount += Convert.ToDouble(item.skuUnitPrice) * Convert.ToDouble(item.quantity);
                                        orderItemNew.OrderGoods.Add(item);
                                    }
                                    orderItemNew.tradeAmount = tradeAmount.ToString();
                                    newOrderItemList.Add(orderItemNew);
                                }
                                num++;
                            }
                        }
                        else
                        {
                            double tradeAmount = 0;
                            foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
                            {
                                tradeAmount += Convert.ToDouble(orderGoodsItem.skuUnitPrice) * Convert.ToDouble(orderGoodsItem.quantity);
                            }
                            orderItem.parentOrderId = orderItem.merchantOrderId;
                            orderItem.tradeAmount = tradeAmount.ToString();
                            newOrderItemList.Add(orderItem);
                        }
                    }
                    #endregion

                    if (msg.msg != "")
                    {
                        return msg;
                    }
                    #region 价格分拆

                    ArrayList al = new ArrayList();
                    ArrayList goodsNumAl = new ArrayList();
                    foreach (var orderItem in newOrderItemList)
                    {
                        double freight = 0, tradeAmount = 1;
                        double.TryParse(orderItem.OrderGoods[0].dr["freight"].ToString(), out freight);
                        double.TryParse(orderItem.tradeAmount, out tradeAmount);
                        orderItem.freight = Math.Round(freight, 2);
                        orderItem.platformId = orderItem.OrderGoods[0].dr["platformId"].ToString();
                        orderItem.warehouseId = orderItem.OrderGoods[0].dr["wid"].ToString();
                        orderItem.warehouseCode = orderItem.OrderGoods[0].dr["wcode"].ToString();
                        orderItem.supplier = orderItem.OrderGoods[0].dr["suppliercode"].ToString();
                        orderItem.purchaseId = orderItem.OrderGoods[0].dr["userId"].ToString();
                        orderItem.purchase = uploadParam.userId;
                        double fr = Math.Round(orderItem.freight / tradeAmount, 4);
                        for (int i = 0; i < orderItem.OrderGoods.Count; i++)
                        {
                            OrderGoodsItem orderGoodsItem = orderItem.OrderGoods[i];
                            //处理运费
                            //if (i== orderItem.OrderGoods.Count-1)
                            //{
                            //    orderGoodsItem.waybillPrice = freight;
                            //}
                            //else
                            //{
                            //    orderGoodsItem.waybillPrice = Math.Round(fr * orderGoodsItem.totalPrice,2);
                            //    freight -= orderGoodsItem.waybillPrice;
                            //}
                            //从运费平摊修改为运费都为全部运费。
                            orderGoodsItem.waybillPrice = freight;


                            //处理供货价和销售价和供货商code
                            orderGoodsItem.supplyPrice = Math.Round(Convert.ToDouble(orderGoodsItem.dr["inprice"]), 2);
                            orderGoodsItem.purchasePrice = Math.Round(Convert.ToDouble(orderGoodsItem.dr["pprice"]), 2);
                            orderGoodsItem.suppliercode = orderGoodsItem.dr["suppliercode"].ToString();
                            orderGoodsItem.slt = orderGoodsItem.dr["slt"].ToString();

                            string goodsWarehouseId = orderGoodsItem.dr["goodsWarehouseId"].ToString();//库存id
                            //处理税
                            double taxation = 0;
                            double.TryParse(orderGoodsItem.dr["taxation"].ToString(), out taxation);
                            if (taxation > 0)
                            {
                                double taxation2 = 0, taxation2type = 0, taxation2line = 0, nw = 0;
                                double.TryParse(orderGoodsItem.dr["taxation2"].ToString(), out taxation2);
                                double.TryParse(orderGoodsItem.dr["taxation2type"].ToString(), out taxation2type);
                                double.TryParse(orderGoodsItem.dr["taxation2line"].ToString(), out taxation2line);
                                double.TryParse(orderGoodsItem.dr["NW"].ToString(), out nw);
                                if (taxation2 == 0)
                                {
                                    orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                                }
                                else
                                {
                                    if (taxation2type == 1)//按总价提档
                                    {
                                        if (orderGoodsItem.skuUnitPrice > taxation2line)//价格过线
                                        {
                                            orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation2 / 100;
                                        }
                                        else//价格没过线
                                        {
                                            orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                                        }
                                    }
                                    else if (taxation2type == 2)//按元/克提档
                                    {
                                        if (nw == 0)//如果没有净重，按默认税档
                                        {
                                            orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                                        }
                                        else
                                        {
                                            if (orderGoodsItem.skuUnitPrice / (nw * 1000) > taxation2line)//价格过线
                                            {
                                                orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation2 / 100;
                                            }
                                            else//价格没过线
                                            {
                                                orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                                            }
                                        }
                                        //还要考虑面膜的问题
                                    }
                                    else//都不是按初始税率走
                                    {
                                        orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                                    }
                                }
                            }
                            else
                            {
                                orderGoodsItem.tax = 0;
                            }
                            orderGoodsItem.tax = Math.Round(orderGoodsItem.tax, 2);
                            //处理平台提点
                            orderGoodsItem.platformPrice = 0;
                            double platformCost = 0;
                            double.TryParse(orderGoodsItem.dr["platformCost"].ToString(), out platformCost);
                            if (platformCost > 0)
                            {
                                if (orderGoodsItem.dr["platformCostType"].ToString() == "1")//进价计算
                                {
                                    orderGoodsItem.platformPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * platformCost / 100;
                                }
                                else if (orderGoodsItem.dr["platformCostType"].ToString() == "2")//售价计算
                                {
                                    if (orderGoodsItem.dr["priceType"].ToString() == "1")//按订单售价计算
                                    {
                                        orderGoodsItem.platformPrice = orderGoodsItem.totalPrice * platformCost / 100;
                                    }
                                    else if (orderGoodsItem.dr["priceType"].ToString() == "2")//按供货价计算
                                    {
                                        orderGoodsItem.platformPrice = orderGoodsItem.purchasePrice * orderGoodsItem.quantity * platformCost / 100;
                                    }
                                }
                            }
                            orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);
                            //处理利润
                            //利润为供货价-进价-提点-运费分成-税
                            double profit = orderGoodsItem.purchasePrice * orderGoodsItem.quantity
                                - orderGoodsItem.supplyPrice * orderGoodsItem.quantity
                                - orderGoodsItem.platformPrice
                                - orderGoodsItem.waybillPrice
                                - orderGoodsItem.tax;
                            double profitAgent = 0, profitDealer = 0, profitOther1 = 0, profitOther2 = 0, profitOther3 = 0;
                            double.TryParse(orderGoodsItem.dr["profitAgent"].ToString(), out profitAgent);
                            double.TryParse(orderGoodsItem.dr["profitDealer"].ToString(), out profitDealer);
                            double.TryParse(orderGoodsItem.dr["profitOther1"].ToString(), out profitOther1);
                            double.TryParse(orderGoodsItem.dr["profitOther2"].ToString(), out profitOther2);
                            double.TryParse(orderGoodsItem.dr["profitOther3"].ToString(), out profitOther3);
                            orderGoodsItem.profitAgent = Math.Round(profit * profitAgent / 100, 2);
                            orderGoodsItem.profitDealer = Math.Round(profit * profitDealer / 100, 2);
                            orderGoodsItem.profitOther1 = Math.Round(profit * profitOther1 / 100, 2);
                            orderGoodsItem.profitOther2 = Math.Round(profit * profitOther2 / 100, 2);
                            orderGoodsItem.profitOther3 = Math.Round(profit * profitOther3 / 100, 2);
                            orderGoodsItem.profitPlatform = Math.Round(profit - orderGoodsItem.profitAgent
                                                            - orderGoodsItem.profitDealer - orderGoodsItem.profitOther1
                                                            - orderGoodsItem.profitOther2 - orderGoodsItem.profitOther3, 2);
                            orderGoodsItem.other1Name = orderGoodsItem.dr["profitOther1Name"].ToString();
                            orderGoodsItem.other2Name = orderGoodsItem.dr["profitOther2Name"].ToString();
                            orderGoodsItem.other3Name = orderGoodsItem.dr["profitOther3Name"].ToString();
                            string sqlgoods = "insert into t_order_goods(merchantOrderId,barCode,slt,skuUnitPrice," +
                                          "quantity,skuBillName,batchNo,goodsName," +
                                          "api,fqSkuID,sendType,status," +
                                          "suppliercode,supplyPrice,purchasePrice,waybill," +
                                          "waybillPrice,tax,platformPrice,profitPlatform," +
                                          "profitAgent,profitDealer,profitOther1,other1Name," +
                                          "profitOther2,other2Name,profitOther3,other3Name) " +
                                          "values('" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "','" + orderGoodsItem.slt + "','" + orderGoodsItem.skuUnitPrice + "'" +
                                          ",'" + orderGoodsItem.quantity + "','" + orderGoodsItem.skuBillName + "','','" + orderGoodsItem.skuBillName + "'" +
                                          ",'','','','0'" +
                                          ",'" + orderGoodsItem.suppliercode + "','" + orderGoodsItem.supplyPrice + "','" + orderGoodsItem.purchasePrice + "',''" +
                                          ",'" + orderGoodsItem.waybillPrice + "','" + orderGoodsItem.tax + "','" + orderGoodsItem.platformPrice + "','" + orderGoodsItem.profitPlatform + "'" +
                                          ",'" + orderGoodsItem.profitAgent + "','" + orderGoodsItem.profitDealer + "','" + orderGoodsItem.profitOther1 + "','" + orderGoodsItem.other1Name + "'" +
                                          ",'" + orderGoodsItem.profitOther2 + "','" + orderGoodsItem.other2Name + "','" + orderGoodsItem.profitOther3 + "','" + orderGoodsItem.other3Name + "'" +
                                          ")";
                            al.Add(sqlgoods);
                            string upsql = "update t_goods_warehouse set goodsnum = goodsnum-" + orderGoodsItem.quantity + " where id = " + goodsWarehouseId;
                            goodsNumAl.Add(upsql);
                            string logsql = "insert into t_log_goodsnum(inputType,createtime,wid,wcode,orderid,barcode,goodsnum,state) " +
                                            "values('',now(),'" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "'," +
                                            "'" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "'," +
                                            "" + orderGoodsItem.quantity + ",'" + orderItem.status + "')";
                            goodsNumAl.Add(logsql);
                        }
                        string sqlorder = "insert into t_order_list(warehouseId,warehouseCode,customerCode,actionType," +
                            "orderType,serviceType,parentOrderId,merchantOrderId," +
                            "payType,payNo,tradeTime," +
                            "tradeAmount,goodsTotalAmount,consigneeName,consigneeMobile," +
                            "addrCountry,addrProvince,addrCity,addrDistrict," +
                            "addrDetail,zipCode,idType,idNumber," +
                            "idFountImgUrl,idBackImgUrl,status,purchaserCode," +
                            "purchaserId,distributionCode,apitype,waybillno," +
                            "expressId,inputTime,fqID," +
                            "operate_status,sendapi,platformId,consignorName," +
                            "consignorMobile,consignorAddr,batchid,outNo,waybillOutNo," +
                            "accountsStatus,accountsNo,prePayId,ifPrint,printNo) " +
                            "values('" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "','" + orderItem.supplier + "',''" +
                            ",'','','" + orderItem.parentOrderId + "','" + orderItem.merchantOrderId + "'" +
                            ",'','','" + orderItem.tradeTime + "'" +
                            "," + orderItem.tradeAmount + ",'" + orderItem.tradeAmount + "','" + orderItem.consigneeName + "','" + orderItem.consigneeMobile + "'" +
                            ",'" + orderItem.addrCountry + "','" + orderItem.addrProvince + "','" + orderItem.addrCity + "','" + orderItem.addrDistrict + "'" +
                            ",'" + orderItem.addrDetail + "','','1','" + orderItem.idNumber + "'" +
                            ",'','','1','" + orderItem.purchase + "'" +
                            ",'" + orderItem.purchaseId + "','','',''" +
                            ",'',now(),''" +
                            ",'0','','" + orderItem.platformId + "','" + orderItem.consignorName + "'" +
                            ",'" + orderItem.consignorMobile + "','" + orderItem.consignorAddr + "','','',''" +
                            ",'0','','','0','') ";
                        al.Add(sqlorder);
                    }

                    #endregion


                    if (DatabaseOperationWeb.ExecuteDML(al))
                    {
                        DatabaseOperationWeb.ExecuteDML(goodsNumAl);
                        msg.msg = "导入成功";
                        msg.type = "1";
                    }
                    else
                    {
                        msg.msg = "数据保存失败！";
                    }
                }
                else
                {
                    msg.msg = "文件读取失败！";
                }
            }
            else
            {
                msg.msg = "文件上传失败！";
            }
            return msg;
        }

        /// <summary>
        /// 上传分销商订单
        /// </summary>
        /// <param name="uploadParam"></param>
        /// <returns></returns>
        public MsgResult UploadOrderOfDistribution(FileUploadParam uploadParam)
        {
            MsgResult msg = new MsgResult();
            string logCode = uploadParam.userId + "UploadOrder" + DateTime.Now.ToString("yyyyMMddHHmmssff");
            //20180826 新增分销商上传订单，取得分销商的代理，然后订单相当于代理导入，然后再订单的distributionCode字段放上分销商usercode
            UserDao userDao = new UserDao();
            string ofAgent = userDao.getOfAgent(uploadParam.userId);
            if (ofAgent==null||ofAgent=="")
            {
                msg.msg = "分销商所归属的代理为空，请与平台运营联系！";
                return msg;
            }
            string distribution = uploadParam.userId;
            uploadParam.userId = ofAgent;
            string fileName = logCode + ".xlsx";
            FileManager fm = new FileManager();
            if (fm.fileCopy(uploadParam.fileTemp, fileName))
            {
                DataTable dt = fm.readExcelFileToDataTable(fileName);
                if (dt == null)
                {
                    msg.msg = "导入文档错误，请确认excel里的列是否正确，是否有相同名称的列。";
                    return msg;
                }
                if (dt.Rows.Count > 0)
                {
                    #region 校验导入文档的列
                    if (!dt.Columns.Contains("订单号"))
                    {
                        msg.msg = "缺少“订单号”列，";
                    }
                    if (!dt.Columns.Contains("商品名称"))
                    {
                        msg.msg += "缺少“商品名称”列，";
                    }
                    if (!dt.Columns.Contains("商品数量"))
                    {
                        msg.msg += "缺少“商品数量”列，";
                    }
                    if (!dt.Columns.Contains("商品条码"))
                    {
                        msg.msg += "缺少“商品条码”列，";
                    }
                    if (!dt.Columns.Contains("商品申报单价"))
                    {
                        msg.msg += "缺少“商品申报单价”列，";
                    }
                    if (!dt.Columns.Contains("收货人国家"))
                    {
                        msg.msg += "缺少“收货人国家”列，";
                    }
                    if (!dt.Columns.Contains("收货人省"))
                    {
                        msg.msg += "缺少“收货人省”列，";
                    }
                    if (!dt.Columns.Contains("收货人市"))
                    {
                        msg.msg += "缺少“收货人市”列，";
                    }
                    if (!dt.Columns.Contains("收货人区"))
                    {
                        msg.msg += "缺少“收货人区”列，";
                    }
                    if (!dt.Columns.Contains("收货人地址"))
                    {
                        msg.msg += "缺少“收货人地址”列，";
                    }
                    if (!dt.Columns.Contains("创建时间"))
                    {
                        msg.msg += "缺少“创建时间”列，";
                    }
                    if (!dt.Columns.Contains("收货人电话"))
                    {
                        msg.msg += "缺少“收货人电话”列，";
                    }
                    if (!dt.Columns.Contains("收货人"))
                    {
                        msg.msg += "缺少“收货人”列，";
                    }
                    if (!dt.Columns.Contains("收件人身份证号"))
                    {
                        msg.msg += "缺少“收件人身份证号”列，";
                    }
                    if (!dt.Columns.Contains("发货人姓名"))
                    {
                        msg.msg += "缺少“发货人姓名”列，";
                    }
                    if (!dt.Columns.Contains("发货人电话"))
                    {
                        msg.msg += "缺少“发货人电话”列，";
                    }
                    if (!dt.Columns.Contains("发货地址"))
                    {
                        msg.msg += "缺少“发货地址”列，";
                    }
                    if (msg.msg != null && msg.msg != "")
                    {
                        return msg;
                    }
                    #endregion

                    //UserDao userDao = new UserDao();
                    //string userType = userDao.getUserType(uploadParam.userId);
                    //if (userType == "1")//采购商 
                    //{
                    //    //st = " and customerCode = '" + uploadParam.userId + "' ";
                    //}
                    //else if (userType == "0" || userType == "5")//管理员或客服
                    //{
                    //    msg.msg = "用户权限错误";
                    //    return msg;
                    //}
                    //else
                    //{
                    //    msg.msg = "用户权限错误";
                    //    return msg;
                    //}
                    #region 检查项并给导入list中
                    List<OrderItem> OrderItemList = new List<OrderItem>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string error = "";
                        //判断订单是否已经存在
                        string sqlno = "select id from t_order_list where merchantOrderId = '" + dt.Rows[i]["订单号"].ToString() + "' or  parentOrderId = '" + dt.Rows[i]["订单号"].ToString() + "'";
                        DataTable dtno = DatabaseOperationWeb.ExecuteSelectDS(sqlno, "TABLE").Tables[0];
                        if (dtno.Rows.Count > 0)
                        {
                            error += "序号为" + dt.Rows[i]["序号"].ToString() + "行订单已存在，请核对\r\n";
                        }
                        //判断条码是否已经存在
                        string sqltm = "select id,goodsName from t_goods_list where barcode = '" + dt.Rows[i]["商品条码"].ToString() + "'";
                        DataTable dttm = DatabaseOperationWeb.ExecuteSelectDS(sqltm, "TABLE").Tables[0];
                        if (dttm.Rows.Count == 0)
                        {
                            error += "序号为" + dt.Rows[i]["序号"].ToString() + "行商品条码不存在，请核对\r\n";
                        }
                        //判断商品数量,商品申报单价是否为数字
                        double d = 0;
                        if (!double.TryParse(dt.Rows[i]["商品数量"].ToString(), out d))
                        {
                            error += "序号为" + dt.Rows[i]["序号"].ToString() + "行商品数量填写错误，请核对\r\n";
                        }
                        if (!double.TryParse(dt.Rows[i]["商品申报单价"].ToString(), out d))
                        {
                            error += "序号为" + dt.Rows[i]["序号"].ToString() + "行商品申报单价填写错误，请核对\r\n";
                        }
                        //判断订单日期是否正确
                        DateTime dtime = DateTime.Now;
                        try
                        {
                            dtime = Convert.ToDateTime(dt.Rows[i]["创建时间"].ToString());
                        }
                        catch
                        {
                            error += "序号为" + dt.Rows[i]["序号"].ToString() + "行创建时间日期格式填写错误，请核对\r\n";
                        }
                        if (error != "")
                        {
                            msg.msg += error;
                            continue;
                        }
                        //判断地址是否正确
                        string sqlp = "select provinceid from t_base_provinces where province like '" + dt.Rows[i]["收货人省"].ToString() + "%'";
                        DataTable dtp = DatabaseOperationWeb.ExecuteSelectDS(sqlp, "TABLE").Tables[0];
                        if (dtp.Rows.Count > 0)
                        {
                            string provinceid = dtp.Rows[0][0].ToString();
                            string sqlc = "select cityid from t_base_cities  " +
                                "where city like '" + dt.Rows[i]["收货人市"].ToString() + "%' and provinceid=" + provinceid + "";
                            DataTable dtc = DatabaseOperationWeb.ExecuteSelectDS(sqlc, "TABLE").Tables[0];
                            if (dtc.Rows.Count > 0)
                            {
                                string cityid = dtc.Rows[0][0].ToString();
                                string sqla = "select id from t_base_areas " +
                                    "where area ='" + dt.Rows[i]["收货人区"].ToString() + "' and cityid=" + cityid + "";
                                DataTable dta = DatabaseOperationWeb.ExecuteSelectDS(sqla, "TABLE").Tables[0];
                                if (dta.Rows.Count == 0)
                                {
                                    error += "序号为" + dt.Rows[i]["序号"].ToString() + "行收货人区填写错误，请核对\r\n";
                                }
                            }
                            else
                            {
                                error += "序号为" + dt.Rows[i]["序号"].ToString() + "行收货人市填写错误，请核对\r\n";
                            }
                        }
                        else
                        {
                            error += "序号为" + dt.Rows[i]["序号"].ToString() + "行收货人省填写错误，请核对\r\n";
                        }
                        if (error != "")
                        {
                            msg.msg += error;
                            continue;
                        }
                        bool isNotFound = true;
                        for (int j = 0; j < OrderItemList.Count(); j++)
                        {
                            if (OrderItemList[j].merchantOrderId == dt.Rows[i]["订单号"].ToString())
                            {
                                OrderGoodsItem orderGoodsItem = new OrderGoodsItem();
                                orderGoodsItem.id = dt.Rows[i]["序号"].ToString();
                                orderGoodsItem.barCode = dt.Rows[i]["商品条码"].ToString();
                                orderGoodsItem.skuUnitPrice = Convert.ToDouble(dt.Rows[i]["商品申报单价"]);
                                orderGoodsItem.skuBillName = dt.Rows[i]["商品名称"].ToString();
                                orderGoodsItem.quantity = Convert.ToDouble(dt.Rows[i]["商品数量"]);
                                orderGoodsItem.totalPrice = orderGoodsItem.skuUnitPrice * orderGoodsItem.quantity;
                                OrderItemList[j].OrderGoods.Add(orderGoodsItem);
                                isNotFound = false;
                                break;
                            }
                        }
                        if (isNotFound)//没有对应订单
                        {
                            OrderItem orderItem = new OrderItem();
                            orderItem.merchantOrderId = dt.Rows[i]["订单号"].ToString();
                            orderItem.tradeTime = dtime.ToString("yyyy-MM-dd HH:mm:ss");
                            orderItem.consigneeName = dt.Rows[i]["收货人"].ToString();
                            orderItem.consigneeMobile = dt.Rows[i]["收货人电话"].ToString();
                            orderItem.idNumber = dt.Rows[i]["收件人身份证号"].ToString();
                            orderItem.addrCountry = dt.Rows[i]["收货人国家"].ToString();
                            orderItem.addrProvince = dt.Rows[i]["收货人省"].ToString();
                            orderItem.addrCity = dt.Rows[i]["收货人市"].ToString();
                            orderItem.addrDistrict = dt.Rows[i]["收货人区"].ToString();
                            orderItem.addrDetail = dt.Rows[i]["收货人地址"].ToString();
                            orderItem.consignorName = dt.Rows[i]["发货人姓名"].ToString();
                            orderItem.consignorMobile = dt.Rows[i]["发货人电话"].ToString();
                            orderItem.consignorAddr = dt.Rows[i]["发货地址"].ToString();
                            orderItem.OrderGoods = new List<OrderGoodsItem>();
                            OrderGoodsItem orderGoodsItem = new OrderGoodsItem();
                            orderGoodsItem.id = dt.Rows[i]["序号"].ToString();
                            orderGoodsItem.barCode = dt.Rows[i]["商品条码"].ToString();
                            orderGoodsItem.skuUnitPrice = Convert.ToDouble(dt.Rows[i]["商品申报单价"]);
                            orderGoodsItem.skuBillName = dt.Rows[i]["商品名称"].ToString();
                            orderGoodsItem.quantity = Convert.ToDouble(dt.Rows[i]["商品数量"]);
                            orderGoodsItem.totalPrice = orderGoodsItem.skuUnitPrice * orderGoodsItem.quantity;
                            orderItem.OrderGoods.Add(orderGoodsItem);
                            OrderItemList.Add(orderItem);
                        }
                    }
                    #endregion

                    #region 处理因仓库分单
                    List<OrderItem> newOrderItemList = new List<OrderItem>();
                    foreach (var orderItem in OrderItemList)
                    {
                        Dictionary<int, List<OrderGoodsItem>> myDictionary = new Dictionary<int, List<OrderGoodsItem>>();
                        foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
                        {
                            string wsql = "select d.platformId,u.id as userId,u.platformCost,u.platformCostType,u.priceType," +
                                          "d.pprice,d.profitPlatform,d.profitAgent,d.profitDealer,d.profitOther1," +
                                          "d.profitOther1Name,d.profitOther2,d.profitOther2Name,d.profitOther3," +
                                          "d.profitOther3Name,w.id as goodsWarehouseId,w.wid,w.wcode,w.goodsnum,w.inprice,bw.taxation,bw.taxation2," +
                                          "bw.taxation2type,bw.taxation2line,bw.freight,w.suppliercode,g.NW,g.slt " +
                                          "from t_goods_distributor_price d ,t_goods_warehouse w,t_base_warehouse bw," +
                                          "t_goods_list g,t_user_list u   " +
                                          "where u.usercode = d.usercode and g.barcode = d.barcode and w.wid = bw.id " +
                                          "and d.barcode = w.barcode and w.supplierid = d.supplierid and d.wid = bw.id " +
                                          "and d.usercode = '" + uploadParam.userId + "' " +
                                          "and d.barcode = '" + orderGoodsItem.barCode + "' and w.goodsnum >=" + orderGoodsItem.quantity +
                                          " order by w.goodsnum asc";
                            DataTable wdt = DatabaseOperationWeb.ExecuteSelectDS(wsql, "TABLE").Tables[0];
                            int wid = 0;
                            if (wdt.Rows.Count == 1)
                            {
                                wid = Convert.ToInt16(wdt.Rows[0]["wid"]);
                                orderGoodsItem.dr = wdt.Rows[0];
                            }
                            else if (wdt.Rows.Count > 1)
                            {
                                wid = Convert.ToInt16(wdt.Rows[0]["wid"]);
                                orderGoodsItem.dr = wdt.Rows[0];
                                for (int i = 0; i < wdt.Rows.Count; i++)
                                {
                                    if (myDictionary.ContainsKey(Convert.ToInt16(wdt.Rows[i]["wid"])))
                                    {
                                        wid = Convert.ToInt16(wdt.Rows[i]["wid"]);
                                        orderGoodsItem.dr = wdt.Rows[i];
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                msg.msg += "序号为" + orderGoodsItem.id + "行没找到对应默认供货信息，请核对\r\n";
                                continue;
                            }
                            if (!myDictionary.ContainsKey(wid))
                            {
                                myDictionary.Add(wid, new List<OrderGoodsItem>());
                            }
                            myDictionary[wid].Add(orderGoodsItem);
                        }
                        if (myDictionary.Count() > 1)
                        {
                            int num = 0;
                            foreach (var kvp in myDictionary)
                            {
                                if (num == 0)
                                {
                                    orderItem.parentOrderId = orderItem.merchantOrderId;
                                    orderItem.merchantOrderId += kvp.Key;
                                    orderItem.warehouseId = kvp.Key.ToString();
                                    orderItem.OrderGoods = new List<OrderGoodsItem>();
                                    double tradeAmount = 0;
                                    foreach (var item in kvp.Value)
                                    {
                                        tradeAmount += Convert.ToDouble(item.skuUnitPrice) * Convert.ToDouble(item.quantity);
                                        orderItem.OrderGoods.Add(item);
                                    }
                                    orderItem.tradeAmount = tradeAmount.ToString();
                                    newOrderItemList.Add(orderItem);
                                }
                                else
                                {
                                    OrderItem orderItemNew = new OrderItem();
                                    orderItemNew.parentOrderId = orderItem.parentOrderId;
                                    orderItemNew.merchantOrderId = orderItem.parentOrderId + kvp.Key;
                                    orderItemNew.tradeTime = orderItem.tradeTime;
                                    orderItemNew.consigneeName = orderItem.consigneeName;
                                    orderItemNew.consigneeMobile = orderItem.consigneeMobile;
                                    orderItemNew.idNumber = orderItem.idNumber;
                                    orderItemNew.addrCountry = orderItem.addrCountry;
                                    orderItemNew.addrProvince = orderItem.addrProvince;
                                    orderItemNew.addrCity = orderItem.addrCity;
                                    orderItemNew.addrDistrict = orderItem.addrDistrict;
                                    orderItemNew.addrDetail = orderItem.addrDetail;
                                    orderItemNew.consignorName = orderItem.consignorName;
                                    orderItemNew.consignorMobile = orderItem.consignorMobile;
                                    orderItemNew.consignorAddr = orderItem.consignorAddr;
                                    orderItemNew.OrderGoods = new List<OrderGoodsItem>();
                                    double tradeAmount = 0;
                                    foreach (var item in kvp.Value)
                                    {
                                        tradeAmount += Convert.ToDouble(item.skuUnitPrice) * Convert.ToDouble(item.quantity);
                                        orderItemNew.OrderGoods.Add(item);
                                    }
                                    orderItemNew.tradeAmount = tradeAmount.ToString();
                                    newOrderItemList.Add(orderItemNew);
                                }
                                num++;
                            }
                        }
                        else
                        {
                            double tradeAmount = 0;
                            foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
                            {
                                tradeAmount += Convert.ToDouble(orderGoodsItem.skuUnitPrice) * Convert.ToDouble(orderGoodsItem.quantity);
                            }
                            orderItem.parentOrderId = orderItem.merchantOrderId;
                            orderItem.tradeAmount = tradeAmount.ToString();
                            newOrderItemList.Add(orderItem);
                        }
                    }
                    #endregion

                    if (msg.msg != "")
                    {
                        return msg;
                    }
                    #region 价格分拆

                    ArrayList al = new ArrayList();
                    ArrayList goodsNumAl = new ArrayList();
                    foreach (var orderItem in newOrderItemList)
                    {
                        double freight = 0, tradeAmount = 1;
                        double.TryParse(orderItem.OrderGoods[0].dr["freight"].ToString(), out freight);
                        double.TryParse(orderItem.tradeAmount, out tradeAmount);
                        orderItem.freight = Math.Round(freight, 2);
                        orderItem.platformId = orderItem.OrderGoods[0].dr["platformId"].ToString();
                        orderItem.warehouseId = orderItem.OrderGoods[0].dr["wid"].ToString();
                        orderItem.warehouseCode = orderItem.OrderGoods[0].dr["wcode"].ToString();
                        orderItem.supplier = orderItem.OrderGoods[0].dr["suppliercode"].ToString();
                        orderItem.purchaseId = orderItem.OrderGoods[0].dr["userId"].ToString();
                        orderItem.purchase = uploadParam.userId;
                        orderItem.distribution = distribution;
                        double fr = Math.Round(orderItem.freight / tradeAmount, 4);
                        for (int i = 0; i < orderItem.OrderGoods.Count; i++)
                        {
                            OrderGoodsItem orderGoodsItem = orderItem.OrderGoods[i];
                            //处理运费
                            //if (i== orderItem.OrderGoods.Count-1)
                            //{
                            //    orderGoodsItem.waybillPrice = freight;
                            //}
                            //else
                            //{
                            //    orderGoodsItem.waybillPrice = Math.Round(fr * orderGoodsItem.totalPrice,2);
                            //    freight -= orderGoodsItem.waybillPrice;
                            //}
                            //从运费平摊修改为运费都为全部运费。
                            orderGoodsItem.waybillPrice = freight;


                            //处理供货价和销售价和供货商code
                            orderGoodsItem.supplyPrice = Math.Round(Convert.ToDouble(orderGoodsItem.dr["inprice"]), 2);
                            orderGoodsItem.purchasePrice = Math.Round(Convert.ToDouble(orderGoodsItem.dr["pprice"]), 2);
                            orderGoodsItem.suppliercode = orderGoodsItem.dr["suppliercode"].ToString();
                            orderGoodsItem.slt = orderGoodsItem.dr["slt"].ToString();

                            string goodsWarehouseId = orderGoodsItem.dr["goodsWarehouseId"].ToString();//库存id
                            //处理税
                            double taxation = 0;
                            double.TryParse(orderGoodsItem.dr["taxation"].ToString(), out taxation);
                            if (taxation > 0)
                            {
                                double taxation2 = 0, taxation2type = 0, taxation2line = 0, nw = 0;
                                double.TryParse(orderGoodsItem.dr["taxation2"].ToString(), out taxation2);
                                double.TryParse(orderGoodsItem.dr["taxation2type"].ToString(), out taxation2type);
                                double.TryParse(orderGoodsItem.dr["taxation2line"].ToString(), out taxation2line);
                                double.TryParse(orderGoodsItem.dr["NW"].ToString(), out nw);
                                if (taxation2 == 0)
                                {
                                    orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                                }
                                else
                                {
                                    if (taxation2type == 1)//按总价提档
                                    {
                                        if (orderGoodsItem.skuUnitPrice > taxation2line)//价格过线
                                        {
                                            orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation2 / 100;
                                        }
                                        else//价格没过线
                                        {
                                            orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                                        }
                                    }
                                    else if (taxation2type == 2)//按元/克提档
                                    {
                                        if (nw == 0)//如果没有净重，按默认税档
                                        {
                                            orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                                        }
                                        else
                                        {
                                            if (orderGoodsItem.skuUnitPrice / (nw * 1000) > taxation2line)//价格过线
                                            {
                                                orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation2 / 100;
                                            }
                                            else//价格没过线
                                            {
                                                orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                                            }
                                        }
                                        //还要考虑面膜的问题
                                    }
                                    else//都不是按初始税率走
                                    {
                                        orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                                    }
                                }
                            }
                            else
                            {
                                orderGoodsItem.tax = 0;
                            }
                            orderGoodsItem.tax = Math.Round(orderGoodsItem.tax, 2);
                            //处理平台提点
                            orderGoodsItem.platformPrice = 0;
                            double platformCost = 0;
                            double.TryParse(orderGoodsItem.dr["platformCost"].ToString(), out platformCost);
                            if (platformCost > 0)
                            {
                                if (orderGoodsItem.dr["platformCostType"].ToString() == "1")//进价计算
                                {
                                    orderGoodsItem.platformPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * platformCost / 100;
                                }
                                else if (orderGoodsItem.dr["platformCostType"].ToString() == "2")//售价计算
                                {
                                    if (orderGoodsItem.dr["priceType"].ToString() == "1")//按订单售价计算
                                    {
                                        orderGoodsItem.platformPrice = orderGoodsItem.totalPrice * platformCost / 100;
                                    }
                                    else if (orderGoodsItem.dr["priceType"].ToString() == "2")//按供货价计算
                                    {
                                        orderGoodsItem.platformPrice = orderGoodsItem.purchasePrice * orderGoodsItem.quantity * platformCost / 100;
                                    }
                                }
                            }
                            orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);
                            //处理利润
                            //利润为供货价-进价-提点-运费分成-税
                            double profit = orderGoodsItem.purchasePrice * orderGoodsItem.quantity
                                - orderGoodsItem.supplyPrice * orderGoodsItem.quantity
                                - orderGoodsItem.platformPrice
                                - orderGoodsItem.waybillPrice
                                - orderGoodsItem.tax;
                            double profitAgent = 0, profitDealer = 0, profitOther1 = 0, profitOther2 = 0, profitOther3 = 0;
                            double.TryParse(orderGoodsItem.dr["profitAgent"].ToString(), out profitAgent);
                            double.TryParse(orderGoodsItem.dr["profitDealer"].ToString(), out profitDealer);
                            double.TryParse(orderGoodsItem.dr["profitOther1"].ToString(), out profitOther1);
                            double.TryParse(orderGoodsItem.dr["profitOther2"].ToString(), out profitOther2);
                            double.TryParse(orderGoodsItem.dr["profitOther3"].ToString(), out profitOther3);
                            orderGoodsItem.profitAgent = Math.Round(profit * profitAgent / 100, 2);
                            orderGoodsItem.profitDealer = Math.Round(profit * profitDealer / 100, 2);
                            orderGoodsItem.profitOther1 = Math.Round(profit * profitOther1 / 100, 2);
                            orderGoodsItem.profitOther2 = Math.Round(profit * profitOther2 / 100, 2);
                            orderGoodsItem.profitOther3 = Math.Round(profit * profitOther3 / 100, 2);
                            orderGoodsItem.profitPlatform = Math.Round(profit - orderGoodsItem.profitAgent
                                                            - orderGoodsItem.profitDealer - orderGoodsItem.profitOther1
                                                            - orderGoodsItem.profitOther2 - orderGoodsItem.profitOther3, 2);
                            orderGoodsItem.other1Name = orderGoodsItem.dr["profitOther1Name"].ToString();
                            orderGoodsItem.other2Name = orderGoodsItem.dr["profitOther2Name"].ToString();
                            orderGoodsItem.other3Name = orderGoodsItem.dr["profitOther3Name"].ToString();
                            string sqlgoods = "insert into t_order_goods(merchantOrderId,barCode,slt,skuUnitPrice," +
                                          "quantity,skuBillName,batchNo,goodsName," +
                                          "api,fqSkuID,sendType,status," +
                                          "suppliercode,supplyPrice,purchasePrice,waybill," +
                                          "waybillPrice,tax,platformPrice,profitPlatform," +
                                          "profitAgent,profitDealer,profitOther1,other1Name," +
                                          "profitOther2,other2Name,profitOther3,other3Name) " +
                                          "values('" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "','" + orderGoodsItem.slt + "','" + orderGoodsItem.skuUnitPrice + "'" +
                                          ",'" + orderGoodsItem.quantity + "','" + orderGoodsItem.skuBillName + "','','" + orderGoodsItem.skuBillName + "'" +
                                          ",'','','','0'" +
                                          ",'" + orderGoodsItem.suppliercode + "','" + orderGoodsItem.supplyPrice + "','" + orderGoodsItem.purchasePrice + "',''" +
                                          ",'" + orderGoodsItem.waybillPrice + "','" + orderGoodsItem.tax + "','" + orderGoodsItem.platformPrice + "','" + orderGoodsItem.profitPlatform + "'" +
                                          ",'" + orderGoodsItem.profitAgent + "','" + orderGoodsItem.profitDealer + "','" + orderGoodsItem.profitOther1 + "','" + orderGoodsItem.other1Name + "'" +
                                          ",'" + orderGoodsItem.profitOther2 + "','" + orderGoodsItem.other2Name + "','" + orderGoodsItem.profitOther3 + "','" + orderGoodsItem.other3Name + "'" +
                                          ")";
                            al.Add(sqlgoods);
                            string upsql = "update t_goods_warehouse set goodsnum = goodsnum-" + orderGoodsItem.quantity + " where id = " + goodsWarehouseId;
                            goodsNumAl.Add(upsql);
                            string logsql = "insert into t_log_goodsnum(inputType,createtime,wid,wcode,orderid,barcode,goodsnum,state) " +
                                            "values('',now(),'" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "'," +
                                            "'" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "'," +
                                            "" + orderGoodsItem.quantity + ",'" + orderItem.status + "')";
                            goodsNumAl.Add(logsql);
                        }
                        string sqlorder = "insert into t_order_list(warehouseId,warehouseCode,customerCode,actionType," +
                            "orderType,serviceType,parentOrderId,merchantOrderId," +
                            "payType,payNo,tradeTime," +
                            "tradeAmount,goodsTotalAmount,consigneeName,consigneeMobile," +
                            "addrCountry,addrProvince,addrCity,addrDistrict," +
                            "addrDetail,zipCode,idType,idNumber," +
                            "idFountImgUrl,idBackImgUrl,status,purchaserCode," +
                            "purchaserId,distributionCode,apitype,waybillno," +
                            "expressId,inputTime,fqID," +
                            "operate_status,sendapi,platformId,consignorName," +
                            "consignorMobile,consignorAddr,batchid,outNo,waybillOutNo," +
                            "accountsStatus,accountsNo,prePayId,ifPrint,printNo) " +
                            "values('" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "','" + orderItem.supplier + "',''" +
                            ",'','','" + orderItem.parentOrderId + "','" + orderItem.merchantOrderId + "'" +
                            ",'','','" + orderItem.tradeTime + "'" +
                            "," + orderItem.tradeAmount + ",'" + orderItem.tradeAmount + "','" + orderItem.consigneeName + "','" + orderItem.consigneeMobile + "'" +
                            ",'" + orderItem.addrCountry + "','" + orderItem.addrProvince + "','" + orderItem.addrCity + "','" + orderItem.addrDistrict + "'" +
                            ",'" + orderItem.addrDetail + "','','1','" + orderItem.idNumber + "'" +
                            ",'','','1','" + orderItem.purchase + "'" +
                            ",'" + orderItem.purchaseId + "','" + orderItem.distribution + "','',''" +
                            ",'',now(),''" +
                            ",'0','','" + orderItem.platformId + "','" + orderItem.consignorName + "'" +
                            ",'" + orderItem.consignorMobile + "','" + orderItem.consignorAddr + "','','',''" +
                            ",'0','','','0','') ";
                        al.Add(sqlorder);
                    }

                    #endregion


                    if (DatabaseOperationWeb.ExecuteDML(al))
                    {
                        DatabaseOperationWeb.ExecuteDML(goodsNumAl);
                        msg.msg = "导入成功";
                        msg.type = "1";
                    }
                    else
                    {
                        msg.msg = "数据保存失败！";
                    }
                }
                else
                {
                    msg.msg = "文件读取失败！";
                }
            }
            else
            {
                msg.msg = "文件上传失败！";
            }
            return msg;
        }

        /// <summary>
        /// 导出查询出来的订单
        /// </summary>
        /// <param name="orderParam">查询信息</param>
        /// <returns></returns>
        public MsgResult exportSelectOrder(OrderParam orderParam)
        {
            MsgResult msg = new MsgResult();
            string st = "";
            //处理用户账号对应的查询条件
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(orderParam.userId);
            if (userType != "0" && userType != "5")//管理员或客服
            {
                msg.msg = "权限错误，只有运营有权限";
                return msg;
            }

            if (orderParam.date != null && orderParam.date.Length == 2)
            {
                st += " and t.tradeTime BETWEEN str_to_date('" + orderParam.date[0] + "', '%Y-%m-%d') " +
                            "AND DATE_ADD(str_to_date('" + orderParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            if (orderParam.orderId != null && orderParam.orderId.Trim() != "")
            {
                st += " and t.merchantOrderId like '%" + orderParam.orderId.Trim() + "%' ";
            }
            if (orderParam.status != null && orderParam.status.Trim() != "" && orderParam.status.Trim() != "全部")
            {
                st += " and t.status = '" + orderParam.status.Trim() + "' ";
            }
            if (orderParam.wcode != null && orderParam.wcode.Trim() != "")
            {
                st += " and t.warehouseCode = '" + orderParam.wcode.Trim() + "' ";
            }
            if (orderParam.wid != null && orderParam.wid.Trim() != "")
            {
                st += " and t.warehouseId = '" + orderParam.wid.Trim() + "' ";
            }
            if (orderParam.shopId != null && orderParam.shopId.Trim() != "")
            {
                st += " and t.purchaserCode = '" + orderParam.shopId.Trim() + "' ";
            }
            if (orderParam.waybillno != null && orderParam.waybillno.Trim() != "")
            {
                st += " and t.waybillno = '" + orderParam.waybillno.Trim() + "' ";
            }
            if (orderParam.platformId != null && orderParam.platformId.Trim() != "")
            {
                st += " and t.platformId = '" + orderParam.platformId.Trim() + "' ";
            }
            if (orderParam.supplier != null && orderParam.supplier.Trim() != "")
            {
                st += " and t.customerCode in (select usercode from t_user_list ul " +
                    "where ul.email like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.tel like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.usercode like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.username like '%" + orderParam.supplier.Trim() + "%' " +
                    "or ul.company like '%" + orderParam.supplier.Trim() + "%') ";
            }
            string sql = "select '' as 序号,t.tradeTime as 订单日期,t.parentOrderId as 父订单号,t.merchantOrderId as 子订单号," +
                "t.tradeAmount as 订单销售额,g.barCode as 商品条码,g.goodsName as 商品名,g.quantity as 销量," +
                "(select username from t_user_list where usercode =customerCode) as 供应商,g.supplyPrice as 供货单价," +
                "g.supplyPrice*g.quantity as 供货额,(select username from t_user_list where usercode =purchaserCode) as 销售渠道 ," +
                "e.expressName as 平台渠道,g.purchasePrice as 销售单价,g.purchasePrice*g.quantity as 商品销售额," +
                "s.statusName as 订单状态,t.waybillno as 运单编号,t.addrCountry as 收货人国家,t.addrProvince as 收货人省," +
                "t.addrCity as 收货人市,t.addrDistrict as 收货人区,t.addrDetail as 收货人地址,t.consigneeMobile as 收货人电话," +
                "t.consigneeName as 收货人,t.idNumber as 收件人身份证号 " +
                " from t_base_status s ,t_order_goods g ,t_order_list t left join t_base_express e on t.expressId = e.expressId  " +
                " where s.statusId=t.status and t.merchantOrderId = g.merchantOrderId and t.id >5270 " + st +
                " order by t.tradeTime , t.parentOrderId ,t.merchantOrderId";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                FileManager fm = new FileManager();
                string fileName = orderParam.userId + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx";
                if (fm.writeSelectOrderToExcel(dt, fileName))
                {
                    if (fm.updateFileToOSS(fileName, Global.OssDirOrder, fileName))
                    {
                        msg.msg = Global.OssUrl + Global.OssDirOrder + fileName;
                        msg.type = "1";
                    }
                    else
                    {
                        msg.msg = "上传云端失败，请联系客服";
                    }
                }
                else
                {
                    msg.msg = "生成文档失败，请联系客服";
                }
            }
            else
            {
                msg.msg = "未查询到对应订单";
            }
            return msg;
        }
        #endregion
    }
}
