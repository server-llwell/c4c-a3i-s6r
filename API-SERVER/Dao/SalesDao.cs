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
            if (salesGoods.select != null && salesGoods.select!="")
            {
                st = "and ( a.barCode like '%"+ salesGoods.select + "%' or a.goodsName like '%"+ salesGoods.select+ "%' or brand like '%"+ salesGoods.select+"%')";
            }
            string time = "";
            if (salesGoods.date!= null && salesGoods.date.Length ==2)
            {
                time = " and tradeTime between  str_to_date('" + salesGoods.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + salesGoods.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
           
            string sql = "select t_goods_list.slt,a.barCode,a.goodsName,brand,a.skuUnitPrice,a.quantity,a.supplyPrice,tradeTime FROM t_order_list,t_order_goods a,t_goods_list where t_order_list.merchantOrderId = a.merchantOrderId  and a.barCode = t_goods_list.barcode  and purchaserCode = '"+ 
                purchaserCode+ "' "+st +time+ " order by tradeTime desc  limit "+ (salesGoods.current-1)*salesGoods.pageSize+","+ salesGoods.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
           
            if (dt.Rows.Count> 0)
            {
                for (int i=0;i< dt.Rows.Count;i++)
                {
                    SalesGoodsItem salesGoodsItem = new SalesGoodsItem();
                    salesGoodsItem.barCode = dt.Rows[i]["barCode"].ToString();
                    salesGoodsItem.keyId = Convert.ToString((salesGoods.current - 1) * salesGoods.pageSize + i + 1);
                    salesGoodsItem.brand = dt.Rows[i]["brand"].ToString();
                    salesGoodsItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                    salesGoodsItem.quantity = Convert.ToInt16(dt.Rows[i]["quantity"].ToString());
                    salesGoodsItem.skuUnitPrice = Convert.ToDouble(dt.Rows[i]["skuUnitPrice"].ToString());
                    salesGoodsItem.supplyPrice = Convert.ToDouble(dt.Rows[i]["supplyPrice"].ToString());
                    salesGoodsItem.tradeTime = dt.Rows[i]["tradeTime"].ToString();
                    salesGoodsItem.money = Convert.ToDouble(salesGoodsItem.quantity * salesGoodsItem.skuUnitPrice);
                    salesGoodsItem.slt = dt.Rows[i]["slt"].ToString();
                    pageResult.list.Add(salesGoodsItem); 
                }
            }
            string sql1 = "select count(*) FROM t_order_list,t_order_goods a, t_goods_list where t_order_list.merchantOrderId = a.merchantOrderId  and a.barCode = t_goods_list.barcode  and purchaserCode = '"+ 
                purchaserCode + "' " + st + time;
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "table").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return pageResult;
        }

       


    }
}
