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

        #region 商品管理
        /// <summary>
        /// 获取品牌下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetBrand(object param,string userId)
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
        public object Do_GetWarehouse(object param,string userId)
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
        public object Do_GetGoodsList(object param,string userId)
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
                goodsSeachParam.pageSize = 10;
            }
            if (goodsSeachParam.current == 0)
            {
                goodsSeachParam.current = 1;
            }
            GoodsDao goodsDao = new GoodsDao();
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(goodsSeachParam.userId);
            if (userType == "1")//供应商 
            {
                return goodsDao.GetGoodsListForSupplier(goodsSeachParam);
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {
                return goodsDao.GetGoodsListForOperator(goodsSeachParam);
            }
            else if (userType == "2" || userType == "3")//代理或渠道商
            {
                return goodsDao.GetGoodsListForAgent(goodsSeachParam);
            }
            else
            {
                PageResult goodsResult = new PageResult();
                goodsResult.pagination = new Page(goodsSeachParam.current, goodsSeachParam.pageSize);
                return goodsResult;
            }
        }
        /// <summary>
        /// 获取单个商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetGoods(object param,string userId)
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

            GoodsDao goodsDao = new GoodsDao();
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(goodsSeachParam.userId);
            if (userType == "1")//供应商 
            {
                return goodsDao.GetGoodsById(goodsSeachParam);
            }
            else if (userType == "0" || userType == "5")//管理员或运营
            {
                return goodsDao.GetGoodsByIdForOperator(goodsSeachParam);
            }
            else if (userType == "2" || userType == "3")//代理或渠道商
            {
                return goodsDao.GetGoodsByIdForAgent(goodsSeachParam);
            }
            else
            {
                PageResult goodsResult = new PageResult();
                goodsResult.pagination = new Page(goodsSeachParam.current, goodsSeachParam.pageSize);
                return goodsResult;
            }
        }
        /// <summary>
        /// 修改商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UpdateGoods(object param, string userId)
        {
            //GoodsSeachParam goodsSeachParam = JsonConvert.DeserializeObject<GoodsSeachParam>(param.ToString());
            //if (goodsSeachParam == null)
            //{
            //    throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            //}
            //if (goodsSeachParam.userId == null || goodsSeachParam.userId == "")
            //{
            //    throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            //}
            //if (goodsSeachParam.goodsId == null || goodsSeachParam.goodsId == "")
            //{
            //    throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            //}

            //GoodsDao goodsDao = new GoodsDao();

            //return goodsDao.GetGoodsById(goodsSeachParam);
            return "";
        }
        /// <summary>
        /// 获取单个商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetGoodsForOperator(object param, string userId)
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
                goodsSeachParam.pageSize = 10;
            }
            if (goodsSeachParam.current == 0)
            {
                goodsSeachParam.current = 1;
            }

            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.GetGoodsByIdForOperator(goodsSeachParam);
        }
        #endregion

        #region 商品库存上传
        /// <summary>
        /// 商品库存上传列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UploadList(object param,string userId)
        {
            GoodsUserParam goodsUserParam = JsonConvert.DeserializeObject<GoodsUserParam>(param.ToString());
            if (goodsUserParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (goodsUserParam.pageSize == 0)
            {
                goodsUserParam.pageSize = 10;
            }
            if (goodsUserParam.current == 0)
            {
                goodsUserParam.current = 1;
            }
            GoodsDao goodsDao = new GoodsDao();
            goodsUserParam.userId = userId;
            return goodsDao.getUploadList(goodsUserParam);
        }
        /// <summary>
        /// 查询补充信息的接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetUploadStatus(object param,string userId)
        {
            FileUploadParam fileUploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (fileUploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fileUploadParam.logId == null || fileUploadParam.logId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.getUploadStatus(fileUploadParam);
        }
        /// <summary>
        /// 查询补充信息的接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetUploadStatusOne(object param,string userId)
        {
            FileUploadParam fileUploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (fileUploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fileUploadParam.logId == null || fileUploadParam.logId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.getUploadStatusOne(fileUploadParam);
        }
        /// <summary>
        /// 查询等待审批信息的接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetUploadStatusTwo(object param,string userId)
        {
            FileUploadParam fileUploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (fileUploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fileUploadParam.logId == null || fileUploadParam.logId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.getUploadStatusTwo(fileUploadParam);
        }
        /// <summary>
        /// 查询审批完成信息的接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetUploadStatusThree(object param,string userId)
        {
            FileUploadParam fileUploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (fileUploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fileUploadParam.logId == null || fileUploadParam.logId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.getUploadStatusThree(fileUploadParam);
        }
        /// <summary>
        /// 查询审批完成信息的接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetUploadStatusFour(object param,string userId)
        {
            FileUploadParam fileUploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (fileUploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fileUploadParam.logId == null || fileUploadParam.logId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.getUploadStatusFour(fileUploadParam);
        }
        /// <summary>
        /// 上传商品库存信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UploadWarehouseGoods(object param,string userId)
        {
            FileUploadParam fileUploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (fileUploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fileUploadParam.fileTemp == null || fileUploadParam.fileTemp == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            fileUploadParam.userId = userId;
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.UploadWarehouseGoods(fileUploadParam);
        }
        /// <summary>
        /// 上传商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UploadGoods(object param,string userId)
        {
            FileUploadParam fileUploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (fileUploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fileUploadParam.logId == null || fileUploadParam.logId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            fileUploadParam.userId = userId;
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.UploadGoods(fileUploadParam);
        }
        /// <summary>
        /// 商品上架详情
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_WarehouseGoodsList(object param, string userId)
        {
            FileUploadParam fileUploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (fileUploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fileUploadParam.logId == null || fileUploadParam.logId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.getWarehouseGoodsList(fileUploadParam);
        }
        /// <summary>
        /// 商品上架详情
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_ExamineWarehouseGoods(object param, string userId)
        {
            ExamineParam examineParam = JsonConvert.DeserializeObject<ExamineParam>(param.ToString());
            if (examineParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (examineParam.logId == null || examineParam.logId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.examineWarehouseGood(examineParam);
        }
        #endregion

        #region 仓库列表

        /// <summary>
        /// 供应商下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetSupplier(object param,string userId)
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
            List<SupplierItem> ls = new List<SupplierItem>();
            if (userType == "0" || userType == "5")//管理员或客服
            {
                ls = goodsDao.getSupplier(goodsUserParam.userId); 
            }
            return ls;
        }
        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetWarehouseList(object param,string userId)
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
                goodsUserParam.pageSize = 10;
            }
            if (goodsUserParam.current == 0)
            {
                goodsUserParam.current = 1;
            }
            GoodsDao goodsDao = new GoodsDao();

            return goodsDao.GetWarehouseList(goodsUserParam);
        }
        ///// <summary>
        ///// 新增仓库信息 -未完成
        ///// </summary>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //public object Do_AddWarehouse(object param,string userId)
        //{
        //    return new MsgResult();
        //}
        /// <summary>
        /// 修改仓库信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UpdateWarehouse(object param,string userId)
        {
            WarehouseItem warehouseItem = JsonConvert.DeserializeObject<WarehouseItem>(param.ToString());
            if (warehouseItem == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (warehouseItem.wname == null || warehouseItem.wname == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (warehouseItem.supplierId == null || warehouseItem.supplierId == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (warehouseItem.taxation == null || warehouseItem.taxation == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (warehouseItem.taxation2 == null || warehouseItem.taxation2 == "")
            {
                warehouseItem.taxation2 = "0";
            }
            if (warehouseItem.taxation2type == null || warehouseItem.taxation2type == "")
            {
                warehouseItem.taxation2type = "0";
            }
            if (warehouseItem.taxation2line == null || warehouseItem.taxation2line == "")
            {
                warehouseItem.taxation2line = "0";
            }
            if (warehouseItem.freight == null || warehouseItem.freight == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            GoodsDao goodsDao = new GoodsDao();

            if (warehouseItem.wid == null|| warehouseItem.wid =="")
            {
                return goodsDao.AddWareHouse(warehouseItem);
            }
            else
            {
                return goodsDao.UpdateWareHouse(warehouseItem);
            }
        }
        /// <summary>
        /// 删除仓库信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_DeleteWarehouse(object param,string userId)
        {
            WarehouseItem warehouseItem = JsonConvert.DeserializeObject<WarehouseItem>(param.ToString());
            if (warehouseItem == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (warehouseItem.wid == null || warehouseItem.wid == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            GoodsDao goodsDao = new GoodsDao();
            
            return goodsDao.DelWareHouse(warehouseItem);
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
        public string logId;
        public string fileTemp;
        public string fileTemp1;
        public string byte64;//文件
        public string byte64Zip;//文件
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
    public class ExamineParam
    {
        public string logId;//记录id
        public string[] logGoodsId;//记录信息
        public string logText;//审批信息
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
        public string price;//供货价
        public string flag;//是否上架0下架，1上架
        public int week;//周销量
        public int month;//月销量
        public string selPrice;//默认供货价
        public string selSupplier;//默认供货商
        public string selGoodsNum;//默认库存
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
        public List<GoodsSelectSupplier> goodsSelectSupplierList;
    }
    public class GoodsSelectSupplier
    {
        public string id;//id修改时传回
        public string ifSel;//是否是默认选中1是默认选中 0 不选中
        public string supplierName;//供应商
        public string wname;//仓库
        public string goodsnum;//库存
        public string inprice;//供货价
    }
    public class BrandItem
    {
        public string brand;//品牌名
    }
    public class SupplierItem
    {
        public string supplierId;//供应商id
        public string supplier;//供应商名称
    }
    public class WarehouseItem
    {
        public string wid;//仓库编号
        public string wcode;//仓库code
        public string wname;//仓库名
        public string supplierId;//供应商id标记是那个供应商的仓库
        public string supplier;//供应商名称
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
    public class UploadLogItem
    {
        public string id;//记录id
        public string log;//记录信息
        public string url;//文档下载地址
        public string status;//状态
    }
    public class UploadMsgItem
    {
        public string msg=""; //提示信息
        public string type="0";//标志：0失败，1成功
        public string id;//logid
        public string status;//状态
    }
    public class WarehouseGoodsListItem
    {
        public string logId;//供应商
        public string username;//供应商
        public string goodsUrl;//商品信息文件下载地址
        public string goodsImgUrl;//商品图片文件下载地址
        public List<WarehouseGoodsItem> warehouseGoodsList;
    }
    public class WarehouseGoodsItem
    {
        public string id;
        public string barcode;//商品条码
        public string goodsName;//商品名称
        public string wname;//仓库名称
        public double inprice;//供货价
        public double goodsnum;//供货数量
        public string status;//审核状态
    }
}
