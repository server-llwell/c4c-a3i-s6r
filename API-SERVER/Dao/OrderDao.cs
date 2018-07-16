using Aliyun.OSS;
using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
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
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="orderParam">查询信息</param>
        /// <param name="apiType">订单的类别，空白就不区分类别</param>
        /// <param name="ifShowConsignee">是否显示收货人信息</param>
        /// <returns></returns>
        public PageResult getOrderList(OrderParam orderParam,string apiType,bool ifShowConsignee)
        {
            PageResult OrderResult = new PageResult();
            OrderResult.pagination = new Page(orderParam.current, orderParam.pageSize);
            OrderResult.list = new List<Object>();
            string st = "";
            if (apiType!=null && apiType!="")
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

            if (orderParam.date != null && orderParam.date.Length ==2)
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
            if (orderParam.shopId != null && orderParam.shopId != "")
            {
                st += " and purchaserId = '" + orderParam.shopId + "' ";
            }
            string sql = "SELECT id,status,merchantOrderId,tradeTime,waybillno,consigneeName,tradeAmount FROM t_order_list t " +
                         " where 1=1 " + st +
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
                    orderItem.waybillno = dt.Rows[i]["waybillno"].ToString();
                    orderItem.status = dt.Rows[i]["status"].ToString();
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
                orderItem.status = dt.Rows[0]["status"].ToString();

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
        

        //上传图片到
        private bool updateCVSToOSS(string fileName)
        {
            try
            {
                OssClient client = OssManager.GetInstance();
                ObjectMetadata metadata = new ObjectMetadata();
                // 可以设定自定义的metadata。
                metadata.UserMetadata.Add("uname", "airong");
                metadata.UserMetadata.Add("fromfileName", fileName);
                using (var fs = File.OpenRead(path + "\\" + fileName))
                {
                    var ret = client.PutObject(Global.OssBucket, Global.OssDirOrder + fileName, fs, metadata);
                }
                return true;
            }
            catch (Exception e)
            {
                try
                {
                    string sql = "insert into t_log_error(code,errLog) values('exportOrderCVS','" + e.ToString().Replace("'", "‘") + "')";
                    DatabaseOperationWeb.ExecuteDML(sql);
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

        }
    }
}
