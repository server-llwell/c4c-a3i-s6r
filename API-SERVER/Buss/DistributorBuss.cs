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
        #region 渠道商费用
        /// <summary>
        /// 获取渠道商类型下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetPlatform(object param,string userId)
        {
            DistributorDao distributorDao = new DistributorDao();
            return distributorDao.getPlatform();
        }

        /// <summary>
        /// 获取渠道商费用列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_DistributorList(object param,string userId)
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

        /// <summary>
        /// 修改渠道商费用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UpdateDistributor(object param,string userId)
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
        #endregion

        #region 渠道商商品
        /// <summary>
        /// 获取渠道商商品列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_DGoodsList(object param, string userId)
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
            return distributorDao.getDGoodsList(distributorParam);
        }
        /// <summary>
        /// 修改渠道商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UpdateDGoods(object param, string userId)
        {
            DistributorGoodsItem distributorGoodsItem = JsonConvert.DeserializeObject<DistributorGoodsItem>(param.ToString());
            if (distributorGoodsItem == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (distributorGoodsItem.id == null || distributorGoodsItem.id == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            DistributorDao distributorDao = new DistributorDao();
            return distributorDao.updateDGoods(distributorGoodsItem);
        }

        /// <summary>
        /// 上传渠道商品信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UploadDGoods(object param, string userId)
        {
            FileUploadParam uploadParam = JsonConvert.DeserializeObject<FileUploadParam>(param.ToString());
            if (uploadParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (uploadParam.fileTemp == null || uploadParam.fileTemp == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            DistributorDao distributorDao = new DistributorDao();
            userId = "admin";
            uploadParam.userId = userId;
            return distributorDao.uploadDGoods(uploadParam);
        }

        #endregion
    }
    public class DistributorParam
    {
        public string purchase;//渠道商账号
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }
    public class PlatformItem
    {
        public string platformId;//平台类型id
        public string platformType;//平台类型name
    }
    public class DistributorItem
    {
        public string id;
        public string usercode;
        public string username;//用户名称
        public string platformId;//平台类型id
        public string platformType;//平台类型name
        public string platformCostType;//提点类型：1：进价基础计算，2：售价基础计算
        public string priceType;//渠道商价格类型：1按订单售价计算，2按供货价计算
        public double platformCost;//提点费用
    }

    public class DistributorGoodsItem
    {
        public string id;//序号
        public string usercode;//渠道商id
        public string purchase;//渠道商名称
        public string goodsid;//商品id
        public string barcode;//商品条码
        public string goodsName;//商品名称
        public string slt;//商品缩略图
        public string platformId;//采购类型id
        public string platformType;//采购类型id
        public double pprice;//采购单价
        public double pNum;//采购数量
        public string supplierid;//默认供应商id
        public string suppliername;//默认供应商
        public double profitPlatform;//利润分成百分比（平台）
        public double profitAgent;//利润分成百分比（代理）
        public double profitDealer;//利润分成百分比（分销商）
        public double profitOther1;//利润分成百分比（其他1）
        public string profitOther1Name;//其他1命名
        public double profitOther2;//利润分成百分比（其他2）
        public string profitOther2Name;//其他2命名
        public double profitOther3;//利润分成百分比（其他3）
        public string profitOther3Name;//其他3命名
    }
}
