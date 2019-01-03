using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static API_SERVER.Buss.HomePageBuss;

namespace API_SERVER.Dao
{
    public class HomePageDao
    {
        private string path = System.Environment.CurrentDirectory;

        public HomePageDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        /// <summary>
        /// 首页接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public AllMessageItem AllMessage()
        {
            AllMessageItem allMessageItem = new AllMessageItem();
            allMessageItem.goods = new List<AllGoodMessage>();
            string sql = ""
                + " select sort,imgurl,advtype,advname"
                + " from t_base_adv"
                + " where flag='1' order by sort asc";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            if (dt.Rows.Count > 0)
            {
                DataRow[] dr = dt.Select("advtype='banner'");
                if (dr.Length > 0)
                {
                    foreach (DataRow dataRow in dr)
                    {
                        ImgList imgList = new ImgList();
                        imgList.src = dataRow["imgurl"].ToString();
                        allMessageItem.imgList.Add(imgList);
                    }
                }
                DataRow[] dr1 = dt.Select("advtype='brands'");
                if (dr1.Length > 0)
                {
                    foreach (DataRow dataRow in dr1)
                    {
                        BrandsList brandsList = new BrandsList();
                        brandsList.src = dataRow["imgurl"].ToString();
                        brandsList.name = dataRow["advname"].ToString();
                        allMessageItem.brands.Add(brandsList);
                    }
                }
            }

            string sql1 = ""
                + "select a.categoryId,a.categoryName"
                + " from t_newpage_category a,t_goods_category b"
                + " where a.categoryId=b.id and a.flag='1'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];

            if (dt1.Rows.Count > 0)
            {
                foreach (DataRow dr in dt1.Rows)
                {
                    Classification classification = new Classification();
                    classification.name = dr["categoryName"].ToString();
                    classification.url = "http://www.llwell.net/app/index.php?i=2&c=entry&m=ewei_shopv2&do=mobile&r=pc.goods.detail&id=" + dr["categoryId"].ToString();
                    allMessageItem.classificationsList.Add(classification);
                }
            }

            string sql2 = ""
                + " select *"
                + " from t_newpage_category"
                + " where flag='1' ";
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "T").Tables[0];

            string sql3 = ""
                + "select a.id,b.categoryName,a.title,a.thumb,a.marketprice,a.isnew,a.ishot  "
                + " from view_eshop_goods a,t_newpage_category b"
                + " where  a.pcate=b.categoryId";
            DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "T").Tables[0];

            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                AllGoodMessage allGoodMessage = new AllGoodMessage();
                allGoodMessage.goodsList = new List<GoodsList>();
                allGoodMessage.title = dt2.Rows[i]["categoryName"].ToString();
                if (dt2.Rows[i]["categoryName"].ToString() == "新品推荐" || dt2.Rows[i]["categoryName"].ToString() == "热卖商品")
                {
                    allGoodMessage.checkAllurl = "http://www.llwell.net/app/index.php?i=2&c=entry&m=ewei_shopv2&do=mobile&r=pc.goods.detail";
                }
                else
                {
                    allGoodMessage.checkAllurl = "http://www.llwell.net/app/index.php?i=2&c=entry&m=ewei_shopv2&do=mobile&r=pc.goods.detail&id=" + dt2.Rows[i]["categoryId"].ToString();
                }
                Side side = new Side();
                side.keyword = allGoodMessage.title;
                side.des = dt2.Rows[i]["des"].ToString();
                side.src = dt2.Rows[i]["src"].ToString();
                allGoodMessage.side = side;
                allGoodMessage.ad = dt2.Rows[i]["ad"].ToString();

                DataRow[] dr;
                if (allGoodMessage.title == "新品推荐")
                {
                    dr = dt3.Select("isnew='true'");
                }
                else if (allGoodMessage.title == "热卖商品")
                {
                    dr = dt3.Select("ishot='true'");
                }
                else
                    dr = dt3.Select("categoryName='" + allGoodMessage.title + "'");
                if (dr.Length > 0)
                {
                    for (int j = 0; j < 8 && j < dr.Length; j++)
                    {
                        GoodsList goodsList = new GoodsList();
                        goodsList.src = dr[j]["thumb"].ToString();
                        goodsList.title = dr[j]["title"].ToString();
                        goodsList.price = dr[j]["marketprice"].ToString();
                        goodsList.url = "http://www.llwell.net/app/index.php?i=2&c=entry&m=ewei_shopv2&do=mobile&r=pc.goods.detail&id=" + dr[j]["id"].ToString();
                        allGoodMessage.goodsList.Add(goodsList);
                    }
                }
                allMessageItem.goods.Add(allGoodMessage);
            }
            return allMessageItem;
        }

        /// <summary>
        /// 国家馆接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public CountryGoodsListItem CountrySalseList(CountryGoodsListParam countryGoodsListParam, string userId)
        {
            CountryGoodsListItem countryGoodsListItem = new CountryGoodsListItem();
            countryGoodsListItem.brands = new List<BrandsList>();
            countryGoodsListItem.filtrate = new List<CategoryAndEffect>();
            countryGoodsListItem.goodsList = new List<GoodsList>();
            countryGoodsListItem.imgList = new List<ImgList>();
           

            #region 品牌
            string country = "";
            string advtype = "";
            if (countryGoodsListParam.country == "韩国")
            {
                country = " and country='韩国'";
                advtype = " advtype='hgbanner' ";
            }
            else if (countryGoodsListParam.country == "日本")
            {
                country = " and country='日本'";
                advtype = " advtype='rbbanner' ";
            }
            else
            {
                country = " and country='欧美'";
                advtype = " advtype='ombanner' ";
            }

            string sql = ""
               + " select sort,imgurl,advtype,advname"
               + " from t_base_adv"
               + " where flag='1' and  advtype='brands'" + country +" order by sort asc" ;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {               
                foreach (DataRow dataRow in dt.Rows)
                {
                    BrandsList brandsList = new BrandsList();
                    brandsList.src = dataRow["imgurl"].ToString();
                    brandsList.name = dataRow["advname"].ToString();
                    countryGoodsListItem.brands.Add(brandsList);
                }
            }
            #endregion

            #region 商品与最高价
            CountryGoodsParam countryGoodsParam = new CountryGoodsParam();
            CountryGoodsItem countryGoodsItem = new CountryGoodsItem();
            countryGoodsParam.country = countryGoodsListParam.country;
            countryGoodsItem = CountryGoods(countryGoodsParam, userId);

            countryGoodsListItem.priceSlider = countryGoodsItem.priceSlider;
            countryGoodsListItem.goodsList = countryGoodsItem.goodsList;
            #endregion

            #region 品类list
            string sql1 = ""
                + "select name,c.source "
                + "  from t_goods_category a,view_eshop_goods b,t_goods_list c "
                + " where a.id=b.pcate and c.barcode=b.productsn and c.source='"+ countryGoodsListParam.country + "' GROUP BY pcate";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1,"T").Tables[0];
            if (dt1.Rows.Count>0)
            {
                CategoryAndEffect categoryAndEffect = new CategoryAndEffect();
                categoryAndEffect.classify = "品类";
                for (int i=0;i< dt1.Rows.Count;i++)
                {
                    categoryAndEffect.details.Add(dt1.Rows[i]["name"].ToString());
                }
                countryGoodsListItem.filtrate.Add(categoryAndEffect);
            }

            string sql2 = ""
                + " select imgurl"
                + " from t_base_adv "
                + " where "+advtype;
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2,"T").Tables[0];
            if (dt2.Rows.Count > 0)
            {
                
                for (int i=0;i< dt2.Rows.Count;i++)
                {
                    ImgList imgList = new ImgList();
                    imgList.src = dt2.Rows[i]["imgurl"].ToString();
                    countryGoodsListItem.imgList.Add(imgList);
                }
                
            }


            #endregion

            return countryGoodsListItem;
        }


        /// <summary>
        /// 国家馆商品接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public CountryGoodsItem CountryGoods(CountryGoodsParam countryGoodsParam, string userId)
        {
            CountryGoodsItem countryGoodsItem = new CountryGoodsItem();
            countryGoodsItem.goodsList = new List<GoodsList>();
            countryGoodsItem.priceSlider = new string[2] {"0","0"};

            string price = "";
            string categoryName = "";
            string name = "";
            if (countryGoodsParam.categoryName!=null)
            {
                for (int i=0;i< countryGoodsParam.categoryName.Length;i++)
                {
                    if (i==0)
                        name = name +"'"+ countryGoodsParam.categoryName[i]+"'";
                    else 
                        name = name +"," + "'" + countryGoodsParam.categoryName[i] + "'";
                }
                categoryName = " and c.name in(" + name + ") ";
            }
            if (countryGoodsParam.price!=null)
            {
                price = " and marketprice>'"+ countryGoodsParam.price[0]+ "'  and marketprice<'"+ countryGoodsParam.price[1] + "'";              
            }
            string sql = " "
                + "select max(a.marketprice) maxmarketprice,a.id,b.source,a.title,a.thumb,a.marketprice  "
                + " from view_eshop_goods a,t_goods_list b,t_goods_category c"
                + " where  a.productsn=b.barcode and a.pcate=c.id and b.source='"+ countryGoodsParam.country+ "' "+ price + categoryName
                + "  group by a.id  order by a.marketprice desc  limit 0,20";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int j = 0;j < dt.Rows.Count; j++)
                {
                    GoodsList goodsList = new GoodsList();
                    goodsList.src = dt.Rows[j]["thumb"].ToString();
                    goodsList.title = dt.Rows[j]["title"].ToString();
                    goodsList.price = dt.Rows[j]["marketprice"].ToString();
                    goodsList.url = "http://www.llwell.net/app/index.php?i=2&c=entry&m=ewei_shopv2&do=mobile&r=pc.goods.detail&id=" + dt.Rows[j]["id"].ToString();
                    countryGoodsItem.goodsList.Add(goodsList);
                }            
                countryGoodsItem.priceSlider[1]= dt.Rows[0]["maxmarketprice"].ToString();
            }
            return countryGoodsItem;
        }
    }
}
