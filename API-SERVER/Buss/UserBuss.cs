using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace API_SERVER.Buss
{
    public class UserBuss :IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.UserApi;
        }

        public bool NeedCheckToken()
        {
            return true;
        }
        /// <summary>
        /// 获取运营客户接口
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_GetOperateCustomer(object param, string userId)
        {
            OperateCustomerParam ocParam = JsonConvert.DeserializeObject<OperateCustomerParam>(param.ToString());
            if (ocParam.pageSize == 0)
            {
                ocParam.pageSize = 10;
            }
            if (ocParam.current == 0)
            {
                ocParam.current = 1;
            }
            UserDao UserDao = new UserDao();
            return UserDao.GetOperateCustomer(ocParam, userId);
        }

    }
        public class OperateCustomerParam
        {
            public int search;//查询信息
            public int usertype;//用户权限
            public int pageSize; //页面显示多少个商品
            public int current;//多少页

    }

}
