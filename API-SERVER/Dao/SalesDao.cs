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
    public class SalesDao
    {
        private string path = System.Environment.CurrentDirectory;

        public SalesDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        public List<PurchaseItem> getPurchase()
        {
            List<PurchaseItem> lp = new List<PurchaseItem>();

            string sql = "SELECT usercode,username FROM t_user_list where usertype ='2'";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                PurchaseItem purchaseItem = new PurchaseItem();
                purchaseItem.purchaseCode = dt.Rows[i]["usercode"].ToString();
                purchaseItem.purchaseName = dt.Rows[i]["username"].ToString();
                lp.Add(purchaseItem);
            }
            return lp;
        }
        public List<DistributionItem> getDistribution()
        {
            List<DistributionItem> lp = new List<DistributionItem>();

            string sql = "SELECT usercode,username FROM t_user_list where usertype ='4'";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DistributionItem distributionItem = new DistributionItem();
                distributionItem.distributionCode = dt.Rows[i]["usercode"].ToString();
                distributionItem.distributionName = dt.Rows[i]["username"].ToString();
                lp.Add(distributionItem);
            }
            return lp;
        }
        public PageResult getSalesListByOperator(SalesSeachParam salesSeachParam)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(salesSeachParam.current, salesSeachParam.pageSize);
            pageResult.list = new List<Object>();
            string st = "";
            if (salesSeachParam.barcode != null && salesSeachParam.barcode != "")
            {
                st += " and g.barcode like '%" + salesSeachParam.barcode + "%' ";
            }
            if (salesSeachParam.goodsName != null && salesSeachParam.goodsName != "")
            {
                st += " and g.goodsName like '%" + salesSeachParam.goodsName + "%' ";
            }
            if (salesSeachParam.brand != null && salesSeachParam.brand != "")
            {
                st += " and g.brand like '%" + salesSeachParam.brand + "%' ";
            }
            if (salesSeachParam.purchaseCode != null && salesSeachParam.purchaseCode != "")
            {
                st += " and o.purchaserCode ='" + salesSeachParam.purchaseCode + "' ";
            }
            if (salesSeachParam.platformId != null && salesSeachParam.platformId != "")
            {
                st += " and o.platformId ='" + salesSeachParam.platformId + "' ";
            }
            if (salesSeachParam.date != null && salesSeachParam.date.Length == 2)
            {
                st += " and o.tradeTime BETWEEN str_to_date('" + salesSeachParam.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + salesSeachParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }

            string totalsql = "select sum(IFNULL(g.quantity,0)) as salesNumTotal,sum(IFNULL(g.purchasePrice,0)) as salesPriceTotal ," +
                              "sum(IFNULL(g.supplyPrice,0)) as costTotal, sum(IFNULL(g.profitDealer,0)) as brokerageTotal ," +
                              "sum(IFNULL(g.purchasePrice,0))-sum(IFNULL(g.supplyPrice,0)) as grossProfitTotal " +
                              "from t_order_goods g,t_order_list o " +
                              "where g.merchantOrderId = o.merchantOrderId " + st +
                              " group by barcode";
            DataTable totaldt = DatabaseOperationWeb.ExecuteSelectDS(totalsql, "TABLE").Tables[0];
            if (totaldt.Rows.Count > 0)
            {
                SalesListItem salesListItem = new SalesListItem();
                for (int j = 0; j < totaldt.Rows.Count; j++)
                {
                    salesListItem.salesNumTotal += Convert.ToInt16(totaldt.Rows[j]["salesNumTotal"].ToString());
                    salesListItem.salesPriceTotal += Convert.ToDouble(totaldt.Rows[j]["salesPriceTotal"].ToString());
                    salesListItem.costTotal += Convert.ToDouble(totaldt.Rows[j]["costTotal"].ToString());
                    salesListItem.grossProfitTotal += Convert.ToDouble(totaldt.Rows[j]["grossProfitTotal"].ToString());
                }
                pageResult.pagination.total = totaldt.Rows.Count;
                pageResult.item = salesListItem;
                string sql = "select (select platformType from t_base_platform where platformId= o.platformId) as platformType," +
                             " g.barcode,u.username as purchaserName, sum(IFNULL(g.quantity,0)) as salesNum," +
                             "sum(IFNULL(g.purchasePrice,0)) as salesPrice,sum(IFNULL(g.supplyPrice,0)) as cost," +
                             "sum(IFNULL(g.purchasePrice,0))-sum(IFNULL(g.supplyPrice,0)) as grossProfit," +
                             "sum(IFNULL(g.profitDealer,0)) as brokerage " +
                             "from t_order_goods g,t_order_list o left join t_user_list u on o.purchaserCode = u.usercode " +
                             "where g.merchantOrderId = o.merchantOrderId " + st +
                             "group by g.barCode,o.platformId,o.purchaserCode " +
                             "ORDER BY g.barCode asc LIMIT " + (salesSeachParam.current - 1) * salesSeachParam.pageSize + "," + salesSeachParam.pageSize;
                DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SalesItem salesItem = new SalesItem();
                    salesItem.id = (salesSeachParam.current - 1) * salesSeachParam.pageSize + 1 + i;
                    salesItem.barcode = dt.Rows[i]["barcode"].ToString();
                    string gsql = "select (select c1.name from t_goods_category c1 where (c1.id = g.catelog1)) AS c1 ," +
                                  "(select c1.name from t_goods_category c1 where (c1.id = g.catelog2)) AS c2," +
                                  "(select c1.name from t_goods_category c1 where (c1.id = g.catelog3)) AS c3," +
                                  " g.brand,g.slt,g.goodsName " +
                                  "from t_goods_list g " +
                                  "where g.barcode = '" + dt.Rows[i]["barcode"].ToString() + "'";
                    DataTable gdt = DatabaseOperationWeb.ExecuteSelectDS(gsql, "TABLE").Tables[0];
                    if (gdt.Rows.Count > 0)
                    {
                        salesItem.goodsName = gdt.Rows[0]["goodsName"].ToString();
                        salesItem.brand = gdt.Rows[0]["brand"].ToString();
                        salesItem.slt = gdt.Rows[0]["slt"].ToString();
                        salesItem.category = new string[3];
                        salesItem.category[0] = gdt.Rows[0]["c1"].ToString();
                        salesItem.category[1] = gdt.Rows[0]["c2"].ToString();
                        salesItem.category[2] = gdt.Rows[0]["c3"].ToString();
                    }
                    salesItem.salesNum = Convert.ToInt16(dt.Rows[i]["salesNum"].ToString());
                    salesItem.salesPrice = Convert.ToDouble(dt.Rows[i]["salesPrice"].ToString());
                    salesItem.cost = Convert.ToDouble(dt.Rows[i]["cost"].ToString());
                    salesItem.grossProfit = Convert.ToDouble(dt.Rows[i]["grossProfit"].ToString());
                    salesItem.brokerage = Convert.ToDouble(dt.Rows[i]["brokerage"].ToString());
                    salesItem.platformType = dt.Rows[i]["platformType"].ToString();
                    salesItem.purchaserName = dt.Rows[i]["purchaserName"].ToString();
                    pageResult.list.Add(salesItem);
                }
            }
            else
            {
                SalesListItem salesListItem = new SalesListItem();
                pageResult.item = salesListItem;
            }
            return pageResult;
        }
        public PageResult getSalesListBySupplier(SalesSeachParam salesSeachParam)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(salesSeachParam.current, salesSeachParam.pageSize);
            pageResult.list = new List<Object>();
            string st = "";
            if (salesSeachParam.barcode != null && salesSeachParam.barcode != "")
            {
                st += " and g.barcode like '%" + salesSeachParam.barcode + "%' ";
            }
            if (salesSeachParam.goodsName != null && salesSeachParam.goodsName != "")
            {
                st += " and g.goodsName like '%" + salesSeachParam.goodsName + "%' ";
            }
            if (salesSeachParam.brand != null && salesSeachParam.brand != "")
            {
                st += " and l.brand like '%" + salesSeachParam.brand + "%' ";
            }
            if (salesSeachParam.date != null && salesSeachParam.date.Length == 2)
            {
                st += " and o.tradeTime BETWEEN str_to_date('" + salesSeachParam.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + salesSeachParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }

            string totalsql = "select sum(IFNULL(g.quantity,0)) as salesNumTotal," +
                              "sum(IFNULL(g.supplyPrice,0)*IFNULL(g.quantity,0)) as salesPriceTotal " +
                              "from t_order_goods g,t_order_list o  " +
                              "where g.merchantOrderId = o.merchantOrderId " +
                              "and o.customerCode='" + salesSeachParam.userCode + "'" + st +
                              " group by barcode";
            DataTable totaldt = DatabaseOperationWeb.ExecuteSelectDS(totalsql, "TABLE").Tables[0];
            if (totaldt.Rows.Count > 0)
            {
                SalesListItem salesListItem = new SalesListItem();
                for (int j = 0; j < totaldt.Rows.Count; j++)
                {
                    salesListItem.salesNumTotal = Convert.ToInt16(totaldt.Rows[0]["salesNumTotal"].ToString());
                    salesListItem.salesPriceTotal = Convert.ToDouble(totaldt.Rows[0]["salesPriceTotal"].ToString());
                }
                pageResult.pagination.total = totaldt.Rows.Count;
                pageResult.item = salesListItem;

                List<SalesItem> ls = new List<SalesItem>();
                string sql = "select g.barcode,sum(IFNULL(g.quantity,0)) as salesNum," +
                             "sum(IFNULL(g.supplyPrice,0)*IFNULL(g.quantity,0)) as salesPrice " +
                             "from t_order_goods g,t_order_list o left join t_user_list u on o.customerCode = u.usercode " +
                             "where g.merchantOrderId = o.merchantOrderId and o.customerCode='" + salesSeachParam.userCode + "'" + st +
                             "group by g.barCode " +
                             "ORDER BY o.id asc LIMIT " + (salesSeachParam.current - 1) * salesSeachParam.pageSize + "," + salesSeachParam.pageSize;
                DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SalesItem salesItem = new SalesItem();
                    salesItem.id = (salesSeachParam.current - 1) * salesSeachParam.pageSize + 1 + i;
                    salesItem.barcode = dt.Rows[i]["barcode"].ToString();
                    string gsql = "select (select c1.name from t_goods_category c1 where (c1.id = g.catelog1)) AS c1 ," +
                                  "(select c1.name from t_goods_category c1 where (c1.id = g.catelog2)) AS c2," +
                                  "(select c1.name from t_goods_category c1 where (c1.id = g.catelog3)) AS c3," +
                                  " g.brand,g.slt,g.goodsName " +
                                  "from t_goods_list g " +
                                  "where g.barcode = '" + dt.Rows[i]["barcode"].ToString() + "'";
                    DataTable gdt = DatabaseOperationWeb.ExecuteSelectDS(gsql, "TABLE").Tables[0];
                    if (gdt.Rows.Count > 0)
                    {
                        salesItem.goodsName = gdt.Rows[0]["goodsName"].ToString();
                        salesItem.brand = gdt.Rows[0]["brand"].ToString();
                        salesItem.slt = gdt.Rows[0]["slt"].ToString();
                        salesItem.category = new string[3];
                        salesItem.category[0] = gdt.Rows[0]["c1"].ToString();
                        salesItem.category[1] = gdt.Rows[0]["c2"].ToString();
                        salesItem.category[2] = gdt.Rows[0]["c3"].ToString();
                    }
                    salesItem.salesNum = Convert.ToInt16(dt.Rows[i]["salesNum"].ToString());
                    salesItem.salesPrice = Convert.ToDouble(dt.Rows[i]["salesPrice"].ToString());
                    pageResult.list.Add(salesItem);
                }
                //string sql1 = "select count(*) from (select g.barcode " +
                //             "from t_order_goods g,t_order_list o left join t_user_list u on o.customerCode = u.usercode " +
                //             "where g.merchantOrderId = o.merchantOrderId and o.customerCode='" + salesSeachParam.userCode + "'" + st +
                //             "group by g.barCode) x ";
                //DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "TABLE").Tables[0];
                //pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0].ToString());
            }
            else
            {
                SalesListItem salesListItem = new SalesListItem();
                pageResult.item = salesListItem;
            }
            return pageResult;
        }
        public PageResult getSalesListByAgent(SalesSeachParam salesSeachParam)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(salesSeachParam.current, salesSeachParam.pageSize);
            pageResult.list = new List<Object>();
            string st = "";
            if (salesSeachParam.barcode != null && salesSeachParam.barcode != "")
            {
                st += " and g.barcode like '%" + salesSeachParam.barcode + "%' ";
            }
            if (salesSeachParam.goodsName != null && salesSeachParam.goodsName != "")
            {
                st += " and g.goodsName like '%" + salesSeachParam.goodsName + "%' ";
            }
            if (salesSeachParam.brand != null && salesSeachParam.brand != "")
            {
                st += " and g.brand like '%" + salesSeachParam.brand + "%' ";
            }
            if (salesSeachParam.distributionCode != null && salesSeachParam.distributionCode != "")
            {
                st += " and o.distributionCode = '" + salesSeachParam.distributionCode + "' ";
            }
            if (salesSeachParam.date != null && salesSeachParam.date.Length == 2)
            {
                st += " and o.tradeTime BETWEEN str_to_date('" + salesSeachParam.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + salesSeachParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }

            string totalsql = "select sum(IFNULL(g.quantity,0)) as salesNumTotal," +
                              "sum(IFNULL(g.purchasePrice,0)) as salesPriceTotal , sum(IFNULL(g.profitDealer,0)) as brokerageTotal " +
                              "from t_order_goods g,t_order_list o " +
                              "where g.merchantOrderId = o.merchantOrderId and purchaserCode='" + salesSeachParam.userCode + "' " + st +
                              " group by barcode";
            DataTable totaldt = DatabaseOperationWeb.ExecuteSelectDS(totalsql, "TABLE").Tables[0];
            if (totaldt.Rows.Count > 0)
            {
                SalesListItem salesListItem = new SalesListItem();
                for (int j = 0; j < totaldt.Rows.Count; j++)
                {
                    salesListItem.salesNumTotal += Convert.ToInt16(totaldt.Rows[j]["salesNumTotal"].ToString());
                    salesListItem.salesPriceTotal += Convert.ToDouble(totaldt.Rows[j]["salesPriceTotal"].ToString());
                    salesListItem.brokerageTotal += Convert.ToDouble(totaldt.Rows[j]["brokerageTotal"].ToString());
                }
                pageResult.pagination.total = totaldt.Rows.Count;
                pageResult.item = salesListItem;
                List<SalesItem> ls = new List<SalesItem>();
                string sql = "select g.barcode,u.username as distribution,sum(IFNULL(g.quantity,0)) as salesNum," +
                             "sum(IFNULL(g.supplyPrice,0)) as salesPrice, " +
                             "sum(IFNULL(g.profitDealer,0)) as brokerage " +
                             "from t_order_goods g,t_order_list o left join t_user_list u on o.distributionCode = u.usercode " +
                             "where g.merchantOrderId = o.merchantOrderId and purchaserCode='" + salesSeachParam.userCode + "' " + st +
                             "group by g.barCode, o.distributionCode " +
                             "ORDER BY o.id asc LIMIT " + (salesSeachParam.current - 1) * salesSeachParam.pageSize + "," + salesSeachParam.pageSize;
                DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SalesItem salesItem = new SalesItem();
                    salesItem.id = (salesSeachParam.current - 1) * salesSeachParam.pageSize + 1 + i;
                    salesItem.barcode = dt.Rows[i]["barcode"].ToString();
                    string gsql = "select (select c1.name from t_goods_category c1 where (c1.id = g.catelog1)) AS c1 ," +
                                  "(select c1.name from t_goods_category c1 where (c1.id = g.catelog2)) AS c2," +
                                  "(select c1.name from t_goods_category c1 where (c1.id = g.catelog3)) AS c3," +
                                  " g.brand,g.slt,g.goodsName " +
                                  "from t_goods_list g " +
                                  "where g.barcode = '" + dt.Rows[i]["barcode"].ToString() + "'";
                    DataTable gdt = DatabaseOperationWeb.ExecuteSelectDS(gsql, "TABLE").Tables[0];
                    if (gdt.Rows.Count > 0)
                    {
                        salesItem.goodsName = gdt.Rows[0]["goodsName"].ToString();
                        salesItem.brand = gdt.Rows[0]["brand"].ToString();
                        salesItem.slt = gdt.Rows[0]["slt"].ToString();
                        salesItem.category = new string[3];
                        salesItem.category[0] = gdt.Rows[0]["c1"].ToString();
                        salesItem.category[1] = gdt.Rows[0]["c2"].ToString();
                        salesItem.category[2] = gdt.Rows[0]["c3"].ToString();
                    }
                    salesItem.salesNum = Convert.ToInt16(dt.Rows[i]["salesNum"].ToString());
                    salesItem.salesPrice = Convert.ToDouble(dt.Rows[i]["salesPrice"].ToString());
                    salesItem.brokerage = Convert.ToDouble(dt.Rows[i]["brokerage"].ToString());
                    salesItem.distribution = dt.Rows[i]["distribution"].ToString();
                    pageResult.list.Add(salesItem);
                }
            }
            else
            {
                SalesListItem salesListItem = new SalesListItem();
                pageResult.item = salesListItem;
            }
            return pageResult;
        }
        public PageResult getClient(ClientSeachParam clientSeachParam)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(clientSeachParam.current, clientSeachParam.pageSize);
            pageResult.list = new List<Object>();
            string st = "";
            if (clientSeachParam.userName!=null&& clientSeachParam.userName!="")
            {
                st = " and username like '%"+ clientSeachParam.userName + "%' ";
            }
            string sql = "select * from t_user_list where ofAgent = '"+ clientSeachParam.userId + "' " +st+
                " order by id  LIMIT " + (clientSeachParam.current - 1) * clientSeachParam.pageSize + "," + clientSeachParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ClientItem clientItem = new ClientItem();
                clientItem.keyId = Convert.ToString((clientSeachParam.current - 1) * clientSeachParam.pageSize + i + 1);
                clientItem.id = dt.Rows[i]["id"].ToString();
                clientItem.createDate = dt.Rows[i]["createtime"].ToString();
                clientItem.userName = dt.Rows[i]["username"].ToString();
                clientItem.agentCost = dt.Rows[i]["agentCost"].ToString()+"%";
                pageResult.list.Add(clientItem);
            }
            string sql1 = "select count(*) from t_user_list where ofAgent = '" + clientSeachParam.userId + "' " + st ;

            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "TABLE").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return pageResult;
        }



        public PageResult getGoods(SalesGoods salesGoods, string purchaserCode)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(salesGoods.current, salesGoods.pageSize);
            pageResult.list = new List<Object>();

            string st = "";
            if (salesGoods.select != null && salesGoods.select != "")
            {
                st = " and ( A.MERCHANTORDERID like '%" + salesGoods.select + "%' or GOODSNAME like '%" + salesGoods.select + "%' )";
            }
            string time = "";
            if (salesGoods.date != null && salesGoods.date.Length == 2)
            {
                time = " and PAYTIME between  str_to_date('" + salesGoods.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + salesGoods.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }

            SalesTotal salesTotal = new SalesTotal();


            string sql = ""
                + "SELECT A.MERCHANTORDERID,A.PAYTYPE,A.PAYTIME,A.TRADEAMOUNT,A.PREFERENTIALNAME,A.PREFERENTIALPRICE,B.GOODSNAME,B.QUANTITY,B.SKUUNITPRICE,B.PURCHASEPRICE "
                + " FROM T_ORDER_LIST A,T_ORDER_GOODS B"
                + " WHERE A.MERCHANTORDERID=B.MERCHANTORDERID AND A.APITYPE='2' AND PURCHASERCODE= '" + purchaserCode + "' " + st + time + " GROUP BY  A.MERCHANTORDERID"
                + " ORDER BY A.PAYTIME desc LIMIT " + (salesGoods.current - 1) * salesGoods.pageSize + "," + salesGoods.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];

            string sql2 = ""
                + "SELECT A.MERCHANTORDERID,A.PAYTYPE,A.PAYTIME,A.TRADEAMOUNT,A.PREFERENTIALNAME,A.PREFERENTIALPRICE,B.GOODSNAME,B.QUANTITY,B.SKUUNITPRICE,B.PURCHASEPRICE "
                + " FROM T_ORDER_LIST A,T_ORDER_GOODS B"
                + " WHERE A.MERCHANTORDERID=B.MERCHANTORDERID AND A.APITYPE='2'   AND  PURCHASERCODE= '" + purchaserCode + "' " + st + time
                + " ORDER BY A.PAYTIME desc ";
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "TABLE").Tables[0];

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SalesOrderItem salesOrderItem = new SalesOrderItem();
                    salesOrderItem.orderId = dt.Rows[i]["MERCHANTORDERID"].ToString();//订单号
                    salesOrderItem.keyId = Convert.ToString((salesGoods.current - 1) * salesGoods.pageSize + i + 1);
                    salesOrderItem.payTime = dt.Rows[i]["PAYTIME"].ToString();//结账时间
                    salesOrderItem.receivable = Convert.ToDouble(String.Format("{0:F}", dt.Rows[i]["TRADEAMOUNT"].ToString()));//应收金额
                    salesOrderItem.payType = dt.Rows[i]["PAYTYPE"].ToString();//支付方式
                    salesOrderItem.paymoney = Convert.ToDouble(String.Format("{0:F}", dt.Rows[i]["TRADEAMOUNT"].ToString()));//支付金额
                    salesOrderItem.discountName = dt.Rows[i]["PREFERENTIALNAME"].ToString();//优惠名称

                    if (dt.Rows[i]["PREFERENTIALPRICE"].ToString() == "")
                        salesOrderItem.discountMoney = 0;//优惠金额
                    else
                        salesOrderItem.discountMoney = Convert.ToDouble(String.Format("{0:F}", dt.Rows[i]["PREFERENTIALPRICE"].ToString()));

                    salesOrderItem.orderMoney = salesOrderItem.discountMoney + salesOrderItem.receivable;//订单金额


                    int keyId = 1;
                    for (int j = 0; j < dt2.Rows.Count; j++)
                    {

                        
                        if (dt2.Rows[j]["MERCHANTORDERID"].ToString() == salesOrderItem.orderId)
                        {
                            
                            SalesGoodsItem salesGoodsItem = new SalesGoodsItem();
                            salesGoodsItem.keyId = Convert.ToString(keyId);
                            salesGoodsItem.goodsName = dt2.Rows[j]["GOODSNAME"].ToString();
                            salesGoodsItem.quantity = Convert.ToInt16(dt2.Rows[j]["QUANTITY"].ToString());
                            salesGoodsItem.goodsPrice = Convert.ToDouble(String.Format("{0:F}", dt2.Rows[j]["SKUUNITPRICE"].ToString()));
                            salesOrderItem.list.Add(salesGoodsItem);
                            salesOrderItem.num += Convert.ToInt16(dt2.Rows[j]["QUANTITY"].ToString());//商品数量
                            keyId += 1;
                        }



                    }
                    pageResult.list.Add(salesOrderItem);
                }
            }
            string sql3 = ""
               + "SELECT sum(A.TRADEAMOUNT) TRADEAMOUNT,sum(A.PREFERENTIALPRICE) PREFERENTIALPRICE,sum(B.QUANTITY) QUANTITY,sum(B.SKUUNITPRICE) SKUUNITPRICE,sum(B.PURCHASEPRICE) PURCHASEPRICE "
               + " FROM T_ORDER_LIST A,T_ORDER_GOODS B"
               + " WHERE A.MERCHANTORDERID=B.MERCHANTORDERID AND A.APITYPE='2'  AND PURCHASERCODE= '" + purchaserCode + "' " + st + time;
            DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "TABLE").Tables[0];
            if (String.IsNullOrWhiteSpace(dt3.Rows[0]["PURCHASEPRICE"].ToString()))
                salesTotal.totalSupplyMoney = 0;
            else
                salesTotal.totalSupplyMoney = Convert.ToDouble(String.Format("{0:F}", dt3.Rows[0]["PURCHASEPRICE"].ToString()));
            if (String.IsNullOrWhiteSpace(dt3.Rows[0]["QUANTITY"].ToString()))
                salesTotal.totalnum = 0;
            else
                salesTotal.totalnum = Convert.ToInt16(dt3.Rows[0]["QUANTITY"].ToString());//商品数量    
            if (String.IsNullOrWhiteSpace(dt3.Rows[0]["PREFERENTIALPRICE"].ToString()))
                salesTotal.totalDiscountMoney = 0;//优惠金额
            else
                salesTotal.totalDiscountMoney = Convert.ToDouble(String.Format("{0:F}", dt3.Rows[0]["PREFERENTIALPRICE"].ToString()));
            if (String.IsNullOrWhiteSpace(dt3.Rows[0]["TRADEAMOUNT"].ToString()))
                salesTotal.totalReceivable = 0;
            else
                salesTotal.totalReceivable = Convert.ToDouble(String.Format("{0:F}", dt3.Rows[0]["TRADEAMOUNT"].ToString()));
            salesTotal.totalOrderMoney = salesTotal.totalReceivable + salesTotal.totalDiscountMoney;

            pageResult.item = salesTotal;

            string sql4 = ""
                + "SELECT count(*)"
                + " FROM T_ORDER_LIST A,T_ORDER_GOODS B"
                + " WHERE A.MERCHANTORDERID=B.MERCHANTORDERID AND A.APITYPE='2'  AND PURCHASERCODE= '" + purchaserCode + "' " + st + time + " GROUP BY  A.MERCHANTORDERID";

            DataTable dt4 = DatabaseOperationWeb.ExecuteSelectDS(sql4, "TABLE").Tables[0];

            pageResult.pagination.total = Convert.ToInt16(dt4.Rows.Count);
            return pageResult;
        }



    }
}
