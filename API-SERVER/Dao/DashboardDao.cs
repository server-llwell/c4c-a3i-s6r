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
    public class DashboardDao
    {
        private string path = System.Environment.CurrentDirectory;

        public DashboardDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }
        #region 旧工作台
        public Dashboard getWorkBenchS(string userId)
        {
            Dashboard dashboard = new Dashboard();
            //超时
            string sql1 = "SELECT count(*) from t_order_list " +
                "where (`status` = '1' or `status` = '2') and customerCode='" + userId + "' " +
                "and tradeTime < '" + DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss")+"'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
            dashboard.overtime = dt1.Rows[0][0].ToString();
            //待发货
            string sql2 = "SELECT count(*) from t_order_list where (`status` = '1' or `status` = '2') and customerCode='" + userId + "' ";
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "Table").Tables[0];
            dashboard.wait = dt2.Rows[0][0].ToString();
            //已发货
            string sql3 = "SELECT count(*) from t_order_list where `status` = '3' and customerCode='" + userId + "' ";
            DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "Table").Tables[0];
            dashboard.already = dt3.Rows[0][0].ToString();
            //待收货
            string sql4 = "SELECT count(*) from t_order_list where `status` = '4' and customerCode='" + userId + "' ";
            DataTable dt4 = DatabaseOperationWeb.ExecuteSelectDS(sql4, "Table").Tables[0];
            dashboard.take = dt4.Rows[0][0].ToString();
            //已完成
            string sql5 = "SELECT count(*) from t_order_list where `status` = '5' and customerCode='" + userId + "' ";
            DataTable dt5 = DatabaseOperationWeb.ExecuteSelectDS(sql5, "Table").Tables[0];
            dashboard.done = dt5.Rows[0][0].ToString();
            //待确认
            string sql6 = "SELECT sum(uploadNum) from t_log_upload where (`status` = '1' or `status` = '9')  and userCode = '"+userId+"'";
            DataTable dt6 = DatabaseOperationWeb.ExecuteSelectDS(sql6, "Table").Tables[0];
            if (dt6.Rows.Count>0&& dt6.Rows[0][0].ToString()!=null && dt6.Rows[0][0].ToString() != "")
            {
                dashboard.confirm = dt6.Rows[0][0].ToString();
            }
            else
            {
                dashboard.confirm = "0";
            }
            //库存小于100
            string sql7 = "SELECT count(*) from t_goods_warehouse where suppliercode ='" + userId + "' and goodsnum <100";
            DataTable dt7 = DatabaseOperationWeb.ExecuteSelectDS(sql7, "Table").Tables[0];
            dashboard.goodsNum100 = dt7.Rows[0][0].ToString();
            //库存小于20
            string sql8 = "SELECT count(*) from t_goods_warehouse where suppliercode ='" + userId + "' and goodsnum <20";
            DataTable dt8 = DatabaseOperationWeb.ExecuteSelectDS(sql8, "Table").Tables[0];
            dashboard.goodsNum20 = dt8.Rows[0][0].ToString();
            //昨日销售额
            string sql9 = "SELECT IFNULL(sum(IFNULL(g.supplyPrice,0)),0) from t_order_list o ,t_order_goods g " +
                "where o.merchantOrderId = g.merchantOrderId " +
                "and  o.tradeTime BETWEEN '" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 00:00:00'  and  '" 
                + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 23:59:59'  and customerCode='" + userId + "' ";
            DataTable dt9 = DatabaseOperationWeb.ExecuteSelectDS(sql9, "Table").Tables[0];
            dashboard.yesterdaySales = new DashboardDoubleItem();
            dashboard.yesterdaySales.x = "昨日销售额";
            dashboard.yesterdaySales.y = Convert.ToDouble(dt9.Rows[0][0]) == 0 ? 0.1 : Convert.ToInt16(dt9.Rows[0][0]);
            //今日销售额
            string sql10 = "SELECT IFNULL(sum(IFNULL(g.supplyPrice,0)),0) from t_order_list o ,t_order_goods g " +
                "where o.merchantOrderId = g.merchantOrderId " +
                "and  o.tradeTime BETWEEN '" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00'  and  '" 
                + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59'  and customerCode='" + userId + "' ";
            DataTable dt10 = DatabaseOperationWeb.ExecuteSelectDS(sql10, "Table").Tables[0];
            dashboard.todaySales = new DashboardDoubleItem();
            dashboard.todaySales.x = "今日销售额";
            dashboard.todaySales.y = Convert.ToDouble(dt10.Rows[0][0])==0?0.1: Convert.ToInt16(dt10.Rows[0][0]);
            //本周销售额
            string sql11 = "SELECT IFNULL(sum(IFNULL(g.supplyPrice,0)),0) from t_order_list o ,t_order_goods g " +
                "where o.merchantOrderId = g.merchantOrderId " +
                "and  o.tradeTime BETWEEN '" + DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd") + " 00:00:00'  and  '" 
                + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59'  and customerCode='" + userId + "' ";
            DataTable dt11 = DatabaseOperationWeb.ExecuteSelectDS(sql11, "Table").Tables[0];
            dashboard.weekSales = new DashboardDoubleItem();
            dashboard.weekSales.x = "本周销售额";
            dashboard.weekSales.y = Convert.ToDouble(dt11.Rows[0][0]) == 0 ? 0.1 : Convert.ToInt16(dt11.Rows[0][0]);
            //本月销售额
            string sql12 = "SELECT IFNULL(sum(IFNULL(g.supplyPrice,0)),0) from t_order_list o ,t_order_goods g " +
                "where o.merchantOrderId = g.merchantOrderId " +
                "and  o.tradeTime BETWEEN '" + DateTime.Now.AddDays(-29).ToString("yyyy-MM-dd") + " 00:00:00'  and  '" 
                + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59'  and customerCode='" + userId + "' ";
            DataTable dt12 = DatabaseOperationWeb.ExecuteSelectDS(sql12, "Table").Tables[0];
            dashboard.monthSales = new DashboardDoubleItem();
            dashboard.monthSales.x = "本月销售额";
            dashboard.monthSales.y = Convert.ToDouble(dt12.Rows[0][0]) == 0 ? 0.1 : Convert.ToInt16(dt12.Rows[0][0]);
            //最畅销的十款已供商品
            string sql13 = "select g.barCode,g.goodsName ,IFNULL(sum(IFNULL(g.quantity,0)),0) as xl " +
                "from t_order_list l,t_order_goods g " +
                "where l.merchantOrderId = g.merchantOrderId and g.suppliercode = '" + userId + "' " +
                "group by g.barCode,g.goodsName " +
                "order by sum(g.quantity) desc LIMIT 1,10";
            DataTable dt13 = DatabaseOperationWeb.ExecuteSelectDS(sql13, "Table").Tables[0];
            if (dt13.Rows.Count > 0)
            {
                dashboard.bestSellingSupplier = new List<DashboardItem>();
                for (int i = 0; i < dt13.Rows.Count; i++)
                {
                    DashboardItem dashboardItem = new DashboardItem();
                    dashboardItem.x = dt13.Rows[i]["barCode"].ToString() + "\n" + dt13.Rows[i]["goodsName"].ToString();
                    dashboardItem.y = Convert.ToInt32(dt13.Rows[i]["xl"]);
                    dashboard.bestSellingSupplier.Add(dashboardItem);
                }
            }
            //最畅销的十款平台商品
            string sql14 = "select g.barCode,g.goodsName ,IFNULL(sum(IFNULL(g.quantity,0)),0) as xl " +
                "from t_order_list l,t_order_goods g " +
                "where l.merchantOrderId = g.merchantOrderId  " +
                "group by g.barCode,g.goodsName " +
                "order by sum(g.quantity) desc LIMIT 1,10";
            DataTable dt14 = DatabaseOperationWeb.ExecuteSelectDS(sql14, "Table").Tables[0];
            if (dt14.Rows.Count > 0)
            {
                dashboard.bestSellingPlatform = new List<DashboardItem>();
                for (int i = 0; i < dt14.Rows.Count; i++)
                {
                    DashboardItem dashboardItem = new DashboardItem();
                    dashboardItem.x = dt14.Rows[i]["barCode"].ToString() + "\n" + dt14.Rows[i]["goodsName"].ToString();
                    dashboardItem.y = Convert.ToInt32(dt14.Rows[i]["xl"]);
                    dashboard.bestSellingPlatform.Add(dashboardItem);
                }
            }
            return dashboard;
        }
        public Dashboard getWorkBenchO()
        {
            Dashboard dashboard = new Dashboard();
            //超时
            string sql1 = "SELECT count(*) from t_order_list " +
                "where (`status` = '1' or `status` = '2')  " +
                "and tradeTime < '" + DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss") + "'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
            dashboard.overtime = dt1.Rows[0][0].ToString();
            //待发货
            string sql2 = "SELECT count(*) from t_order_list where (`status` = '1' or `status` = '2') ";
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "Table").Tables[0];
            dashboard.wait = dt2.Rows[0][0].ToString();
            //已发货
            string sql3 = "SELECT count(*) from t_order_list where `status` = '3'";
            DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "Table").Tables[0];
            dashboard.already = dt3.Rows[0][0].ToString();
            //待收货
            string sql4 = "SELECT count(*) from t_order_list where `status` = '4'";
            DataTable dt4 = DatabaseOperationWeb.ExecuteSelectDS(sql4, "Table").Tables[0];
            dashboard.take = dt4.Rows[0][0].ToString();
            //已完成
            string sql5 = "SELECT count(*) from t_order_list where `status` = '5' ";
            DataTable dt5 = DatabaseOperationWeb.ExecuteSelectDS(sql5, "Table").Tables[0];
            dashboard.done = dt5.Rows[0][0].ToString();
            ////待确认
            //string sql6 = "SELECT sum(uploadNum) from t_log_upload where (`status` = '1' or `status` = '9')  and userCode = '" + userId + "'";
            //DataTable dt6 = DatabaseOperationWeb.ExecuteSelectDS(sql6, "Table").Tables[0];
            //if (dt6.Rows.Count > 0 && dt6.Rows[0][0].ToString() != null && dt6.Rows[0][0].ToString() != "")
            //{
            //    dashboard.confirm = dt6.Rows[0][0].ToString();
            //}
            //else
            //{
            //    dashboard.confirm = "0";
            //}
            //库存小于100
            string sql7 = "SELECT count(*) from t_goods_warehouse where goodsnum <100";
            DataTable dt7 = DatabaseOperationWeb.ExecuteSelectDS(sql7, "Table").Tables[0];
            dashboard.goodsNum100 = dt7.Rows[0][0].ToString();
            //库存小于20
            string sql8 = "SELECT count(*) from t_goods_warehouse where goodsnum <20";
            DataTable dt8 = DatabaseOperationWeb.ExecuteSelectDS(sql8, "Table").Tables[0];
            dashboard.goodsNum20 = dt8.Rows[0][0].ToString();
            //昨日销售额
            string sql9 = "SELECT p.platformType ,sum(tradeAmount) as tradeAmount" +
                " from t_order_list o ,t_base_platform p " +
                "where o.platformId = p.platformId and tradeTime BETWEEN '" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 00:00:00'  and  '"
                + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 23:59:59' group by p.platformType ";
            DataTable dt9 = DatabaseOperationWeb.ExecuteSelectDS(sql9, "Table").Tables[0];
            //今日销售额
            string sql10 = "SELECT p.platformType ,sum(tradeAmount) as tradeAmount " +
                "from t_order_list o ,t_base_platform p " +
                "where o.platformId = p.platformId and tradeTime BETWEEN '" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00'  and  '"
                + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59' group by p.platformType ";
            DataTable dt10 = DatabaseOperationWeb.ExecuteSelectDS(sql10, "Table").Tables[0];
            //本周销售额
            string sql11 = "SELECT p.platformType ,sum(tradeAmount) as tradeAmount " +
                "from t_order_list o ,t_base_platform p " +
                "where o.platformId = p.platformId and tradeTime BETWEEN '" + DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd") + " 00:00:00'  and  '"
                + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59' group by p.platformType ";
            DataTable dt11 = DatabaseOperationWeb.ExecuteSelectDS(sql11, "Table").Tables[0];
            //本月销售额
            string sql12 = "SELECT p.platformType ,sum(tradeAmount) as tradeAmount " +
                "from t_order_list o ,t_base_platform p " +
                "where o.platformId = p.platformId and tradeTime BETWEEN '" + DateTime.Now.AddDays(-29).ToString("yyyy-MM-dd") + " 00:00:00'  and  '"
                + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59' group by p.platformType  ";
            DataTable dt12 = DatabaseOperationWeb.ExecuteSelectDS(sql12, "Table").Tables[0];
            string platformSql = "select * from t_base_platform";
            DataTable platformDT= DatabaseOperationWeb.ExecuteSelectDS(platformSql, "Table").Tables[0];
            if (platformDT.Rows.Count>0)
            {
                dashboard.dashboardSales = new List<DashboardSales>();
                double y = 0, t = 0, w = 0, m = 0;
                for (int i = 0; i < platformDT.Rows.Count; i++)
                {
                    DashboardSales dashboardSales1 = new DashboardSales();
                    dashboardSales1.id = i + 1;
                    dashboardSales1.PlatformType = platformDT.Rows[i]["platformType"].ToString();
                    dashboardSales1.yesterdaySales = "0";
                    dashboardSales1.todaySales = "0";
                    dashboardSales1.weekSales = "0";
                    dashboardSales1.monthSales = "0";
                    for (int j = 0; j < dt9.Rows.Count; j++)
                    {
                        if (platformDT.Rows[i]["platformType"].ToString() == dt9.Rows[j]["platformType"].ToString())
                        {
                            dashboardSales1.yesterdaySales = dt9.Rows[j]["tradeAmount"].ToString();
                            y += Convert.ToDouble(dt9.Rows[j]["tradeAmount"]);
                        }
                    }
                    for (int j = 0; j < dt10.Rows.Count; j++)
                    {
                        if (platformDT.Rows[i]["platformType"].ToString() == dt10.Rows[j]["platformType"].ToString())
                        {
                            dashboardSales1.todaySales = dt10.Rows[j]["tradeAmount"].ToString();
                            t += Convert.ToDouble(dt10.Rows[j]["tradeAmount"]);
                        }
                    }
                    for (int j = 0; j < dt11.Rows.Count; j++)
                    {
                        if (platformDT.Rows[i]["platformType"].ToString() == dt11.Rows[j]["platformType"].ToString())
                        {
                            dashboardSales1.weekSales = dt11.Rows[j]["tradeAmount"].ToString();
                            w += Convert.ToDouble(dt11.Rows[j]["tradeAmount"]);
                        }
                    }
                    for (int j = 0; j < dt12.Rows.Count; j++)
                    {
                        if (platformDT.Rows[i]["platformType"].ToString() == dt12.Rows[j]["platformType"].ToString())
                        {
                            dashboardSales1.monthSales = dt12.Rows[j]["tradeAmount"].ToString();
                            m += Convert.ToDouble(dt12.Rows[j]["tradeAmount"]);
                        }
                    }
                    dashboard.dashboardSales.Add(dashboardSales1);
                }
                DashboardSales dashboardSales = new DashboardSales();
                dashboardSales.id = platformDT.Rows.Count+1;
                dashboardSales.PlatformType = "总计";
                dashboardSales.yesterdaySales = y.ToString();
                dashboardSales.todaySales = t.ToString();
                dashboardSales.weekSales = w.ToString();
                dashboardSales.monthSales = m.ToString();
                dashboard.dashboardSales.Add(dashboardSales);
            }
            //最畅销的十款平台商品
            string sql14 = "select g.barCode,g.goodsName ,sum(g.quantity) as xl " +
                "from t_order_list l,t_order_goods g " +
                "where l.merchantOrderId = g.merchantOrderId  " +
                "group by g.barCode,g.goodsName " +
                "order by sum(g.quantity) desc LIMIT 1,10";
            DataTable dt14 = DatabaseOperationWeb.ExecuteSelectDS(sql14, "Table").Tables[0];
            if (dt14.Rows.Count > 0)
            {
                dashboard.bestSellingPlatform = new List<DashboardItem>();
                for (int i = 0; i < dt14.Rows.Count; i++)
                {
                    DashboardItem dashboardItem = new DashboardItem();
                    dashboardItem.x = dt14.Rows[i]["barCode"].ToString() + "\n" + dt14.Rows[i]["goodsName"].ToString();
                    dashboardItem.y = Convert.ToInt32(dt14.Rows[i]["xl"]);
                    dashboard.bestSellingPlatform.Add(dashboardItem);
                }
            }
            //平台各渠道订单数分布图
            string sql15 = "SELECT p.platformType ,count(*) as count " +
                "from t_order_list o ,t_base_platform p " +
                "where o.platformId = p.platformId group by p.platformType";
            DataTable dt15 = DatabaseOperationWeb.ExecuteSelectDS(sql15, "Table").Tables[0];
            if (dt15.Rows.Count > 0)
            {
                dashboard.platformOrder = new List<DashboardItem>();
                for (int i = 0; i < dt15.Rows.Count; i++)
                {
                    DashboardItem dashboardItem = new DashboardItem();
                    dashboardItem.x = dt15.Rows[i]["platformType"].ToString();
                    dashboardItem.y = Convert.ToInt32(dt15.Rows[i]["count"]);
                    dashboard.platformOrder.Add(dashboardItem);
                }
            }



            return dashboard;
        }
        #endregion

        #region 新工作台
        public NewDashboard GetNewWorkBenchS(string userId)
        {            
            NewDashboard newDashboard = new NewDashboard();
            newDashboard.batchSupply = new SalseDate();
            newDashboard.substitute = new SalseDate();
            newDashboard.distribution = new SalseDate();
            newDashboard.goods = new GoodsSign();
            newDashboard.bussness = new BussnessSign();           
            GoodsSign goodsSign = new GoodsSign();
            BussnessSign bussnessSign = new BussnessSign();

            string time = DateTime.Now.ToString("yyyy-MM-dd");
            string time1 = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            string time2 = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string time3 = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");

            //公司名
            string selectCompany = "select merchantName from t_contract_list where userCode='"+ userId + "'";
            DataTable dtCompany = DatabaseOperationWeb.ExecuteSelectDS(selectCompany,"T").Tables[0];
            if (dtCompany.Rows.Count>0)
            {
                newDashboard.company = dtCompany.Rows[0][0].ToString();
            }
            //最后登陆时间
            string selectLastTime = "select lasttime from t_user_list where usercode='"+ userId + "'";
            DataTable dtLastTime = DatabaseOperationWeb.ExecuteSelectDS(selectLastTime, "T").Tables[0];
            if (dtLastTime.Rows.Count > 0)
            {
                newDashboard.lastTime = dtLastTime.Rows[0][0].ToString();
            }

            //批量供货  
            SalseDate salseDate = new SalseDate();
            string selectbatchSupply = ""
                + "select count(purchasesn) purchasesn,sum(totalPrice) totalPrice "
                + " from t_purchase_inquiry where usercode='" + userId + "' and flag='3' and (createtime between '"+time+"' and '"+time1+"')";
            DataTable dtbatchSupply = DatabaseOperationWeb.ExecuteSelectDS(selectbatchSupply, "T").Tables[0];
            string selectYestBatchSupply = ""
                + "select count(purchasesn) purchasesn,sum(totalPrice) totalPrice "
                + " from t_purchase_inquiry where usercode='" + userId + "' and flag='3' and (createtime between '" + time3 + "' and '" + time2 + "')";
            DataTable dtYestBatchSupply = DatabaseOperationWeb.ExecuteSelectDS(selectYestBatchSupply, "T").Tables[0];
            salseDate.avgOrderPrice = "0";
            if (dtbatchSupply.Rows.Count > 0)
            {               
                salseDate.todayOrder = dtbatchSupply.Rows[0]["purchasesn"].ToString();
                salseDate.todayOrderPrice= dtbatchSupply.Rows[0]["totalPrice"].ToString();                           
            }
            if (dtYestBatchSupply.Rows.Count > 0)
            {
                salseDate.yestOrder = dtYestBatchSupply.Rows[0]["purchasesn"].ToString();
                salseDate.yestOrderPrice = dtYestBatchSupply.Rows[0]["totalPrice"].ToString();
            }
            if (salseDate.yestOrderPrice==null || salseDate.yestOrderPrice == "")
            {
                salseDate.yestOrderPrice = "0";
            }
            if (salseDate.todayOrderPrice == null || salseDate.todayOrderPrice == "")
            {
                salseDate.todayOrderPrice = "0";
            }
            newDashboard.batchSupply=salseDate;

            //一件代发 
            SalseDate salseDate1 = new SalseDate();
            string selectavgOrderPrice = ""
                + "select (sum(a.supplyPrice*a.quantity)/count(a.merchantOrderId)) avg  "
                + " from t_order_goods a,t_order_list b "
                + " where a.merchantOrderId=b.merchantOrderId and suppliercode='"+ userId + "' and platformId='4'";//客单价
            DataTable dtavgOrderPrice = DatabaseOperationWeb.ExecuteSelectDS(selectavgOrderPrice, "T").Tables[0];
            if (dtavgOrderPrice.Rows[0][0].ToString()!=null && dtavgOrderPrice.Rows[0][0].ToString() != "")
            {
                salseDate1.avgOrderPrice = string.Format("{0:F}", Convert.ToDouble(dtavgOrderPrice.Rows[0][0]));
            }
            else
            {
                salseDate1.avgOrderPrice = "0";
            }
            string selectsubstitute = ""//今日数据
                + "select count(a.merchantOrderId) merchantOrderId,sum(a.supplyPrice*a.quantity) totalPrice "
                + " from t_order_goods a,t_order_list b  "
                + " where a.merchantOrderId=b.merchantOrderId and suppliercode='" + userId + "' and platformId='4' and (inputTime between '" + time + "' and '" + time1 + "')";
            DataTable dtsubstitutee = DatabaseOperationWeb.ExecuteSelectDS(selectsubstitute, "T").Tables[0];
            if(dtsubstitutee.Rows.Count > 0)
            {
                salseDate1.todayOrder = dtsubstitutee.Rows[0]["merchantOrderId"].ToString();
                salseDate1.todayOrderPrice = dtsubstitutee.Rows[0]["totalPrice"].ToString();
            }

            string selectYestsubstitutey = ""//昨日数据
                + "select count(a.merchantOrderId) merchantOrderId,sum(a.supplyPrice*a.quantity) totalPrice "
                + " from t_order_goods a,t_order_list b  "
                + " where a.merchantOrderId=b.merchantOrderId and suppliercode='" + userId + "' and platformId='4' and (inputTime between '" + time3 + "' and '" + time2 + "')";
            DataTable dtYestsubstitute = DatabaseOperationWeb.ExecuteSelectDS(selectYestsubstitutey, "T").Tables[0];
            
            if (dtYestsubstitute.Rows.Count > 0)
            {
                salseDate1.yestOrder = dtYestsubstitute.Rows[0]["merchantOrderId"].ToString();
                salseDate1.yestOrderPrice = dtYestsubstitute.Rows[0]["totalPrice"].ToString();               
            }
            if (salseDate1.yestOrderPrice == null || salseDate1.yestOrderPrice == "")
            {
                salseDate1.yestOrderPrice = "0";
            }
            if (salseDate1.todayOrderPrice == null || salseDate1.todayOrderPrice == "")
            {
                salseDate1.todayOrderPrice = "0";
            }
            newDashboard.substitute=salseDate1;

            //铺货
            SalseDate salseDate2 = new SalseDate();
            string selectPHavgOrderPrice = ""
                + "select (sum(a.supplyPrice*a.quantity)/count(a.merchantOrderId)) avg  "
                + " from t_order_goods a,t_order_list b "
                + " where a.merchantOrderId=b.merchantOrderId and suppliercode='" + userId + "' and platformId='3'";//客单价
            DataTable dtPHavgOrderPrice = DatabaseOperationWeb.ExecuteSelectDS(selectPHavgOrderPrice, "T").Tables[0];
            if (dtPHavgOrderPrice.Rows[0][0] !=DBNull.Value)
            {
                salseDate2.avgOrderPrice = string.Format("{0:F}", Convert.ToDouble(dtPHavgOrderPrice.Rows[0][0]));
            }
            else
            {
                salseDate2.avgOrderPrice = "0";
            }
            string selectdistribution = ""//今日数据
                + "select count(a.merchantOrderId) merchantOrderId,sum(a.supplyPrice*a.quantity) totalPrice "
                + " from t_order_goods a,t_order_list b  "
                + " where a.merchantOrderId=b.merchantOrderId and suppliercode='" + userId + "' and platformId='3' and (inputTime between '" + time + "' and '" + time1 + "')";
            DataTable dtdistribution = DatabaseOperationWeb.ExecuteSelectDS(selectdistribution, "T").Tables[0];
            if (dtsubstitutee.Rows.Count > 0)
            {
                salseDate2.todayOrder = dtdistribution.Rows[0]["merchantOrderId"].ToString();
                salseDate2.todayOrderPrice = dtdistribution.Rows[0]["totalPrice"].ToString();
            }

            string selectYestdistribution = ""//昨日数据
                + "select count(a.merchantOrderId) merchantOrderId,sum(a.supplyPrice*a.quantity) totalPrice "
                + " from t_order_goods a,t_order_list b  "
                + " where a.merchantOrderId=b.merchantOrderId and suppliercode='" + userId + "' and platformId='3' and (inputTime between '" + time3 + "' and '" + time2 + "')";
            DataTable dtYestdistribution = DatabaseOperationWeb.ExecuteSelectDS(selectYestdistribution, "T").Tables[0];

            if (dtYestsubstitute.Rows.Count > 0)
            {
                salseDate2.yestOrder = dtYestdistribution.Rows[0]["merchantOrderId"].ToString();
                salseDate2.yestOrderPrice = dtYestdistribution.Rows[0]["totalPrice"].ToString();
            }
            if (salseDate2.yestOrderPrice == null || salseDate2.yestOrderPrice == "")
            {
                salseDate2.yestOrderPrice = "0";
            }
            if (salseDate2.todayOrderPrice == null || salseDate2.todayOrderPrice == "")
            {
                salseDate2.todayOrderPrice = "0";
            }
            newDashboard.distribution=salseDate2;

            //商品提示
            string selectgoodsSign = ""//销售过的商品
                + " select barcode from t_order_goods   where  suppliercode='"+ userId + "' "
                + " union "
                + " select barcode from   t_purchase_inquiry usercode  where  usercode='" + userId + "' ";
            DataTable dtselectgoodsSign = DatabaseOperationWeb.ExecuteSelectDS(selectgoodsSign,"T").Tables[0];
            if (dtselectgoodsSign.Rows.Count > 0)
            {
                goodsSign.salsedGoods = dtselectgoodsSign.Rows.Count.ToString();
            }
            else
            {
                goodsSign.salsedGoods = "0";
            }
            string selectsalsingGoods = ""//销售中的商品
                + " select count(barcode) from t_goods_warehouse where suppliercode='"+ userId + "'";
            DataTable dtsalsingGoods = DatabaseOperationWeb.ExecuteSelectDS(selectsalsingGoods, "T").Tables[0];
            if (dtsalsingGoods.Rows[0][0].ToString()!=null && dtsalsingGoods.Rows[0][0].ToString() != "")
            {
                goodsSign.salsingGoods = dtsalsingGoods.Rows[0][0].ToString();
            }
            else
            {
                goodsSign.salsingGoods = "0";
            }
            goodsSign.otherGoods = (Convert.ToInt16(goodsSign.salsingGoods) - Convert.ToInt16(goodsSign.salsedGoods)).ToString() ;//从未销售的商品数

            string selectwarningGoods = ""//库存预警商品
                + "select count(barcode) from t_goods_warehouse where goodsnum<'20' and suppliercode='"+ userId + "' and flag='1'";
            DataTable dtwarningGoods = DatabaseOperationWeb.ExecuteSelectDS(selectwarningGoods,"T").Tables[0];
            if (dtwarningGoods.Rows[0][0].ToString()!=null && dtwarningGoods.Rows[0][0].ToString() != "")
            {
                goodsSign.warningGoods = dtwarningGoods.Rows[0][0].ToString();
            }
            else
            {
                goodsSign.warningGoods = "0";
            }
            newDashboard.goods=goodsSign;
            //待报价订单
            string selectofferOrders = ""//待报价订单
                + "select count(b.purchasesn) "
                + " from t_purchase_goods a,t_purchase_list b,t_goods_warehouse c"
                + " where a.purchasesn=b.purchasesn and a.barcode=c.barcode and c.suppliercode='"+ userId + "' and b.status='1'";
            DataTable dtofferOrders = DatabaseOperationWeb.ExecuteSelectDS(selectofferOrders,"T").Tables[0];
            if (dtofferOrders.Rows[0][0].ToString() != null && dtofferOrders.Rows[0][0].ToString() != "")
            {
                bussnessSign.offerOrders = dtofferOrders.Rows[0][0].ToString();
            }
            else
                bussnessSign.offerOrders = "0";

            string selectdeliveryOreders = ""//待发货订单order表
                + " select a.merchantOrderId from t_order_list a,t_order_goods b  "
                + " where  a.merchantOrderId=b.merchantOrderId and  a.`status` in('1','2') and b.suppliercode='"+ userId + "' GROUP BY a.merchantOrderId";
            DataTable dtdeliveryOreders = DatabaseOperationWeb.ExecuteSelectDS(selectdeliveryOreders,"T").Tables[0];
            if (dtdeliveryOreders.Rows.Count > 0)
            {
                bussnessSign.deliveryOreders = dtdeliveryOreders.Rows.Count.ToString();
            }
            else
            {
                bussnessSign.deliveryOreders = "0";
            }
            string selectdeliveryOreders1 = ""//待发货订单批采表   
               + " select a.purchasesn from t_purchase_list a,t_purchase_inquiry b "
               + " where a.purchasesn=b.purchasesn and b.usercode='" + userId + "' and a.stage='1' GROUP BY a.purchasesn";
            DataTable dtdeliveryOreders1 = DatabaseOperationWeb.ExecuteSelectDS(selectdeliveryOreders1, "T").Tables[0];
            if (dtdeliveryOreders1.Rows.Count > 0)
            {
                bussnessSign.pendingReturnOrders =( Convert.ToInt16(bussnessSign.pendingReturnOrders)+ Convert.ToInt16(dtdeliveryOreders1.Rows.Count)).ToString();
            }


            string selectpendingReturnOrders = ""//待处理退货订单
                + " select a.merchantOrderId from t_order_list a,t_order_goods b "
                + " where  a.merchantOrderId=b.merchantOrderId and  a.`status` ='6' and b.suppliercode='" + userId + "' GROUP BY a.merchantOrderId";
            DataTable dtpendingReturnOrders = DatabaseOperationWeb.ExecuteSelectDS(selectpendingReturnOrders, "T").Tables[0];
            if (dtpendingReturnOrders.Rows.Count > 0)
            {
                bussnessSign.pendingReturnOrders = dtpendingReturnOrders.Rows.Count.ToString();
            }
            else
            {
                bussnessSign.pendingReturnOrders = "0";
            }

            string selectReturnedOrders = ""//已退货待我签收订单
                + " select a.merchantOrderId from t_order_list a,t_order_goods b "
                + " where  a.merchantOrderId=b.merchantOrderId and  a.`status` ='7' and b.suppliercode='" + userId + "' GROUP BY a.merchantOrderId";
            DataTable dtReturnedOrders = DatabaseOperationWeb.ExecuteSelectDS(selectReturnedOrders, "T").Tables[0];
            if (dtpendingReturnOrders.Rows.Count > 0)
            {
                bussnessSign.ReturnedOrders = dtReturnedOrders.Rows.Count.ToString();
            }
            else
            {
                bussnessSign.ReturnedOrders = "0";
            }
            
            newDashboard.bussness=bussnessSign;
            return newDashboard;
        }
            #endregion
    }
}
