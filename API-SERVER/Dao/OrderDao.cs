﻿using Aliyun.OSS;
using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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

        #region 2019/1/30：财务-报表管理-销售日报表 ：未完成返回值、退单金额、分页的编写
        /// <summary>
        /// 获取销售日报表
        /// </summary>
        /// <param name="orderParam">查询信息</param>
        /// <returns></returns>
        //public PageResult GetSalesFrom(GetSalesFromParam gsfp, string userId)
        //{
        //    PageResult pageResult = new PageResult();
        //    pageResult.list = new List<object>();
        //    pageResult.pagination = new Page(gsfp.current, gsfp.pageSize);

        //    string sql = ""
        //        + " select a.tradeTime,(select d.username from t_user_list  d where  a.customerCode=d.usercode) customerName,c.platformType,a.merchantOrderId,(select e.username from t_user_list e where a.purchaserCode=e.usercode) purchaserName,sum(b.waybillPrice) waybillPrice,sum(b.platformPrice) platformPrice,a.payType,a.tradeAmount "
        //        + " from t_order_list a,t_order_goods b,t_base_platform c"
        //        + " where a.`status` in ('1','2','3','4','5') and a.merchantOrderId = b.merchantOrderId  and c.platformId=a.platformId"
        //        + "group by a.merchantOrderId  ORDER BY a.tradeTime DESC  limit " + (gsfp.current - 1) * gsfp.pageSize + "," + gsfp.pageSize;
        //    DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            GetSalesFromItem getSalesFromItem = new GetSalesFromItem();
        //            getSalesFromItem.customerName = dt.Rows[i]["customerName"].ToString();
        //            getSalesFromItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
        //            getSalesFromItem.platformType = dt.Rows[i]["platformType"].ToString();
        //            getSalesFromItem.merchantOrderId = dt.Rows[i]["merchantOrderId"].ToString();
        //            getSalesFromItem.purchaserName = dt.Rows[i]["purchaserName"].ToString();
        //            getSalesFromItem.waybillPrice = string.Format("{0:F2}",Convert.ToDouble(dt.Rows[i]["waybillPrice"]));
        //            getSalesFromItem.platformPrice = dt.Rows[i]["platformPrice"].ToString();
        //            getSalesFromItem.payType = dt.Rows[i]["payType"].ToString();
        //            getSalesFromItem.tradeAmount = dt.Rows[i]["tradeAmount"].ToString();
        //            getSalesFromItem.waybillBelong = Convert.ToDouble(getSalesFromItem.waybillPrice) > 0 ? "平台承担":"供货承担";                   
        //            getSalesFromItem.noTaxPrice = string.Format("{0:F2}", Convert.ToDouble(getSalesFromItem.platformPrice) / 1.06) ;

        //        }
        //    }

        //    return pageResult;
        //}
        #endregion

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
            if (orderParam.waybillno != null && orderParam.waybillno.Trim() != "")
            {
                st += " and t.waybillno = '" + orderParam.waybillno.Trim() + "' ";
            }
            if (orderParam.platformId != null && orderParam.platformId.Trim() != "")
            {
                st += " and t.purchaserCode = '" + orderParam.platformId.Trim() + "' ";
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
                    if (dt.Rows[i]["sales"].ToString() == "")
                    {
                        orderItem.sales = 0;
                    }
                    else
                    {
                        orderItem.sales = Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    }

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
                    orderItem.ifAgree = "0";
                    if (dt.Rows[i]["status"].ToString() == "6")
                    {
                        orderItem.ifAgree = "1";
                    }
                    orderItem.ifFinish = "0";
                    if (dt.Rows[i]["status"].ToString() == "7")
                    {
                        orderItem.ifFinish = "1";
                    }
                    if (ifShowConsignee)
                    {
                        orderItem.consigneeName = dt.Rows[i]["consigneeName"].ToString();
                    }

                    OrderResult.list.Add(orderItem);

                }
                string sql1 = "SELECT t.id,w.wname,status,(select username from t_user_list where usercode =customerCode) customerCode," +
                         "(select username from t_user_list where usercode =purchaserCode) purchaser,merchantOrderId," +
                         "tradeTime,e.expressName,waybillno,consigneeName,tradeAmount,s.statusName, " +
                         "(select sum(g.quantity) from t_order_goods g where g.merchantOrderId = t.merchantOrderId) sales " +
                         "FROM t_base_status s,t_order_list t left join t_base_express e on t.expressId = e.expressId  " +
                         " LEFT JOIN t_base_warehouse w on w.id= t.warehouseId " +
                         " where s.statusId=t.status " + st;

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    if (dt1.Rows[i]["sales"].ToString() != "")
                    {
                        orderTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["sales"].ToString());
                    }
                    orderTotalItem.totalTradeAmount += Convert.ToDouble(dt1.Rows[i]["tradeAmount"].ToString());
                }
                OrderResult.pagination.total = dt1.Rows.Count;
                orderTotalItem.total = dt1.Rows.Count;
                orderTotalItem.totalSales = Math.Round(orderTotalItem.totalSales, 2);
                orderTotalItem.totalTradeAmount = Math.Round(orderTotalItem.totalTradeAmount, 2);
                OrderResult.item = orderTotalItem;
            }
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
                if (orderParam.status == "待发货")
                {
                    st += " and s.statusName in ('等待发货','新订单') ";
                }
                else
                {
                    st += " and s.statusName = '" + orderParam.status.Trim() + "' ";
                }
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
                    if (dt.Rows[i]["sales"].ToString() == "")
                    {
                        orderItem.sales = 0;
                    }
                    else
                    {
                        orderItem.sales = Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    }
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
                }
                string sql1 = "SELECT t.id,w.wname,status,(select username from t_user_list where usercode =customerCode) customerCode," +
                         "(select username from t_user_list where usercode =purchaserCode) purchaser,merchantOrderId," +
                         "tradeTime,e.expressName,waybillno,consigneeName,tradeAmount,s.statusName," +
                         "(select sum(g.quantity) from t_order_goods g where g.merchantOrderId = t.merchantOrderId) sales " +
                         "FROM t_base_status s,t_order_list t left join t_base_express e on t.expressId = e.expressId  " +
                         " LEFT JOIN t_base_warehouse w on w.id= t.warehouseId " +
                         " where s.statusId=t.status " + st;

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    if (dt1.Rows[i]["sales"].ToString() != "")
                    {
                        orderTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["sales"].ToString());
                    }
                    orderTotalItem.totalTradeAmount += Convert.ToDouble(dt1.Rows[i]["tradeAmount"].ToString());
                }
                OrderResult.pagination.total = dt1.Rows.Count;
                orderTotalItem.total = dt1.Rows.Count;
                orderTotalItem.totalSales = Math.Round(orderTotalItem.totalSales, 2);
                orderTotalItem.totalTradeAmount = Math.Round(orderTotalItem.totalTradeAmount, 2);
                OrderResult.item = orderTotalItem;

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
            string sql = "select x.id,x.tradeTime,x.merchantOrderId,sum(x.tradeAmount) tradeAmount,sum(sales) sales," +
                        "group_concat(x.waybillno) waybillno ,x.statusName,sum(purchase)  purchase,sum(agent)  agent,sum(dealer)  dealer," +
                        "distributionCode,expressName,status " +
                        "from( select t.id, t.tradeTime, t.parentOrderId merchantOrderId, t.tradeAmount, sum(g.quantity) sales, t.waybillno, s.statusName," +
                        " sum((g.skuUnitPrice - g.purchasePrice) * g.quantity) purchase, sum(g.profitAgent) agent," +
                        " sum(g.profitDealer) dealer, t.distributionCode, e.expressName, t.status " +
                        "from t_order_goods g, t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
                        "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
                        " group by t.merchantOrderId) x " +
                        "GROUP BY x.merchantOrderId " +
                        "ORDER BY x.distributionCode ,x.id desc  LIMIT " + (orderParam.current - 1) * orderParam.pageSize + "," + orderParam.pageSize + ";";


            //string sql = "select t.id,t.tradeTime,t.parentOrderId merchantOrderId,t.tradeAmount,sum(g.quantity) sales,t.waybillno,s.statusName," +
            //             "sum((g.skuUnitPrice-g.purchasePrice)*g.quantity) purchase,sum(g.profitAgent) agent ," +
            //             "sum(g.profitDealer) dealer,t.distributionCode ,e.expressName,t.status " +
            //             "from t_order_goods g,t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
            //             "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
            //             " group by t.parentOrderId " +
            //             " ORDER BY t.distributionCode,t.id desc LIMIT " + (orderParam.current - 1) * orderParam.pageSize + "," + orderParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
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
                    if (dt.Rows[i]["sales"].ToString() == "")
                    {
                        orderItem.sales = 0;
                    }
                    else
                    {
                        orderItem.sales = Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    }
                    orderItem.purchaseTotal = Convert.ToDouble(dt.Rows[i]["purchase"].ToString());

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

                    pageResult.list.Add(orderItem);
                }
                string sql1 = "select x.id,x.tradeTime,x.merchantOrderId,sum(x.tradeAmount) tradeAmount,sum(sales) sales," +
                       "group_concat(x.waybillno) waybillno ,x.statusName,sum(purchase)  purchase,sum(agent)  agent,sum(dealer)  dealer," +
                       "distributionCode,expressName,status " +
                       "from( select t.id, t.tradeTime, t.parentOrderId merchantOrderId, t.tradeAmount, sum(g.quantity) sales, t.waybillno, s.statusName," +
                       " sum((g.skuUnitPrice - g.purchasePrice) * g.quantity) purchase, sum(g.profitAgent) agent," +
                       " sum(g.profitDealer) dealer, t.distributionCode, e.expressName, t.status " +
                       "from t_order_goods g, t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
                       "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
                       " group by t.merchantOrderId) x " +
                       "GROUP BY x.merchantOrderId ";
                //string sql1 = "select t.id,t.tradeTime,t.merchantOrderId,t.tradeAmount,sum(g.quantity) sales,t.waybillno,s.statusName," +
                //         "sum((g.skuUnitPrice-g.purchasePrice)*g.quantity) purchase,sum(g.profitAgent) agent ," +
                //         "sum(g.profitDealer) dealer,t.distributionCode ,e.expressName,t.status " +
                //         "from t_order_goods g,t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
                //         "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
                //         " group by t.merchantOrderId ";

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    if (dt1.Rows[i]["sales"].ToString() != "")
                    {
                        orderTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["sales"].ToString());
                    }
                    orderTotalItem.totalTradeAmount += Convert.ToDouble(dt1.Rows[i]["tradeAmount"].ToString());
                    orderTotalItem.totalPurchase += Convert.ToDouble(dt1.Rows[i]["purchase"].ToString());
                }
                pageResult.pagination.total = dt1.Rows.Count;
                orderTotalItem.total = dt1.Rows.Count;
                orderTotalItem.totalSales = Math.Round(orderTotalItem.totalSales, 2);
                orderTotalItem.totalTradeAmount = Math.Round(orderTotalItem.totalTradeAmount, 2);
                orderTotalItem.totalPurchase = Math.Round(orderTotalItem.totalPurchase, 2);
                pageResult.item = orderTotalItem;
            }
            pageResult.item = orderTotalItem;
            return pageResult;
        }

        /// <summary>
        /// 零售退货申请
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult ReGoodsApply(PayOrderParam param, string userId)
        {
            MsgResult msg = new MsgResult();
            DateTime dateTime = DateTime.Now;
            StringBuilder updatebuilder = new StringBuilder();
            updatebuilder.AppendFormat("update t_order_list set status='6', refundRemark='{2}',refundApplyTime='{3}'  where parentOrderId='{0}' and purchaserCode='{1}'", param.parentOrderId, userId, param.refundRemark, dateTime);
            string update = updatebuilder.ToString();
            if (DatabaseOperationWeb.ExecuteDML(update))
            {
                msg.type = "1";
            }
            else
            {
                msg.msg = "申请失败";
            }
            return msg;
        }

        /// <summary>
        /// 零售订单同意退货接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult AgreeReGoods(PayOrderParam param, string userId)
        {
            MsgResult msg = new MsgResult();
            StringBuilder updatebuilder = new StringBuilder();
            updatebuilder.AppendFormat("update t_order_list set status='7' where parentOrderId='{0}' and status='6' ", param.parentOrderId);
            string update = updatebuilder.ToString();
            if (DatabaseOperationWeb.ExecuteDML(update))
            {
                msg.type = "1";
            }
            else
            {
                msg.msg = "申请失败";
            }
            return msg;
        }

        /// <summary>
        /// 零售退货运单号填写接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult ReGoodsFundId(MakeSureReGoodsParam param, string userId)
        {
            MsgResult msg = new MsgResult();
            StringBuilder selectbuilder = new StringBuilder();
            selectbuilder.AppendFormat("select status from t_order_list where parentOrderId='{1}'", param.parentOrderId);
            string select = selectbuilder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(select,"T").Tables[0];
            if (dt.Rows.Count > 0 && dt.Rows[0]["status"].ToString() == "7")
            {
                StringBuilder updatebuilder = new StringBuilder();
                updatebuilder.AppendFormat("update t_order_list set refundId='{0}',refundExpressId=(select expressId from t_base_express where expressName='{2}')  where parentOrderId='{1}' and status='7' ", param.refundId, param.parentOrderId, param.refundExpressId);
                string update = updatebuilder.ToString();

                if (DatabaseOperationWeb.ExecuteDML(update))
                {
                    msg.type = "1";
                }
            }
            else
            {
                msg.msg = "无法填写运单号";
            }
            return msg;
        }

        /// <summary>
        /// 零售退货运单号信息接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MakeSureReGoodsParam ReGoodsFundIdMessage(PayOrderParam param, string userId)
        {
            MakeSureReGoodsParam Item = new MakeSureReGoodsParam();
            StringBuilder selectbuilder = new StringBuilder();
            selectbuilder.AppendFormat("select a.refundId,b.expressName from t_base_express b,t_order_list a where a.refundExpressId=b.expressId and a.parentOrderId='{0}'", param.parentOrderId);
            string select = selectbuilder.ToString();
            DataTable dtselect = DatabaseOperationWeb.ExecuteSelectDS(select,"T").Tables[0];
            if (dtselect.Rows.Count>0)
            {
                Item.refundId = dtselect.Rows[0]["refundId"].ToString();
                Item.expressName= dtselect.Rows[0]["expressName"].ToString();
                Item.type = 1;
            }
            return Item;
        }

        /// <summary>
        /// 零售订单退货成功接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult MakeSureReGoods(PayOrderParam param, string userId)
        {
            MsgResult msg = new MsgResult();
            DateTime dateTime = DateTime.Now;
            StringBuilder selectbuilder = new StringBuilder();
            selectbuilder.AppendFormat("select refundId from t_order_list where parentOrderId='{0}' ", param.parentOrderId);
            string select = selectbuilder.ToString();
            DataTable dtselect = DatabaseOperationWeb.ExecuteSelectDS(select, "T").Tables[0];
            string refundId = "";
            if (dtselect.Rows.Count > 0)
            {
                refundId = (dtselect.Rows[0][0]==DBNull.Value)?"": dtselect.Rows[0][0].ToString();
            }
            if (refundId!="" && refundId!=null)
            {
                StringBuilder updatebuilder = new StringBuilder();
                updatebuilder.AppendFormat("update t_order_list set refundTime='{0}',status='-1'  where parentOrderId='{1}' and status='7' ", dateTime, param.parentOrderId);
                string update = updatebuilder.ToString();
                DatabaseOperationWeb.ExecuteDML(update);
                //填入结算分拆表中
                string sqlAA = "select * from t_account_analysis where status='1' and merchantOrderId = '" + param.parentOrderId + "'";
                DataTable dtAA = DatabaseOperationWeb.ExecuteSelectDS(sqlAA, "t_account_analysis").Tables[0];
                if (dtAA.Rows.Count == 0)
                {
                    string sqlGoods = "select sum(skuUnitPrice*quantity) as price, sum(g.supplyPrice*quantity) as supplyPrice, " +
                        "sum(g.purchasePrice*quantity) as purchasePrice , sum(g.waybillPrice) as waybillPrice,sum(g.tax) as tax," +
                        "sum(g.platformPrice) as platformPrice,sum(g.supplierAgentPrice) as supplierAgentPrice," +
                        "sum(g.purchaseAgentPrice) as purchaseAgentPrice, sum(g.profitPlatform) as profitPlatform," +
                        "sum(g.profitAgent) as profitAgent,sum(g.profitDealer) as profitDealer,sum(g.profitOther1) as profitOther1," +
                        "sum(g.profitOther2) as profitOther2,sum(g.profitOther3) as profitOther3 " +
                        "from t_order_list o,t_order_goods g " +
                        "where o.merchantOrderId = g.merchantOrderId and o.merchantOrderId = '" + param.parentOrderId + "'";
                    DataTable dtGoods = DatabaseOperationWeb.ExecuteSelectDS(sqlGoods, "t_account_analysis").Tables[0];
                    if (dtGoods.Rows.Count > 0)
                    {

                        string insql = "insert into t_account_analysis(merchantOrderId,createTime,price,supplyPrice,purchasePrice," +
                        "waybillPrice,tax,platformPrice,supplierAgentPrice," +
                        "purchaseAgentPrice,profitPlatform,profitAgent,profitDealer," +
                        "profitOther1,profitOther2,profitOther3,status) " +
                        "values('" + param.parentOrderId + "',now()," + dtGoods.Rows[0]["price"].ToString() + "," + dtGoods.Rows[0]["supplyPrice"].ToString() + "," + dtGoods.Rows[0]["purchasePrice"].ToString() + ","
                        + dtGoods.Rows[0]["waybillPrice"].ToString() + "," + dtGoods.Rows[0]["tax"].ToString() + "," + dtGoods.Rows[0]["platformPrice"].ToString() + "," + dtGoods.Rows[0]["supplierAgentPrice"].ToString() + ","
                        + dtGoods.Rows[0]["purchaseAgentPrice"].ToString() + "," + dtGoods.Rows[0]["profitPlatform"].ToString() + "," + dtGoods.Rows[0]["profitAgent"].ToString() + "," + dtGoods.Rows[0]["profitDealer"].ToString() + ","
                        + dtGoods.Rows[0]["profitOther1"].ToString() + "," + dtGoods.Rows[0]["profitOther2"].ToString() + "," + dtGoods.Rows[0]["profitOther3"].ToString() + ",'1') ";
                        DatabaseOperationWeb.ExecuteDML(insql);
                        msg.type = "1";
                    }
                }
                else
                {
                    msg.msg = "已有退款记录，请联系客服";
                }
            }
            else
            {
                msg.msg = "请先填写退货运单号";
            }
            return msg;
        }

        /// <summary>
        /// 零售退货信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ReGoodsMessageItem ReGoodsMessage(PayOrderParam param, string userId)
        {
            ReGoodsMessageItem msg = new ReGoodsMessageItem();
            StringBuilder selectbuilder = new StringBuilder();
            selectbuilder.AppendFormat("select a.refundRemark,a.purchaserCode,b.tel purchaserTel,a.customerCode,c.tel customerTel from t_order_list a,t_user_list b,t_user_list c "
                + " where a.parentOrderId='{0}'  and  a.purchaserCode=b.usercode and a.customerCode=c.usercode   ", param.parentOrderId, userId);
            string select = selectbuilder.ToString();
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(select,"T").Tables[0];
            if(dt.Rows.Count>0)
            {
                msg.customerCode = dt.Rows[0]["customerCode"].ToString();
                msg.customerTel= dt.Rows[0]["customerTel"].ToString();
                msg.purchaserCode= dt.Rows[0]["purchaserCode"].ToString();
                msg.purchaserTel= dt.Rows[0]["purchaserTel"].ToString();
                msg.refundRemark = dt.Rows[0]["refundRemark"].ToString();
            }
            
            return msg;
        }


        /// <summary>
        /// 获取订单列表-零售
        /// </summary>
        /// <param name="orderParam">查询信息</param>
        /// <param name="apiType">订单的类别，空白就不区分类别</param>
        /// <param name="ifShowConsignee">是否显示收货人信息</param>
        /// <returns></returns>
        public PageResult getOrderListOfRetail(OrderParam orderParam, string apiType, bool ifShowConsignee,string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(orderParam.current, orderParam.pageSize);
            pageResult.list = new List<Object>();
            OrderTotalItem orderTotalItem = new OrderTotalItem();
            string st = " and t.purchaserCode='" + userId + "' ";
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
            string sql = "select x.id,x.tradeTime,x.merchantOrderId,sum(x.tradeAmount) tradeAmount,sum(sales) sales," +
                        "group_concat(x.waybillno) waybillno ,x.statusName,sum(purchase)  purchase,sum(agent)  agent,sum(dealer)  dealer," +
                        "distributionCode,expressName,status " +
                        "from( select t.id, t.tradeTime, t.parentOrderId merchantOrderId, t.tradeAmount, sum(g.quantity) sales, t.waybillno, s.statusName," +
                        " sum((g.skuUnitPrice - g.purchasePrice) * g.quantity) purchase, sum(g.profitAgent) agent," +
                        " sum(g.profitDealer) dealer, t.distributionCode, e.expressName, t.status " +
                        "from t_order_goods g, t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
                        "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
                        " group by t.merchantOrderId) x " +
                        "GROUP BY x.merchantOrderId " +
                        "ORDER BY x.distributionCode ,x.id desc  LIMIT " + (orderParam.current - 1) * orderParam.pageSize + "," + orderParam.pageSize + ";";
            

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            string moneySql = ""
                + "select fund "
                + " from t_user_list "
                + " where usercode='" + userId + "'";
            DataTable dtmoneySql = DatabaseOperationWeb.ExecuteSelectDS(moneySql, "T").Tables[0];
            orderTotalItem.accountBalance = 0;
            if (dtmoneySql.Rows.Count>0 && dtmoneySql.Rows[0][0] != DBNull.Value)
            {
                orderTotalItem.accountBalance = Convert.ToDouble(dtmoneySql.Rows[0][0]);
            }
            if (dt.Rows.Count > 0)
            {
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

                    if (dt.Rows[i]["sales"].ToString() == "")
                    {
                        orderItem.sales = 0;
                    }
                    else
                    {
                        orderItem.sales = Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    }
                    orderItem.purchaseTotal = Convert.ToDouble(dt.Rows[i]["purchase"].ToString());

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

                    pageResult.list.Add(orderItem);
                }
                string sql1 = "select x.id,x.tradeTime,x.merchantOrderId,sum(x.tradeAmount) tradeAmount,sum(sales) sales," +
                       "group_concat(x.waybillno) waybillno ,x.statusName,sum(purchase)  purchase,sum(agent)  agent,sum(dealer)  dealer," +
                       "distributionCode,expressName,status " +
                       "from( select t.id, t.tradeTime, t.parentOrderId merchantOrderId, t.tradeAmount, sum(g.quantity) sales, t.waybillno, s.statusName," +
                       " sum((g.skuUnitPrice - g.purchasePrice) * g.quantity) purchase, sum(g.profitAgent) agent," +
                       " sum(g.profitDealer) dealer, t.distributionCode, e.expressName, t.status " +
                       "from t_order_goods g, t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
                       "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
                       " group by t.merchantOrderId) x " +
                       "GROUP BY x.merchantOrderId ";
               

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    if (dt1.Rows[i]["sales"].ToString() != "")
                    {
                        orderTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["sales"].ToString());
                    }
                    orderTotalItem.totalTradeAmount += Convert.ToDouble(dt1.Rows[i]["tradeAmount"].ToString());
                    orderTotalItem.totalPurchase += Convert.ToDouble(dt1.Rows[i]["purchase"].ToString());
                }
                pageResult.pagination.total = dt1.Rows.Count;
                orderTotalItem.total = dt1.Rows.Count;
                orderTotalItem.totalSales = Math.Round(orderTotalItem.totalSales, 2);
                orderTotalItem.totalTradeAmount = Math.Round(orderTotalItem.totalTradeAmount, 2);
                orderTotalItem.totalPurchase = Math.Round(orderTotalItem.totalPurchase, 2);
                         
                pageResult.item = orderTotalItem;               
            }
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

            string sql = "select x.id,x.tradeTime,x.merchantOrderId,sum(x.tradeAmount) tradeAmount,sum(sales) sales," +
                "group_concat(x.waybillno) waybillno ,x.statusName,sum(purchase)  purchase,sum(agent)  agent,sum(dealer)  dealer," +
                "distributionCode,expressName,status " +
                "from( select t.id, t.tradeTime, t.parentOrderId merchantOrderId, t.tradeAmount, sum(g.quantity) sales, " +
                "t.waybillno, s.statusName, sum((g.skuUnitPrice - g.purchasePrice) * g.quantity) purchase, " +
                "sum(g.profitAgent) agent, sum(g.profitDealer) dealer, t.distributionCode, e.expressName, t.status " +
                "from t_order_goods g, t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
                "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
                " group by t.merchantOrderId) x " +
                "GROUP BY x.merchantOrderId " +
                "ORDER BY x.distributionCode ,x.id desc  LIMIT " + (orderParam.current - 1) * orderParam.pageSize + "," + orderParam.pageSize + ";";


            //string sql = "select t.id,t.tradeTime,t.parentOrderId merchantOrderId,t.tradeAmount,sum(g.quantity) sales,t.waybillno,s.statusName," +
            //             "sum((g.skuUnitPrice-g.purchasePrice)*g.quantity) purchase,sum(g.profitAgent) agent ," +
            //             "sum(g.profitDealer) dealer,t.distributionCode ,e.expressName,t.status " +
            //             "from t_order_goods g,t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
            //             "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
            //             " group by t.parentOrderId " +
            //             " ORDER BY t.distributionCode,t.id desc LIMIT " + (orderParam.current - 1) * orderParam.pageSize + "," + orderParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
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
                    if (dt.Rows[i]["sales"].ToString() == "")
                    {
                        orderItem.sales = 0;
                    }
                    else
                    {
                        orderItem.sales = Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    }
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

                    pageResult.list.Add(orderItem);
                }
                string distribution = dt.Rows[0]["distributionCode"].ToString();
                orderTotalItem.totalDistribution = 1;

                string sql1 = "select x.id,x.tradeTime,x.merchantOrderId,sum(x.tradeAmount) tradeAmount,sum(sales) sales," +
               "group_concat(x.waybillno) waybillno ,x.statusName,sum(purchase)  purchase,sum(agent)  agent,sum(dealer)  dealer," +
               "distributionCode,expressName,status " +
               "from( select t.id, t.tradeTime, t.parentOrderId merchantOrderId, t.tradeAmount, sum(g.quantity) sales, " +
               "t.waybillno, s.statusName, sum((g.skuUnitPrice - g.purchasePrice) * g.quantity) purchase, " +
               "sum(g.profitAgent) agent, sum(g.profitDealer) dealer, t.distributionCode, e.expressName, t.status " +
               "from t_order_goods g, t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
               "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
               " group by t.merchantOrderId) x " +
               "GROUP BY x.merchantOrderId ";

                //string sql1 = "select t.id,t.tradeTime,t.merchantOrderId,t.tradeAmount,sum(g.quantity) sales,t.waybillno,s.statusName," +
                //         "sum((g.skuUnitPrice-g.purchasePrice)*g.quantity) purchase,sum(g.profitAgent) agent ," +
                //         "sum(g.profitDealer) dealer,t.distributionCode ,e.expressName,t.status " +
                //         "from t_order_goods g,t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
                //         "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
                //         " group by t.merchantOrderId ";

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    if (dt1.Rows[i]["sales"].ToString() != "")
                    {
                        orderTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["sales"].ToString());
                    }
                    orderTotalItem.totalTradeAmount += Convert.ToDouble(dt1.Rows[i]["tradeAmount"].ToString());
                    //orderTotalItem.totalPurchase += Convert.ToDouble(dt.Rows[i]["purchase"].ToString());
                    orderTotalItem.totalAgent += Convert.ToDouble(dt1.Rows[i]["agent"].ToString());
                    orderTotalItem.totalDealer += Convert.ToDouble(dt1.Rows[i]["dealer"].ToString());

                    if (dt1.Rows[i]["distributionCode"].ToString() != distribution)
                    {
                        orderTotalItem.totalDistribution += 1;
                        distribution = dt1.Rows[i]["distributionCode"].ToString();
                    }
                }
                pageResult.pagination.total = dt1.Rows.Count;
                orderTotalItem.total = dt1.Rows.Count;
                orderTotalItem.totalSales = Math.Round(orderTotalItem.totalSales, 2);
                orderTotalItem.totalTradeAmount = Math.Round(orderTotalItem.totalTradeAmount, 2);
                orderTotalItem.totalAgent = Math.Round(orderTotalItem.totalAgent, 2);
                orderTotalItem.totalDealer = Math.Round(orderTotalItem.totalDealer, 2);
                pageResult.item = orderTotalItem;
            }
            pageResult.item = orderTotalItem;
            return pageResult;
        }

        /// <summary>
        /// 零售订单支付
        /// </summary>
        /// <param name="orderParam">查询信息</param>
        /// <param name="apiType">订单的类别，空白就不区分类别</param>
        /// <param name="ifShowConsignee">是否显示收货人信息</param>
        /// <returns></returns>
        public MsgResult PayOrder(PayOrderParam payOrderParam, string userId)
        {
            MsgResult msgResult = new MsgResult();
            DateTime dateTime = DateTime.Now;
            string createTime = dateTime.ToString("yyyy-MM-dd hh:mm:ss");
            string fundId = userId + dateTime.ToString("yyyyMMddhhmmssff");
            double fundprice = 0;
            double newfund = 0;
            string selectFundPrice = ""
                + "select  a.tradeAmount,b.fund"
                + " from t_order_list a,t_user_list b "
                + " where a.purchaserCode=b.userCode and a.parentOrderId='"+ payOrderParam.parentOrderId + "'";
            DataTable dtselectFundPrice = DatabaseOperationWeb.ExecuteSelectDS(selectFundPrice,"T").Tables[0];
            if (dtselectFundPrice.Rows.Count > 0)
            {
                fundprice = Convert.ToDouble(dtselectFundPrice.Rows[0]["tradeAmount"]);
                newfund = Math.Round((Convert.ToDouble(dtselectFundPrice.Rows[0]["fund"]) - fundprice), 2);
                if (newfund < 0)
                {
                    msgResult.msg = "余额不足";
                }
                else
                {
                    ArrayList arrayList = new ArrayList();
                    string updateUserFund = ""
                        + " insert t_user_fund(usercode,fundId,createTime,fundtype,fundprice,newfund,paytime,orderId,inputUser,status) "
                        + " values('" + userId + "','" + fundId + "','" + createTime + "','2','" + fundprice + "','" + newfund + "','" + createTime + "','" + payOrderParam.parentOrderId + "','" + userId + "','1')";
                    arrayList.Add(updateUserFund);

                    string updateOrderList = ""
                        + " update t_order_list set status='1' "
                        + " where parentOrderId='" + payOrderParam.parentOrderId + "'";
                    arrayList.Add(updateOrderList);

                    string updateUserList = ""
                        + "update t_user_list set fund='" + newfund + "'"
                        + " where userCode='" + userId + "'";
                    arrayList.Add(updateUserList);

                    if (DatabaseOperationWeb.ExecuteDML(arrayList))
                    {
                        msgResult.type = "1";
                    }
                    else
                    {
                        msgResult.msg = "系统错误，请联系客服";
                    }
                }
            }
            else
            {
                msgResult.msg = "余额不足";
            }
            return msgResult;
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
                    if (dt.Rows[i]["sales"].ToString() == "")
                    {
                        orderItem.sales = 0;
                    }
                    else
                    {
                        orderItem.sales = Convert.ToDouble(dt.Rows[i]["sales"].ToString());
                    }
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

                    pageResult.list.Add(orderItem);
                }
                string sql1 = "select t.id,t.tradeTime,t.merchantOrderId,t.tradeAmount,sum(g.quantity) sales,t.waybillno,s.statusName," +
                         "sum((g.skuUnitPrice-g.purchasePrice)*g.quantity) purchase,sum(g.profitAgent) agent ," +
                         "sum(g.profitDealer) dealer,t.distributionCode ,e.expressName,t.status " +
                         "from t_order_goods g,t_base_status s, t_order_list t left join t_base_express e on t.expressId = e.expressId " +
                         "where t.merchantOrderId = g.merchantOrderId and s.statusId = t.`status`  " + st +
                         " group by t.merchantOrderId ";

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    if (dt1.Rows[i]["sales"].ToString() != "")
                    {
                        orderTotalItem.totalSales += Convert.ToDouble(dt1.Rows[i]["sales"].ToString());
                    }
                    orderTotalItem.totalTradeAmount += Convert.ToDouble(dt1.Rows[i]["tradeAmount"].ToString());
                    orderTotalItem.totalDealer += Convert.ToDouble(dt1.Rows[i]["dealer"].ToString());
                }
                pageResult.pagination.total = dt1.Rows.Count;
                orderTotalItem.total = dt1.Rows.Count;
                orderTotalItem.totalSales = Math.Round(orderTotalItem.totalSales, 2);
                orderTotalItem.totalTradeAmount = Math.Round(orderTotalItem.totalTradeAmount, 2);
                orderTotalItem.totalDealer = Math.Round(orderTotalItem.totalDealer, 2);
                pageResult.item = orderTotalItem;
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
        /// <summary>
        /// 查询单个订单的详情-用父订单号
        /// </summary>
        /// <param name="orderParam">查询信息</param>
        /// <param name="ifShowConsignee">是否显示收货人信息</param>
        /// <returns></returns>
        public OrderItem getOrderItemByParent(OrderParam orderParam, bool ifShowConsignee)
        {
            OrderItem orderItem = new OrderItem();
            string sql1 = "select id,parentOrderId as merchantOrderId,tradeAmount,tradeTime,waybillno,idNumber," +
                "consigneeName,consigneeMobile,addrProvince,addrCity,addrDistrict,addrDetail  " +
                "FROM t_order_list where parentOrderId  = '" + orderParam.orderId + "'";
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
                string sql2 = "select g.id,slt,barCode,IFNULL(skuUnitPrice,0) as skuUnitPrice,skuBillName,IFNULL(quantity,0) as quantity," +
                    "IFNULL(purchasePrice,0) as purchasePrice,IFNULL(profitAgent,0) as profitAgent,IFNULL(profitDealer,0) as profitDealer," +
                    "(IFNULL(skuUnitPrice,0)-IFNULL(purchasePrice,0))* IFNULL(quantity,0) as purchaseP " +
                    "from t_order_goods g,t_order_list o where o.merchantOrderId= g.merchantOrderId and  o.parentOrderId  = '" + orderParam.orderId + "'";
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


        /// <summary>
        /// 获取发货信息
        /// </summary>
        /// <param name="singleWaybillParam"></param>
        /// <returns></returns>
        public GetConsigneeMsgItem GetConsigneeMsg(GetConsigneeMsgParam gcmp, string userId)
        {
            GetConsigneeMsgItem gcmi = new GetConsigneeMsgItem();
            string sql = ""
                + " select  consigneeName,consigneeMobile,addrCountry,addrProvince,addrCity,addrDistrict,addrDetail"
                + " from t_order_list "
                + " where merchantOrderId='" + gcmp.merchantOrderId + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                gcmi.consigneeName = dt.Rows[0]["consigneeName"].ToString();
                gcmi.consigneeMobile = dt.Rows[0]["consigneeMobile"].ToString();
                gcmi.consigneeAdr = dt.Rows[0]["addrCountry"].ToString() + dt.Rows[0]["addrProvince"].ToString() + dt.Rows[0]["addrCity"].ToString() + dt.Rows[0]["addrDistrict"].ToString() + dt.Rows[0]["addrDetail"].ToString();
            }

            return gcmi;
        }




        /// <summary>
        /// 确认发货
        /// </summary>
        /// <param name="singleWaybillParam"></param>
        /// <returns></returns>
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
                    double inputFreight = 0;
                    double.TryParse(singleWaybillParam.inputFreight, out inputFreight);
                    string upsql = "update t_order_list set  status=3,expressId = '" + singleWaybillParam.expressId + "'," +
                           "waybillno= '" + singleWaybillParam.waybillno + "',waybilltime=now() ,inputFreight=" + inputFreight + " " +
                           " where merchantOrderId='" + singleWaybillParam.orderId + "' " + st;
                    if (DatabaseOperationWeb.ExecuteDML(upsql))
                    {
                        //填入结算分拆表中
                        string sqlAA = "select * from t_account_analysis where status='0' and merchantOrderId = '" + singleWaybillParam.orderId + "'";
                        DataTable dtAA = DatabaseOperationWeb.ExecuteSelectDS(sqlAA, "t_account_analysis").Tables[0];
                        if (dtAA.Rows.Count == 0)
                        {
                            string sqlGoods = "select sum(skuUnitPrice*quantity) as price, sum(g.supplyPrice*quantity) as supplyPrice, " +
                                "sum(g.purchasePrice*quantity) as purchasePrice , sum(g.waybillPrice) as waybillPrice,sum(g.tax) as tax," +
                                "sum(g.platformPrice) as platformPrice,sum(g.supplierAgentPrice) as supplierAgentPrice," +
                                "sum(g.purchaseAgentPrice) as purchaseAgentPrice, sum(g.profitPlatform) as profitPlatform," +
                                "sum(g.profitAgent) as profitAgent,sum(g.profitDealer) as profitDealer,sum(g.profitOther1) as profitOther1," +
                                "sum(g.profitOther2) as profitOther2,sum(g.profitOther3) as profitOther3 " +
                                "from t_order_list o,t_order_goods g " +
                                "where o.merchantOrderId = g.merchantOrderId and o.merchantOrderId = '" + singleWaybillParam.orderId + "'";
                            DataTable dtGoods = DatabaseOperationWeb.ExecuteSelectDS(sqlGoods, "t_account_analysis").Tables[0];
                            if (dtGoods.Rows.Count > 0)
                            {
                                //double price = ;//售价
                                //double supplyPrice = 0;//售价
                                //double purchasePrice = 0;//售价
                                //double waybillPrice = 0;//售价
                                //double tax = 0;//售价
                                //double platformPrice = 0;//售价
                                //double supplierAgentPrice = 0;//售价
                                //double purchaseAgentPrice = 0;//售价
                                //double profitPlatform = 0;//售价
                                //double profitAgent = 0;//售价
                                //double profitDealer = 0;//售价
                                //double profitOther1 = 0;//售价
                                //double profitOther2 = 0;//售价
                                //double profitOther3 = 0;//售价


                                string insql = "insert into t_account_analysis(merchantOrderId,createTime,price,supplyPrice,purchasePrice," +
                                "waybillPrice,tax,platformPrice,supplierAgentPrice," +
                                "purchaseAgentPrice,profitPlatform,profitAgent,profitDealer," +
                                "profitOther1,profitOther2,profitOther3) " +
                                "values('" + singleWaybillParam.orderId + "',now()," + dtGoods.Rows[0]["price"].ToString() + "," + dtGoods.Rows[0]["supplyPrice"].ToString() + "," + dtGoods.Rows[0]["purchasePrice"].ToString() + ","
                                + dtGoods.Rows[0]["waybillPrice"].ToString() + "," + dtGoods.Rows[0]["tax"].ToString() + "," + dtGoods.Rows[0]["platformPrice"].ToString() + "," + dtGoods.Rows[0]["supplierAgentPrice"].ToString() + ","
                                + dtGoods.Rows[0]["purchaseAgentPrice"].ToString() + "," + dtGoods.Rows[0]["profitPlatform"].ToString() + "," + dtGoods.Rows[0]["profitAgent"].ToString() + "," + dtGoods.Rows[0]["profitDealer"].ToString() + ","
                                + dtGoods.Rows[0]["profitOther1"].ToString() + "," + dtGoods.Rows[0]["profitOther2"].ToString() + "," + dtGoods.Rows[0]["profitOther3"].ToString() + ") ";
                                DatabaseOperationWeb.ExecuteDML(insql);
                            }
                        }
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

        public List<CustomsStateItem> getCustomsState(string orderId)
        {
            List<CustomsStateItem> list = new List<CustomsStateItem>();
            string sql = "select * from t_log_reportstatus where orderNo = '" + orderId + "' order by id asc";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CustomsStateItem customsStateItem = new CustomsStateItem();
                customsStateItem.applyTime = dt.Rows[i]["applyTime"].ToString();
                customsStateItem.orderNo = dt.Rows[i]["orderNo"].ToString();
                customsStateItem.wayBillNo = dt.Rows[i]["wayBillNo"].ToString();
                customsStateItem.logisticsName = dt.Rows[i]["logisticsName"].ToString();
                customsStateItem.notes = dt.Rows[i]["notes"].ToString();
                customsStateItem.ratifyDate = dt.Rows[i]["ratifyDate"].ToString();

                list.Add(customsStateItem);
            }
            return list;
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
            string sqlwh = "select apitype from t_base_warehouse where id =" + orderParam.wid;
            DataTable dtwh = DatabaseOperationWeb.ExecuteSelectDS(sqlwh, "t_base_warehouse").Tables[0];
            if (dtwh.Rows.Count > 0)
            {
                string sql = "";
                if (dtwh.Rows[0]["apitype"].ToString() == "0")//普通模板
                {
                    sql = "select t.consigneeName as '收货人',t.consigneeMobile as '收货人电话',t.idNumber as '身份证号', " +
                             "concat_ws('',t.addrCountry,t.addrProvince,t.addrCity,t.addrDistrict,t.addrDetail) as '地址'," +
                             "t.merchantOrderId as '订单号', g.barCode as '商品条码',g.goodsName as '商品名',g.quantity as '数量'," +
                             "g.supplyPrice as '供货价' ,'' as '快递公司', '' as '运单号', '' as '快递费' " +
                             "from t_order_list t ,t_order_goods g " +
                             "where t.merchantOrderId = g.merchantOrderId and t.warehouseId ='" + orderParam.wid + "' " +
                             "and (t.status = 1 or t.status= 2 or (t.status= 3 and waybillno= '海外已出库' )) and t.apitype='1' " + st;
                }
                else if (dtwh.Rows[0]["apitype"].ToString() == "1")//丰趣模板
                {
                    sql = "select '' as '序号', t.consigneeName as '收货人(Name)',t.consigneeMobile as '收货人电话(Mobile)',t.idNumber as '收件人身份证号(ID NO#)', " +
                                "t.addrCountry as '收货人国家(Country)',t.addrProvince as '收货人省(State)',t.addrCity as '收货人市(City)'," +
                                "t.addrDistrict as '收货人区(District)',t.addrDetail as '收货人地址(Address)',tradeTime as '创建时间'," +
                                "tradeTime as '订单成功推送时间(DailyOrder Date)',t.merchantOrderId as '销售订单号(Order #)'," +
                                "t.merchantOrderId as '销售子单号(Sub Order #)',g.barCode as '商品条码(UPCCode)',g.goodsName as '商品名称(Product_CH)'," +
                                "g.quantity as '商品数量(QTY)',g.supplyPrice as '商品申报单价(Declare Price)','' as '物流订单号(Logistics#)' " +
                                "from t_order_list t ,t_order_goods g  " +
                                "where t.merchantOrderId = g.merchantOrderId and t.warehouseId ='" + orderParam.wid + "' " +
                                "and (t.status = 1 or t.status= 2 or (t.status= 3 and waybillno= '海外已出库' )) and t.apitype='1' " + st;

                }
                else if (dtwh.Rows[0]["apitype"].ToString() == "2")//海外报关--等调整
                {
                    sql = "select t.consigneeName as '收货人',t.consigneeMobile as '收货人电话',t.idNumber as '身份证号', " +
                             "concat_ws('',t.addrCountry,t.addrProvince,t.addrCity,t.addrDistrict,t.addrDetail) as '地址'," +
                             "t.merchantOrderId as '订单号', g.barCode as '商品条码',g.goodsName as '商品名',g.quantity as '数量'," +
                             "g.supplyPrice as '供货价' ,'' as '快递公司', '' as '运单号', '' as '快递费' " +
                             "from t_order_list t ,t_order_goods g " +
                             "where t.merchantOrderId = g.merchantOrderId and t.warehouseId ='" + orderParam.wid + "' " +
                             "and (t.status = 1 or t.status= 2 or (t.status= 3 and waybillno= '海外已出库' )) and t.apitype='1' " + st;
                }
                else //其他--等调整
                {
                    sql = "select t.consigneeName as '收货人',t.consigneeMobile as '收货人电话',t.idNumber as '身份证号', " +
                             "concat_ws('',t.addrCountry,t.addrProvince,t.addrCity,t.addrDistrict,t.addrDetail) as '地址'," +
                             "t.merchantOrderId as '订单号', g.barCode as '商品条码',g.goodsName as '商品名',g.quantity as '数量'," +
                             "g.supplyPrice as '供货价' ,'' as '快递公司', '' as '运单号', '' as '快递费' " +
                             "from t_order_list t ,t_order_goods g " +
                             "where t.merchantOrderId = g.merchantOrderId and t.warehouseId ='" + orderParam.wid + "' " +
                             "and (t.status = 1 or t.status= 2 or (t.status= 3 and waybillno= '海外已出库' )) and t.apitype='1' " + st;
                }

                DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_ticket").Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dtwh.Rows[0]["apitype"].ToString() == "1")
                    {
                        for (int i = 1; i <= dt.Rows.Count; i++)
                        {
                            dt.Rows[i - 1]["序号"] = i.ToString();
                        }
                    }
                    FileManager fm = new FileManager();
                    string info = fm.writeDataTableToExcel1(dt, fileName);
                    if (info == "true")
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
                        msg.msg = info;
                    }
                }
                else
                {
                    msg.msg = "没有需要导出的订单！";
                }
            }
            else
            {
                msg.msg = "没有找到对应的仓库！";
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
            else if (userType == "2" || userType == "3")//采购和代理
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
                         "FROM t_order_list t left join t_base_express e on t.expressId = e.expressId where t.apitype='1' " + st;

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
                    if (!dt.Columns.Contains("快递费"))
                    {
                        msg.msg += "缺少“快递费”列，";
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
                                string freight ="0";
                                if (dt.Rows[i]["快递费"].ToString()!=null && dt.Rows[i]["快递费"].ToString() != "")
                                {
                                    freight = dt.Rows[i]["快递费"].ToString();
                                }
                                string upsql = "update t_order_list set  status=3,expressId = '" + expressId + "'," +
                                       "waybillno= '" + dt.Rows[i]["运单号"].ToString() + "',waybilltime=now() ,freight='"+ freight + "'" +
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

                    if (msg.msg != "")
                    {
                        return msg;
                    }
                    #endregion
                    #region 改为调用公共方法处理 20181104 -韩明

                    //#region 处理因仓库分单
                    //List<OrderItem> newOrderItemList = new List<OrderItem>();
                    //foreach (var orderItem in OrderItemList)
                    //{
                    //    Dictionary<int, List<OrderGoodsItem>> myDictionary = new Dictionary<int, List<OrderGoodsItem>>();
                    //    foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
                    //    {
                    //        string wsql = "select d.platformId,u.id as userId,u.platformCost,u.platformCostType,u.priceType," +
                    //                      "d.pprice,d.profitPlatform,d.profitAgent,d.profitDealer,d.profitOther1," +
                    //                      "d.profitOther1Name,d.profitOther2,d.profitOther2Name,d.profitOther3," +
                    //                      "d.profitOther3Name,w.id as goodsWarehouseId,w.wid,w.wcode,w.goodsnum,w.inprice,bw.taxation,bw.taxation2," +
                    //                      "bw.taxation2type,bw.taxation2line,bw.freight,w.suppliercode,g.NW,g.slt " +
                    //                      "from t_goods_distributor_price d ,t_goods_warehouse w,t_base_warehouse bw," +
                    //                      "t_goods_list g,t_user_list u   " +
                    //                      "where u.usercode = d.usercode and g.barcode = d.barcode and w.wid = bw.id " +
                    //                      "and d.barcode = w.barcode and w.supplierid = d.supplierid and d.wid = bw.id " +
                    //                      "and d.usercode = '" + uploadParam.userId + "' " +
                    //                      "and d.barcode = '" + orderGoodsItem.barCode + "' and w.goodsnum >=" + orderGoodsItem.quantity +
                    //                      " order by w.goodsnum asc";
                    //        DataTable wdt = DatabaseOperationWeb.ExecuteSelectDS(wsql, "TABLE").Tables[0];
                    //        int wid = 0;
                    //        if (wdt.Rows.Count == 1)
                    //        {
                    //            wid = Convert.ToInt16(wdt.Rows[0]["wid"]);
                    //            orderGoodsItem.dr = wdt.Rows[0];
                    //        }
                    //        else if (wdt.Rows.Count > 1)
                    //        {
                    //            wid = Convert.ToInt16(wdt.Rows[0]["wid"]);
                    //            orderGoodsItem.dr = wdt.Rows[0];
                    //            for (int i = 0; i < wdt.Rows.Count; i++)
                    //            {
                    //                if (myDictionary.ContainsKey(Convert.ToInt16(wdt.Rows[i]["wid"])))
                    //                {
                    //                    wid = Convert.ToInt16(wdt.Rows[i]["wid"]);
                    //                    orderGoodsItem.dr = wdt.Rows[i];
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            msg.msg += "序号为" + orderGoodsItem.id + "行没找到对应默认供货信息，请核对\r\n";
                    //            continue;
                    //        }
                    //        if (!myDictionary.ContainsKey(wid))
                    //        {
                    //            myDictionary.Add(wid, new List<OrderGoodsItem>());
                    //        }
                    //        myDictionary[wid].Add(orderGoodsItem);
                    //    }
                    //    if (myDictionary.Count() > 1)
                    //    {
                    //        int num = 0;
                    //        foreach (var kvp in myDictionary)
                    //        {
                    //            if (num == 0)
                    //            {
                    //                orderItem.parentOrderId = orderItem.merchantOrderId;
                    //                orderItem.merchantOrderId += kvp.Key;
                    //                orderItem.warehouseId = kvp.Key.ToString();
                    //                orderItem.OrderGoods = new List<OrderGoodsItem>();
                    //                double tradeAmount = 0;
                    //                foreach (var item in kvp.Value)
                    //                {
                    //                    tradeAmount += Convert.ToDouble(item.skuUnitPrice) * Convert.ToDouble(item.quantity);
                    //                    orderItem.OrderGoods.Add(item);
                    //                }
                    //                orderItem.tradeAmount = tradeAmount.ToString();
                    //                newOrderItemList.Add(orderItem);
                    //            }
                    //            else
                    //            {
                    //                OrderItem orderItemNew = new OrderItem();
                    //                orderItemNew.parentOrderId = orderItem.parentOrderId;
                    //                orderItemNew.merchantOrderId = orderItem.parentOrderId + kvp.Key;
                    //                orderItemNew.tradeTime = orderItem.tradeTime;
                    //                orderItemNew.consigneeName = orderItem.consigneeName;
                    //                orderItemNew.consigneeMobile = orderItem.consigneeMobile;
                    //                orderItemNew.idNumber = orderItem.idNumber;
                    //                orderItemNew.addrCountry = orderItem.addrCountry;
                    //                orderItemNew.addrProvince = orderItem.addrProvince;
                    //                orderItemNew.addrCity = orderItem.addrCity;
                    //                orderItemNew.addrDistrict = orderItem.addrDistrict;
                    //                orderItemNew.addrDetail = orderItem.addrDetail;
                    //                orderItemNew.consignorName = orderItem.consignorName;
                    //                orderItemNew.consignorMobile = orderItem.consignorMobile;
                    //                orderItemNew.consignorAddr = orderItem.consignorAddr;
                    //                orderItemNew.OrderGoods = new List<OrderGoodsItem>();
                    //                double tradeAmount = 0;
                    //                foreach (var item in kvp.Value)
                    //                {
                    //                    tradeAmount += Convert.ToDouble(item.skuUnitPrice) * Convert.ToDouble(item.quantity);
                    //                    orderItemNew.OrderGoods.Add(item);
                    //                }
                    //                orderItemNew.tradeAmount = tradeAmount.ToString();
                    //                newOrderItemList.Add(orderItemNew);
                    //            }
                    //            num++;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        double tradeAmount = 0;
                    //        foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
                    //        {
                    //            tradeAmount += Convert.ToDouble(orderGoodsItem.skuUnitPrice) * Convert.ToDouble(orderGoodsItem.quantity);
                    //        }
                    //        orderItem.parentOrderId = orderItem.merchantOrderId;
                    //        orderItem.tradeAmount = tradeAmount.ToString();
                    //        newOrderItemList.Add(orderItem);
                    //    }
                    //}
                    //#endregion

                    //if (msg.msg != "")
                    //{
                    //    return msg;
                    //}
                    //#region 价格分拆

                    //ArrayList al = new ArrayList();
                    //ArrayList goodsNumAl = new ArrayList();
                    //foreach (var orderItem in newOrderItemList)
                    //{
                    //    double freight = 0, tradeAmount = 1;
                    //    double.TryParse(orderItem.OrderGoods[0].dr["freight"].ToString(), out freight);
                    //    double.TryParse(orderItem.tradeAmount, out tradeAmount);
                    //    orderItem.freight = Math.Round(freight, 2);
                    //    orderItem.platformId = orderItem.OrderGoods[0].dr["platformId"].ToString();
                    //    orderItem.warehouseId = orderItem.OrderGoods[0].dr["wid"].ToString();
                    //    orderItem.warehouseCode = orderItem.OrderGoods[0].dr["wcode"].ToString();
                    //    orderItem.supplier = orderItem.OrderGoods[0].dr["suppliercode"].ToString();
                    //    orderItem.purchaseId = orderItem.OrderGoods[0].dr["userId"].ToString();
                    //    orderItem.purchase = uploadParam.userId;
                    //    double fr = Math.Round(orderItem.freight / tradeAmount, 4);
                    //    for (int i = 0; i < orderItem.OrderGoods.Count; i++)
                    //    {
                    //        OrderGoodsItem orderGoodsItem = orderItem.OrderGoods[i];
                    //        //处理运费
                    //        //if (i== orderItem.OrderGoods.Count-1)
                    //        //{
                    //        //    orderGoodsItem.waybillPrice = freight;
                    //        //}
                    //        //else
                    //        //{
                    //        //    orderGoodsItem.waybillPrice = Math.Round(fr * orderGoodsItem.totalPrice,2);
                    //        //    freight -= orderGoodsItem.waybillPrice;
                    //        //}
                    //        //从运费平摊修改为运费都为全部运费。
                    //        orderGoodsItem.waybillPrice = freight;


                    //        //处理供货价和销售价和供货商code
                    //        orderGoodsItem.supplyPrice = Math.Round(Convert.ToDouble(orderGoodsItem.dr["inprice"]), 2);
                    //        orderGoodsItem.purchasePrice = Math.Round(Convert.ToDouble(orderGoodsItem.dr["pprice"]), 2);
                    //        orderGoodsItem.suppliercode = orderGoodsItem.dr["suppliercode"].ToString();
                    //        orderGoodsItem.slt = orderGoodsItem.dr["slt"].ToString();

                    //        string goodsWarehouseId = orderGoodsItem.dr["goodsWarehouseId"].ToString();//库存id
                    //        //处理税
                    //        double taxation = 0;
                    //        double.TryParse(orderGoodsItem.dr["taxation"].ToString(), out taxation);
                    //        if (taxation > 0)
                    //        {
                    //            double taxation2 = 0, taxation2type = 0, taxation2line = 0, nw = 0;
                    //            double.TryParse(orderGoodsItem.dr["taxation2"].ToString(), out taxation2);
                    //            double.TryParse(orderGoodsItem.dr["taxation2type"].ToString(), out taxation2type);
                    //            double.TryParse(orderGoodsItem.dr["taxation2line"].ToString(), out taxation2line);
                    //            double.TryParse(orderGoodsItem.dr["NW"].ToString(), out nw);
                    //            if (taxation2 == 0)
                    //            {
                    //                orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                    //            }
                    //            else
                    //            {
                    //                if (taxation2type == 1)//按总价提档
                    //                {
                    //                    if (orderGoodsItem.skuUnitPrice > taxation2line)//价格过线
                    //                    {
                    //                        orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation2 / 100;
                    //                    }
                    //                    else//价格没过线
                    //                    {
                    //                        orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                    //                    }
                    //                }
                    //                else if (taxation2type == 2)//按元/克提档
                    //                {
                    //                    if (nw == 0)//如果没有净重，按默认税档
                    //                    {
                    //                        orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                    //                    }
                    //                    else
                    //                    {
                    //                        if (orderGoodsItem.skuUnitPrice / (nw * 1000) > taxation2line)//价格过线
                    //                        {
                    //                            orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation2 / 100;
                    //                        }
                    //                        else//价格没过线
                    //                        {
                    //                            orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                    //                        }
                    //                    }
                    //                    //还要考虑面膜的问题
                    //                }
                    //                else//都不是按初始税率走
                    //                {
                    //                    orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            orderGoodsItem.tax = 0;
                    //        }
                    //        orderGoodsItem.tax = Math.Round(orderGoodsItem.tax, 2);
                    //        //处理平台提点
                    //        orderGoodsItem.platformPrice = 0;
                    //        double platformCost = 0;
                    //        double.TryParse(orderGoodsItem.dr["platformCost"].ToString(), out platformCost);
                    //        if (platformCost > 0)
                    //        {
                    //            if (orderGoodsItem.dr["platformCostType"].ToString() == "1")//进价计算
                    //            {
                    //                orderGoodsItem.platformPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * platformCost / (100 - platformCost);
                    //            }
                    //            else if (orderGoodsItem.dr["platformCostType"].ToString() == "2")//售价计算
                    //            {
                    //                if (orderGoodsItem.dr["priceType"].ToString() == "1")//按订单售价计算
                    //                {
                    //                    orderGoodsItem.platformPrice = orderGoodsItem.totalPrice * platformCost / 100;
                    //                }
                    //                else if (orderGoodsItem.dr["priceType"].ToString() == "2")//按供货价计算
                    //                {
                    //                    orderGoodsItem.platformPrice = orderGoodsItem.purchasePrice * orderGoodsItem.quantity * platformCost / 100;
                    //                }
                    //            }
                    //        }
                    //        orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);
                    //        //处理利润
                    //        //利润为供货价-进价-提点-运费分成-税
                    //        double profit = orderGoodsItem.purchasePrice * orderGoodsItem.quantity
                    //            - orderGoodsItem.supplyPrice * orderGoodsItem.quantity
                    //            - orderGoodsItem.platformPrice
                    //            - orderGoodsItem.waybillPrice
                    //            - orderGoodsItem.tax;
                    //        double profitAgent = 0, profitDealer = 0, profitOther1 = 0, profitOther2 = 0, profitOther3 = 0;
                    //        double.TryParse(orderGoodsItem.dr["profitAgent"].ToString(), out profitAgent);
                    //        double.TryParse(orderGoodsItem.dr["profitDealer"].ToString(), out profitDealer);
                    //        double.TryParse(orderGoodsItem.dr["profitOther1"].ToString(), out profitOther1);
                    //        double.TryParse(orderGoodsItem.dr["profitOther2"].ToString(), out profitOther2);
                    //        double.TryParse(orderGoodsItem.dr["profitOther3"].ToString(), out profitOther3);
                    //        orderGoodsItem.profitAgent = Math.Round(profit * profitAgent / 100, 2);
                    //        orderGoodsItem.profitDealer = Math.Round(profit * profitDealer / 100, 2);
                    //        orderGoodsItem.profitOther1 = Math.Round(profit * profitOther1 / 100, 2);
                    //        orderGoodsItem.profitOther2 = Math.Round(profit * profitOther2 / 100, 2);
                    //        orderGoodsItem.profitOther3 = Math.Round(profit * profitOther3 / 100, 2);
                    //        orderGoodsItem.profitPlatform = Math.Round(profit - orderGoodsItem.profitAgent
                    //                                        - orderGoodsItem.profitDealer - orderGoodsItem.profitOther1
                    //                                        - orderGoodsItem.profitOther2 - orderGoodsItem.profitOther3, 2);
                    //        orderGoodsItem.other1Name = orderGoodsItem.dr["profitOther1Name"].ToString();
                    //        orderGoodsItem.other2Name = orderGoodsItem.dr["profitOther2Name"].ToString();
                    //        orderGoodsItem.other3Name = orderGoodsItem.dr["profitOther3Name"].ToString();
                    //        string sqlgoods = "insert into t_order_goods(merchantOrderId,barCode,slt,skuUnitPrice," +
                    //                      "quantity,skuBillName,batchNo,goodsName," +
                    //                      "api,fqSkuID,sendType,status," +
                    //                      "suppliercode,supplyPrice,purchasePrice,waybill," +
                    //                      "waybillPrice,tax,platformPrice,profitPlatform," +
                    //                      "profitAgent,profitDealer,profitOther1,other1Name," +
                    //                      "profitOther2,other2Name,profitOther3,other3Name) " +
                    //                      "values('" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "','" + orderGoodsItem.slt + "','" + orderGoodsItem.skuUnitPrice + "'" +
                    //                      ",'" + orderGoodsItem.quantity + "','" + orderGoodsItem.skuBillName + "','','" + orderGoodsItem.skuBillName + "'" +
                    //                      ",'','','','0'" +
                    //                      ",'" + orderGoodsItem.suppliercode + "','" + orderGoodsItem.supplyPrice + "','" + orderGoodsItem.purchasePrice + "',''" +
                    //                      ",'" + orderGoodsItem.waybillPrice + "','" + orderGoodsItem.tax + "','" + orderGoodsItem.platformPrice + "','" + orderGoodsItem.profitPlatform + "'" +
                    //                      ",'" + orderGoodsItem.profitAgent + "','" + orderGoodsItem.profitDealer + "','" + orderGoodsItem.profitOther1 + "','" + orderGoodsItem.other1Name + "'" +
                    //                      ",'" + orderGoodsItem.profitOther2 + "','" + orderGoodsItem.other2Name + "','" + orderGoodsItem.profitOther3 + "','" + orderGoodsItem.other3Name + "'" +
                    //                      ")";
                    //        al.Add(sqlgoods);
                    //        string upsql = "update t_goods_warehouse set goodsnum = goodsnum-" + orderGoodsItem.quantity + " where id = " + goodsWarehouseId;
                    //        goodsNumAl.Add(upsql);
                    //        string logsql = "insert into t_log_goodsnum(inputType,createtime,wid,wcode,orderid,barcode,goodsnum,state) " +
                    //                        "values('',now(),'" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "'," +
                    //                        "'" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "'," +
                    //                        "" + orderGoodsItem.quantity + ",'" + orderItem.status + "')";
                    //        goodsNumAl.Add(logsql);
                    //    }
                    //    string sqlorder = "insert into t_order_list(warehouseId,warehouseCode,customerCode,actionType," +
                    //        "orderType,serviceType,parentOrderId,merchantOrderId," +
                    //        "payType,payNo,tradeTime," +
                    //        "tradeAmount,goodsTotalAmount,consigneeName,consigneeMobile," +
                    //        "addrCountry,addrProvince,addrCity,addrDistrict," +
                    //        "addrDetail,zipCode,idType,idNumber," +
                    //        "idFountImgUrl,idBackImgUrl,status,purchaserCode," +
                    //        "purchaserId,distributionCode,apitype,waybillno," +
                    //        "expressId,inputTime,fqID," +
                    //        "operate_status,sendapi,platformId,consignorName," +
                    //        "consignorMobile,consignorAddr,batchid,outNo,waybillOutNo," +
                    //        "accountsStatus,accountsNo,prePayId,ifPrint,printNo) " +
                    //        "values('" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "','" + orderItem.supplier + "',''" +
                    //        ",'','','" + orderItem.parentOrderId + "','" + orderItem.merchantOrderId + "'" +
                    //        ",'','','" + orderItem.tradeTime + "'" +
                    //        "," + orderItem.tradeAmount + ",'" + orderItem.tradeAmount + "','" + orderItem.consigneeName + "','" + orderItem.consigneeMobile + "'" +
                    //        ",'" + orderItem.addrCountry + "','" + orderItem.addrProvince + "','" + orderItem.addrCity + "','" + orderItem.addrDistrict + "'" +
                    //        ",'" + orderItem.addrDetail + "','','1','" + orderItem.idNumber + "'" +
                    //        ",'','','1','" + orderItem.purchase + "'" +
                    //        ",'" + orderItem.purchaseId + "','','',''" +
                    //        ",'',now(),''" +
                    //        ",'0','','" + orderItem.platformId + "','" + orderItem.consignorName + "'" +
                    //        ",'" + orderItem.consignorMobile + "','" + orderItem.consignorAddr + "','','',''" +
                    //        ",'0','','','0','') ";
                    //    al.Add(sqlorder);
                    //}

                    //#endregion


                    //if (DatabaseOperationWeb.ExecuteDML(al))
                    //{
                    //    DatabaseOperationWeb.ExecuteDML(goodsNumAl);
                    //    msg.msg = "导入成功";
                    //    msg.type = "1";
                    //}
                    //else
                    //{
                    //    msg.msg = "数据保存失败！";
                    //}
                    #endregion
                    Dictionary<string, string> errorDictionary = new Dictionary<string, string>();
                    msg = orderHandle(OrderItemList, uploadParam.userId, "0", ref errorDictionary);
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
        /// 上传零售商订单
        /// </summary>
        /// <param name="uploadParam"></param>
        /// <returns></returns>
        public MsgResult UploadOrderLS(FileUploadParam uploadParam)
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

                    if (msg.msg != "")
                    {
                        return msg;
                    }
                    #endregion

                    Dictionary<string, string> errorDictionary = new Dictionary<string, string>();
                    msg = orderHandleLS(OrderItemList, uploadParam.userId, "0", ref errorDictionary);
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
            if (ofAgent == null || ofAgent == "")
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
                                            "values('1',now(),'" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "'," +
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
        /// 上传代销订单-沈阳模板
        /// </summary>
        /// <param name="uploadParam"></param>
        /// <returns></returns>
        public MsgResult UploadOrderDXSY(FileUploadParam uploadParam)
        {
            MsgResult msg = new MsgResult();
            string logCode = uploadParam.userId + "UploadOrderDXSY" + DateTime.Now.ToString("yyyyMMddHHmmssff");
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
                    if (!dt.Columns.Contains("商品金额"))
                    {
                        msg.msg += "缺少“商品金额”列，";
                    }
                    if (!dt.Columns.Contains("订单金额"))
                    {
                        msg.msg += "缺少“订单金额”列，";
                    }
                    if (!dt.Columns.Contains("应收金额"))
                    {
                        msg.msg += "缺少“应收金额”列，";
                    }
                    if (!dt.Columns.Contains("结账时间"))
                    {
                        msg.msg += "缺少“结账时间”列，";
                    }
                    if (msg.msg != null && msg.msg != "")
                    {
                        return msg;
                    }
                    #endregion

                    //处理沈阳店卖别人家货的问题
                    dt.Columns.Add("FLAG");

                    #region 检查项并给导入list中
                    string tempOrderNo = "";
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    List<OrderItem> OrderItemList = new List<OrderItem>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string error = "";
                        //判断订单是否已经存在
                        string sqlno = "select id from t_order_list where merchantOrderId = '" + dt.Rows[i]["订单号"].ToString() + "' or  parentOrderId = '" + dt.Rows[i]["订单号"].ToString() + "'";
                        DataTable dtno = DatabaseOperationWeb.ExecuteSelectDS(sqlno, "TABLE").Tables[0];
                        if (dtno.Rows.Count > 0)
                        {
                            error += "序号为" + (i + 1).ToString() + "条订单订单已存在，请核对\r\n";
                        }
                        //判断商品名是否存在
                        string sqltm = "select id,barcode,goodsName from t_goods_list where trim( goodsName) = '" + dt.Rows[i]["商品名称"].ToString() + "'";
                        DataTable dttm = DatabaseOperationWeb.ExecuteSelectDS(sqltm, "TABLE").Tables[0];
                        if (dttm.Rows.Count == 0)
                        {
                            //如果不存在就给去掉-20190225 韩
                            //error += "序号为" + (i + 1).ToString() + "条订单订单商品名称不存在，请核对\r\n"; 
                            dt.Rows[i]["FLAG"] = "true";
                        }

                        //判断商品数量,商品申报单价是否为数字
                        double d = 0;
                        if (!double.TryParse(dt.Rows[i]["商品数量"].ToString(), out d))
                        {
                            error += "序号为" + (i + 1).ToString() + "条订单商品数量填写错误，请核对\r\n";
                        }
                        if (!double.TryParse(dt.Rows[i]["商品金额"].ToString(), out d))
                        {
                            error += "序号为" + (i + 1).ToString() + "条订单商品金额填写错误，请核对\r\n";
                        }
                        DateTime dtime = DateTime.Now;
                        if (dt.Rows[i]["订单号"].ToString() != "")
                        {
                            if (!double.TryParse(dt.Rows[i]["订单金额"].ToString(), out d))
                            {
                                error += "序号为" + (i + 1).ToString() + "条订单订单金额填写错误，请核对\r\n";
                            }
                            if (!double.TryParse(dt.Rows[i]["应收金额"].ToString(), out d))
                            {
                                error += "序号为" + (i + 1).ToString() + "条订单应收金额填写错误，请核对\r\n";
                            }
                            //判断订单日期是否正确
                            try
                            {
                                dtime = Convert.ToDateTime(dt.Rows[i]["结账时间"].ToString());
                            }
                            catch
                            {
                                error += "序号为" + (i + 1).ToString() + "条订单结账时间日期格式填写错误，请核对\r\n";
                            }
                            tempOrderNo = dt.Rows[i]["订单号"].ToString();
                        }
                        else
                        {
                            dt.Rows[i]["订单号"] = tempOrderNo;
                        }

                        if (error != "")
                        {
                            msg.msg += error;
                            if (!dic.ContainsKey(dt.Rows[i]["订单号"].ToString()))
                            {
                                dic.Add(dt.Rows[i]["订单号"].ToString(), "");
                            }
                            continue;
                        }
                        if (dic.ContainsKey(dt.Rows[i]["订单号"].ToString()))
                        {
                            continue;
                        }
                        bool isNotFound = true;
                        for (int j = 0; j < OrderItemList.Count(); j++)
                        {
                            if (OrderItemList[j].merchantOrderId == dt.Rows[i]["订单号"].ToString())
                            {
                                if (dt.Rows[i]["FLAG"].ToString() != "true")
                                {
                                    OrderGoodsItem orderGoodsItem = new OrderGoodsItem();
                                    orderGoodsItem.id = (i + 1).ToString();
                                    orderGoodsItem.barCode = dttm.Rows[0]["barcode"].ToString();
                                    orderGoodsItem.skuUnitPrice = Math.Round(Convert.ToDouble(dt.Rows[i]["商品金额"]) / Convert.ToDouble(dt.Rows[i]["商品数量"]), 2);
                                    orderGoodsItem.skuBillName = dt.Rows[i]["商品名称"].ToString();
                                    orderGoodsItem.quantity = Convert.ToDouble(dt.Rows[i]["商品数量"]);
                                    orderGoodsItem.totalPrice = orderGoodsItem.skuUnitPrice * orderGoodsItem.quantity;
                                    OrderItemList[j].OrderGoods.Add(orderGoodsItem);
                                }
                                isNotFound = false;
                                break;
                            }
                        }
                        if (isNotFound)//没有对应订单
                        {
                            string payType = "99";
                            switch (dt.Rows[i]["支付方式"].ToString())
                            {
                                case "会员储值卡":
                                    payType = "1";
                                    break;
                                case "现金支付":
                                    payType = "4";
                                    break;
                                case "微信记账":
                                    payType = "21";
                                    break;
                                case "支付宝记账":
                                    payType = "22";
                                    break;
                            }
                            OrderItem orderItem = new OrderItem();
                            orderItem.merchantOrderId = dt.Rows[i]["订单号"].ToString();
                            orderItem.tradeTime = dtime.ToString("yyyy-MM-dd HH:mm:ss");
                            orderItem.tradeAmount = dt.Rows[i]["应收金额"].ToString();
                            orderItem.derateName = dt.Rows[i]["优惠名称"].ToString();
                            orderItem.derate = Convert.ToDouble(dt.Rows[i]["订单金额"]) - Convert.ToDouble(dt.Rows[i]["应收金额"]);
                            orderItem.OrderGoods = new List<OrderGoodsItem>();
                            orderItem.payType = payType;
                            if (dt.Rows[i]["FLAG"].ToString()!= "true")
                            {
                                OrderGoodsItem orderGoodsItem = new OrderGoodsItem();
                                orderGoodsItem.id = (i + 1).ToString();
                                orderGoodsItem.barCode = dttm.Rows[0]["barcode"].ToString();
                                orderGoodsItem.skuUnitPrice = Math.Round(Convert.ToDouble(dt.Rows[i]["商品金额"]) / Convert.ToDouble(dt.Rows[i]["商品数量"]), 2);
                                orderGoodsItem.skuBillName = dt.Rows[i]["商品名称"].ToString();
                                orderGoodsItem.quantity = Convert.ToDouble(dt.Rows[i]["商品数量"]);
                                orderGoodsItem.totalPrice = orderGoodsItem.skuUnitPrice * orderGoodsItem.quantity;
                                orderItem.OrderGoods.Add(orderGoodsItem);
                            }
                            OrderItemList.Add(orderItem);
                        }
                    }

                    if (msg.msg != "")
                    {
                        return msg;
                    }
                    //处理订单中不是我们公司商品的信息
                    for (int i = 0; i < OrderItemList.Count; i++)
                    {
                        //for (int j = 0; j < OrderItemList[i].OrderGoods.Count; j++)
                        //{
                        //    if (OrderItemList[i].OrderGoods[j].ifOther)
                        //    {
                        //        OrderItemList[i].OrderGoods.RemoveAt(j);
                        //        j--;
                        //    }
                        //}
                        if (OrderItemList[i].OrderGoods.Count==0)
                        {
                            OrderItemList.RemoveAt(i);
                            i--;
                        }
                    }

                    #endregion
                    Dictionary<string, string> errorDictionary = new Dictionary<string, string>();
                    msg = orderHandleDXSY(OrderItemList, uploadParam.userId, "0", ref errorDictionary);
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
        /// 上传代销订单-集万模板
        /// </summary>
        /// <param name="uploadParam"></param>
        /// <returns></returns>
        public MsgResult UploadOrderDXJW(FileUploadParam uploadParam)
        {
            MsgResult msg = new MsgResult();
            string logCode = uploadParam.userId + "UploadOrderDXJW" + DateTime.Now.ToString("yyyyMMddHHmmssff");
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
                    if (!dt.Columns.Contains("订单时间"))
                    {
                        msg.msg += "缺少“订单时间”列，";
                    }
                    if (msg.msg != null && msg.msg != "")
                    {
                        return msg;
                    }
                    #endregion

                    dt.Columns.Add("订单号");

                    #region 检查项并给导入list中
                    List<OrderItem> OrderItemList = new List<OrderItem>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string error = "";
                        //判断订单日期是否正确
                        DateTime dtime = DateTime.Now;
                        try
                        {
                            dtime = Convert.ToDateTime(dt.Rows[i]["订单时间"].ToString()).AddSeconds(1);
                        }
                        catch
                        {
                            error += "序号为" + (i + 1).ToString() + "条订单结账时间日期格式填写错误，请核对\r\n";
                        }
                        //根据订单时间给订单号
                        dt.Rows[i]["订单号"] = "JW" + uploadParam.userId.ToUpper() + dtime.ToString("yyyyMMdd");
                        //判断订单是否已经存在
                        string sqlno = "select id from t_order_list where merchantOrderId = '" + dt.Rows[i]["订单号"].ToString() + "' or  parentOrderId = '" + dt.Rows[i]["订单号"].ToString() + "'";
                        DataTable dtno = DatabaseOperationWeb.ExecuteSelectDS(sqlno, "TABLE").Tables[0];
                        if (dtno.Rows.Count > 0)
                        {
                            error += "序号为" + (i + 1).ToString() + "条订单订单已存在，请核对\r\n";
                        }
                        //判断商品条码是否存在
                        string sqltm = "select id from t_goods_list where barcode = '" + dt.Rows[i]["商品条码"].ToString() + "'";
                        DataTable dttm = DatabaseOperationWeb.ExecuteSelectDS(sqltm, "TABLE").Tables[0];
                        if (dttm.Rows.Count == 0)
                        {
                            error += "序号为" + (i + 1).ToString() + "条订单订单商品条码不存在，请核对\r\n";
                        }

                        //判断商品数量是否为数字
                        double d = 0;
                        if (!double.TryParse(dt.Rows[i]["商品数量"].ToString(), out d))
                        {
                            error += "序号为" + (i + 1).ToString() + "条订单商品数量填写错误，请核对\r\n";
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
                                orderGoodsItem.id = (i + 1).ToString();
                                orderGoodsItem.barCode = dt.Rows[i]["商品条码"].ToString();
                                orderGoodsItem.skuUnitPrice = 0;
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
                            orderItem.tradeAmount = "0";
                            orderItem.OrderGoods = new List<OrderGoodsItem>();
                            OrderGoodsItem orderGoodsItem = new OrderGoodsItem();
                            orderGoodsItem.id = (i + 1).ToString();
                            orderGoodsItem.barCode = dt.Rows[i]["商品条码"].ToString();
                            orderGoodsItem.skuUnitPrice = 0;
                            orderGoodsItem.skuBillName = dt.Rows[i]["商品名称"].ToString();
                            orderGoodsItem.quantity = Convert.ToDouble(dt.Rows[i]["商品数量"]);
                            orderGoodsItem.totalPrice = orderGoodsItem.skuUnitPrice * orderGoodsItem.quantity;
                            orderItem.OrderGoods.Add(orderGoodsItem);
                            OrderItemList.Add(orderItem);
                        }
                    }

                    if (msg.msg != "")
                    {
                        return msg;
                    }
                    #endregion
                    Dictionary<string, string> errorDictionary = new Dictionary<string, string>();
                    msg = orderHandleDXJW(OrderItemList, uploadParam.userId, "0", ref errorDictionary);
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
        /// 上传代销订单-普通模板
        /// </summary>
        /// <param name="uploadParam"></param>
        /// <returns></returns>
        public MsgResult UploadOrderDX(FileUploadParam uploadParam)
        {
            MsgResult msg = new MsgResult();
            string logCode = uploadParam.userId + "UploadOrderDX" + DateTime.Now.ToString("yyyyMMddHHmmssff");
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
                        msg.msg += "缺少“订单号”列，";
                    }
                    if (!dt.Columns.Contains("商品条码"))
                    {
                        msg.msg += "缺少“商品条码”列，";
                    }
                    if (!dt.Columns.Contains("商品名称"))
                    {
                        msg.msg += "缺少“商品名称”列，";
                    }
                    if (!dt.Columns.Contains("商品数量"))
                    {
                        msg.msg += "缺少“商品数量”列，";
                    }
                    if (!dt.Columns.Contains("销售单价"))
                    {
                        msg.msg += "缺少“销售单价”列，";
                    }
                    if (!dt.Columns.Contains("订单时间"))
                    {
                        msg.msg += "缺少“订单时间”列，";
                    }
                    if (msg.msg != null && msg.msg != "")
                    {
                        return msg;
                    }
                    #endregion


                    #region 检查项并给导入list中
                    List<OrderItem> OrderItemList = new List<OrderItem>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string error = "";
                        //判断订单日期是否正确
                        DateTime dtime = DateTime.Now;
                        try
                        {
                            dtime = Convert.ToDateTime(dt.Rows[i]["订单时间"].ToString()).AddSeconds(1);
                        }
                        catch
                        {
                            error += "序号为" + (i + 1).ToString() + "条订单结账时间日期格式填写错误，请核对\r\n";
                        }

                        //判断订单是否已经存在
                        string sqlno = "select id from t_order_list where merchantOrderId = '" + dt.Rows[i]["订单号"].ToString() + "' or  parentOrderId = '" + dt.Rows[i]["订单号"].ToString() + "'";
                        DataTable dtno = DatabaseOperationWeb.ExecuteSelectDS(sqlno, "TABLE").Tables[0];
                        if (dtno.Rows.Count > 0)
                        {
                            error += "序号为" + (i + 1).ToString() + "条订单订单已存在，请核对\r\n";
                        }
                        //判断商品条码是否存在
                        string sqltm = "select id from t_goods_list where barcode = '" + dt.Rows[i]["商品条码"].ToString() + "'";
                        DataTable dttm = DatabaseOperationWeb.ExecuteSelectDS(sqltm, "TABLE").Tables[0];
                        if (dttm.Rows.Count == 0)
                        {
                            error += "序号为" + (i + 1).ToString() + "条订单订单商品条码不存在，请核对\r\n";
                        }

                        //判断商品数量是否为数字
                        double d = 0;
                        if (!double.TryParse(dt.Rows[i]["商品数量"].ToString(), out d))
                        {
                            error += "序号为" + (i + 1).ToString() + "条订单商品数量填写错误，请核对\r\n";
                        }

                        //判断销售单价是否为数字
                        double d1 = 0;
                        if (!double.TryParse(dt.Rows[i]["销售单价"].ToString(), out d1))
                        {
                            error += "序号为" + (i + 1).ToString() + "条订单销售单价填写错误，请核对\r\n";
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
                                orderGoodsItem.id = (i + 1).ToString();
                                orderGoodsItem.barCode = dt.Rows[i]["商品条码"].ToString();
                                orderGoodsItem.skuUnitPrice = Convert.ToDouble(dt.Rows[i]["销售单价"]);
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
                            orderItem.tradeAmount = "0";
                            orderItem.OrderGoods = new List<OrderGoodsItem>();
                            OrderGoodsItem orderGoodsItem = new OrderGoodsItem();
                            orderGoodsItem.id = (i + 1).ToString();
                            orderGoodsItem.barCode = dt.Rows[i]["商品条码"].ToString();
                            orderGoodsItem.skuUnitPrice = Convert.ToDouble(dt.Rows[i]["销售单价"]);
                            orderGoodsItem.skuBillName = dt.Rows[i]["商品名称"].ToString();
                            orderGoodsItem.quantity = Convert.ToDouble(dt.Rows[i]["商品数量"]);
                            orderGoodsItem.totalPrice = orderGoodsItem.skuUnitPrice * orderGoodsItem.quantity;
                            orderItem.OrderGoods.Add(orderGoodsItem);
                            OrderItemList.Add(orderItem);
                        }
                    }

                    if (msg.msg != "")
                    {
                        return msg;
                    }
                    #endregion
                    Dictionary<string, string> errorDictionary = new Dictionary<string, string>();
                    msg = orderHandleDX(OrderItemList, uploadParam.userId, "0", ref errorDictionary);
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
                st += " and t.purchaserCode = '" + orderParam.platformId.Trim() + "' ";
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
                "t.tradeAmount- IfNULL(discountPrice,0) as 订单销售额,,g.barCode as 商品条码,g.goodsName as 商品名,g.quantity as 销量," +
                "(select username from t_user_list where usercode =customerCode) as 供应商,g.supplyPrice as 供货单价," +
                "g.supplyPrice*g.quantity as 供货额,(select username from t_user_list where usercode =purchaserCode) as 销售渠道 ," +
                "e.expressName as 平台渠道,g.purchasePrice as 销售单价,g.purchasePrice*g.quantity as 商品销售额," +
                "s.statusName as 订单状态,t.waybillno as 运单编号,t.waybilltime as 发货时间,t.addrCountry as 收货人国家,t.addrProvince as 收货人省," +
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
        #region 处理订单新方法
        /// <summary>
        /// 处理直邮订单的方法
        /// </summary>
        /// <param name="OrderItemList">订单bean</param>
        /// <param name="userCode">导入的用户code</param>
        /// <param name="apiType">判断是否是api接口，是1</param>
        /// <param name="errorDictionary">错误的集合</param>
        /// <returns></returns>
        public MsgResult orderHandle(List<OrderItem> OrderItemList, string userCode, string apiType, ref Dictionary<string, string> errorDictionary)
        {
            MsgResult msg = new MsgResult();
            string userSql = "select * from t_user_list";
            DataTable userDT = DatabaseOperationWeb.ExecuteSelectDS(userSql, "TABLE").Tables[0];
            #region 处理因仓库分单
            List<OrderItem> newOrderItemList = new List<OrderItem>();
            foreach (var orderItem in OrderItemList)
            {
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && errorDictionary[orderItem.merchantOrderId] != "")
                {
                    continue;
                }
                string error = ""; //当是接口调用的时候存放错误信息。
                Dictionary<int, List<OrderGoodsItem>> myDictionary = new Dictionary<int, List<OrderGoodsItem>>();
                foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
                {
                    string wsql = "select d.platformId,u.id as userId,u.platformCost,u.platformCostType,u.priceType,u.agentCost,u.usertype,u.ofAgent," +
                                  "d.pprice,d.profitPlatform,d.profitAgent,d.profitDealer,d.profitOther1," +
                                  "d.profitOther1Name,d.profitOther2,d.profitOther2Name,d.profitOther3," +
                                  "d.profitOther3Name,w.id as goodsWarehouseId,w.wid,w.wcode,w.goodsnum,w.inprice,bw.taxation,bw.taxation2," +
                                  "bw.taxation2type,bw.taxation2line,bw.freight,w.suppliercode,g.NW,g.slt " +
                                  "from t_goods_distributor_price d ,t_goods_warehouse w,t_base_warehouse bw," +
                                  "t_goods_list g,t_user_list u   " +
                                  "where u.usercode = d.usercode and g.barcode = d.barcode and w.wid = bw.id " +
                                  "and d.barcode = w.barcode and w.supplierid = d.supplierid and d.wid = bw.id " +
                                   "and d.usercode = '" + userCode + "' " +
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
                        if (apiType == "1")
                        {
                            error += orderGoodsItem.barCode + "没找到对应默认供货信息，";
                            continue;
                        }
                        else
                        {
                            msg.msg += "序号为" + orderGoodsItem.id + "行没找到对应默认供货信息，请核对\r\n";
                            continue;
                        }

                    }
                    if (!myDictionary.ContainsKey(wid))
                    {
                        myDictionary.Add(wid, new List<OrderGoodsItem>());
                    }
                    myDictionary[wid].Add(orderGoodsItem);
                }
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && error != "")
                {
                    errorDictionary[orderItem.merchantOrderId] += error;
                    continue;
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
            //如果不是接口调用就判断是否有错误信息，有的话返回错误信息
            if (apiType != "1" && msg.msg != "")
            {
                return msg;
            }
            #region 价格分拆

            ArrayList al = new ArrayList();
            ArrayList goodsNumAl = new ArrayList();
            foreach (var orderItem in newOrderItemList)
            {
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && errorDictionary[orderItem.merchantOrderId] != "")
                {
                    continue;
                }
                double freight = 0, tradeAmount = 1;
                double.TryParse(orderItem.OrderGoods[0].dr["freight"].ToString(), out freight);
                double.TryParse(orderItem.tradeAmount, out tradeAmount);
                orderItem.freight = Math.Round(freight, 2);
                orderItem.platformId = orderItem.OrderGoods[0].dr["platformId"].ToString();
                orderItem.warehouseId = orderItem.OrderGoods[0].dr["wid"].ToString();
                orderItem.warehouseCode = orderItem.OrderGoods[0].dr["wcode"].ToString();
                orderItem.supplier = orderItem.OrderGoods[0].dr["suppliercode"].ToString();
                orderItem.purchaseId = orderItem.OrderGoods[0].dr["userId"].ToString();
                orderItem.purchase = userCode;
                double fr = Math.Round(orderItem.freight / tradeAmount, 4);

                //处理供货代理提点
                double supplierAgentCost = 0;
                DataRow[] drs = userDT.Select("userCode = '" + orderItem.supplier + "'");
                if (drs.Length > 0)
                {
                    if (drs[0]["ofAgent"].ToString() != "")
                    {
                        orderItem.supplierAgentCode = drs[0]["ofAgent"].ToString();
                        double.TryParse(drs[0]["agentCost"].ToString(), out supplierAgentCost);
                    }
                }

                //处理采购代理提点
                double purchaseAgentCost = 0;
                if (orderItem.OrderGoods[0].dr["ofAgent"].ToString() != "")
                {
                    //分销商需要看分销代理有没有采购代理。
                    if (orderItem.OrderGoods[0].dr["usertype"].ToString() == "4")
                    {
                        DataRow[] drs1 = userDT.Select("userCode = '" + orderItem.OrderGoods[0].dr["ofAgent"].ToString() + "'");
                        if (drs1.Length > 0)
                        {
                            if (drs1[0]["ofAgent"].ToString() != "")
                            {
                                orderItem.purchaseAgentCode = drs1[0]["ofAgent"].ToString();
                                double.TryParse(drs1[0]["agentCost"].ToString(), out purchaseAgentCost);
                            }
                        }
                    }
                    else
                    {
                        orderItem.purchaseAgentCode = orderItem.OrderGoods[0].dr["ofAgent"].ToString();
                        double.TryParse(orderItem.OrderGoods[0].dr["agentCost"].ToString(), out purchaseAgentCost);
                    }
                }
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
                        orderGoodsItem.tax = Math.Round(orderGoodsItem.tax, 2);
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
                            orderGoodsItem.platformPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * platformCost / (100 - platformCost);
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
                        orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);
                    }
                    //处理供货代理提点
                    orderGoodsItem.supplierAgentPrice = 0;
                    if (supplierAgentCost > 0)
                    {
                        //按供货价计算
                        orderGoodsItem.supplierAgentPrice = Math.Round(orderGoodsItem.supplyPrice * orderGoodsItem.quantity * supplierAgentCost / 100, 2);
                        orderGoodsItem.supplierAgentCode = orderItem.supplierAgentCode;
                        orderGoodsItem.supplierAgentPrice = Math.Round(orderGoodsItem.supplierAgentPrice, 2);
                    }
                    //处理采购代理提点
                    orderGoodsItem.purchaseAgentPrice = 0;
                    if (purchaseAgentCost > 0)
                    {
                        if (orderGoodsItem.dr["platformCostType"].ToString() == "1")//进价计算
                        {
                            orderGoodsItem.purchaseAgentPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * purchaseAgentCost / (100 - platformCost);
                        }
                        else if (orderGoodsItem.dr["platformCostType"].ToString() == "2")//售价计算
                        {
                            if (orderGoodsItem.dr["priceType"].ToString() == "1")//按订单售价计算
                            {
                                orderGoodsItem.purchaseAgentPrice = orderGoodsItem.totalPrice * purchaseAgentCost / 100;
                            }
                            else if (orderGoodsItem.dr["priceType"].ToString() == "2")//按供货价计算
                            {
                                orderGoodsItem.purchaseAgentPrice = orderGoodsItem.purchasePrice * orderGoodsItem.quantity * purchaseAgentCost / 100;
                            }
                        }
                        orderGoodsItem.purchaseAgentPrice = Math.Round(orderGoodsItem.purchaseAgentPrice, 2);
                    }




                    orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);

                    //判断误差，误差=供货价-进价-平台提点-供货代理提点-采购代理提点-运费分成-税
                    double deviation = orderGoodsItem.purchasePrice * orderGoodsItem.quantity
                        - orderGoodsItem.supplyPrice * orderGoodsItem.quantity
                        - orderGoodsItem.platformPrice
                        - orderGoodsItem.supplierAgentPrice
                        - orderGoodsItem.purchaseAgentPrice
                        - orderGoodsItem.waybillPrice
                        - orderGoodsItem.tax;
                    //如果有误差了，就从平台提点扣除
                    if (deviation != 0)
                    {
                        if (orderGoodsItem.platformPrice + deviation > 0)
                        {
                            orderGoodsItem.platformPrice = orderGoodsItem.platformPrice + deviation;
                        }
                        else
                        {
                            msg.msg = "订单" + orderItem.merchantOrderId + "价格有误差，请查对！";
                        }
                    }
                    //处理利润
                    //利润=售价-供货价
                    double profit = (orderGoodsItem.skuUnitPrice - orderGoodsItem.purchasePrice) * orderGoodsItem.quantity;
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
                                  "supplierAgentPrice,supplierAgentCode,purchaseAgentPrice,purchaseAgentCode," +
                                  "profitAgent,profitDealer,profitOther1,other1Name," +
                                  "profitOther2,other2Name,profitOther3,other3Name) " +
                                  "values('" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "','" + orderGoodsItem.slt + "','" + orderGoodsItem.skuUnitPrice + "'" +
                                  ",'" + orderGoodsItem.quantity + "','" + orderGoodsItem.skuBillName + "','','" + orderGoodsItem.skuBillName + "'" +
                                  ",'','','','0'" +
                                  ",'" + orderGoodsItem.suppliercode + "','" + orderGoodsItem.supplyPrice + "','" + orderGoodsItem.purchasePrice + "',''" +
                                  ",'" + orderGoodsItem.waybillPrice + "','" + orderGoodsItem.tax + "','" + orderGoodsItem.platformPrice + "','" + orderGoodsItem.profitPlatform + "'" +
                                  ",'" + orderGoodsItem.supplierAgentPrice + "','" + orderGoodsItem.supplierAgentCode + "','" + orderGoodsItem.purchaseAgentPrice + "','" + orderGoodsItem.purchaseAgentCode + "'" +
                                  ",'" + orderGoodsItem.profitAgent + "','" + orderGoodsItem.profitDealer + "','" + orderGoodsItem.profitOther1 + "','" + orderGoodsItem.other1Name + "'" +
                                  ",'" + orderGoodsItem.profitOther2 + "','" + orderGoodsItem.other2Name + "','" + orderGoodsItem.profitOther3 + "','" + orderGoodsItem.other3Name + "'" +
                                  ")";
                    al.Add(sqlgoods);
                    string upsql = "update t_goods_warehouse set goodsnum = goodsnum-" + orderGoodsItem.quantity + " where id = " + goodsWarehouseId;
                    goodsNumAl.Add(upsql);
                    string logsql = "insert into t_log_goodsnum(inputType,createtime,wid,wcode,orderid,barcode,goodsnum,state) " +
                                    "values('1',now(),'" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "'," +
                                    "'" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "'," +
                                    "" + orderGoodsItem.quantity + ",'" + orderItem.status + "')";
                    goodsNumAl.Add(logsql);
                }
                string sqlorder = "insert into t_order_list(warehouseId,warehouseCode,customerCode,actionType," +
                    "orderType,serviceType,parentOrderId,merchantOrderId," +
                    "payType,payNo,tradeTime,consigneeCode," +
                    "tradeAmount,goodsTotalAmount,consigneeName,consigneeMobile," +
                    "addrCountry,addrProvince,addrCity,addrDistrict," +
                    "addrDetail,zipCode,idType,idNumber," +
                    "idFountImgUrl,idBackImgUrl,status,purchaserCode," +
                    "purchaserId,distributionCode,apitype,waybillno," +
                    "inputTime,fqID,supplierAgentCode,purchaseAgentCode," +
                    "operate_status,sendapi,platformId,consignorName," +
                    "consignorMobile,consignorAddr,batchid,outNo,waybillOutNo," +
                    "accountsStatus,accountsNo,prePayId,ifPrint,printNo) " +
                    "values('" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "','" + orderItem.supplier + "',''" +
                    ",'','','" + orderItem.parentOrderId + "','" + orderItem.merchantOrderId + "'" +
                    ",'','','" + orderItem.tradeTime + "',''" +
                    "," + orderItem.tradeAmount + ",'" + orderItem.tradeAmount + "','" + orderItem.consigneeName + "','" + orderItem.consigneeMobile + "'" +
                    ",'" + orderItem.addrCountry + "','" + orderItem.addrProvince + "','" + orderItem.addrCity + "','" + orderItem.addrDistrict + "'" +
                    ",'" + orderItem.addrDetail + "','','1','" + orderItem.idNumber + "'" +
                    ",'','','1','" + orderItem.purchase + "'" +
                    ",'" + orderItem.purchaseId + "','','1',''" +
                    ",now(),'','" + orderItem.supplierAgentCode + "','" + orderItem.purchaseAgentCode + "'" +
                    ",'0','','" + orderItem.platformId + "','" + orderItem.consignorName + "'" +
                    ",'" + orderItem.consignorMobile + "','" + orderItem.consignorAddr + "','','',''" +
                    ",'0','','','0','') ";
                al.Add(sqlorder);
            }

            #endregion
            if (apiType != "1" && msg.msg != "")
            {
                return msg;
            }

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
            return msg;
        }

        /// <summary>
        /// 处理零售订单的方法
        /// </summary>
        /// <param name="OrderItemList">订单bean</param>
        /// <param name="userCode">导入的用户code</param>
        /// <param name="apiType">判断是否是api接口，是1</param>
        /// <param name="errorDictionary">错误的集合</param>
        /// <returns></returns>
        public MsgResult orderHandleLS(List<OrderItem> OrderItemList, string userCode, string apiType, ref Dictionary<string, string> errorDictionary)
        {
            MsgResult msg = new MsgResult();
            string userSql = "select * from t_user_list";
            DataTable userDT = DatabaseOperationWeb.ExecuteSelectDS(userSql, "TABLE").Tables[0];
            #region 处理因仓库分单
            List<OrderItem> newOrderItemList = new List<OrderItem>();
            foreach (var orderItem in OrderItemList)
            {
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && errorDictionary[orderItem.merchantOrderId] != "")
                {
                    continue;
                }
                string error = ""; //当是接口调用的时候存放错误信息。
                Dictionary<int, List<OrderGoodsItem>> myDictionary = new Dictionary<int, List<OrderGoodsItem>>();
                foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
                {
                    string wsql = "select d.platformId,u.id as userId,u.platformCost,u.platformCostType,u.priceType,u.agentCost,u.usertype,u.ofAgent," +
                                  "d.pprice,d.profitPlatform,d.profitAgent,d.profitDealer,d.profitOther1," +
                                  "d.profitOther1Name,d.profitOther2,d.profitOther2Name,d.profitOther3," +
                                  "d.profitOther3Name,w.id as goodsWarehouseId,w.wid,w.wcode,w.goodsnum,w.inprice,bw.taxation,bw.taxation2," +
                                  "bw.taxation2type,bw.taxation2line,bw.freight,w.suppliercode,g.NW,g.slt " +
                                  "from t_goods_distributor_price d ,t_goods_warehouse w,t_base_warehouse bw," +
                                  "t_goods_list g,t_user_list u   " +
                                  "where u.usercode = d.usercode and g.barcode = d.barcode and w.wid = bw.id " +
                                  "and d.barcode = w.barcode and w.supplierid = d.supplierid and d.wid = bw.id " +
                                  "and d.usercode = 'bbcagent@llwell.net' " +
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
                        if (apiType == "1")
                        {
                            error += orderGoodsItem.barCode + "没找到对应默认供货信息，";
                            continue;
                        }
                        else
                        {
                            msg.msg += "序号为" + orderGoodsItem.id + "行没找到对应默认供货信息，请核对\r\n";
                            continue;
                        }

                    }
                    if (!myDictionary.ContainsKey(wid))
                    {
                        myDictionary.Add(wid, new List<OrderGoodsItem>());
                    }
                    myDictionary[wid].Add(orderGoodsItem);
                }
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && error != "")
                {
                    errorDictionary[orderItem.merchantOrderId] += error;
                    continue;
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
            //如果不是接口调用就判断是否有错误信息，有的话返回错误信息
            if (apiType != "1" && msg.msg != "")
            {
                return msg;
            }
            #region 价格分拆

            ArrayList al = new ArrayList();
            ArrayList goodsNumAl = new ArrayList();
            foreach (var orderItem in newOrderItemList)
            {
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && errorDictionary[orderItem.merchantOrderId] != "")
                {
                    continue;
                }
                double freight = 0, tradeAmount = 0;
                double.TryParse(orderItem.OrderGoods[0].dr["freight"].ToString(), out freight);
                //double.TryParse(orderItem.tradeAmount, out tradeAmount);
                orderItem.freight = Math.Round(freight, 2);
                orderItem.platformId = orderItem.OrderGoods[0].dr["platformId"].ToString();
                orderItem.warehouseId = orderItem.OrderGoods[0].dr["wid"].ToString();
                orderItem.warehouseCode = orderItem.OrderGoods[0].dr["wcode"].ToString();
                orderItem.supplier = orderItem.OrderGoods[0].dr["suppliercode"].ToString();
                orderItem.purchaseId = orderItem.OrderGoods[0].dr["userId"].ToString();
                orderItem.purchase = userCode;
                //double fr = Math.Round(orderItem.freight / tradeAmount, 4);

                //处理供货代理提点
                double supplierAgentCost = 0;
                DataRow[] drs = userDT.Select("userCode = '" + orderItem.supplier + "'");
                if (drs.Length > 0)
                {
                    if (drs[0]["ofAgent"].ToString() != "")
                    {
                        orderItem.supplierAgentCode = drs[0]["ofAgent"].ToString();
                        double.TryParse(drs[0]["agentCost"].ToString(), out supplierAgentCost);
                    }
                }

                //处理采购代理提点
                double purchaseAgentCost = 0;
                if (orderItem.OrderGoods[0].dr["ofAgent"].ToString() != "")
                {
                    //分销商需要看分销代理有没有采购代理。
                    if (orderItem.OrderGoods[0].dr["usertype"].ToString() == "4")
                    {
                        DataRow[] drs1 = userDT.Select("userCode = '" + orderItem.OrderGoods[0].dr["ofAgent"].ToString() + "'");
                        if (drs1.Length > 0)
                        {
                            if (drs1[0]["ofAgent"].ToString() != "")
                            {
                                orderItem.purchaseAgentCode = drs1[0]["ofAgent"].ToString();
                                double.TryParse(drs1[0]["agentCost"].ToString(), out purchaseAgentCost);
                            }
                        }
                    }
                    else
                    {
                        orderItem.purchaseAgentCode = orderItem.OrderGoods[0].dr["ofAgent"].ToString();
                        double.TryParse(orderItem.OrderGoods[0].dr["agentCost"].ToString(), out purchaseAgentCost);
                    }
                }
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
                    //新增订单金额使用平台供货价 --- 20190505 han
                    tradeAmount += orderGoodsItem.purchasePrice;


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
                        orderGoodsItem.tax = Math.Round(orderGoodsItem.tax, 2);
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
                            orderGoodsItem.platformPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * platformCost / (100 - platformCost);
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
                        orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);
                    }
                    //处理供货代理提点
                    orderGoodsItem.supplierAgentPrice = 0;
                    if (supplierAgentCost > 0)
                    {
                        //按供货价计算
                        orderGoodsItem.supplierAgentPrice = Math.Round(orderGoodsItem.supplyPrice * orderGoodsItem.quantity * supplierAgentCost / 100, 2);
                        orderGoodsItem.supplierAgentCode = orderItem.supplierAgentCode;
                        orderGoodsItem.supplierAgentPrice = Math.Round(orderGoodsItem.supplierAgentPrice, 2);
                    }
                    //处理采购代理提点
                    orderGoodsItem.purchaseAgentPrice = 0;
                    if (purchaseAgentCost > 0)
                    {
                        if (orderGoodsItem.dr["platformCostType"].ToString() == "1")//进价计算
                        {
                            orderGoodsItem.purchaseAgentPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * purchaseAgentCost / (100 - platformCost);
                        }
                        else if (orderGoodsItem.dr["platformCostType"].ToString() == "2")//售价计算
                        {
                            if (orderGoodsItem.dr["priceType"].ToString() == "1")//按订单售价计算
                            {
                                orderGoodsItem.purchaseAgentPrice = orderGoodsItem.totalPrice * purchaseAgentCost / 100;
                            }
                            else if (orderGoodsItem.dr["priceType"].ToString() == "2")//按供货价计算
                            {
                                orderGoodsItem.purchaseAgentPrice = orderGoodsItem.purchasePrice * orderGoodsItem.quantity * purchaseAgentCost / 100;
                            }
                        }
                        orderGoodsItem.purchaseAgentPrice = Math.Round(orderGoodsItem.purchaseAgentPrice, 2);
                    }




                    orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);

                    //判断误差，误差=供货价-进价-平台提点-供货代理提点-采购代理提点-运费分成-税
                    double deviation = orderGoodsItem.purchasePrice * orderGoodsItem.quantity
                        - orderGoodsItem.supplyPrice * orderGoodsItem.quantity
                        - orderGoodsItem.platformPrice
                        - orderGoodsItem.supplierAgentPrice
                        - orderGoodsItem.purchaseAgentPrice
                        - orderGoodsItem.waybillPrice
                        - orderGoodsItem.tax;
                    //如果有误差了，就从平台提点扣除
                    if (deviation != 0)
                    {
                        if (orderGoodsItem.platformPrice + deviation > 0)
                        {
                            orderGoodsItem.platformPrice = orderGoodsItem.platformPrice + deviation;
                        }
                        else
                        {
                            msg.msg = "订单" + orderItem.merchantOrderId + "价格有误差，请查对！";
                        }
                    }
                    //处理利润
                    //利润=售价-供货价
                    double profit = (orderGoodsItem.skuUnitPrice - orderGoodsItem.purchasePrice) * orderGoodsItem.quantity;
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
                                  "supplierAgentPrice,supplierAgentCode,purchaseAgentPrice,purchaseAgentCode," +
                                  "profitAgent,profitDealer,profitOther1,other1Name," +
                                  "profitOther2,other2Name,profitOther3,other3Name) " +
                                  "values('" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "','" + orderGoodsItem.slt + "','" + orderGoodsItem.skuUnitPrice + "'" +
                                  ",'" + orderGoodsItem.quantity + "','" + orderGoodsItem.skuBillName + "','','" + orderGoodsItem.skuBillName + "'" +
                                  ",'','','','0'" +
                                  ",'" + orderGoodsItem.suppliercode + "','" + orderGoodsItem.supplyPrice + "','" + orderGoodsItem.purchasePrice + "',''" +
                                  ",'" + orderGoodsItem.waybillPrice + "','" + orderGoodsItem.tax + "','" + orderGoodsItem.platformPrice + "','" + orderGoodsItem.profitPlatform + "'" +
                                  ",'" + orderGoodsItem.supplierAgentPrice + "','" + orderGoodsItem.supplierAgentCode + "','" + orderGoodsItem.purchaseAgentPrice + "','" + orderGoodsItem.purchaseAgentCode + "'" +
                                  ",'" + orderGoodsItem.profitAgent + "','" + orderGoodsItem.profitDealer + "','" + orderGoodsItem.profitOther1 + "','" + orderGoodsItem.other1Name + "'" +
                                  ",'" + orderGoodsItem.profitOther2 + "','" + orderGoodsItem.other2Name + "','" + orderGoodsItem.profitOther3 + "','" + orderGoodsItem.other3Name + "'" +
                                  ")";
                    al.Add(sqlgoods);
                    string upsql = "update t_goods_warehouse set goodsnum = goodsnum-" + orderGoodsItem.quantity + " where id = " + goodsWarehouseId;
                    goodsNumAl.Add(upsql);
                    string logsql = "insert into t_log_goodsnum(inputType,createtime,wid,wcode,orderid,barcode,goodsnum,state) " +
                                    "values('1',now(),'" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "'," +
                                    "'" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "'," +
                                    "" + orderGoodsItem.quantity + ",'" + orderItem.status + "')";
                    goodsNumAl.Add(logsql);
                }
                string sqlorder = "insert into t_order_list(warehouseId,warehouseCode,customerCode,actionType," +
                    "orderType,serviceType,parentOrderId,merchantOrderId," +
                    "payType,payNo,tradeTime,consigneeCode," +
                    "tradeAmount,goodsTotalAmount,consigneeName,consigneeMobile," +
                    "addrCountry,addrProvince,addrCity,addrDistrict," +
                    "addrDetail,zipCode,idType,idNumber," +
                    "idFountImgUrl,idBackImgUrl,status,purchaserCode," +
                    "purchaserId,distributionCode,apitype,waybillno," +
                    "expressId,inputTime,fqID,supplierAgentCode,purchaseAgentCode," +
                    "operate_status,sendapi,platformId,consignorName," +
                    "consignorMobile,consignorAddr,batchid,outNo,waybillOutNo," +
                    "accountsStatus,accountsNo,prePayId,ifPrint,printNo) " +
                    "values('" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "','" + orderItem.supplier + "',''" +
                    ",'','','" + orderItem.parentOrderId + "','" + orderItem.merchantOrderId + "'" +
                    ",'','','" + orderItem.tradeTime + "',''" +
                    "," + tradeAmount + ",'" + tradeAmount + "','" + orderItem.consigneeName + "','" + orderItem.consigneeMobile + "'" +
                    ",'" + orderItem.addrCountry + "','" + orderItem.addrProvince + "','" + orderItem.addrCity + "','" + orderItem.addrDistrict + "'" +
                    ",'" + orderItem.addrDetail + "','','1','" + orderItem.idNumber + "'" +
                    ",'','','0','" + orderItem.purchase + "'" +
                    ",'" + orderItem.purchaseId + "','','1',''" +
                    ",'',now(),'','" + orderItem.supplierAgentCode + "','" + orderItem.purchaseAgentCode + "'" +
                    ",'0','','" + orderItem.platformId + "','" + orderItem.consignorName + "'" +
                    ",'" + orderItem.consignorMobile + "','" + orderItem.consignorAddr + "','','',''" +
                    ",'0','','','0','') ";
                al.Add(sqlorder);
            }

            #endregion
            if (apiType != "1" && msg.msg != "")
            {
                return msg;
            }

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
            return msg;
        }

        /// <summary>
        /// 处理代销订单的方法-沈阳模板
        /// </summary>
        /// <param name="OrderItemList">订单bean</param>
        /// <param name="userCode">导入的用户code</param>
        /// <param name="apiType">判断是否是api接口，是1</param>
        /// <param name="errorDictionary">错误的集合</param>
        /// <returns></returns>
        public MsgResult orderHandleDXSY(List<OrderItem> OrderItemList, string userCode, string apiType, ref Dictionary<string, string> errorDictionary)
        {
            MsgResult msg = new MsgResult();
            string userSql = "select * from t_user_list";
            DataTable userDT = DatabaseOperationWeb.ExecuteSelectDS(userSql, "TABLE").Tables[0];
            #region 处理订单的优惠减免
            foreach (var orderItem in OrderItemList)
            {
                double derate = Math.Round(orderItem.derate, 2);
                if (orderItem.OrderGoods.Count == 1)
                {
                    orderItem.OrderGoods[0].skuUnitPrice = Math.Round(Convert.ToDouble(orderItem.tradeAmount) / orderItem.OrderGoods[0].quantity, 2);
                }
                else
                {
                    for (int i = 0; i < orderItem.OrderGoods.Count; i++)
                    {
                        if (i == orderItem.OrderGoods.Count - 1)
                        {
                            orderItem.OrderGoods[i].skuUnitPrice -= Math.Round(derate / orderItem.OrderGoods[i].quantity, 2);
                        }
                        else
                        {
                            double differ = Math.Round(orderItem.OrderGoods[i].skuUnitPrice * orderItem.OrderGoods[i].quantity * orderItem.derate / (orderItem.derate + Convert.ToDouble(orderItem.tradeAmount)), 2);
                            derate -= differ;
                            orderItem.OrderGoods[i].skuUnitPrice -= Math.Round(differ / orderItem.OrderGoods[i].quantity, 2);
                        }
                    }
                }

            }
            #endregion
            #region 处理因仓库分单
            List<OrderItem> newOrderItemList = new List<OrderItem>();
            foreach (var orderItem in OrderItemList)
            {
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && errorDictionary[orderItem.merchantOrderId] != "")
                {
                    continue;
                }
                string error = ""; //当是接口调用的时候存放错误信息。
                Dictionary<int, List<OrderGoodsItem>> myDictionary = new Dictionary<int, List<OrderGoodsItem>>();
                foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
                {
                    string wsql = "select d.platformId,u.id as userId,u.platformCost,u.platformCostType,u.priceType,u.agentCost,u.usertype,u.ofAgent,d.pnum,d.id as distributorId," +
                                  "d.pprice,d.profitPlatform,d.profitAgent,d.profitDealer,d.profitOther1," +
                                  "d.profitOther1Name,d.profitOther2,d.profitOther2Name,d.profitOther3," +
                                  "d.profitOther3Name,w.id as goodsWarehouseId,w.wid,w.wcode,w.goodsnum,w.inprice,bw.taxation,bw.taxation2," +
                                  "bw.taxation2type,bw.taxation2line,bw.freight,w.suppliercode,g.NW,g.slt " +
                                  "from t_goods_distributor_price d ,t_goods_warehouse w,t_base_warehouse bw," +
                                  "t_goods_list g,t_user_list u   " +
                                  "where u.usercode = d.usercode and g.barcode = d.barcode and w.wid = bw.id " +
                                  "and d.barcode = w.barcode and w.supplierid = d.supplierid and d.wid = bw.id " +
                                  "and d.usercode = '" + userCode + "' " +
                                  "and d.barcode = '" + orderGoodsItem.barCode + "' " +
                                  " order by d.pnum asc";
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
                        if (apiType == "1")
                        {
                            error += orderGoodsItem.barCode + "没找到对应默认供货信息，";
                            continue;
                        }
                        else
                        {
                            msg.msg += "序号为" + orderGoodsItem.id + "行没找到对应默认供货信息，请核对\r\n";
                            continue;
                        }

                    }
                    if (!myDictionary.ContainsKey(wid))
                    {
                        myDictionary.Add(wid, new List<OrderGoodsItem>());
                    }
                    myDictionary[wid].Add(orderGoodsItem);
                }
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && error != "")
                {
                    errorDictionary[orderItem.merchantOrderId] += error;
                    continue;
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
                            orderItemNew.payType = orderItem.payType;
                            orderItemNew.derateName = orderItem.derateName;
                            orderItemNew.derate = orderItem.derate;
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
            //如果不是接口调用就判断是否有错误信息，有的话返回错误信息
            if (apiType != "1" && msg.msg != "")
            {
                return msg;
            }
            #region 价格分拆

            ArrayList al = new ArrayList();
            ArrayList goodsNumAl = new ArrayList();
            foreach (var orderItem in newOrderItemList)
            {
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && errorDictionary[orderItem.merchantOrderId] != "")
                {
                    continue;
                }
                double freight = 0, tradeAmount = 1;
                double.TryParse(orderItem.OrderGoods[0].dr["freight"].ToString(), out freight);
                double.TryParse(orderItem.tradeAmount, out tradeAmount);
                orderItem.freight = Math.Round(freight, 2);
                orderItem.platformId = orderItem.OrderGoods[0].dr["platformId"].ToString();
                orderItem.warehouseId = orderItem.OrderGoods[0].dr["wid"].ToString();
                orderItem.warehouseCode = orderItem.OrderGoods[0].dr["wcode"].ToString();
                orderItem.supplier = orderItem.OrderGoods[0].dr["suppliercode"].ToString();
                orderItem.purchaseId = orderItem.OrderGoods[0].dr["userId"].ToString();
                orderItem.purchase = userCode;
                double fr = Math.Round(orderItem.freight / tradeAmount, 4);

                //处理供货代理提点
                double supplierAgentCost = 0;
                DataRow[] drs = userDT.Select("userCode = '" + orderItem.supplier + "'");
                if (drs.Length > 0)
                {
                    if (drs[0]["ofAgent"].ToString() != "")
                    {
                        orderItem.supplierAgentCode = drs[0]["ofAgent"].ToString();
                        double.TryParse(drs[0]["agentCost"].ToString(), out supplierAgentCost);
                    }
                }

                //处理采购代理提点
                double purchaseAgentCost = 0;
                if (orderItem.OrderGoods[0].dr["ofAgent"].ToString() != "")
                {
                    //分销商需要看分销代理有没有采购代理。
                    if (orderItem.OrderGoods[0].dr["usertype"].ToString() == "4")
                    {
                        DataRow[] drs1 = userDT.Select("userCode = '" + orderItem.OrderGoods[0].dr["ofAgent"].ToString() + "'");
                        if (drs1.Length > 0)
                        {
                            if (drs1[0]["ofAgent"].ToString() != "")
                            {
                                orderItem.purchaseAgentCode = drs1[0]["ofAgent"].ToString();
                                double.TryParse(drs1[0]["agentCost"].ToString(), out purchaseAgentCost);
                            }
                        }
                    }
                    else
                    {
                        orderItem.purchaseAgentCode = orderItem.OrderGoods[0].dr["ofAgent"].ToString();
                        double.TryParse(orderItem.OrderGoods[0].dr["agentCost"].ToString(), out purchaseAgentCost);
                    }
                }
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
                        orderGoodsItem.tax = Math.Round(orderGoodsItem.tax, 2);
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
                            orderGoodsItem.platformPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * platformCost / (100 - platformCost);
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
                        orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);
                    }
                    //处理供货代理提点
                    orderGoodsItem.supplierAgentPrice = 0;
                    if (supplierAgentCost > 0)
                    {
                        //按供货价计算
                        orderGoodsItem.supplierAgentPrice = Math.Round(orderGoodsItem.supplyPrice * orderGoodsItem.quantity * supplierAgentCost / 100, 2);
                        orderGoodsItem.supplierAgentCode = orderItem.supplierAgentCode;
                        orderGoodsItem.supplierAgentPrice = Math.Round(orderGoodsItem.supplierAgentPrice, 2);
                    }
                    //处理采购代理提点
                    orderGoodsItem.purchaseAgentPrice = 0;
                    if (purchaseAgentCost > 0)
                    {
                        if (orderGoodsItem.dr["platformCostType"].ToString() == "1")//进价计算
                        {
                            orderGoodsItem.purchaseAgentPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * purchaseAgentCost / (100 - platformCost);
                        }
                        else if (orderGoodsItem.dr["platformCostType"].ToString() == "2")//售价计算
                        {
                            if (orderGoodsItem.dr["priceType"].ToString() == "1")//按订单售价计算
                            {
                                orderGoodsItem.purchaseAgentPrice = orderGoodsItem.totalPrice * purchaseAgentCost / 100;
                            }
                            else if (orderGoodsItem.dr["priceType"].ToString() == "2")//按供货价计算
                            {
                                orderGoodsItem.purchaseAgentPrice = orderGoodsItem.purchasePrice * orderGoodsItem.quantity * purchaseAgentCost / 100;
                            }
                        }
                        orderGoodsItem.purchaseAgentPrice = Math.Round(orderGoodsItem.purchaseAgentPrice, 2);
                    }




                    orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);

                    //判断误差，误差=供货价-进价-平台提点-供货代理提点-采购代理提点-运费分成-税
                    double deviation = orderGoodsItem.purchasePrice * orderGoodsItem.quantity
                        - orderGoodsItem.supplyPrice * orderGoodsItem.quantity
                        - orderGoodsItem.platformPrice
                        - orderGoodsItem.supplierAgentPrice
                        - orderGoodsItem.purchaseAgentPrice
                        - orderGoodsItem.waybillPrice
                        - orderGoodsItem.tax;
                    //如果有误差了，就从平台提点扣除
                    if (deviation != 0)
                    {
                        if (orderGoodsItem.platformPrice + deviation > 0)
                        {
                            orderGoodsItem.platformPrice = orderGoodsItem.platformPrice + deviation;
                        }
                        else
                        {
                            msg.msg = "订单" + orderItem.merchantOrderId + "价格有误差，请查对！";
                        }
                    }
                    //处理利润
                    //利润=售价-供货价
                    double profit = (orderGoodsItem.skuUnitPrice - orderGoodsItem.purchasePrice) * orderGoodsItem.quantity;
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
                                  "supplierAgentPrice,supplierAgentCode,purchaseAgentPrice,purchaseAgentCode," +
                                  "profitAgent,profitDealer,profitOther1,other1Name," +
                                  "profitOther2,other2Name,profitOther3,other3Name) " +
                                  "values('" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "','" + orderGoodsItem.slt + "','" + orderGoodsItem.skuUnitPrice + "'" +
                                  ",'" + orderGoodsItem.quantity + "','" + orderGoodsItem.skuBillName + "','','" + orderGoodsItem.skuBillName + "'" +
                                  ",'','','','0'" +
                                  ",'" + orderGoodsItem.suppliercode + "','" + orderGoodsItem.supplyPrice + "','" + orderGoodsItem.purchasePrice + "',''" +
                                  ",'" + orderGoodsItem.waybillPrice + "','" + orderGoodsItem.tax + "','" + orderGoodsItem.platformPrice + "','" + orderGoodsItem.profitPlatform + "'" +
                                  ",'" + orderGoodsItem.supplierAgentPrice + "','" + orderGoodsItem.supplierAgentCode + "','" + orderGoodsItem.purchaseAgentPrice + "','" + orderGoodsItem.purchaseAgentCode + "'" +
                                  ",'" + orderGoodsItem.profitAgent + "','" + orderGoodsItem.profitDealer + "','" + orderGoodsItem.profitOther1 + "','" + orderGoodsItem.other1Name + "'" +
                                  ",'" + orderGoodsItem.profitOther2 + "','" + orderGoodsItem.other2Name + "','" + orderGoodsItem.profitOther3 + "','" + orderGoodsItem.other3Name + "'" +
                                  ")";
                    al.Add(sqlgoods);

                    //if (orderGoodsItem.dr["pnum"].ToString() != "" && orderGoodsItem.dr["pnum"].ToString() != "0")
                    //{
                    string upsql = "update t_goods_distributor_price set pnum = pnum-" + orderGoodsItem.quantity + " where id = " + orderGoodsItem.dr["distributorId"].ToString();
                    goodsNumAl.Add(upsql);
                    string logsql = "insert into t_log_goodsnum(inputType,createtime,userCode,orderid,barcode,goodsnum,state) " +
                                "values('2',now(),'" + orderItem.purchase + "'," +
                                "'" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "'," +
                                "" + orderGoodsItem.quantity + ",'" + orderItem.status + "')";
                    goodsNumAl.Add(logsql);
                    //}
                    //else
                    //{
                    //    string upsql = "update t_goods_warehouse set goodsnum = goodsnum-" + orderGoodsItem.quantity + " where id = " + goodsWarehouseId;
                    //    goodsNumAl.Add(upsql);
                    //    string logsql = "insert into t_log_goodsnum(inputType,createtime,wid,wcode,orderid,barcode,goodsnum,state) " +
                    //                "values('1',now(),'" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "'," +
                    //                "'" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "'," +
                    //                "" + orderGoodsItem.quantity + ",'" + orderItem.status + "')";
                    //    goodsNumAl.Add(logsql);
                    //}


                }
                string sqlorder = "insert into t_order_list(warehouseId,warehouseCode,customerCode,actionType," +
                    "orderType,serviceType,parentOrderId,merchantOrderId," +
                    "payType,payNo,tradeTime,consigneeCode," +
                    "tradeAmount,goodsTotalAmount,consigneeName,consigneeMobile," +
                    "addrCountry,addrProvince,addrCity,addrDistrict," +
                    "addrDetail,zipCode,idType,idNumber," +
                    "idFountImgUrl,idBackImgUrl,status,purchaserCode," +
                    "purchaserId,distributionCode,apitype,waybillno," +
                    "expressId,inputTime,fqID,supplierAgentCode,purchaseAgentCode," +
                    "operate_status,sendapi,platformId,consignorName," +
                    "consignorMobile,consignorAddr,batchid,outNo,waybillOutNo," +
                    "accountsStatus,accountsNo,prePayId,ifPrint,printNo,preferentialName,preferentialPrice) " +
                    "values('" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "','" + orderItem.supplier + "',''" +
                    ",'','','" + orderItem.parentOrderId + "','" + orderItem.merchantOrderId + "'" +
                    ",'" + orderItem.payType + "','','" + orderItem.tradeTime + "',''" +
                    "," + orderItem.tradeAmount + ",'" + orderItem.tradeAmount + "','" + orderItem.consigneeName + "','" + orderItem.consigneeMobile + "'" +
                    ",'" + orderItem.addrCountry + "','" + orderItem.addrProvince + "','" + orderItem.addrCity + "','" + orderItem.addrDistrict + "'" +
                    ",'" + orderItem.addrDetail + "','','1','" + orderItem.idNumber + "'" +
                    ",'','','1','" + orderItem.purchase + "'" +
                    ",'" + orderItem.purchaseId + "','','2',''" +
                    ",'',now(),'','" + orderItem.supplierAgentCode + "','" + orderItem.purchaseAgentCode + "'" +
                    ",'0','','" + orderItem.platformId + "','" + orderItem.consignorName + "'" +
                    ",'" + orderItem.consignorMobile + "','" + orderItem.consignorAddr + "','','',''" +
                    ",'0','','','0','','" + orderItem.derateName + "','" + orderItem.derate + "') ";
                al.Add(sqlorder);
            }

            #endregion
            if (apiType != "1" && msg.msg != "")
            {
                return msg;
            }

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
            return msg;
        }
        /// <summary>
        /// 处理代销订单的方法-集万模板
        /// </summary>
        /// <param name="OrderItemList">订单bean</param>
        /// <param name="userCode">导入的用户code</param>
        /// <param name="apiType">判断是否是api接口，是1</param>
        /// <param name="errorDictionary">错误的集合</param>
        /// <returns></returns>
        public MsgResult orderHandleDXJW(List<OrderItem> OrderItemList, string userCode, string apiType, ref Dictionary<string, string> errorDictionary)
        {
            MsgResult msg = new MsgResult();
            string userSql = "select * from t_user_list";
            DataTable userDT = DatabaseOperationWeb.ExecuteSelectDS(userSql, "TABLE").Tables[0];

            #region 处理因仓库分单
            List<OrderItem> newOrderItemList = new List<OrderItem>();
            foreach (var orderItem in OrderItemList)
            {
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && errorDictionary[orderItem.merchantOrderId] != "")
                {
                    continue;
                }
                string error = ""; //当是接口调用的时候存放错误信息。
                Dictionary<int, List<OrderGoodsItem>> myDictionary = new Dictionary<int, List<OrderGoodsItem>>();
                foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
                {
                    string wsql = "select d.platformId,u.id as userId,u.platformCost,u.platformCostType,u.priceType,u.agentCost,u.usertype,u.ofAgent,d.pnum,d.id as distributorId," +
                                  "d.pprice,d.profitPlatform,d.profitAgent,d.profitDealer,d.profitOther1," +
                                  "d.profitOther1Name,d.profitOther2,d.profitOther2Name,d.profitOther3," +
                                  "d.profitOther3Name,w.id as goodsWarehouseId,w.wid,w.wcode,w.goodsnum,w.inprice,bw.taxation,bw.taxation2," +
                                  "bw.taxation2type,bw.taxation2line,bw.freight,w.suppliercode,g.NW,g.slt " +
                                  "from t_goods_distributor_price d ,t_goods_warehouse w,t_base_warehouse bw," +
                                  "t_goods_list g,t_user_list u   " +
                                  "where u.usercode = d.usercode and g.barcode = d.barcode and w.wid = bw.id " +
                                  "and d.barcode = w.barcode and w.supplierid = d.supplierid and d.wid = bw.id " +
                                  "and d.usercode = '" + userCode + "' " +
                                  "and d.barcode = '" + orderGoodsItem.barCode + "' " +
                                  " order by d.pnum asc";
                    DataTable wdt = DatabaseOperationWeb.ExecuteSelectDS(wsql, "TABLE").Tables[0];
                    int wid = 0;
                    if (wdt.Rows.Count == 1)
                    {
                        //处理商品单价
                        orderGoodsItem.skuUnitPrice = Convert.ToDouble(wdt.Rows[0]["pprice"]);
                        wid = Convert.ToInt16(wdt.Rows[0]["wid"]);
                        orderGoodsItem.dr = wdt.Rows[0];
                    }
                    else if (wdt.Rows.Count > 1)
                    {
                        //处理商品单价
                        orderGoodsItem.skuUnitPrice = Convert.ToDouble(wdt.Rows[0]["pprice"]);
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
                        if (apiType == "1")
                        {
                            error += orderGoodsItem.barCode + "没找到对应默认供货信息，";
                            continue;
                        }
                        else
                        {
                            msg.msg += "序号为" + orderGoodsItem.id + "行没找到对应默认供货信息，请核对\r\n";
                            continue;
                        }

                    }
                    if (!myDictionary.ContainsKey(wid))
                    {
                        myDictionary.Add(wid, new List<OrderGoodsItem>());
                    }
                    myDictionary[wid].Add(orderGoodsItem);
                }
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && error != "")
                {
                    errorDictionary[orderItem.merchantOrderId] += error;
                    continue;
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
                            orderItemNew.payType = orderItem.payType;
                            orderItemNew.derateName = orderItem.derateName;
                            orderItemNew.derate = orderItem.derate;
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
            //如果不是接口调用就判断是否有错误信息，有的话返回错误信息
            if (apiType != "1" && msg.msg != "")
            {
                return msg;
            }
            #region 价格分拆

            ArrayList al = new ArrayList();
            ArrayList goodsNumAl = new ArrayList();
            foreach (var orderItem in newOrderItemList)
            {
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && errorDictionary[orderItem.merchantOrderId] != "")
                {
                    continue;
                }
                double freight = 0, tradeAmount = 1;
                double.TryParse(orderItem.OrderGoods[0].dr["freight"].ToString(), out freight);
                double.TryParse(orderItem.tradeAmount, out tradeAmount);
                orderItem.freight = Math.Round(freight, 2);
                orderItem.platformId = orderItem.OrderGoods[0].dr["platformId"].ToString();
                orderItem.warehouseId = orderItem.OrderGoods[0].dr["wid"].ToString();
                orderItem.warehouseCode = orderItem.OrderGoods[0].dr["wcode"].ToString();
                orderItem.supplier = orderItem.OrderGoods[0].dr["suppliercode"].ToString();
                orderItem.purchaseId = orderItem.OrderGoods[0].dr["userId"].ToString();
                orderItem.purchase = userCode;
                double fr = Math.Round(orderItem.freight / tradeAmount, 4);

                //处理供货代理提点
                double supplierAgentCost = 0;
                DataRow[] drs = userDT.Select("userCode = '" + orderItem.supplier + "'");
                if (drs.Length > 0)
                {
                    if (drs[0]["ofAgent"].ToString() != "")
                    {
                        orderItem.supplierAgentCode = drs[0]["ofAgent"].ToString();
                        double.TryParse(drs[0]["agentCost"].ToString(), out supplierAgentCost);
                    }
                }

                //处理采购代理提点
                double purchaseAgentCost = 0;
                if (orderItem.OrderGoods[0].dr["ofAgent"].ToString() != "")
                {
                    //分销商需要看分销代理有没有采购代理。
                    if (orderItem.OrderGoods[0].dr["usertype"].ToString() == "4")
                    {
                        DataRow[] drs1 = userDT.Select("userCode = '" + orderItem.OrderGoods[0].dr["ofAgent"].ToString() + "'");
                        if (drs1.Length > 0)
                        {
                            if (drs1[0]["ofAgent"].ToString() != "")
                            {
                                orderItem.purchaseAgentCode = drs1[0]["ofAgent"].ToString();
                                double.TryParse(drs1[0]["agentCost"].ToString(), out purchaseAgentCost);
                            }
                        }
                    }
                    else
                    {
                        orderItem.purchaseAgentCode = orderItem.OrderGoods[0].dr["ofAgent"].ToString();
                        double.TryParse(orderItem.OrderGoods[0].dr["agentCost"].ToString(), out purchaseAgentCost);
                    }
                }
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
                        orderGoodsItem.tax = Math.Round(orderGoodsItem.tax, 2);
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
                            orderGoodsItem.platformPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * platformCost / (100 - platformCost);
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
                        orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);
                    }
                    //处理供货代理提点
                    orderGoodsItem.supplierAgentPrice = 0;
                    if (supplierAgentCost > 0)
                    {
                        //按供货价计算
                        orderGoodsItem.supplierAgentPrice = Math.Round(orderGoodsItem.supplyPrice * orderGoodsItem.quantity * supplierAgentCost / 100, 2);
                        orderGoodsItem.supplierAgentCode = orderItem.supplierAgentCode;
                        orderGoodsItem.supplierAgentPrice = Math.Round(orderGoodsItem.supplierAgentPrice, 2);
                    }
                    //处理采购代理提点
                    orderGoodsItem.purchaseAgentPrice = 0;
                    if (purchaseAgentCost > 0)
                    {
                        if (orderGoodsItem.dr["platformCostType"].ToString() == "1")//进价计算
                        {
                            orderGoodsItem.purchaseAgentPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * purchaseAgentCost / (100 - platformCost);
                        }
                        else if (orderGoodsItem.dr["platformCostType"].ToString() == "2")//售价计算
                        {
                            if (orderGoodsItem.dr["priceType"].ToString() == "1")//按订单售价计算
                            {
                                orderGoodsItem.purchaseAgentPrice = orderGoodsItem.totalPrice * purchaseAgentCost / 100;
                            }
                            else if (orderGoodsItem.dr["priceType"].ToString() == "2")//按供货价计算
                            {
                                orderGoodsItem.purchaseAgentPrice = orderGoodsItem.purchasePrice * orderGoodsItem.quantity * purchaseAgentCost / 100;
                            }
                        }
                        orderGoodsItem.purchaseAgentPrice = Math.Round(orderGoodsItem.purchaseAgentPrice, 2);
                    }




                    orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);

                    //判断误差，误差=供货价-进价-平台提点-供货代理提点-采购代理提点-运费分成-税
                    double deviation = orderGoodsItem.purchasePrice * orderGoodsItem.quantity
                        - orderGoodsItem.supplyPrice * orderGoodsItem.quantity
                        - orderGoodsItem.platformPrice
                        - orderGoodsItem.supplierAgentPrice
                        - orderGoodsItem.purchaseAgentPrice
                        - orderGoodsItem.waybillPrice
                        - orderGoodsItem.tax;
                    //如果有误差了，就从平台提点扣除
                    if (deviation != 0)
                    {
                        if (orderGoodsItem.platformPrice + deviation > 0)
                        {
                            orderGoodsItem.platformPrice = orderGoodsItem.platformPrice + deviation;
                        }
                        else
                        {
                            msg.msg = "订单" + orderItem.merchantOrderId + "价格有误差，请查对！";
                        }
                    }
                    //处理利润
                    //利润=售价-供货价
                    double profit = (orderGoodsItem.skuUnitPrice - orderGoodsItem.purchasePrice) * orderGoodsItem.quantity;
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
                                  "supplierAgentPrice,supplierAgentCode,purchaseAgentPrice,purchaseAgentCode," +
                                  "profitAgent,profitDealer,profitOther1,other1Name," +
                                  "profitOther2,other2Name,profitOther3,other3Name) " +
                                  "values('" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "','" + orderGoodsItem.slt + "','" + orderGoodsItem.skuUnitPrice + "'" +
                                  ",'" + orderGoodsItem.quantity + "','" + orderGoodsItem.skuBillName + "','','" + orderGoodsItem.skuBillName + "'" +
                                  ",'','','','0'" +
                                  ",'" + orderGoodsItem.suppliercode + "','" + orderGoodsItem.supplyPrice + "','" + orderGoodsItem.purchasePrice + "',''" +
                                  ",'" + orderGoodsItem.waybillPrice + "','" + orderGoodsItem.tax + "','" + orderGoodsItem.platformPrice + "','" + orderGoodsItem.profitPlatform + "'" +
                                  ",'" + orderGoodsItem.supplierAgentPrice + "','" + orderGoodsItem.supplierAgentCode + "','" + orderGoodsItem.purchaseAgentPrice + "','" + orderGoodsItem.purchaseAgentCode + "'" +
                                  ",'" + orderGoodsItem.profitAgent + "','" + orderGoodsItem.profitDealer + "','" + orderGoodsItem.profitOther1 + "','" + orderGoodsItem.other1Name + "'" +
                                  ",'" + orderGoodsItem.profitOther2 + "','" + orderGoodsItem.other2Name + "','" + orderGoodsItem.profitOther3 + "','" + orderGoodsItem.other3Name + "'" +
                                  ")";
                    al.Add(sqlgoods);

                    //if (orderGoodsItem.dr["pnum"].ToString() != "" && orderGoodsItem.dr["pnum"].ToString() != "0")
                    //{
                    string upsql = "update t_goods_distributor_price set pnum = pnum-" + orderGoodsItem.quantity + " where id = " + orderGoodsItem.dr["distributorId"].ToString();
                    goodsNumAl.Add(upsql);
                    string logsql = "insert into t_log_goodsnum(inputType,createtime,userCode,orderid,barcode,goodsnum,state) " +
                                "values('2',now(),'" + orderItem.purchase + "'," +
                                "'" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "'," +
                                "" + orderGoodsItem.quantity + ",'" + orderItem.status + "')";
                    goodsNumAl.Add(logsql);
                    //}
                    //else
                    //{
                    //    string upsql = "update t_goods_warehouse set goodsnum = goodsnum-" + orderGoodsItem.quantity + " where id = " + goodsWarehouseId;
                    //    goodsNumAl.Add(upsql);
                    //    string logsql = "insert into t_log_goodsnum(inputType,createtime,wid,wcode,orderid,barcode,goodsnum,state) " +
                    //                "values('1',now(),'" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "'," +
                    //                "'" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "'," +
                    //                "" + orderGoodsItem.quantity + ",'" + orderItem.status + "')";
                    //    goodsNumAl.Add(logsql);
                    //}


                }
                string sqlorder = "insert into t_order_list(warehouseId,warehouseCode,customerCode,actionType," +
                    "orderType,serviceType,parentOrderId,merchantOrderId," +
                    "payType,payNo,tradeTime,consigneeCode," +
                    "tradeAmount,goodsTotalAmount,consigneeName,consigneeMobile," +
                    "addrCountry,addrProvince,addrCity,addrDistrict," +
                    "addrDetail,zipCode,idType,idNumber," +
                    "idFountImgUrl,idBackImgUrl,status,purchaserCode," +
                    "purchaserId,distributionCode,apitype,waybillno," +
                    "expressId,inputTime,fqID,supplierAgentCode,purchaseAgentCode," +
                    "operate_status,sendapi,platformId,consignorName," +
                    "consignorMobile,consignorAddr,batchid,outNo,waybillOutNo," +
                    "accountsStatus,accountsNo,prePayId,ifPrint,printNo,preferentialName,preferentialPrice) " +
                    "values('" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "','" + orderItem.supplier + "',''" +
                    ",'','','" + orderItem.parentOrderId + "','" + orderItem.merchantOrderId + "'" +
                    ",'" + orderItem.payType + "','','" + orderItem.tradeTime + "',''" +
                    "," + orderItem.tradeAmount + ",'" + orderItem.tradeAmount + "','" + orderItem.consigneeName + "','" + orderItem.consigneeMobile + "'" +
                    ",'" + orderItem.addrCountry + "','" + orderItem.addrProvince + "','" + orderItem.addrCity + "','" + orderItem.addrDistrict + "'" +
                    ",'" + orderItem.addrDetail + "','','1','" + orderItem.idNumber + "'" +
                    ",'','','1','" + orderItem.purchase + "'" +
                    ",'" + orderItem.purchaseId + "','','2',''" +
                    ",'',now(),'','" + orderItem.supplierAgentCode + "','" + orderItem.purchaseAgentCode + "'" +
                    ",'0','','" + orderItem.platformId + "','" + orderItem.consignorName + "'" +
                    ",'" + orderItem.consignorMobile + "','" + orderItem.consignorAddr + "','','',''" +
                    ",'0','','','0','','" + orderItem.derateName + "','" + orderItem.derate + "') ";
                al.Add(sqlorder);
            }

            #endregion
            if (apiType != "1" && msg.msg != "")
            {
                return msg;
            }

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
            return msg;
        }

        /// <summary>
        /// 处理代销订单的方法-普通模板
        /// </summary>
        /// <param name="OrderItemList">订单bean</param>
        /// <param name="userCode">导入的用户code</param>
        /// <param name="apiType">判断是否是api接口，是1</param>
        /// <param name="errorDictionary">错误的集合</param>
        /// <returns></returns>
        public MsgResult orderHandleDX(List<OrderItem> OrderItemList, string userCode, string apiType, ref Dictionary<string, string> errorDictionary)
        {
            MsgResult msg = new MsgResult();
            string userSql = "select * from t_user_list";
            DataTable userDT = DatabaseOperationWeb.ExecuteSelectDS(userSql, "TABLE").Tables[0];

            #region 处理因仓库分单
            List<OrderItem> newOrderItemList = new List<OrderItem>();
            foreach (var orderItem in OrderItemList)
            {
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && errorDictionary[orderItem.merchantOrderId] != "")
                {
                    continue;
                }
                string error = ""; //当是接口调用的时候存放错误信息。
                Dictionary<int, List<OrderGoodsItem>> myDictionary = new Dictionary<int, List<OrderGoodsItem>>();
                foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
                {
                    string wsql = "select d.platformId,u.id as userId,u.platformCost,u.platformCostType,u.priceType,u.agentCost,u.usertype,u.ofAgent,d.pnum,d.id as distributorId," +
                                  "d.pprice,d.profitPlatform,d.profitAgent,d.profitDealer,d.profitOther1," +
                                  "d.profitOther1Name,d.profitOther2,d.profitOther2Name,d.profitOther3," +
                                  "d.profitOther3Name,w.id as goodsWarehouseId,w.wid,w.wcode,w.goodsnum,w.inprice,bw.taxation,bw.taxation2," +
                                  "bw.taxation2type,bw.taxation2line,bw.freight,w.suppliercode,g.NW,g.slt " +
                                  "from t_goods_distributor_price d ,t_goods_warehouse w,t_base_warehouse bw," +
                                  "t_goods_list g,t_user_list u   " +
                                  "where u.usercode = d.usercode and g.barcode = d.barcode and w.wid = bw.id " +
                                  "and d.barcode = w.barcode and w.supplierid = d.supplierid and d.wid = bw.id " +
                                  "and d.usercode = '" + userCode + "' " +
                                  "and d.barcode = '" + orderGoodsItem.barCode + "' " +
                                  " order by d.pnum asc";
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
                        if (apiType == "1")
                        {
                            error += orderGoodsItem.barCode + "没找到对应默认供货信息，";
                            continue;
                        }
                        else
                        {
                            msg.msg += "序号为" + orderGoodsItem.id + "行没找到对应默认供货信息，请核对\r\n";
                            continue;
                        }

                    }
                    if (!myDictionary.ContainsKey(wid))
                    {
                        myDictionary.Add(wid, new List<OrderGoodsItem>());
                    }
                    myDictionary[wid].Add(orderGoodsItem);
                }
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && error != "")
                {
                    errorDictionary[orderItem.merchantOrderId] += error;
                    continue;
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
                            orderItemNew.payType = orderItem.payType;
                            orderItemNew.derateName = orderItem.derateName;
                            orderItemNew.derate = orderItem.derate;
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
            //如果不是接口调用就判断是否有错误信息，有的话返回错误信息
            if (apiType != "1" && msg.msg != "")
            {
                return msg;
            }
            #region 价格分拆

            ArrayList al = new ArrayList();
            ArrayList goodsNumAl = new ArrayList();
            foreach (var orderItem in newOrderItemList)
            {
                //判断是否是接口，如果是接口就判断订单是否有问题，有问题的就不处理。
                if (apiType == "1" && errorDictionary[orderItem.merchantOrderId] != "")
                {
                    continue;
                }
                double tradeAmount = 1;
                double.TryParse(orderItem.tradeAmount, out tradeAmount);
                orderItem.platformId = orderItem.OrderGoods[0].dr["platformId"].ToString();
                orderItem.warehouseId = orderItem.OrderGoods[0].dr["wid"].ToString();
                orderItem.warehouseCode = orderItem.OrderGoods[0].dr["wcode"].ToString();
                orderItem.supplier = orderItem.OrderGoods[0].dr["suppliercode"].ToString();
                orderItem.purchaseId = orderItem.OrderGoods[0].dr["userId"].ToString();
                orderItem.purchase = userCode;

                //处理供货代理提点
                double supplierAgentCost = 0;
                DataRow[] drs = userDT.Select("userCode = '" + orderItem.supplier + "'");
                if (drs.Length > 0)
                {
                    if (drs[0]["ofAgent"].ToString() != "")
                    {
                        orderItem.supplierAgentCode = drs[0]["ofAgent"].ToString();
                        double.TryParse(drs[0]["agentCost"].ToString(), out supplierAgentCost);
                    }
                }

                //处理采购代理提点
                double purchaseAgentCost = 0;
                if (orderItem.OrderGoods[0].dr["ofAgent"].ToString() != "")
                {
                    //分销商需要看分销代理有没有采购代理。
                    if (orderItem.OrderGoods[0].dr["usertype"].ToString() == "4")
                    {
                        DataRow[] drs1 = userDT.Select("userCode = '" + orderItem.OrderGoods[0].dr["ofAgent"].ToString() + "'");
                        if (drs1.Length > 0)
                        {
                            if (drs1[0]["ofAgent"].ToString() != "")
                            {
                                orderItem.purchaseAgentCode = drs1[0]["ofAgent"].ToString();
                                double.TryParse(drs1[0]["agentCost"].ToString(), out purchaseAgentCost);
                            }
                        }
                    }
                    else
                    {
                        orderItem.purchaseAgentCode = orderItem.OrderGoods[0].dr["ofAgent"].ToString();
                        double.TryParse(orderItem.OrderGoods[0].dr["agentCost"].ToString(), out purchaseAgentCost);
                    }
                }
                for (int i = 0; i < orderItem.OrderGoods.Count; i++)
                {
                    OrderGoodsItem orderGoodsItem = orderItem.OrderGoods[i];

                    //代销无运费
                    orderGoodsItem.waybillPrice = 0;


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
                        orderGoodsItem.tax = Math.Round(orderGoodsItem.tax, 2);
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
                            orderGoodsItem.platformPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * platformCost / (100 - platformCost);
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
                        orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);
                    }
                    //处理供货代理提点
                    orderGoodsItem.supplierAgentPrice = 0;
                    if (supplierAgentCost > 0)
                    {
                        //按供货价计算
                        orderGoodsItem.supplierAgentPrice = Math.Round(orderGoodsItem.supplyPrice * orderGoodsItem.quantity * supplierAgentCost / 100, 2);
                        orderGoodsItem.supplierAgentCode = orderItem.supplierAgentCode;
                        orderGoodsItem.supplierAgentPrice = Math.Round(orderGoodsItem.supplierAgentPrice, 2);
                    }
                    //处理采购代理提点
                    orderGoodsItem.purchaseAgentPrice = 0;
                    if (purchaseAgentCost > 0)
                    {
                        if (orderGoodsItem.dr["platformCostType"].ToString() == "1")//进价计算
                        {
                            orderGoodsItem.purchaseAgentPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * purchaseAgentCost / (100 - platformCost);
                        }
                        else if (orderGoodsItem.dr["platformCostType"].ToString() == "2")//售价计算
                        {
                            if (orderGoodsItem.dr["priceType"].ToString() == "1")//按订单售价计算
                            {
                                orderGoodsItem.purchaseAgentPrice = orderGoodsItem.totalPrice * purchaseAgentCost / 100;
                            }
                            else if (orderGoodsItem.dr["priceType"].ToString() == "2")//按供货价计算
                            {
                                orderGoodsItem.purchaseAgentPrice = orderGoodsItem.purchasePrice * orderGoodsItem.quantity * purchaseAgentCost / 100;
                            }
                        }
                        orderGoodsItem.purchaseAgentPrice = Math.Round(orderGoodsItem.purchaseAgentPrice, 2);
                    }




                    orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);

                    //判断误差，误差=供货价-进价-平台提点-供货代理提点-采购代理提点-运费分成-税
                    double deviation = orderGoodsItem.purchasePrice * orderGoodsItem.quantity
                        - orderGoodsItem.supplyPrice * orderGoodsItem.quantity
                        - orderGoodsItem.platformPrice
                        - orderGoodsItem.supplierAgentPrice
                        - orderGoodsItem.purchaseAgentPrice
                        - orderGoodsItem.waybillPrice
                        - orderGoodsItem.tax;
                    //如果有误差了，就从平台提点扣除
                    if (deviation != 0)
                    {
                        if (orderGoodsItem.platformPrice + deviation > 0)
                        {
                            orderGoodsItem.platformPrice = orderGoodsItem.platformPrice + deviation;
                        }
                        else
                        {
                            msg.msg = "订单" + orderItem.merchantOrderId + "价格有误差，请查对！";
                        }
                    }
                    //处理利润
                    //利润=售价-供货价
                    double profit = (orderGoodsItem.skuUnitPrice - orderGoodsItem.purchasePrice) * orderGoodsItem.quantity;
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
                                  "supplierAgentPrice,supplierAgentCode,purchaseAgentPrice,purchaseAgentCode," +
                                  "profitAgent,profitDealer,profitOther1,other1Name," +
                                  "profitOther2,other2Name,profitOther3,other3Name) " +
                                  "values('" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "','" + orderGoodsItem.slt + "','" + orderGoodsItem.skuUnitPrice + "'" +
                                  ",'" + orderGoodsItem.quantity + "','" + orderGoodsItem.skuBillName + "','','" + orderGoodsItem.skuBillName + "'" +
                                  ",'','','','0'" +
                                  ",'" + orderGoodsItem.suppliercode + "','" + orderGoodsItem.supplyPrice + "','" + orderGoodsItem.purchasePrice + "',''" +
                                  ",'" + orderGoodsItem.waybillPrice + "','" + orderGoodsItem.tax + "','" + orderGoodsItem.platformPrice + "','" + orderGoodsItem.profitPlatform + "'" +
                                  ",'" + orderGoodsItem.supplierAgentPrice + "','" + orderGoodsItem.supplierAgentCode + "','" + orderGoodsItem.purchaseAgentPrice + "','" + orderGoodsItem.purchaseAgentCode + "'" +
                                  ",'" + orderGoodsItem.profitAgent + "','" + orderGoodsItem.profitDealer + "','" + orderGoodsItem.profitOther1 + "','" + orderGoodsItem.other1Name + "'" +
                                  ",'" + orderGoodsItem.profitOther2 + "','" + orderGoodsItem.other2Name + "','" + orderGoodsItem.profitOther3 + "','" + orderGoodsItem.other3Name + "'" +
                                  ")";
                    al.Add(sqlgoods);

                    //if (orderGoodsItem.dr["pnum"].ToString() != "" && orderGoodsItem.dr["pnum"].ToString() != "0")
                    //{
                    string upsql = "update t_goods_distributor_price set pnum = pnum-" + orderGoodsItem.quantity + " where id = " + orderGoodsItem.dr["distributorId"].ToString();
                    goodsNumAl.Add(upsql);
                    string logsql = "insert into t_log_goodsnum(inputType,createtime,userCode,orderid,barcode,goodsnum,state) " +
                                "values('2',now(),'" + orderItem.purchase + "'," +
                                "'" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "'," +
                                "" + orderGoodsItem.quantity + ",'" + orderItem.status + "')";
                    goodsNumAl.Add(logsql);
                    //}
                    //else
                    //{
                    //    string upsql = "update t_goods_warehouse set goodsnum = goodsnum-" + orderGoodsItem.quantity + " where id = " + goodsWarehouseId;
                    //    goodsNumAl.Add(upsql);
                    //    string logsql = "insert into t_log_goodsnum(inputType,createtime,wid,wcode,orderid,barcode,goodsnum,state) " +
                    //                "values('1',now(),'" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "'," +
                    //                "'" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "'," +
                    //                "" + orderGoodsItem.quantity + ",'" + orderItem.status + "')";
                    //    goodsNumAl.Add(logsql);
                    //}


                }
                string sqlorder = "insert into t_order_list(warehouseId,warehouseCode,customerCode,actionType," +
                    "orderType,serviceType,parentOrderId,merchantOrderId," +
                    "payType,payNo,tradeTime,consigneeCode," +
                    "tradeAmount,goodsTotalAmount,consigneeName,consigneeMobile," +
                    "addrCountry,addrProvince,addrCity,addrDistrict," +
                    "addrDetail,zipCode,idType,idNumber," +
                    "idFountImgUrl,idBackImgUrl,status,purchaserCode," +
                    "purchaserId,distributionCode,apitype,waybillno," +
                    "expressId,inputTime,fqID,supplierAgentCode,purchaseAgentCode," +
                    "operate_status,sendapi,platformId,consignorName," +
                    "consignorMobile,consignorAddr,batchid,outNo,waybillOutNo," +
                    "accountsStatus,accountsNo,prePayId,ifPrint,printNo,preferentialName,preferentialPrice) " +
                    "values('" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "','" + orderItem.supplier + "',''" +
                    ",'','','" + orderItem.parentOrderId + "','" + orderItem.merchantOrderId + "'" +
                    ",'" + orderItem.payType + "','','" + orderItem.tradeTime + "',''" +
                    "," + orderItem.tradeAmount + ",'" + orderItem.tradeAmount + "','" + orderItem.consigneeName + "','" + orderItem.consigneeMobile + "'" +
                    ",'" + orderItem.addrCountry + "','" + orderItem.addrProvince + "','" + orderItem.addrCity + "','" + orderItem.addrDistrict + "'" +
                    ",'" + orderItem.addrDetail + "','','1','" + orderItem.idNumber + "'" +
                    ",'','','1','" + orderItem.purchase + "'" +
                    ",'" + orderItem.purchaseId + "','','2',''" +
                    ",'',now(),'','" + orderItem.supplierAgentCode + "','" + orderItem.purchaseAgentCode + "'" +
                    ",'0','','" + orderItem.platformId + "','" + orderItem.consignorName + "'" +
                    ",'" + orderItem.consignorMobile + "','" + orderItem.consignorAddr + "','','',''" +
                    ",'0','','','0','','" + orderItem.derateName + "','" + orderItem.derate + "') ";
                al.Add(sqlorder);
            }

            #endregion
            if (apiType != "1" && msg.msg != "")
            {
                return msg;
            }

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
            return msg;
        }
        #endregion


    }
}
