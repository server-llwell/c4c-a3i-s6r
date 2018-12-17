using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Buss
{
    public class PurchaseBuss :IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.PurchaseApi;
        }
        public bool NeedCheckToken()
        {
            return true;
        }

        /// <summary>
        /// 导入询价商品
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_OnLoadGoodsList(object param, string userId)
        {
            OnLoadGoodsListParam fup = JsonConvert.DeserializeObject<OnLoadGoodsListParam>(param.ToString());
            if (fup==null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fup.fileTemp==null || fup.fileTemp=="")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            
            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.OnLoadGoodsList(fup, userId) ;

        }


        /// <summary>
        /// 询价、询价中、待提交状态的商品分页
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_Goodspagination(object param, string userId)
        {
            GoodspaginationParam fup = JsonConvert.DeserializeObject<GoodspaginationParam>(param.ToString());
            if (fup == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fup.purchasesn == null || fup.purchasesn == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (fup.current == 0)
            {
                fup.current = 1;
            }
            if (fup.pageSize == 0)
            {
                fup.pageSize = 10;
            }
            
            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.Goodspagination(fup, userId);

        }

        /// <summary>
        /// 询价商品删除接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_GoodsDelete(object param, string userId)
        {
            GoodsDeleteParam fup = JsonConvert.DeserializeObject<GoodsDeleteParam>(param.ToString());
            if (fup == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fup.purchasesn == null || fup.purchasesn == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (fup.barcode == null || fup.barcode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            
            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.GoodsDelete(fup, userId);

        }

        /// <summary>
        /// 询价表保存接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_InquiryPreservation(object param, string userId)
        {
            InquiryPreservationParam fup = JsonConvert.DeserializeObject<InquiryPreservationParam>(param.ToString());
            if (fup == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fup.remark == null || fup.remark == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (fup.contacts == null || fup.contacts == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (fup.deliveryTime == null || fup.deliveryTime == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (fup.sex == null || fup.sex == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (fup.tel == null || fup.tel == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (fup.sendType == null || fup.sendType == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            
            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.InquiryPreservation(fup, userId);

        }

        /// <summary>
        /// 询价表提交接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_InquirySubmission(object param, string userId)
        {
            InquiryPreservationParam fup = JsonConvert.DeserializeObject<InquiryPreservationParam>(param.ToString());
            if (fup == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fup.remark == null || fup.remark == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (fup.contacts == null || fup.contacts == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (fup.deliveryTime == null || fup.deliveryTime == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (fup.sex == null || fup.sex == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (fup.tel == null || fup.tel == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (fup.sendType == null || fup.sendType == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }

            
            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.InquirySubmission(fup, userId);
        }

        /// <summary>
        /// 询价列表接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_InquiryList(object param, string userId)
        {
            InquiryListParam  ilp = JsonConvert.DeserializeObject<InquiryListParam>(param.ToString());
            if (ilp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (ilp.current == 0)
            {
                ilp.current = 1;
            }
            if (ilp.pageSize == 0)
            {
                ilp.pageSize = 10;
            }
            
            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.InquiryList(ilp, userId);
        }

        /// <summary>
        /// 询价列表查看接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_InquiryListDetailed(object param, string userId)
        {
            GoodsDeleteParam gdp = JsonConvert.DeserializeObject<GoodsDeleteParam>(param.ToString());
            if (gdp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (gdp.purchasesn == null || gdp.purchasesn=="")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (gdp.status == null || gdp.status == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
           
            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.InquiryListDetailed(gdp, userId);
        }

        /// <summary>
        /// 表单删除接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_InquiryListDelete(object param, string userId)
        {
            GoodsDeleteParam gdp = JsonConvert.DeserializeObject<GoodsDeleteParam>(param.ToString());
            if (gdp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (gdp.purchasesn == null || gdp.purchasesn == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }

            
            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.InquiryListDelete(gdp, userId);
        }

        /// <summary>
        /// 已报价、报价中、已报价（二次）、已完成状态的商品分页
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_InquiryPagesn(object param, string userId)
        {
            GoodspaginationParam fup = JsonConvert.DeserializeObject<GoodspaginationParam>(param.ToString());
            if (fup == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fup.purchasesn == null || fup.purchasesn == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            
            if (fup.current == 0)
            {
                fup.current = 1;
            }
            if (fup.pageSize == 0)
            {
                fup.pageSize = 10;
            }
           
            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.InquiryPagesn(fup, userId);

        }


    }

    


    public class InquiryListDetailedItem
    {
        public string sendType;//提货地点
        public string contacts;//姓名
        public string sex;//性别
        public string tel;//电话
        public string deliveryTime;//截止日期
        public string remark;//描述
        public string tax;//税
        public string waybillfee;//运费
        public string purchasePrice;//采购总金额
     
    }

    public class InquiryListParam
    {
        public string[] date;//时间区间
        public string select;//查询条件
        public string status;//状态
        public int current;//多少页
        public int pageSize;//页面显示多少个表单
    }

    public class InquiryListItem
    {
        public string keyId;//序号
        public string purchasesn;//询价单号
        public string createtime;//询价日期
        public string remark;//询价单描述
        public string status;//询价单状态

    }

    public class InquiryPreservationParam
    {
        public string purchasesn;//询价单号
        public string sendType;//期货方式
        public string contacts;//联系人姓名
        public string sex;//性别
        public string tel;//联系电话
        public string deliveryTime;//采购截止日期
        public string remark;//询价单描述
    }

    

    public class GoodsDeleteParam
    {
        public string  purchasesn;//询价单号
        public string status;//状态
        public string barcode;//商品条码

    }

    public class GoodspaginationParam
    {
        public string purchasesn;//询价单号
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class OnLoadGoodsListParam
    {
        public string purchasesn;//询价单号
        public string fileTemp;//文件
    }

    public class OnLoadGoodsListItem
    {
        public string keyId;//序号
        public string purchasesn;//询价单号
        public string goodsName;//商品名
        public string barcode;//商品条码
        public string brand;//品牌
        public string total;//询价数量
        public string  availableNum;//可供数量
        public string   supplyPrice;//供货单价
        public string   purchaseNum;//采购数量
        public string  totalPrice;//总金额
    }


}
