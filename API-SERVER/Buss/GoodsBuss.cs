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

        public object Do_GetWareHouse(object param)
        {
            GoodsUserParam goodsUserParam = JsonConvert.DeserializeObject<GoodsUserParam>(param.ToString());
            if (goodsUserParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (goodsUserParam.userId == null || goodsUserParam.userId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(goodsUserParam.userId);
            GoodsDao goodsDao = new GoodsDao();
            List<WarehouseItem> whList = new List<WarehouseItem>();
            if (userType == "1")//供应商 
            {
                whList = goodsDao.GetWarehouse(goodsUserParam.userId);
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {
                whList = goodsDao.GetWarehouse();
            }

            return whList;
        }
        public object Do_GetGoodsList(object param)
        {
            GoodsSeachParam goodsSeachParam = JsonConvert.DeserializeObject<GoodsSeachParam>(param.ToString());
            if (goodsSeachParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (goodsSeachParam.userId == null || goodsSeachParam.userId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (goodsSeachParam.pageSize == 0)
            {
                goodsSeachParam.pageSize = 20;
            }
            if (goodsSeachParam.current == 0)
            {
                goodsSeachParam.current = 1;
            }

            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.GetGoodsList(goodsSeachParam);
        }

        public object Do_GetWareHouseList(object param)
        {
            GoodsUserParam goodsUserParam = JsonConvert.DeserializeObject<GoodsUserParam>(param.ToString());
            if (goodsUserParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (goodsUserParam.userId == null || goodsUserParam.userId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (goodsUserParam.pageSize == 0)
            {
                goodsUserParam.pageSize = 100;
            }
            if (goodsUserParam.current == 0)
            {
                goodsUserParam.current = 1;
            }
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.GetWareHouseList(goodsUserParam);
        }
    }

    public class GoodsUserParam
    {
        public string userId;
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class GoodsSeachParam
    {
        public string userId;//用户名
        public string status;//状态
        public string wid;//仓库编号
        public string goodsName;//商品名称
        public string brand;//品牌
        public string barcode;//条码
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class GoodsListItem
    {
        public string id;//商品编号
        public string brand;//品牌
        public string goodsName;//商品名称
        public string barcode;//条码
        public string slt;//主图
        public string source;//原产地
        public string wname;//仓库
        public string goodsnum;//库存
        public string flag;//是否上架0下架，1上架
        public int week;//周销量
        public int month;//月销量
        public string status;//状态：0正常，1申请中，2已驳回
    }
    public class BrandItem
    {
        public string brand;//品牌名
    }
    public class WarehouseItem
    {
        public string wid;//仓库编号
        public string wcode;//仓库code
        public string wname;//仓库名
        public string supplierid;//供应商id标记是那个供应商的仓库
        public string taxation;//税率
        public string taxation2;//税率2
        public string taxation2type;//税率2提档线类别：1，按总价提档，2，按元/克提档
        public string taxation2line;//税率2提档线
        public string freight;//运费
        public string orderCode;//在订单号末尾添加的字符
        public string if_send;//是否需要供应商填写运单号0不用，1需要
        public string if_CK;//是否是仓库业务的仓库
    }
}
