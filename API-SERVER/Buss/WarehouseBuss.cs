using API_SERVER.Common;
using Newtonsoft.Json;
using API_SERVER.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Buss
{
    public class WarehouseBuss :IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.WarehouseApi;
        }

        public bool NeedCheckToken()
        {
            return true;
        }

        /// <summary>
        /// 获取收货订单-代销接口
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_CollectGoods(object param, string userId)
        {
            CollectGoodsIn cgi = JsonConvert.DeserializeObject<CollectGoodsIn>(param.ToString());
            if (cgi.pageSize == 0)
            {
                cgi.pageSize = 10;
            }
            if (cgi.current == 0)
            {
                cgi.current = 1;
            }
            userId = "cgs";
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.CollectGoods(cgi, userId);
        }
        /// <summary>
        /// 获取收货订单详情-代销接口
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object Do_CollectGoodsList(object param, string userId)
        {
            CollectGoodsListIn cgi = JsonConvert.DeserializeObject<CollectGoodsListIn>(param.ToString());
            if (cgi.pageSize == 0)
            {
                cgi.pageSize = 10;
            }
            if (cgi.current == 0)
            {
                cgi.current = 1;
            }
            userId = "cgs";
            WarehouseDao warehouseDao = new WarehouseDao();
            return warehouseDao.CollectGoodsList(cgi, userId);
        }

        public class CollectGoodsIn
        {
            public string[] date;//日期区间
            public string sendType;//订单类型
            public string status;//状态
            public int current;//多少页
            public int pageSize;//页面显示多少个商品

        }

        public class CollectGoodsItem
        {
            public string keyId;//序号
            public string sendType;//订单类型
            public string goodsTotal;//发货数量
            public string sendTime;//发货日期
            public string sendName;//发货人
            public string sendTel;//联系人电话
            public string status;//状态
        }

        public class CollectGoodsListIn
        {
            public string sendid;//订单号
            public int current;//多少页
            public int pageSize;//页面显示多少个商品

        }
        public class CollectGoodsListSum
        {
            public double money=0;//合计
        }

        public class CollectGoodsListItem
        {
            public string keyId;//序号
            public string barcode;//商品条码
            public string slt;//商品图片地址
            public string goodsName;//商品名称
            public string brand;//品牌
            public string supplyPrice;//供货价
            public string goodsNum;//商品数量
            public string goodsTotal;//总金额
            
        }
    }

}

