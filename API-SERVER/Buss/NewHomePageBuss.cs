using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;

namespace API_SERVER.Buss
{
    public class NewHomePageBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.NewHomePageApi;
        }
        public bool NeedCheckToken()
        {
            return true;
        }

        /// <summary>
        /// 总分类接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_AllClassification(object param, string userId)
        {
            NewHomePageDao newHomePageDao = new NewHomePageDao();
            return newHomePageDao.AllClassification();
        }

        /// <summary>
        /// 首页上半部接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_HomePage(object param, string userId)
        {
            HomePageParam homePageParam = JsonConvert.DeserializeObject< HomePageParam >(param.ToString());
            if (homePageParam==null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            NewHomePageDao newHomePageDao = new NewHomePageDao();
            return newHomePageDao.HomePage(userId);

        }

        /// <summary>
        /// 首页下半部接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_HomePageDownPart(object param, string userId)
        {
            HomePageParam homePageParam = JsonConvert.DeserializeObject<HomePageParam>(param.ToString());
            if (homePageParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (homePageParam.pageSize == 0)
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (homePageParam.page == 0)
            {
                homePageParam.page =0;
            }
            NewHomePageDao newHomePageDao = new NewHomePageDao();
            return newHomePageDao.HomePageDownPart(homePageParam,userId);

        }


        /// <summary>
        /// 首页各馆换一批接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_HomePageChangeGoods(object param, string userId)
        {
            HomePageParam homePageParam = JsonConvert.DeserializeObject<HomePageParam>(param.ToString());
            if (homePageParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (homePageParam.country == null || homePageParam.country == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (homePageParam.page.ToString() ==null || homePageParam.page.ToString() =="")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            NewHomePageDao newHomePageDao = new NewHomePageDao();
            return newHomePageDao.HomePageChangeGoods(homePageParam, userId);

        }

        /// <summary>
        /// 品类页接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_CategoryGoods(object param, string userId)
        {
            CategoryGoodsParam categoryGoodsParam = JsonConvert.DeserializeObject<CategoryGoodsParam>(param.ToString());
            if (categoryGoodsParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (categoryGoodsParam.pageSize == 0)
            {
                categoryGoodsParam.pageSize = 40;
            }
            if (categoryGoodsParam.current == 0)
            {
                categoryGoodsParam.current = 1;
            }
            NewHomePageDao newHomePageDao = new NewHomePageDao();
            return newHomePageDao.CategoryGoods(categoryGoodsParam, userId);
        }

        /// <summary>
        /// 搜索页接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_SelectGoods(object param, string userId)
        {
            CategoryGoodsParam categoryGoodsParam = JsonConvert.DeserializeObject<CategoryGoodsParam>(param.ToString());
            if (categoryGoodsParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (categoryGoodsParam.pageSize == 0)
            {
                categoryGoodsParam.pageSize = 40;
            }
            if (categoryGoodsParam.current == 0)
            {
                categoryGoodsParam.current = 1;
            }
            NewHomePageDao newHomePageDao = new NewHomePageDao();
            return newHomePageDao.SelectGoods(categoryGoodsParam, userId);
        }

        /// <summary>
        /// 国家馆接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_CountryGoods(object param, string userId)
        {
            HomePageParam homePageParam = JsonConvert.DeserializeObject<HomePageParam>(param.ToString());
            if (homePageParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (homePageParam.country == null || homePageParam.country == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
           
            NewHomePageDao newHomePageDao = new NewHomePageDao();
            return newHomePageDao.CountryGoods(homePageParam, userId);

        }

        /// <summary>
        /// 品牌页接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_BrandsGoods(object param, string userId)
        {
            Brands brands = JsonConvert.DeserializeObject<Brands>(param.ToString());
            if (brands==null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (brands.brandsName==null )
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (brands.pageSize == 0)
            {
                brands.pageSize = 40;
            }
            if (brands.current == 0)
            {
                brands.current = 1;
            }
            NewHomePageDao newHomePageDao = new NewHomePageDao();
            return newHomePageDao.BrandsGoods(brands, userId);
        }

        /// <summary>
        /// 商品详情页接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_GoodsDetails(object param, string userId)
        {
            NewGoodsParam goodsParam = JsonConvert.DeserializeObject<NewGoodsParam>(param.ToString());
            if (goodsParam==null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (goodsParam.barcode==null || goodsParam.barcode =="")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            NewHomePageDao newHomePageDao = new NewHomePageDao();
            return newHomePageDao.GoodsDetails(goodsParam, userId);
        }

        /// <summary>
        /// 添加取消收藏接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_UserCollection(object param, string userId)
        {
            UserCollectionParam userCollectionParam = JsonConvert.DeserializeObject<UserCollectionParam>(param.ToString());
            if (userCollectionParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (userCollectionParam.collectionType==null || userCollectionParam.collectionType == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (userCollectionParam.collectionValue == null || userCollectionParam.collectionValue == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            NewHomePageDao newHomePageDao = new NewHomePageDao();
            return newHomePageDao.UserCollection(userCollectionParam, userId);
        }

        /// <summary>
        /// 收藏商品接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_UserCollectionGoods(object param, string userId)
        {
            
            if (userId == null || userId == "" || userId == "undefined")
            {
                throw new ApiException(CodeMessage.InvalidParam, "InterfaceValueError");
            }
            UserCollectionGoodsParam userCollectionGoodsParam = JsonConvert.DeserializeObject<UserCollectionGoodsParam>(param.ToString());             
            if (userCollectionGoodsParam.pageSize == 0)
            {
                userCollectionGoodsParam.pageSize = 40;
            }
            if (userCollectionGoodsParam.current == 0)
            {
                userCollectionGoodsParam.current = 1;
            }         
            NewHomePageDao newHomePageDao = new NewHomePageDao();
            return  newHomePageDao.UserCollectionGoods(userCollectionGoodsParam,userId);           
        }

        /// <summary>
        /// 关注品牌展示接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_UserCollectionBrands(object param, string userId)
        {
            
            if (userId == null || userId == "" || userId == "undefined")
            {
                throw new ApiException(CodeMessage.InvalidParam, "InterfaceValueError");
            }
            UserCollectionGoodsParam userCollectionGoodsParam = JsonConvert.DeserializeObject<UserCollectionGoodsParam>(param.ToString());
            if (userCollectionGoodsParam.pageSize == 0)
            {
                userCollectionGoodsParam.pageSize = 5;
            }
            if (userCollectionGoodsParam.current == 0)
            {
                userCollectionGoodsParam.current = 1;
            }
            NewHomePageDao newHomePageDao = new NewHomePageDao();
            return newHomePageDao.UserCollectionBrands(userCollectionGoodsParam, userId);

        }

    }

    public class UserCollectionBrandsItem
    {
        public string ifOnload = "0";//是否登录1：已登陆，0：未登录
        public string type = "0";//0失败，1成功
        public List<UserCollectionBrandsList> brandsList;//品牌、商品信息
        public Page pagination;//翻页
    }

    public class UserCollectionBrandsList
    {
        public string brand;//品牌名
        public string slt;//品牌图
        public List<ChangeGoods> goodsList;//商品信息
    }

    public class UserCollectionGoodsItem
    {
        public string ifOnload = "0";//是否登录1：已登陆，0：未登录
        public string type = "0";//0失败，1成功
        public List<ChangeGoods> goodsList;//商品信息
        public Page pagination;  //翻页      
    }

    public class UserCollectionGoodsParam
    {
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class UserCollectionParam
    {
        public string collectionType;//1：收藏商品，2：关注品牌
        public string collectionValue;//商品条码或者品牌名
        public string type;//1:添加， 0：删除
    }

    public class NewGoodsDetailsItem
    {
        public string ifOnload = "0";//是否登录1：已登陆，0：未登录
        public string type = "0";//0失败，1成功
        public string attentionType="0";//1已收藏，0未收藏
        public string barcode;//商品条码
        public string goodsName;//商品名
        public string discription;//商品描述
        public string price;//价格
        public List<string> img;//图片
        public List<GoodsParameters> goodsParameters;       
        public List<string> goodsDetailImgArr;//商品详情图
    }
    public class GoodsParameters
    {
        public int key;
        public string name;
        public string content;
    }

    public class BrandsGoodsItem
    {
        public string ifOnload = "0";//是否登录1：已登陆，0：未登录
        public string type = "0";//0失败，1成功
        public string attentionType="0";//1已关注，0未关注
        public List<string> advimg;//广告图
        public string brandName;//品牌名
        public string description;//描述
        public string brandimg;//品牌图
        public List<ChangeGoods> goods;//商品信息
        public Page pagination;//分页信息
    }

    public class CountryBrandsGoodsItem
    {
        public List<string> banner;//banner图
        public List<Brands> brands;//品牌信息
        public List<ChangeGoods> goods;//商品信息
        public string ifOnload = "0";//是否登录1：已登陆，0：未登录 
        public string type = "0";//0失败，1成功
    }

    public class CategoryGoodsItem
    {
        public string ifOnload = "0";//是否登录1：已登陆，0：未登录
        public string select;//查询条件
        public List<string> categoryImg;//品类banner图
        public List<AllClassificationItem> classificationSED;//二级分类
        public List<string> brands;//品牌
        public List<ChangeGoods>  changeGoods;//商品信息
        public string type = "0";//0失败，1成功
        public Page pagination;
    }

    public class CategoryGoodsParam
    {
        public string select;//搜索信息
        public string country;//国家
        public string classificationST;//一级分类
        public string classificationSED;//二级分类
        public string brand;//品牌
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class HomePageParam
    {       
        public string country;//国家
        public int page;//页数
        public int pageSize;//数量
    }

    public class AllClassificationItem
    {
        public string allclassification;//分类 
        public string classificationST;//分类编号
        public string country;//国家
    }
    public class ListAllClassificationItem
    {
        public List<AllClassificationItem> allClassificationItems;//分类信息
        public string type = "0";//0失败，1成功
    }

    public class HomePageDownPartItem
    {
        public string ifOnload = "0";//是否登录1：已登陆，0：未登录
        public string type = "0";//0失败，1成功
        public List<ChangeGoods> goodsList;//商品信息
        public int page;//页数
    }

    public class HomePageItem
    {
        public string  ifOnload="0";//是否登录1：已登陆，0：未登录
        public string type = "0";//0失败，1成功
        public ListAllClassificationItem allclassification;//一级分类   
        public List<string> banner;//banner图
        public List<HomePageChangeGoodsItem> homePageChangeGoodsItem;//国家馆信息
       
    }


    public class HomePageChangeGoodsItem
    {
        public string country;//国家
        public string ifOnload = "0";//是否登录1：已登陆，0：未登录
        public string type = "0";//0失败，1成功
        public int page;//页数
        public string adv;//广告图
        public List<AllClassificationItem> classification;//一级分类     
        public List<ChangeGoods> goodsList;//商品list
        public List<Brands> brandimgs;//品牌图
    }

    public class ChangeGoods//商品信息
    {
        public string imgurl;//图片
        public string goodsName;//名字
        public string price;//价格
        public string barcode;//商品编码
    }

    public class Brands
    {
        public string imgurl;//图片
        public string brandsName;//名字
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }

    public class NewGoodsParam
    {
        public string barcode;//商品编码
    }
}
