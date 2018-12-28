using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Buss
{
    public class HomePageBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.HomePageApi;
        }
        public bool NeedCheckToken()
        {
            return true;
        }

        /// <summary>
        /// 顶部所有分类接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public object Do_AllMessage(object param, string userId)
        {
            
            HomePageDao  homePageDao = new HomePageDao();
            return homePageDao.AllMessage();

        }

        public class AllMessageItem
        {
            public  List<Classification> classificationsList = new List<Classification>();//分类
            public List<ImgList> imgList = new List<ImgList>();//banner图
            public List<BrandsList> brands = new List<BrandsList>();//品牌
            public List<AllGoodMessage>  goods;//商品展示
        }

        public class ImgList
        {
            public string src;//banner地址
        }

        public class BrandsList
        {
            public string name;//brands名称
            public string src;//链接
        }

        public class Classification
        {
            public string name;//分类名称
            public string url;//链接
        }

        public class AllGoodMessage
        {
            public string title;//热卖商品
            public string checkAllurl;//查看全部
            public Side side;//类
            public List<GoodsList> goodsList;//商品类
            public string ad;//横版图片
        }

        public class Side
        {
            public string keyword;//热卖商品
            public string  des;//实时更新热销商品排行
            public string src;//竖版图片地址
        }

        public class GoodsList
        {
            public string src;//图片地址
            public string url;//跳转地址
            public string title;//商品名
            public string price;//价格
        }
    }
}
