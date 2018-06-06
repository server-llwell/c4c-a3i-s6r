using API_SERVER.Buss;
using API_SERVER.Dao;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Common
{
    public class Global
    {
        public const string ROUTE_PX = "llback";

        /// <summary>
        /// 基础业务处理类对象
        /// </summary>
        public static BaseBuss BUSS = new BaseBuss();

        /// <summary>
        /// 初始化启动预加载
        /// </summary>
        public static void StartUp()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        public static string REDIS
        {
            get
            {
#if DEBUG
                var redis = System.Environment.GetEnvironmentVariable("redis", EnvironmentVariableTarget.User);
#endif
#if !DEBUG
                var redis = "redis-api";
#endif
                return redis;
            }
        }
    }
}
