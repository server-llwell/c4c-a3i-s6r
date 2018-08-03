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
            string sql9 = "SELECT count(*) from t_order_list " +
                "where tradeTime BETWEEN '" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 00:00:00'  and  '" 
                + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 23:59:59'  and customerCode='" + userId + "' ";
            DataTable dt9 = DatabaseOperationWeb.ExecuteSelectDS(sql9, "Table").Tables[0];
            dashboard.yesterdaySales = new DashboardDoubleItem();
            dashboard.yesterdaySales.x = "昨日销售额";
            dashboard.yesterdaySales.y = Convert.ToDouble(dt9.Rows[0][0]) == 0 ? 0.1 : Convert.ToInt16(dt9.Rows[0][0]);
            //今日销售额
            string sql10 = "SELECT count(*) from t_order_list " +
                "where tradeTime BETWEEN '" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00'  and  '" 
                + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59'  and customerCode='" + userId + "' ";
            DataTable dt10 = DatabaseOperationWeb.ExecuteSelectDS(sql10, "Table").Tables[0];
            dashboard.todaySales = new DashboardDoubleItem();
            dashboard.todaySales.x = "今日销售额";
            dashboard.todaySales.y = Convert.ToDouble(dt10.Rows[0][0])==0?0.1: Convert.ToInt16(dt10.Rows[0][0]);
            //本周销售额
            string sql11 = "SELECT count(*) from t_order_list " +
                "where tradeTime BETWEEN '" + DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd") + " 00:00:00'  and  '" 
                + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59'  and customerCode='" + userId + "' ";
            DataTable dt11 = DatabaseOperationWeb.ExecuteSelectDS(sql11, "Table").Tables[0];
            dashboard.weekSales = new DashboardDoubleItem();
            dashboard.weekSales.x = "本周销售额";
            dashboard.weekSales.y = Convert.ToDouble(dt11.Rows[0][0]) == 0 ? 0.1 : Convert.ToInt16(dt11.Rows[0][0]);
            //本月销售额
            string sql12 = "SELECT count(*) from t_order_list " +
                "where tradeTime BETWEEN '" + DateTime.Now.AddDays(-29).ToString("yyyy-MM-dd") + " 00:00:00'  and  '" 
                + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59'  and customerCode='" + userId + "' ";
            DataTable dt12 = DatabaseOperationWeb.ExecuteSelectDS(sql12, "Table").Tables[0];
            dashboard.monthSales = new DashboardDoubleItem();
            dashboard.monthSales.x = "本月销售额";
            dashboard.monthSales.y = Convert.ToDouble(dt12.Rows[0][0]) == 0 ? 0.1 : Convert.ToInt16(dt12.Rows[0][0]);
            //最畅销的十款已供商品
            string sql13 = "select g.barCode,g.goodsName ,sum(g.quantity) as xl " +
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

    }
}
