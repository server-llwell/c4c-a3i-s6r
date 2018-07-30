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
                st += " and purchaserCode='" + orderParam.userId + "' ";
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
                st += " and purchaserCode = '" + orderParam.shopId + "' ";
            }
            string sql = "SELECT id,status,(select username from t_user_list where usercode =customerCode) customerCode," +
                         "(select username from t_user_list where usercode =purchaserCode) purchaser,merchantOrderId," +
                         "tradeTime,e.expressName,waybillno,consigneeName,tradeAmount,s.statusName " +
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
                    orderItem.purchase = dt.Rows[i]["purchaser"].ToString();
                    orderItem.supplier = dt.Rows[i]["customerCode"].ToString();
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
            else
            {
                st += " and purchaserCode='" + orderParam.userId + "' ";
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
                    ArrayList al = new ArrayList();
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
                                orderGoodsItem.skuUnitPrice = dt.Rows[i]["商品申报单价"].ToString();
                                orderGoodsItem.skuBillName = dt.Rows[i]["商品名称"].ToString();
                                orderGoodsItem.quantity = dt.Rows[i]["商品数量"].ToString();
                                OrderItemList[j].OrderGoods.Add(orderGoodsItem);
                                isNotFound = false;
                                break;
                            }
                        }
                        if (isNotFound)//没有对应订单
                        {
                            OrderItem orderItem = new OrderItem();
                            orderItem.merchantOrderId = dt.Rows[i]["订单号"].ToString();
                            orderItem.tradeTime = dt.Rows[i]["创建时间"].ToString();
                            orderItem.consigneeName = dt.Rows[i]["收货人"].ToString();
                            orderItem.consigneeMobile = dt.Rows[i]["收货人电话"].ToString();
                            orderItem.idNumber = dt.Rows[i]["收件人身份证号"].ToString();
                            orderItem.addrCountry = dt.Rows[i]["收货人国家"].ToString();
                            orderItem.addrProvince = dt.Rows[i]["收货人省"].ToString();
                            orderItem.addrCity = dt.Rows[i]["收货人市"].ToString();
                            orderItem.addrDistrict = dt.Rows[i]["收货人区"].ToString();
                            orderItem.addrDetail = dt.Rows[i]["收货人地址"].ToString();
                            orderItem.OrderGoods = new List<OrderGoodsItem>();
                            OrderGoodsItem orderGoodsItem = new OrderGoodsItem();
                            orderGoodsItem.id = dt.Rows[i]["序号"].ToString();
                            orderGoodsItem.barCode = dt.Rows[i]["商品条码"].ToString();
                            orderGoodsItem.skuUnitPrice = dt.Rows[i]["商品申报单价"].ToString();
                            orderGoodsItem.skuBillName = dt.Rows[i]["商品名称"].ToString();
                            orderGoodsItem.quantity = dt.Rows[i]["商品数量"].ToString();
                            orderItem.OrderGoods.Add(orderGoodsItem);
                            OrderItemList.Add(orderItem);
                        }
                    }
                    #endregion

                    #region 处理因仓库分单
                    List<OrderItem> newOrderItemList = new List<OrderItem>();
                    foreach (var orderItem in OrderItemList)
                    {
                        //if (orderItem.OrderGoods.Count()>1)
                        //{
                        Dictionary<int, List<OrderGoodsItem>> myDictionary = new Dictionary<int, List<OrderGoodsItem>>();
                        foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
                        {
                            string wsql = "select d.pprice,d.profitPlatform,d.profitAgent,d.profitDealer,d.profitOther1," +
                            "d.profitOther1Name,d.profitOther2,d.profitOther2Name,d.profitOther3,d.profitOther3Name,w.wid," +
                            "w.wcode,w.goodsnum,w.inprice,bw.taxation,bw.taxation2,bw.taxation2type,bw.taxation2line,bw.freight " +
                            "from t_goods_distributor_price d ,t_goods_warehouse w,t_base_warehouse bw where w.wid = bw.id " +
                            "and d.barcode = w.barcode and w.supplierid = d.supplierid and d.usercode = 'admin' " +
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
                                msg.msg += "序号为" + orderGoodsItem.id + "行没找到对应供货信息，请核对\r\n";
                                continue;
                            }
                            if (!myDictionary.ContainsKey(wid))
                            {
                                myDictionary.Add(wid, new List<OrderGoodsItem>());
                            }
                            //分拆订单



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
                        //}
                        //else
                        //{
                        //    newOrderItemList.Add(orderItem);
                        //}
                    }
                    #endregion

                    #region 分拆订单


                    #endregion
                    ////查询渠道信息
                    //string purchaseSql = "select * from t_user_list u ,t_goods_distributor_price d " +
                    //                     "where u.usercode = d.usercode and d.usercode = '" + uploadParam.userId + "'" +
                    //                     " and d.barcode = '" + dt.Rows[i]["商品条码"].ToString() + "'";
                    //DataTable purchaseDT = DatabaseOperationWeb.ExecuteSelectDS(purchaseSql, "TABLE").Tables[0];
                    //if (purchaseDT.Rows.Count > 0)
                    //{
                    //    //查询供货信息
                    //    string supplierSql = "select * from t_goods_warehouse g,t_base_warehouse w " +
                    //        "where w.id = g.wid and  g.supplierid = '" + purchaseDT.Rows[0]["supplierid"].ToString() + "' " +
                    //        "and  g.barcode = '" + dt.Rows[i]["商品条码"].ToString() + "'";
                    //    DataTable supplierDT = DatabaseOperationWeb.ExecuteSelectDS(supplierSql, "TABLE").Tables[0];
                    //    if (supplierDT.Rows.Count > 0)
                    //    {
                    //        //p对外供货价，s供货商价格，w运费，tp税，x平台提点，r毛利，rp平台利润，ra代理利润，rd分销利润，r1-r3其他1-3利润
                    //        double p = 0, s = 0, tp = 0, w = 0, x = 0, r = 0, rp = 0, ra = 0, rd = 0, r1 = 0, r2 = 0, r3 = 0;
                    //        try
                    //        {
                    //            p = Convert.ToDouble(purchaseDT.Rows[0]["pprice"]);//对外供货价
                    //            s = Convert.ToDouble(supplierDT.Rows[0]["inprice"]);//供货商价格
                    //            r = p - s;//毛利
                    //            w = Convert.ToDouble(supplierDT.Rows[0]["inprice"]);//运费
                    //            if (supplierDT.Rows[0]["taxation2type"].ToString() == "1")//按总价提档
                    //            {

                    //            }
                    //            else// 按元/克提档
                    //            {

                    //            }
                    //        }
                    //        catch (Exception)
                    //        {
                    //            msg.msg += "第" + (i + 2).ToString() + "行商品价格分解错误，请核对\r\n";
                    //            continue;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        msg.msg += "第" + (i + 2).ToString() + "行没找到对应供货信息，请核对\r\n";
                    //        continue;
                    //    }
                    //}
                    //else
                    //{
                    //    msg.msg += "第" + (i + 2).ToString() + "行没找到对应渠道商品条码，请核对\r\n";
                    //    continue;
                    //}
                    if (msg.msg != "")
                    {
                        return msg;
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
