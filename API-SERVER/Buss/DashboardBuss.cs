using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace API_SERVER.Buss
{
    public class DashboardBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.DashboardApi;
        }

        public object Do_GetWorkBenchS(object param, string userId)
        {
            DashboardDao dashboardDao = new DashboardDao();
            return dashboardDao.getWorkBenchS(userId);
        }

        public object Do_GetWorkBenchO(object param, string userId)
        {
            DashboardDao dashboardDao = new DashboardDao();
            return dashboardDao.getWorkBenchO();
        }
    }
    public class Dashboard
    {
        public string overtime;//超时
        public string wait;//待发货
        public string already;//已发货
        public string take;//待收货
        public string done;//已完成
        public string confirm;//待确认
        public string goodsNum100;//库存小于100
        public string goodsNum20;//库存小于20
        public DashboardItem yesterdaySales;//昨日销售额
        public DashboardItem todaySales;//今日销售额
        public DashboardItem weekSales;//本周销售额
        public DashboardItem monthSales;//本月销售额
        public List<DashboardItem> bestSellingSupplier;//最畅销的十款已供商品
        public List<DashboardItem> bestSellingPlatform;//最畅销的十款平台商品
        public List<DashboardSales> dashboardSales;//销售概况
        public List<DashboardItem> platformOrder;//平台各渠道订单数分布图
    }
    public class DashboardItem
    {
        public string x;
        public int y;
    }
    public class DashboardSales
    {
        public int id;
        public string PlatformType;//平台渠道
        public string yesterdaySales;//昨日销售额
        public string todaySales;//今日销售额
        public string weekSales;//本周销售额
        public string monthSales;//本月销售额
    }
}






