﻿using API_SERVER.Buss;
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
        public const int REDIS_NO = 1;

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
                var redis = System.Environment.GetEnvironmentVariable("redis");
                return redis;
            }
        }

        #region OSS相关

        /// <summary>
        /// AccessId
        /// </summary>
        public static string AccessId
        {
            get
            {
                var accessId = System.Environment.GetEnvironmentVariable("ossAccessId");
                return accessId;
            }
        }
        /// <summary>
        /// AccessKey
        /// </summary>
        public static string AccessKey
        {
            get
            {
                var accessKey = System.Environment.GetEnvironmentVariable("ossAccessKey");
                return accessKey;
            }
        }
        /// <summary>
        /// OssHttp
        /// </summary>
        public static string OssHttp
        {
            get
            {
                var ossHttp = System.Environment.GetEnvironmentVariable("ossHttp");
                return ossHttp;
            }
        }
        /// <summary>
        /// OssBucket
        /// </summary>
        public static string OssBucket
        {
            get
            {
                var ossBucket = System.Environment.GetEnvironmentVariable("ossBucket");
                return ossBucket;
            }
        }
        /// <summary>
        /// ossUrl
        /// </summary>
        public static string OssUrl
        {
            get
            {
                var ossUrl = System.Environment.GetEnvironmentVariable("ossUrl");
                return ossUrl;
            }
        }
        /// <summary>
        /// OssDir
        /// </summary>
        public static string OssDir
        {
            get
            {
                var ossDir = System.Environment.GetEnvironmentVariable("ossDir");
                return ossDir;
            }
        }

        /// <summary>
        /// OssDirFiles
        /// </summary>
        public static string OssDirFiles
        {
            get
            {
                var ossDirFiles = System.Environment.GetEnvironmentVariable("ossDirFiles");
                return ossDirFiles;
            }
        }

        /// <summary>
        /// OssDirOrder
        /// </summary>
        public static string OssDirOrder
        {
            get
            {
                var ossDir = System.Environment.GetEnvironmentVariable("ossDirOrder");
                return ossDir;
            }
        }

        /// <summary>
        /// ossB2BGoods
        /// </summary>
        public static string ossB2BGoods
        {
            get
            {
                var ossDir = System.Environment.GetEnvironmentVariable("ossB2BGoods");
                return ossDir;
            }
        }

        /// <summary>
        /// ossB2BGoodsNum
        /// </summary>
        public static string ossB2BGoodsNum
        {
            get
            {
                var ossDir = System.Environment.GetEnvironmentVariable("ossB2BGoodsNum");
                return ossDir;
            }
        }

        #endregion

        /// <summary>
        /// 公众号APIid
        /// </summary>
        public static string WXAPI
        {
            get
            {
                var WXAPI = System.Environment.GetEnvironmentVariable("WXAPI");
                return WXAPI;
            }
        }
        /// <summary>
        /// 公众号APPSECRET
        /// </summary>
        public static string WXAPPSECRET
        {
            get
            {
                var WXAPPSECRET = System.Environment.GetEnvironmentVariable("WXAPPSECRET");
                return WXAPPSECRET;
            }
        }

        /// <summary>
        /// MCHID：商户账号
        /// </summary>
        public static string MCHID
        {
            get
            {
                var MCHID = System.Environment.GetEnvironmentVariable("MCHID");
                return MCHID;
            }
        }
        /// <summary>
        /// NotifyUrl：回调地址
        /// </summary>
        public static string NotifyUrl
        {
            get
            {
                var NotifyUrl = System.Environment.GetEnvironmentVariable("NotifyUrl");
                return NotifyUrl;
            }
        }

        /// <summary>
        /// 商户APIKEY
        /// </summary>
        public static string APIKEY
        {
            get
            {
                var APIKEY = System.Environment.GetEnvironmentVariable("APIKEY");
                return APIKEY;
            }
        }

        /// <summary>
        /// 零售userCode
        /// </summary>
        public static string RetailUser
        {
            get
            {
                var RetailUser = System.Environment.GetEnvironmentVariable("RetailUser");
                return RetailUser;
            }
        }
    }
}
