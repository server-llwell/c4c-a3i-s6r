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

                    salesListItem.salesPriceTotal = Math.Round(salesListItem.salesPriceTotal,2);
                    salesListItem.costTotal= Math.Round(salesListItem.costTotal, 2);
                    salesListItem.grossProfitTotal= Math.Round(salesListItem.grossProfitTotal, 2);
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
                    salesItem.salesPrice= Math.Round(salesItem.salesPrice, 2);

                    salesItem.cost = Convert.ToDouble(dt.Rows[i]["cost"].ToString());
                    salesItem.cost = Math.Round(salesItem.cost, 2);

                    salesItem.grossProfit = Convert.ToDouble(dt.Rows[i]["grossProfit"].ToString());
                    salesItem.grossProfit = Math.Round(salesItem.grossProfit, 2);

                    salesItem.brokerage = Convert.ToDouble(dt.Rows[i]["brokerage"].ToString());
                    salesItem.brokerage = Math.Round(salesItem.brokerage, 2);

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
            string st = "";//一件代发或铺货销售条件
            string st1 = "";//一件代发或铺货退货条件
            string stGh = "";//批采销售条件
            string stAll = "";//商品名and品牌条件
            if (salesSeachParam.barcode != null && salesSeachParam.barcode != "")
            {
                st += " and g.barcode like '%" + salesSeachParam.barcode + "%' ";
                st1 += " and g.barcode like '%" + salesSeachParam.barcode + "%' ";
                stGh += " and a.barcode like '%" + salesSeachParam.barcode + "%' ";
            }
            if (salesSeachParam.goodsName != null && salesSeachParam.goodsName != "")
            {
                stAll += " and gs.goodsName like '%" + salesSeachParam.goodsName + "%' ";               
                stGh += " and gs.goodsName like '%" + salesSeachParam.goodsName + "%'";
            }
            if (salesSeachParam.brand != null && salesSeachParam.brand != "")
            {
                stAll += " and gs.brand like '%" + salesSeachParam.brand + "%' ";              
                stGh += " and gs.brand like '%" + salesSeachParam.brand + "%'";
            }
            if (salesSeachParam.date != null && salesSeachParam.date.Length == 2)
            {
                st += " and o.tradeTime BETWEEN str_to_date('" + salesSeachParam.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + salesSeachParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
                st1 += " and o.returnTime BETWEEN str_to_date('" + salesSeachParam.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + salesSeachParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
                stGh += " and pl.purchasetime BETWEEN str_to_date('" + salesSeachParam.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + salesSeachParam.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            
            //铺货或一件代发的总金额与总销量
            string totalsqlPY = "select sum(salesNumTotal) salesNumTotal,sum(salesPriceTotal) salesPriceTotal from("
                + " select sum(IFNULL(g.quantity,0)) as salesNumTotal,-sum(IFNULL(g.supplyPrice,0)*IFNULL(g.quantity,0)) as salesPriceTotal "
                + " from t_order_goods g,t_order_list o,t_goods_list gs  "
                + " where g.merchantOrderId = o.merchantOrderId and gs.barcode=g.barcode and o.customerCode='" + salesSeachParam.userCode + "' and o.status='-1'  " + st1 + stAll 
                + " union ALL "
                + " select sum(IFNULL(g.quantity,0)) as salesNumTotal,sum(IFNULL(g.supplyPrice,0)*IFNULL(g.quantity,0)) as salesPriceTotal "
                + " from t_order_goods g,t_order_list o,t_goods_list gs "
                + " where g.merchantOrderId = o.merchantOrderId and gs.barcode=g.barcode and o.customerCode='" + salesSeachParam.userCode + "' and o.status in('3','4','5') " + st + stAll + ") a";
           
            //批量供货的总金额与总销量
            string totalsqlGH = ""
                + " select sum(IFNULL(a.demand,0)) salesNumTotal, sum(IFNULL(a.totalPrice,0)) salesPriceTotal "
                + " from t_purchase_inquiry a,t_goods_list gs,t_purchase_list pl"
                + " where a.flag='3' and pl.purchasesn=a.purchasesn and a.barcode=gs.barcode and a.usercode='" + salesSeachParam.userCode + "'"+ stGh;
            SalesListItem salesListItem = new SalesListItem();

            //铺货或一件代发的分页
            string sqlPY = "select g.barcode,g.supplyPrice,b.salesNumTotal salesNum,b.salesPriceTotal salesPrice,b.time,o.platformId,w.wname,o.status," +
                            "(select c1.name from t_goods_category c1 where (c1.id = gs.catelog1)) AS c1 , " +
                            "(select c1.name from t_goods_category c1 where (c1.id = gs.catelog2)) AS c2, " +
                            " gs.brand,gs.slt,gs.goodsName " +
                            " from t_order_goods g,t_goods_warehouse gw,t_base_warehouse w ,t_goods_list gs ,(select merchantOrderId,time,barcode,salesNumTotal,salesPriceTotal from( " +
                            " select o.merchantOrderId,o.returnTime time,g.barcode,-sum(IFNULL(g.quantity,0)) as salesNumTotal,-sum(IFNULL(g.supplyPrice,0)*IFNULL(g.quantity,0)) as salesPriceTotal " +//负的为退货
                            " from t_order_goods g,t_order_list o " +
                            " where g.merchantOrderId = o.merchantOrderId and o.customerCode='" + salesSeachParam.userCode + "' " + st1 + " and o.status='-1'  GROUP BY g.barcode" +
                            " union ALL" +
                            " select o.merchantOrderId,o.tradeTime time,g.barcode,sum(IFNULL(g.quantity,0)) as salesNumTotal,sum(IFNULL(g.supplyPrice,0)*IFNULL(g.quantity,0)) as salesPriceTotal " +
                            " from t_order_goods g,t_order_list o " +
                            " where g.merchantOrderId = o.merchantOrderId and o.customerCode='" + salesSeachParam.userCode + "'" + st +" and o.status in('3','4','5')  GROUP BY g.barcode) a ) b,t_order_list o left join t_user_list u on o.customerCode = u.usercode" +
                            " where g.merchantOrderId = o.merchantOrderId and  b.barcode=gw.barcode and g.suppliercode=gw.suppliercode and gw.wid=w.id and gs.barcode=b.barcode and b.merchantOrderId=g.merchantOrderId and b.barcode=g.barcode and o.customerCode='" + salesSeachParam.userCode + "'"+ stAll +
                            " group by g.barCode";

            //铺货或一件代发分页条件
            string orderBySql =" ORDER BY o.status asc ,b.time desc LIMIT " + (salesSeachParam.current - 1) * salesSeachParam.pageSize + "," + salesSeachParam.pageSize;

            //批量供货sql
            string sqlGH = ""
                + " select a.barcode,a.price supplyPrice,a.demand salesNum,a.totalPrice salesPrice,pl.purchasetime time,'' as platformId, w.wname,'6' as `status`,"
                + " (select c1.name from t_goods_category c1 where (c1.id = gs.catelog1)) AS c1 , "
                + " (select c1.name from t_goods_category c1 where (c1.id = gs.catelog2)) AS c2,"
                + " gs.brand,gs.slt,gs.goodsName "
                + " from t_purchase_inquiry a,t_purchase_list pl,t_goods_list gs,t_goods_warehouse gw,t_base_warehouse w "
                + " where pl.purchasesn=a.purchasesn and gw.barcode=a.barcode and a.usercode=gw.suppliercode and gw.wid=w.id and gs.barcode=a.barcode and a.flag='3' and a.usercode='" + salesSeachParam.userCode +   "'"+ stGh +" GROUP BY a.barcode";

            //批量分页
            string orderBySqlGH = " ORDER BY time desc LIMIT " + (salesSeachParam.current - 1) * salesSeachParam.pageSize + "," + salesSeachParam.pageSize;

            string sql = "";
            string countSql = "";

            DataTable dt=new DataTable();
            if (salesSeachParam.platformId == "铺货" || salesSeachParam.platformId == "一件代发")
            {
                string platformId = (salesSeachParam.platformId == "铺货") ? "3" : "4";
                st += " and o.platformId='" + platformId + "'";
                st1 += " and o.platformId='" + platformId + "'";
                //铺货或一件代发的总金额与总销量
                totalsqlPY = "select sum(salesNumTotal) salesNumTotal,sum(salesPriceTotal) salesPriceTotal from("
                    + " select sum(IFNULL(g.quantity,0)) as salesNumTotal,-sum(IFNULL(g.supplyPrice,0)*IFNULL(g.quantity,0)) as salesPriceTotal "
                    + " from t_order_goods g,t_order_list o,t_goods_list gs  "
                    + " where g.merchantOrderId = o.merchantOrderId and gs.barcode=g.barcode and o.customerCode='" + salesSeachParam.userCode + "' and o.status='-1'  " + st1 + stAll
                    + " union ALL "
                    + " select sum(IFNULL(g.quantity,0)) as salesNumTotal,sum(IFNULL(g.supplyPrice,0)*IFNULL(g.quantity,0)) as salesPriceTotal "
                    + " from t_order_goods g,t_order_list o,t_goods_list gs "
                    + " where g.merchantOrderId = o.merchantOrderId and gs.barcode=g.barcode and o.customerCode='" + salesSeachParam.userCode + "' and o.status in('3','4','5') " + st + stAll + ") a";
                DataTable totaldtPY = DatabaseOperationWeb.ExecuteSelectDS(totalsqlPY,"T").Tables[0];
                if (totaldtPY.Rows[0][0].ToString() !=null && totaldtPY.Rows[0][0].ToString() !="")
                {
                    salesListItem.salesNumTotal = Convert.ToInt16(totaldtPY.Rows[0]["salesNumTotal"].ToString());
                    salesListItem.salesPriceTotal = Convert.ToDouble(totaldtPY.Rows[0]["salesPriceTotal"].ToString());
                    //铺货或一件代发的分页
                    sqlPY = "select g.barcode,g.supplyPrice,b.salesNumTotal salesNum,b.salesPriceTotal salesPrice,b.time,o.platformId,w.wname,o.status," +
                                    "(select c1.name from t_goods_category c1 where (c1.id = gs.catelog1)) AS c1 , " +
                                    "(select c1.name from t_goods_category c1 where (c1.id = gs.catelog2)) AS c2, " +
                                    " gs.brand,gs.slt,gs.goodsName " +
                                    " from t_order_goods g,t_goods_warehouse gw,t_base_warehouse w ,t_goods_list gs ,(select merchantOrderId,time,barcode,salesNumTotal,salesPriceTotal from( " +
                                    " select o.merchantOrderId,o.returnTime time,g.barcode,-sum(IFNULL(g.quantity,0)) as salesNumTotal,-sum(IFNULL(g.supplyPrice,0)*IFNULL(g.quantity,0)) as salesPriceTotal " +//负的为退货
                                    " from t_order_goods g,t_order_list o " +
                                    " where g.merchantOrderId = o.merchantOrderId and o.customerCode='" + salesSeachParam.userCode + "' " + st1 + " and o.status='-1'  GROUP BY g.barcode" +
                                    " union ALL" +
                                    " select o.merchantOrderId,o.tradeTime time,g.barcode,sum(IFNULL(g.quantity,0)) as salesNumTotal,sum(IFNULL(g.supplyPrice,0)*IFNULL(g.quantity,0)) as salesPriceTotal " +
                                    " from t_order_goods g,t_order_list o " +
                                    " where g.merchantOrderId = o.merchantOrderId and o.customerCode='" + salesSeachParam.userCode + "'" + st + " and o.status in('3','4','5')  GROUP BY g.barcode) a ) b,t_order_list o left join t_user_list u on o.customerCode = u.usercode" +
                                    " where g.merchantOrderId = o.merchantOrderId and  b.barcode=gw.barcode and g.suppliercode=gw.suppliercode and gw.wid=w.id and gs.barcode=b.barcode and b.merchantOrderId=g.merchantOrderId and b.barcode=g.barcode and o.customerCode='" + salesSeachParam.userCode + "'" + stAll +
                                    " group by g.barCode";
                    sql = sqlPY+ orderBySql;
                    countSql = sqlPY;
                }
                else
                {
                    salesListItem.salesNumTotal = 0;
                    salesListItem.salesPriceTotal = 0;
                    pageResult.item = salesListItem;
                    return pageResult;
                }

            }
            else if (salesSeachParam.platformId == "批量供货")
            {
                DataTable totaldtGH = DatabaseOperationWeb.ExecuteSelectDS(totalsqlGH, "T").Tables[0];
                if (totaldtGH.Rows[0][0].ToString() != null && totaldtGH.Rows[0][0].ToString() != "")
                {
                    salesListItem.salesNumTotal = Convert.ToInt16(totaldtGH.Rows[0]["salesNumTotal"].ToString());
                    salesListItem.salesPriceTotal = Convert.ToDouble(totaldtGH.Rows[0]["salesPriceTotal"].ToString());

                    sql= sqlGH + orderBySqlGH;
                    countSql = sqlGH;
                }
                else
                {
                    salesListItem.salesNumTotal = 0;
                    salesListItem.salesPriceTotal = 0;
                    pageResult.item = salesListItem;
                    return pageResult;
                }
            }
            else//全部
            {
                DataTable totaldtPY = DatabaseOperationWeb.ExecuteSelectDS(totalsqlPY, "T").Tables[0];
                DataTable totaldtGH = DatabaseOperationWeb.ExecuteSelectDS(totalsqlGH, "T").Tables[0];
                if ((totaldtGH.Rows[0][0].ToString() != null && totaldtGH.Rows[0][0].ToString() != ""))
                {
                    salesListItem.salesNumTotal = Convert.ToInt16(totaldtGH.Rows[0]["salesNumTotal"]);
                    salesListItem.salesPriceTotal = Convert.ToDouble(totaldtGH.Rows[0]["salesPriceTotal"]);
                 
                }
                else
                {
                    salesListItem.salesNumTotal = 0;
                    salesListItem.salesPriceTotal = 0;
                }
                if ((totaldtPY.Rows[0][0].ToString() != null && totaldtPY.Rows[0][0].ToString() != ""))
                {
                    salesListItem.salesNumTotal += Convert.ToInt16(totaldtPY.Rows[0]["salesNumTotal"]);
                    salesListItem.salesPriceTotal = Math.Round((Convert.ToDouble(totaldtPY.Rows[0]["salesPriceTotal"]) + salesListItem.salesPriceTotal), 2);
                }
                else
                {
                    salesListItem.salesNumTotal += 0;
                    salesListItem.salesPriceTotal = Math.Round((0 + salesListItem.salesPriceTotal),2);                   
                }
                if (salesListItem.salesNumTotal != 0)
                {
                    sql = ""
                       + " select * from ( "
                       + sqlPY
                       + " union all "
                       + sqlGH
                       + " ) aa   ORDER BY status asc ,time desc LIMIT " + (salesSeachParam.current - 1) * salesSeachParam.pageSize + "," + salesSeachParam.pageSize;
                    countSql = ""
                        + " select * from ( "
                        + sqlPY
                        + " union all "
                        + sqlGH
                        + " ) aa ";
                }
                else
                {
                    pageResult.item = salesListItem;
                    return pageResult;
                }
            }
            dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SalesItem salesItem = new SalesItem();
                    salesItem.id = (salesSeachParam.current - 1) * salesSeachParam.pageSize + 1 + i;
                    salesItem.barcode = dt.Rows[i]["barcode"].ToString();
                    salesItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                    salesItem.brand = dt.Rows[i]["brand"].ToString();
                    salesItem.slt = dt.Rows[i]["slt"].ToString();
                    salesItem.category = new string[2];
                    salesItem.category[0] = dt.Rows[i]["c1"].ToString();
                    salesItem.category[1] = dt.Rows[i]["c2"].ToString();
                    salesItem.supplyPrice = dt.Rows[i]["supplyPrice"].ToString();
                    salesItem.salseTime = dt.Rows[i]["time"].ToString();
                    salesItem.salesNum = Convert.ToInt16(dt.Rows[i]["salesNum"].ToString());
                    salesItem.salesPrice = Convert.ToDouble(dt.Rows[i]["salesPrice"].ToString());
                    salesItem.wname = dt.Rows[i]["wname"].ToString();
                    switch (dt.Rows[i]["platformId"].ToString())
                    {
                        case "1":
                            salesItem.platformType = "";
                            break;
                        case "2":
                            salesItem.platformType = "一件代发";
                            break;
                        case "3":
                            salesItem.platformType = "铺货";
                            break;
                        case "4":
                            salesItem.platformType = "一件代发";
                            break;
                        case "":
                            salesItem.platformType = "批量铺货";
                            break;
                    }

                    pageResult.list.Add(salesItem);
                }
            }
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(countSql, "T").Tables[0];
            if (dt1.Rows.Count>0)
            {
                pageResult.pagination.total = dt1.Rows.Count;
            }
            
            pageResult.item = salesListItem;

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
                st = " and ( A.parentOrderId like '%" + salesGoods.select + "%' or GOODSNAME like '%" + salesGoods.select + "%' )";
            }
            string time = "";
            if (salesGoods.date != null && salesGoods.date.Length == 2)
            {
                time = " and PAYTIME between  str_to_date('" + salesGoods.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + salesGoods.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }

            SalesTotal salesTotal = new SalesTotal();
            salesTotal.totalSupplyMoney = 0;

            string sql = ""
                + "SELECT A.parentOrderId,A.PAYTYPE,A.tradeTime,sum(A.TRADEAMOUNT) TRADEAMOUNT,A.PREFERENTIALNAME,sum(A.PREFERENTIALPRICE) PREFERENTIALPRICE "
                + "  FROM T_ORDER_LIST A"
                + " WHERE  A.APITYPE='2' AND PURCHASERCODE= '" + purchaserCode + "' " + st + time + " GROUP BY  A.parentOrderId"
                + " ORDER BY A.tradeTime desc LIMIT " + (salesGoods.current - 1) * salesGoods.pageSize + "," + salesGoods.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];

            string sql2 = ""
                + "SELECT A.parentOrderId,B.GOODSNAME,B.QUANTITY,B.SKUUNITPRICE,B.PURCHASEPRICE "
                + " FROM T_ORDER_LIST A,T_ORDER_GOODS B"
                + " WHERE A.MERCHANTORDERID=B.MERCHANTORDERID AND A.APITYPE='2'   AND  PURCHASERCODE= '" + purchaserCode + "' " + st + time
                + " ORDER BY A.PAYTIME desc ";
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "TABLE").Tables[0];

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SalesOrderItem salesOrderItem = new SalesOrderItem();
                    salesOrderItem.orderId = dt.Rows[i]["parentOrderId"].ToString();//订单号
                    salesOrderItem.keyId = Convert.ToString((salesGoods.current - 1) * salesGoods.pageSize + i + 1);
                    salesOrderItem.payTime = dt.Rows[i]["tradeTime"].ToString();//结账时间
                    salesOrderItem.receivable = Math.Round(Convert.ToDouble( dt.Rows[i]["TRADEAMOUNT"].ToString()),2);//应收金额
                    
                    salesOrderItem.paymoney = salesOrderItem.receivable;//支付金额
                    switch (dt.Rows[i]["PAYTYPE"].ToString()) //支付方式
                    {
                        case "1":
                            salesOrderItem.payType = "会员储值卡";
                            break;
                        case "2":
                            salesOrderItem.payType = "在线支付";
                            break;
                        case "3":
                            salesOrderItem.payType = "货到付款";
                            break;
                        case "4":
                            salesOrderItem.payType = "现金支付";
                            break;
                        case "11":
                            salesOrderItem.payType = "后台付款";
                            break;
                        case "21":
                            salesOrderItem.payType = "微信支付";
                            break;
                        case "22":
                            salesOrderItem.payType = "支付宝支付";
                            break;
                        case "23":
                            salesOrderItem.payType = "银联支付";
                            break;
                        case "99":
                            salesOrderItem.payType = "其他支付";
                            break;
                    }                  
                    salesOrderItem.discountName = dt.Rows[i]["PREFERENTIALNAME"].ToString();//优惠名称

                    if (dt.Rows[i]["PREFERENTIALPRICE"].ToString() == "")
                        salesOrderItem.discountMoney = 0;//优惠金额
                    else
                        salesOrderItem.discountMoney = Math.Round(Convert.ToDouble(dt.Rows[i]["PREFERENTIALPRICE"].ToString()),2);

                    salesOrderItem.orderMoney = Math.Round(salesOrderItem.discountMoney + salesOrderItem.receivable,2) ;//订单金额


                    int keyId = 1;
                    if (dt2.Select("parentOrderId='" + salesOrderItem.orderId+"'")!=null)
                    {
                        DataRow[] dt1 = dt2.Select("parentOrderId='" + salesOrderItem.orderId+"'");
                        foreach (DataRow dr in dt1)
                        {

                           SalesGoodsItem salesGoodsItem = new SalesGoodsItem();
                            salesGoodsItem.keyId = Convert.ToString(keyId);
                            salesGoodsItem.goodsName = dr["GOODSNAME"].ToString();
                            salesGoodsItem.quantity = Convert.ToInt16(dr["QUANTITY"].ToString());//商品数量
                            salesGoodsItem.goodsPrice = Math.Round(Convert.ToDouble(dr["SKUUNITPRICE"].ToString())* salesGoodsItem.quantity, 2);
                            salesOrderItem.list.Add(salesGoodsItem);
                            salesOrderItem.num += Convert.ToInt16(dr["QUANTITY"].ToString());//商品总数量
                            keyId += 1;
                            

                        }
                    }
                    
                    pageResult.list.Add(salesOrderItem);
                }
            }
            string sql3 = ""
               + "SELECT sum(A.TRADEAMOUNT) TRADEAMOUNT,sum(A.PREFERENTIALPRICE) PREFERENTIALPRICE "
               + " FROM T_ORDER_LIST A"
               + " WHERE  A.APITYPE='2'  AND PURCHASERCODE= '" + purchaserCode + "' " + st + time;
            DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "TABLE").Tables[0];
           
            
            if (String.IsNullOrWhiteSpace(dt3.Rows[0]["PREFERENTIALPRICE"].ToString()))
                salesTotal.totalDiscountMoney = 0;//优惠金额
            else
                salesTotal.totalDiscountMoney = Math.Round(Convert.ToDouble( dt3.Rows[0]["PREFERENTIALPRICE"].ToString()),2);
            if (String.IsNullOrWhiteSpace(dt3.Rows[0]["TRADEAMOUNT"].ToString()))
                salesTotal.totalReceivable = 0;
            else
                salesTotal.totalReceivable = Math.Round(Convert.ToDouble( dt3.Rows[0]["TRADEAMOUNT"].ToString()),2);
            salesTotal.totalOrderMoney = Math.Round(salesTotal.totalReceivable + salesTotal.totalDiscountMoney,2);

            foreach (DataRow dr in dt2.Rows)
            {
                salesTotal.totalnum += Convert.ToInt16(dr["QUANTITY"].ToString());
                salesTotal.totalSupplyMoney += (Convert.ToDouble(dr["PURCHASEPRICE"].ToString()) * Convert.ToInt16(dr["QUANTITY"].ToString()));
            }
            salesTotal.totalSupplyMoney = Math.Round(salesTotal.totalSupplyMoney, 2);
            
            
            pageResult.item = salesTotal;

            string sql4 = ""
               + "SELECT count(*)"
               + " FROM T_ORDER_LIST A"
               + " WHERE  A.APITYPE='2'  AND PURCHASERCODE= '" + purchaserCode + "' " + st + time + " GROUP BY  A.parentOrderId";

            DataTable dt4 = DatabaseOperationWeb.ExecuteSelectDS(sql4, "TABLE").Tables[0];

            pageResult.pagination.total = Convert.ToInt16(dt4.Rows.Count);
            return pageResult;
        }


        /// <summary>
        /// 获取门店销售订单-运营
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public PageResult ShopSalseOrders(ShopSalseOrdersParam salesGoods, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(salesGoods.current, salesGoods.pageSize);
            pageResult.list = new List<Object>();
            string date = "";
            if (salesGoods.date!=null && salesGoods.date.Length==2)
            {
                date = " and tradeTime between  str_to_date('" + salesGoods.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + salesGoods.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            string purchaserName = "";
            if (salesGoods.purchaseName!=null && salesGoods.purchaseName !="")
            {
                purchaserName = " b.username like '%" + salesGoods.purchaseName + "%' and ";
            }

            string sql = ""
                + " select a.parentOrderId,a.tradeTime,b.username,s.rprice,s.pprice,s.inprice,s.platformPrice "
                + " from t_order_list a,t_user_list b,(select sum(c.skuUnitPrice*c.quantity) rprice,sum(c.supplyPrice*c.quantity) pprice,sum(c.purchasePrice*c.quantity) inprice,c.merchantOrderId,sum(c.platformPrice) platformPrice  from t_order_goods c    GROUP BY c.merchantOrderId) s"
                + " where "+ purchaserName + " a.purchaserCode=b.usercode and s.merchantOrderId=a.merchantOrderId"+ date
                + " ORDER BY a.tradeTime DESC limit " + (salesGoods.current - 1) * salesGoods.pageSize + "," + salesGoods.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];
            ShopSalseOrdersItem shopSalseOrdersItem = new ShopSalseOrdersItem();
            if (dt.Rows.Count>0)
            {
                double rprice = 0;
                double pprice = 0;
                double inprice = 0;
                double platformPrice = 0;
                for (int i=0;i< dt.Rows.Count;i++)
                {
                    ShopSalseOrdersList shopSalseOrdersList = new ShopSalseOrdersList();
                    shopSalseOrdersList.keyId =Convert.ToString((salesGoods.current - 1) * salesGoods.pageSize + i + 1) ;
                    shopSalseOrdersList.orderId = dt.Rows[i]["parentOrderId"].ToString();
                    if (dt.Rows[i]["tradeTime"] != DBNull.Value)
                        shopSalseOrdersList.tradeTime = Convert.ToDateTime(dt.Rows[i]["tradeTime"]).ToString("yyyy-MM-dd");
                    shopSalseOrdersList.inprice = dt.Rows[i]["inprice"].ToString();
                    shopSalseOrdersList.platformPrice = dt.Rows[i]["platformPrice"].ToString();
                    shopSalseOrdersList.pprice = dt.Rows[i]["pprice"].ToString();
                    shopSalseOrdersList.purchaseName = dt.Rows[i]["username"].ToString();
                    shopSalseOrdersList.rprice = dt.Rows[i]["rprice"].ToString();
                    pageResult.list.Add(shopSalseOrdersList);
                    rprice += Convert.ToDouble(shopSalseOrdersList.rprice);
                    pprice += Convert.ToDouble(shopSalseOrdersList.pprice);
                    inprice += Convert.ToDouble(shopSalseOrdersList.inprice);
                    platformPrice += Convert.ToDouble(shopSalseOrdersList.platformPrice);
                }
                shopSalseOrdersItem.inprice = string.Format("{0:F}", inprice);
                shopSalseOrdersItem.rprice = string.Format("{0:F}", rprice);
                shopSalseOrdersItem.pprice = string.Format("{0:F}", pprice);
                shopSalseOrdersItem.platformPrice = string.Format("{0:F}", platformPrice);

            }
            pageResult.item = shopSalseOrdersItem;

            string sql1 = ""
               + " select count(*) "
               + " from t_order_list a,t_user_list b,(select sum(c.skuUnitPrice*c.quantity) rprice,sum(c.supplyPrice*c.quantity) pprice,sum(c.purchasePrice*c.quantity) inprice,c.merchantOrderId,sum(c.platformPrice) platformPrice  from t_order_goods c    GROUP BY c.merchantOrderId) s"
               + " where " + purchaserName + " a.purchaserCode=b.usercode and s.merchantOrderId=a.merchantOrderId" + date;
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);

            return pageResult;
        }


        /// <summary>
        /// 获取门店销售订单详情-运营
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public PageResult ShopSalseOrdersDetails(ShopSalseOrdersDetailsParam salesGoods, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(salesGoods.current, salesGoods.pageSize);
            pageResult.list = new List<object>();

            string sql = ""
                + "SELECT a.tradeTime, b.username, c.goodsName, c.barCode, d.model, d.country, d.brand, c.quantity, c.skuUnitPrice, c.supplyPrice, c.purchasePrice, c.platformPrice"
                + " from t_order_list a, t_user_list b, t_order_goods c, t_goods_list d"
                + " where a.parentOrderId= '"+ salesGoods.orderId + "' and a.merchantOrderId= c.merchantOrderId and c.suppliercode= b.usercode and d.barcode= c.barCode"
                + " limit " + (salesGoods.current - 1) * salesGoods.pageSize + "," + salesGoods.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];

            if (dt.Rows.Count>0)
            {
                for (int i=0;i< dt.Rows.Count;i++)
                {
                    ShopSalseOrdersDetailsItem ssodi = new ShopSalseOrdersDetailsItem();
                    ssodi.barcode = dt.Rows[i]["barCode"].ToString();
                    ssodi.brand= dt.Rows[i]["brand"].ToString();
                    ssodi.country = dt.Rows[i]["country"].ToString();
                    ssodi.goodsName = dt.Rows[i]["goodsName"].ToString();
                    ssodi.inprice = dt.Rows[i]["purchasePrice"].ToString();
                    ssodi.keyId = Convert.ToString((salesGoods.current - 1) * salesGoods.pageSize + i + 1);
                    ssodi.model = dt.Rows[i]["model"].ToString();
                    ssodi.num = dt.Rows[i]["quantity"].ToString();
                    ssodi.platformPrice = dt.Rows[i]["platformPrice"].ToString();
                    ssodi.pprice = dt.Rows[i]["supplyPrice"].ToString();
                    ssodi.rprice = dt.Rows[i]["skuUnitPrice"].ToString();
                    ssodi.supplierName = dt.Rows[i]["username"].ToString();
                    ssodi.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    pageResult.list.Add(ssodi);

                }              
            }
            string sql1 = ""
                + "SELECT count(*)"
                + " from t_order_list a, t_user_list b, t_order_goods c, t_goods_list d"
                + " where a.parentOrderId= '" + salesGoods.orderId + "' and a.merchantOrderId= c.merchantOrderId and c.suppliercode= b.usercode and d.barcode= c.barCode";
               
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            pageResult.item = salesGoods.orderId;
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);

            return pageResult;
        }
        



    }
}
