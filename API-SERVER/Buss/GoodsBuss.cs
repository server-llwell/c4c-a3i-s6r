﻿using API_SERVER.Common;
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

        #region 商品管理
        /// <summary>
        /// 获取品牌下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetBrand(object param)
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
            List<BrandItem> brandList = new List<BrandItem>();
            if (userType == "1")//供应商 
            {
                brandList = goodsDao.GetBrand(goodsUserParam.userId);
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {
                brandList = goodsDao.GetBrand();
            }

            return brandList;
        }

        /// <summary>
        /// 获取仓库下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetWarehouse(object param)
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

        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 获取单个商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetGoods(object param)
        {
            GoodsSeachParam goodsSeachParam = JsonConvert.DeserializeObject<GoodsSeachParam>(param.ToString());
            if (goodsSeachParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (goodsSeachParam.goodsId == null || goodsSeachParam.goodsId == "")
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

            return goodsDao.GetGoodsById(goodsSeachParam);
        }
        /// <summary>
        /// 修改商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UpdateGoods(object param)
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
            if (goodsSeachParam.goodsId == null || goodsSeachParam.goodsId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }

            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.GetGoodsById(goodsSeachParam);
        }
        #endregion

        #region 商品库存上传
        /// <summary>
        /// 商品库存上传列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UploadList(object param)
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
                goodsUserParam.pageSize = 15;
            }
            if (goodsUserParam.current == 0)
            {
                goodsUserParam.current = 1;
            }
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.getUploadList(goodsUserParam);
        }
        /// <summary>
        /// 上传商品信息--未完成
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UploadGoods(object param)
        {
            FileUploadParam fileUploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (fileUploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fileUploadParam.userId == null || fileUploadParam.userId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.Do_UploadWarehouseGoods(fileUploadParam);
        }
        /// <summary>
        /// 上传商品库存信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UploadWarehouseGoods(object param)
        {
            FileUploadParam fileUploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (fileUploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fileUploadParam.userId == null || fileUploadParam.userId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.Do_UploadWarehouseGoods(fileUploadParam);
        }
        /// <summary>
        /// 上传商品图片zip -未完成
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UploadGoodsZip(object param)
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

            return goodsDao.GetWarehouseList(goodsUserParam);
        }
        #endregion

        #region 仓库列表
        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetWarehouseList(object param)
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

            return goodsDao.GetWarehouseList(goodsUserParam);
        }
        /// <summary>
        /// 新增仓库信息 -未完成
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_AddWarehouse(object param)
        {
            return "";
        }
        /// <summary>
        /// 修改仓库信息 - 未完成
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UpdateWarehouse(object param)
        {
            return "";
        }
        #endregion
    }

    public class GoodsUserParam
    {
        public string userId;
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class FileUploadParam
    {
        public string userId;
        public string byte64;//文件
    }
    public class GoodsSeachParam
    {
        public string userId;//用户名
        public string goodsId;//商品id
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
        public string supplier;//供应商
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
    public class GoodsItem
    {
        public string id;//商品编号
        public string brand;//品牌
        public string goodsName;//商品名称
        public string barcode;//条码
        public string catelog3;//三级分类
        public string slt;//主图
        public string source;//原产地
        public string model;//型号
        public string applicable;//适用人群
        public string formula;//配料成分含量
        public string shelfLife;//保质期
        public string storage;//贮存方式
        public string wname;//仓库
        public string goodsnum;//库存
        public string inprice;//进价
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

    public class UploadItem
    {
        public string id;
        public string username;//上传人
        public string uploadTime;//上传时间
        public string uploadNum;//商品入库数量
        public string statusText;//入库状态
        public string status;//状态
    }
}
