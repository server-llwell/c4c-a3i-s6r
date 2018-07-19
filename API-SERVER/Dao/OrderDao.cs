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
        /// 获取订单列表
        /// </summary>
        /// <param name="orderParam">查询信息</param>
        /// <param name="apiType">订单的类别，空白就不区分类别</param>
        /// <param name="ifShowConsignee">是否显示收货人信息</param>
        /// <returns></returns>
        public PageResult getOrderList(OrderParam orderParam, string apiType, bool ifShowConsignee)
        {
            PageResult OrderResult = new PageResult();
            OrderResult.pagination = new Page(orderParam.current, orderParam.pageSize);
            OrderResult.list = new List<Object>();
            string st = "";
            if (apiType != null && apiType != "")
            {
                st = " and apitype='" + apiType + "' ";
            }
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
            else
            {
                st += " and purchaserId='" + orderParam.userId + "' ";
            }

            if (orderParam.date != null && orderParam.date.Length == 2)
            {
                st += " and tradeTime BETWEEN '" + orderParam.date[0] + "' AND DATE_ADD('" + orderParam.date[1] + "',INTERVAL 1 DAY) ";
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
                st += " and purchaserId = '" + orderParam.shopId + "' ";
            }
            string sql = "SELECT id,status,merchantOrderId,tradeTime,e.expressName,waybillno,consigneeName,tradeAmount,s.statusName " +
                         "FROM t_base_status s,t_order_list t left join t_base_express e on t.expressId = e.expressId " +
                         " where s.statusId=t.status " + st +
                         " ORDER BY id desc LIMIT " + (orderParam.current - 1) * orderParam.pageSize + "," + orderParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql1 = "SELECT count(*) FROM t_order_list t " +
                         " where 1=1 " + st;

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_order_list").Tables[0];
                OrderResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.id = dt.Rows[i]["id"].ToString();
                    orderItem.tradeAmount = dt.Rows[0]["tradeAmount"].ToString();
                    orderItem.merchantOrderId = dt.Rows[i]["merchantOrderId"].ToString();
                    orderItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    orderItem.expressName = dt.Rows[i]["expressName"].ToString();
                    orderItem.waybillno = dt.Rows[i]["waybillno"].ToString();
                    orderItem.status = dt.Rows[i]["statusName"].ToString();
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

            }
            return OrderResult;
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
            string sql1 = "select * FROM t_order_list " +
                          "where merchantOrderId  = '" + orderParam.orderId + "'";
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

                orderItem.OrderGoods = new List<OrderGoodsItem>();
                string sql2 = "select * from t_order_goods where  merchantOrderId  = '" + orderParam.orderId + "'";
                DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "t_daigou_ticket").Tables[0];
                if (dt2.Rows.Count > 0)
                {
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        OrderGoodsItem orderGoods = new OrderGoodsItem();
                        try
                        {
                            orderGoods.totalPrice = (Convert.ToDouble(dt2.Rows[i]["skuUnitPrice"]) * Convert.ToDouble(dt2.Rows[i]["quantity"])).ToString();
                        }
                        catch (Exception)
                        {
                        }
                        orderGoods.id = dt2.Rows[i]["id"].ToString();
                        orderGoods.slt = dt2.Rows[i]["slt"].ToString();
                        orderGoods.barCode = dt2.Rows[i]["barCode"].ToString();
                        orderGoods.skuUnitPrice = dt2.Rows[i]["skuUnitPrice"].ToString();
                        orderGoods.skuBillName = dt2.Rows[i]["skuBillName"].ToString();
                        orderGoods.quantity = dt2.Rows[i]["quantity"].ToString();
                        orderGoods.purchasePrice = dt2.Rows[i]["purchasePrice"].ToString();
                        orderItem.OrderGoods.Add(orderGoods);
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
            string fileName = "export_" + orderParam.userId + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx";
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
                         "and (t.status = 1 or t.status= 2) " + st;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_ticket").Tables[0];
            if (dt.Rows.Count > 0)
            {
                FileManager fm = new FileManager();
                if (fm.writeDataTableToExcel(dt, fileName))
                {
                    if (fm.updateFileToOSS(fileName, Global.OssDirOrder))
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

        public MsgResult UploadWaybill(FileUploadParam uploadParam)
        {
            MsgResult msg = new MsgResult();
            string logCode = uploadParam.userId + DateTime.Now.ToString("yyyyMMddHHmmssff");
            string fileName = logCode + ".xlsx";
            FileManager fm = new FileManager();
            if (fm.saveFileByBase64String(uploadParam.byte64, fileName))
            {
                DataTable dt = fm.readExcelFileToDataTable(fileName);
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

        #endregion
    }
}