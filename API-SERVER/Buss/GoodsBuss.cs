using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Buss
{
    public class GoodsBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.GoodsApi;
        }
        
        public object Do_GetBrand(object param)
        {
            GoodsUserParam goodsUserParam = JsonConvert.DeserializeObject<GoodsUserParam>(param.ToString());
            if (goodsUserParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (goodsUserParam.userId==null|| goodsUserParam.userId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(goodsUserParam.userId);
            GoodsDao goodsDao = new GoodsDao();
            List<BrandItem> brandList = new List<BrandItem>();
            if (userType=="1")//供应商 
            {
                brandList = goodsDao.GetBrand(goodsUserParam.userId);
            }
            else if (userType == "0"|| userType == "5")//管理员或客服
            {
                brandList = goodsDao.GetBrand();
            }
            
            return brandList;
        }
    }
    
    public class GoodsUserParam
    {
        public string userId;
    }

    public class BrandItem
    {
        public string brand;//品牌名
    }
    public class WarehouseItem
    {
        public string wid;//仓库编号
        public string wcode;//仓库编码
        public string wname;//仓库名
    }
}
