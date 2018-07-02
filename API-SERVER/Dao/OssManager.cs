using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_SERVER.Common;

namespace API_SERVER.Dao
{
    public static class OssManager
    {
        private static OssClient ossClient;

        public static OssClient GetInstance()
        {
            if (ossClient == null)
            {
                ossClient = new OssClient(Global.OssHttp, Global.AccessId, Global.AccessKey);
            }
            return ossClient;
        }
    }
}
