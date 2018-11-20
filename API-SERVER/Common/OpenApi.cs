using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Common
{
    /// <summary>
    /// API类型分组
    /// </summary>
    public enum ApiType
    {
        TicketApi,
        O2OApi,
        GoodsApi,
        OrderApi,
        DistributorApi,
        UploadApi,
        SalesApi,
        DashboardApi,
        AgentApi,
        UserApi,
        ApiApi,
        BalanceApi,
        CustomsApi,
        ScanApi,
        WarehouseApi,
    }

    /// <summary>
    /// 微信用户类API
    /// </summary>
    public class TicketApi
    {
        public object param;
    }
}
