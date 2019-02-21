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
        public bool NeedCheckToken()
        {
            return true;
        }

        #region 旧工作台
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
        #endregion

        #region 新工作台
        public object Do_GetNewWorkBenchS(object param, string userId)
        {
            DashboardDao dashboardDao = new DashboardDao();
            return dashboardDao.GetNewWorkBenchS(userId);
        }

        #endregion
    }

    public class NewDashboard
    {
        public string company;//公司名称
        public string lastTime;//最后登录时间
        public SalseDate batchSupply;//批量供货
        public SalseDate substitute;//一件代发
        public SalseDate distribution;//铺货
        public GoodsSign goods;//商品提示
        public BussnessSign bussness;//交易提示
    }

    public class SalseDate
    {
        public string avgOrderPrice="0";//客单价
        public string todayOrderPrice="0";//今日成交额
        public string todayOrder = "0";//今日订单数
        public string yestOrderPrice = "0";//昨日成交额
        public string yestOrder = "0";//昨日订单数
    }

    public class GoodsSign
    {
        public string salsingGoods = "0";//出售中的商品
        public string salsedGoods = "0";//销售过的商品
        public string otherGoods = "0";//从未销售的商品
        public string warningGoods = "0";//库存预警商品
    }

    public class BussnessSign
    {
        public string offerOrders = "0";//待报价订单
        public string deliveryOreders = "0";//待发货订单
        public string pendingReturnOrders = "0";//待处理退货订单
        public string ReturnedOrders = "0";//已退货待我签收订单
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
        public DashboardDoubleItem yesterdaySales;//昨日销售额
        public DashboardDoubleItem todaySales;//今日销售额
        public DashboardDoubleItem weekSales;//本周销售额
        public DashboardDoubleItem monthSales;//本月销售额
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
    public class DashboardDoubleItem
    {
        public string x;
        public double y;
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






