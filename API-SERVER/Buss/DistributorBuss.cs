using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace API_SERVER.Buss
{
    public class DistributorBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.DistributorApi;
        }

        public object Do_GetPlatform(object param)
        {
            DistributorDao distributorDao = new DistributorDao();
            return distributorDao.getPlatform();
        }

        public object Do_DistributorList(object param)
        {
            DistributorParam distributorParam = JsonConvert.DeserializeObject<DistributorParam>(param.ToString());
            if (distributorParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (distributorParam.pageSize == 0)
            {
                distributorParam.pageSize = 10;
            }
            if (distributorParam.current == 0)
            {
                distributorParam.current = 1;
            }
            DistributorDao distributorDao = new DistributorDao();
            return distributorDao.getDistributorList(distributorParam);
        }

        public object Do_UpdateDistributor(object param)
        {
            DistributorItem distributorItem = JsonConvert.DeserializeObject<DistributorItem>(param.ToString());
            if (distributorItem == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (distributorItem.id == null || distributorItem.id == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            DistributorDao distributorDao = new DistributorDao();
            return distributorDao.updateDistributor(distributorItem);
        }
    }
    public class DistributorParam
    {
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }
    public class PlatformItem
    {
        public string platformId;//渠道商价格类型：1按订单售价计算，2按供货价计算
        public string platformType;//渠道商价格类型：1按订单售价计算，2按供货价计算
    }
    public class DistributorItem
    {
        public string id;
        public string usercode;
        public string username;//用户名称
        public string platformId;//渠道商价格类型：1按订单售价计算，2按供货价计算
        public string platformType;//渠道商价格类型：1按订单售价计算，2按供货价计算
        public string platformCostType;//提点类型：1：进价基础计算，2：售价基础计算
        public string priceType;//平台类型
        public double platformCost;//
    }
}
