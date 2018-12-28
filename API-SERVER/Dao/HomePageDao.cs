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
        /// 顶部所有分类接口
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
                + " where flag='1' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            if (dt.Rows.Count > 0)
            {
                DataRow[] dr = dt.Select("advtype='banner'");
                if (dr.Length>0)
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
                        BrandsList brandsList  = new BrandsList();
                        brandsList.src = dataRow["imgurl"].ToString();
                        brandsList.name= dataRow["advname"].ToString();
                        allMessageItem.brands.Add(brandsList);
                    }
                }
            }

            string sql1 = ""
                + "select a.categoryId,a.categoryName"
                + " from t_newpage_category a,t_goods_category b"
                + " where a.categoryId=b.id and a.flag='1'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1,"T").Tables[0];

            if (dt1.Rows.Count > 0)
            {
                foreach (DataRow dr in dt1.Rows)
                {
                    Classification classification = new Classification();
                    classification.name = dr["categoryName"].ToString();
                    classification.url= "http://www.llwell.net/app/index.php?i=2&c=entry&m=ewei_shopv2&do=mobile&r=pc.goods.detail&id="+ dr["categoryId"].ToString();
                    allMessageItem.classificationsList.Add(classification);
                }              
            }

            string sql2 = ""
                + " select *"
                + " from t_newpage_category"
                + " where flag='1' ";
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2,"T").Tables[0];

            string sql3 = ""
                + "select a.id,b.categoryName,a.title,a.thumb,a.marketprice,a.isnew,a.ishot  "
                + " from view_eshop_goods a,t_newpage_category b"
                + " where  a.pcate=b.categoryId";
            DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql3,"T").Tables[0];

            for (int i=0; i<dt2.Rows.Count;i++)
            {
                AllGoodMessage allGoodMessage = new AllGoodMessage();
                allGoodMessage.goodsList = new List<GoodsList>();
                allGoodMessage.title = dt2.Rows[i]["categoryName"].ToString();
                if (dt2.Rows[i]["categoryName"].ToString() == "新品推荐" || dt2.Rows[i]["categoryName"].ToString() == "热卖商品")
                {
                    allGoodMessage.checkAllurl = "http://www.llwell.net/app/index.php?i=2&c=entry&m=ewei_shopv2&do=mobile&r=pc.goods.detail" ;
                }
                else
                {
                    allGoodMessage.checkAllurl = "http://www.llwell.net/app/index.php?i=2&c=entry&m=ewei_shopv2&do=mobile&r=pc.goods.detail&id=" + dt2.Rows[i]["categoryId"].ToString();
                }
                Side side = new Side();
                side.keyword = allGoodMessage.title;
                side.des= dt2.Rows[i]["des"].ToString();
                side.src= dt2.Rows[i]["src"].ToString();
                allGoodMessage.side = side;
                allGoodMessage.ad= dt2.Rows[i]["ad"].ToString();

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
                if (dr.Length>0)
                {
                    for (int j = 0; j < 8 && j<dr.Length ; j++)
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
    }
}
