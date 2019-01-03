using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Buss
{
    public class PurchaseBuss : IBuss
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
            if (fup == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fup.fileTemp == null || fup.fileTemp == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }

            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.OnLoadGoodsList(fup, userId);

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
            if (fup.purchasesn==null || fup.purchasesn == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
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
            InquiryListParam ilp = JsonConvert.DeserializeObject<InquiryListParam>(param.ToString());
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
            GoodspaginationParam gdp = JsonConvert.DeserializeObject<GoodspaginationParam>(param.ToString());
            if (gdp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (gdp.purchasesn == null || gdp.purchasesn == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (gdp.status == null || gdp.status == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (gdp.current == 0)
            {
                gdp.current = 1;
            }
            if (gdp.pageSize == 0)
            {
                gdp.pageSize = 10;
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
        /// 已报价状态的商品分页
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

        /// <summary>
        /// 已报价商品详情接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_GoodsDetails(object param, string userId)
        {
            GoodsDetailsParam fup = JsonConvert.DeserializeObject<GoodsDetailsParam>(param.ToString());
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
            return purchaseDao.GoodsDetails(fup, userId);

        }


        /// <summary>
        /// 已报价商品详情确定接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_GoodsDetailsDetermine(object param, string userId)
        {
            GoodsDetailsDetermineParam goodsDetailsDetermineParam = JsonConvert.DeserializeObject<GoodsDetailsDetermineParam>(param.ToString());
            if (goodsDetailsDetermineParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (goodsDetailsDetermineParam.purchasesn == null || goodsDetailsDetermineParam.purchasesn == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (goodsDetailsDetermineParam.barcode == null || goodsDetailsDetermineParam.barcode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (goodsDetailsDetermineParam.list == null )
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            for (int i=0;i< goodsDetailsDetermineParam.list.Count;i++)
            {
                if (goodsDetailsDetermineParam.list[i].id=="" || goodsDetailsDetermineParam.list[i].id == null)
                {
                    throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
                }
                if (goodsDetailsDetermineParam.list[i].demand == "" || goodsDetailsDetermineParam.list[i].demand == null)
                {
                    throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
                }
                if (goodsDetailsDetermineParam.list[i].price == "" || goodsDetailsDetermineParam.list[i].price == null)
                {
                    throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
                }
            }
            
            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.GoodsDetailsDetermine(goodsDetailsDetermineParam, userId);
        }


        /// <summary>
        /// 已报价商品确定接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_GoodsDetermine(object param, string userId)
        {
            GoodsDetailsDetermineParam goodsDetailsDetermineParam = JsonConvert.DeserializeObject<GoodsDetailsDetermineParam>(param.ToString());
            if (goodsDetailsDetermineParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (goodsDetailsDetermineParam.purchasesn == null || goodsDetailsDetermineParam.purchasesn == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (goodsDetailsDetermineParam.list == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            for (int i = 0; i < goodsDetailsDetermineParam.list.Count; i++)
            {              
                if (goodsDetailsDetermineParam.list[i].demand == "" || goodsDetailsDetermineParam.list[i].demand == null)
                {
                    throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
                }
                if (goodsDetailsDetermineParam.list[i].price == "" || goodsDetailsDetermineParam.list[i].price == null)
                {
                    throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
                }
            }

            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.GoodsDetailsDetermine(goodsDetailsDetermineParam, userId);
        }



        /// <summary>
        /// 已报价商品删除接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_InquiryGoodsDelete(object param, string userId)
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
            return purchaseDao.InquiryGoodsDelete(fup, userId);

        }

        /// <summary>
        /// 已报价表取消接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_OfferCancel(object param, string userId)
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

            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.OfferCancel(fup, userId);

        }

        /// <summary>
        /// 已报价表提交接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_OfferSub(object param, string userId)
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

            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.OfferSub(fup, userId);

        }

        /// <summary>
        /// 报价中、已报价（二次）、已完成状态的商品分页
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_OtherInquiryPagesn(object param, string userId)
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
            return purchaseDao.OtherInquiryPagesn(fup, userId);

        }

        /// <summary>
        /// 报价中、已报价（二次）、已完成状态的商品详情接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_OtherGoodsDetails(object param, string userId)
        {
            GoodsDetailsParam fup = JsonConvert.DeserializeObject<GoodsDetailsParam>(param.ToString());
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
            return purchaseDao.OtherGoodsDetails(fup, userId);

        }

        /// <summary>
        /// 待提交导入询价商品
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_SubOnLoadGoodsList(object param, string userId)
        {
            OnLoadGoodsListParam fup = JsonConvert.DeserializeObject<OnLoadGoodsListParam>(param.ToString());
            if (fup == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (fup.fileTemp == null || fup.fileTemp == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (fup.purchasesn == null || fup.purchasesn == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }

            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.OnLoadGoodsList(fup, userId);

        }


        /// <summary>
        /// 已报价（二次）提交接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_OfferedSub(object param, string userId)
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
            if (fup.status == null || fup.status == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }

            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.OfferedSub(fup, userId);

        }

        /// <summary>
        /// 采购列表接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_PurchaseList(object param, string userId)
        {
            InquiryListParam ilp = JsonConvert.DeserializeObject<InquiryListParam>(param.ToString());
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
            return purchaseDao.PurchaseList(ilp, userId);
        }


        /// <summary>
        /// 查看采购接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_PurchaseDetails(object param, string userId)
        {
            GoodspaginationParam gdp = JsonConvert.DeserializeObject<GoodspaginationParam>(param.ToString());
            if (gdp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (gdp.purchasesn == null || gdp.purchasesn == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (gdp.stage == null || gdp.stage == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (gdp.current == 0)
            {
                gdp.current = 1;
            }
            if (gdp.pageSize == 0)
            {
                gdp.pageSize = 10;
            }
            
            PurchaseDao purchaseDao = new PurchaseDao();
            return purchaseDao.PurchaseDetails(gdp, userId);
        }

    }

    public class GoodsDetailsParam
    {
        public string purchasesn;//询价单号
        public string barcode;//商品条码
    }


    public class PurchaseListItem
    {
        public string keyId;//序号
        public string purchasesn;//采购单号
        public string purchaseTime;//采购日期
        public string num;//数量
        public string money;//金额
        public string stage;//状态
    }

    public class GoodsDetailsDetermineItem
    {
        public string purchasesn;//询价单号
        public string barcode;//商品条码
        public string supplyPrice;//供货单价
        public string purchaseNum;//采购数量
        public string totalPrice;//采购金额
        public double allPrice;//总金额

    }

    public class GoodsDetailsDetermineParam
    {
        public string purchasesn;//询价单号
        public string barcode;//商品条码
        public  List<GoodsDetailsDetermineList> list=new List<GoodsDetailsDetermineList>();

    }

    public class GoodsDetailsDetermineList
    {
        public string id;//供货商品编号
        public string demand;//采购数量
        public string  price;//供货单价
    }


    public class GoodsDetailsItem
    {
        public string keyId;//序号
        public string id;//供货商品编号
        public string supplyId;//供货商编号
        public string minOfferNum;//可供最小数量
        public string maxOfferNum;//可供最大数量
        public string offerPrice;//单价
        public string demand;//采购数量
        public string purchaseAmount;//采购金额
    }


    public class InquiryListDetailedItem
    {
        public string sendType;//1日本贸易，2韩国贸易，3香港贸易，6国内贸易
        public string status;//1：询价中,2：已报价,3：报价中,4：已报价(二次),5：已完成,6：已关闭,7：待提交
        public string stage;//采购状态
        public string typeName;//提货地点
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
        public string stage;//采购状态
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
        public string status;//询价状态
        public string stage;//采购状态
        public string barcode;//商品条码

    }

    public class GoodspaginationParam
    {
        public string purchasesn;//询价单号
        public string status;//询价状态
        public string stage;//采购状态
        public string barcode;//商品条码
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
        public string maxAvailableNum;//最大可供数量
        public string minAvailableNum;//最小可供数量
        public string supplyPrice;//供货单价
        public string purchaseNum="0";//采购数量
        public string totalPrice="0";//总金额
        public string supplierNumType;//0无供货，1一个供货，2多个供货
    }


}
