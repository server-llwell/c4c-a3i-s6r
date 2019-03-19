using API_SERVER.Buss;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API_SERVER.Common;


namespace API_SERVER.Dao
{
    public class NewHomePageDao
    {
        private string path = System.Environment.CurrentDirectory;

        public NewHomePageDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        /// <summary>
        /// 总分类接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public ListAllClassificationItem AllClassification()
        {
            ListAllClassificationItem allClassificationItem = new ListAllClassificationItem();
            allClassificationItem.allClassificationItems = new List<AllClassificationItem>();
            string t_goods_categorySql = ""
                + " select name,id from t_goods_category where flag='1' and parentid='0'";
            DataTable dtt_goods_categorySql = DatabaseOperationWeb.ExecuteSelectDS(t_goods_categorySql, "T").Tables[0];
            if (dtt_goods_categorySql.Rows.Count > 0)
            {
                for (int i = 0; i < dtt_goods_categorySql.Rows.Count; i++)
                {
                    AllClassificationItem classificationItem = new AllClassificationItem();
                    classificationItem.allclassification = dtt_goods_categorySql.Rows[i]["name"].ToString();
                    classificationItem.classificationST = dtt_goods_categorySql.Rows[i]["id"].ToString();
                    allClassificationItem.allClassificationItems.Add(classificationItem);
                }
                allClassificationItem.type = "1";
            }

            return allClassificationItem;
        }



        /// <summary>
        /// 首页上半部接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public HomePageItem HomePage(string userId)
        {
            HomePageItem homePageItem = new HomePageItem();
            homePageItem.banner = new List<string>();
            homePageItem.homePageChangeGoodsItem = new List<HomePageChangeGoodsItem>();
            homePageItem.allclassification = new ListAllClassificationItem();

            homePageItem.allclassification = AllClassification();
            string bannerSql = ""
                + "select imgurl"
                + " from t_base_adv "
                + " where advtype='toCbanner' and flag='1'"
                + " order by sort asc  limit 0,4";
            DataTable dtbannerSql = DatabaseOperationWeb.ExecuteSelectDS(bannerSql, "T").Tables[0];
            if (dtbannerSql.Rows.Count > 0)
            {
                for (int i = 0; i < dtbannerSql.Rows.Count; i++)
                {
                    homePageItem.banner.Add(dtbannerSql.Rows[i]["imgurl"].ToString());
                }
            }

            HomePageParam JplisthomePageParam = new HomePageParam();
            JplisthomePageParam.country = "日本馆";
            JplisthomePageParam.page = 0;
            homePageItem.homePageChangeGoodsItem.Add(HomePageChangeGoods(JplisthomePageParam, userId));

            HomePageParam KorealisthomePageParam = new HomePageParam();
            KorealisthomePageParam.country = "韩国馆";
            KorealisthomePageParam.page = 0;
            homePageItem.homePageChangeGoodsItem.Add(HomePageChangeGoods(KorealisthomePageParam, userId));

            HomePageParam CHlisthomePageParam = new HomePageParam();
            CHlisthomePageParam.country = "国内购";
            CHlisthomePageParam.page = 0;
            homePageItem.homePageChangeGoodsItem.Add(HomePageChangeGoodsCHINA(CHlisthomePageParam, userId));

            if (userId != null && userId != "")
            {
                homePageItem.ifOnload = "1";
            }
            if (homePageItem.homePageChangeGoodsItem[0].type == "1" || homePageItem.homePageChangeGoodsItem[1].type == "1" || homePageItem.homePageChangeGoodsItem[3].type == "1")
            {
                homePageItem.type = "1";
            }

            return homePageItem;

        }

        /// <summary>
        /// 首页下半部接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public HomePageDownPartItem HomePageDownPart(HomePageParam homePageParam, string userId)
        {
            HomePageDownPartItem homePageDownPartItem = new HomePageDownPartItem();
            homePageDownPartItem.goodsList = new List<ChangeGoods>();
            bool ifShowPrice = true;
            string settingSql = ""
                    + "select settingValue from t_sys_setting where settingCode='B2CSHOWPRICE'";
            if (userId != null && userId != "" && userId != "undefined")
            {
                homePageDownPartItem.ifOnload = "1";
                UserDao userDao = new UserDao();
                string userType = userDao.getUserType(userId);
                if (userType == "6" || userType == "7")
                {
                    DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                    if (dtsettingSql.Rows.Count > 0)
                    {
                        ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                    }
                }
            }
            else
            {
                DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                if (dtsettingSql.Rows.Count > 0)
                {
                    ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                }
            }



            string sql = ""
            + " select a.goodsName,a.barcode,a.slt,max(a.pprice) pprice "
            + " from t_goods_distributor_price a "
            + " where  a.pprice>'0'  and a.show='1' "
            + " GROUP BY a.barcode ORDER BY a.id DESC ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                List<int> sizeList = new List<int>();
                for (int i = 0; i < dt.Rows.Count && i < homePageParam.pageSize; i++)
                {
                    //判断随机数
                    bool s = false;                    
                    Random random = new Random();
                    int size = random.Next(0, dt.Rows.Count);
                    for (int a = 0; a < sizeList.Count; a++)
                    {
                        if (size == sizeList[a])
                        {
                            s = true;
                            break;
                        }
                    }
                    if (s)
                    {
                        i--;
                        continue;
                    }
                    sizeList.Add(size);
                    ChangeGoods changeGoods = new ChangeGoods();
                    changeGoods.goodsName = dt.Rows[size]["goodsName"].ToString();
                    changeGoods.barcode = dt.Rows[size]["barcode"].ToString();
                    changeGoods.imgurl = dt.Rows[size]["slt"].ToString() + "?x-oss-process=image/resize,limit_0,m_fill,w_800,h_800/quality,q_100";

                    if (ifShowPrice)
                    {
                        changeGoods.price = "￥" + dt.Rows[size]["pprice"].ToString();
                    }
                    else
                    {
                        changeGoods.price = "登陆后显示价格";
                    }
                    homePageDownPartItem.goodsList.Add(changeGoods);

                }

            }
            homePageDownPartItem.page = homePageParam.page;


            homePageDownPartItem.type = "1";
            return homePageDownPartItem;
        }

        /// <summary>
        /// 首页各馆换一批接口(中国)
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public HomePageChangeGoodsItem HomePageChangeGoodsCHINA(HomePageParam homePageParam, string userId)
        {
            HomePageChangeGoodsItem homePageChangeGoodsItem = new HomePageChangeGoodsItem();
            homePageChangeGoodsItem.goodsList = new List<ChangeGoods>();
            homePageChangeGoodsItem.classification = new List<AllClassificationItem>();
            homePageChangeGoodsItem.brandimgs = new List<Brands>();
            if (homePageParam.country == "国内购")
            {
                homePageChangeGoodsItem.country = "国内购";
            }
            bool ifShowPrice = true;
            string user = "";
            string settingSql = ""
                   + "select settingValue from t_sys_setting where settingCode='B2CSHOWPRICE'";
            if (userId != null && userId != "" && userId != "undefined")
            {
                user = " and a.usercode='" + userId + "'";
                homePageChangeGoodsItem.ifOnload = "1";
                UserDao userDao = new UserDao();
                string userType = userDao.getUserType(userId);
                if (userType == "6" || userType == "7")
                {
                    DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                    if (dtsettingSql.Rows.Count > 0)
                    {
                        ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                    }
                }
            }
            else
            {
                DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                if (dtsettingSql.Rows.Count > 0)
                {
                    ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                }
            }
            //分类与adv图、页数限定
            string classificationSql = " select c.name,b.catelog1,a.barcode"
                + " from t_goods_distributor_price a,t_base_warehouse d,t_goods_list b  left join t_goods_category c  on c.id=b.catelog1"
                + " where a.barcode=b.barcode  and d.id=a.wid and d.businessType='0' and b.recom='1'  and a.pprice>'0' and a.show='1' "
                + " GROUP BY a.barcode";
            DataTable alldtSql = DatabaseOperationWeb.ExecuteSelectDS(classificationSql, "T").Tables[0];
            DataView dv = new DataView(alldtSql);
            DataTable dtclassificationSql = dv.ToTable(true, "name", "catelog1");
            if (dtclassificationSql.Rows.Count > 0)
            {
                for (int i = 0; i < dtclassificationSql.Rows.Count; i++)
                {
                    AllClassificationItem allClassificationItem = new AllClassificationItem();
                    allClassificationItem.allclassification = dtclassificationSql.Rows[i]["name"].ToString();
                    allClassificationItem.classificationST = dtclassificationSql.Rows[i]["catelog1"].ToString();
                    allClassificationItem.country = homePageParam.country;
                    homePageChangeGoodsItem.classification.Add(allClassificationItem);//分类
                }
            }

            DataTable dtallGoodsSql = dv.ToTable(true, "barcode");
            if (dtallGoodsSql.Rows.Count > 0)
            {
                if (homePageParam.page * 12 > dtallGoodsSql.Rows.Count)
                {
                    homePageParam.page = 0;
                }

                //商品信息
                string goodsSql = ""
                + " select b.goodsName,b.barcode,b.slt,min(a.pprice) pprice "
                + " from t_goods_distributor_price a,t_goods_list b ,t_base_warehouse d"
                + " where a.barcode=b.barcode  and  d.id=a.wid  and  d.businessType='0' and b.recom='1'  and a.pprice>'0'  and a.show='1'"
                + " GROUP BY a.barcode ORDER BY a.id DESC  LIMIT " + homePageParam.page * 12 + "," + 12;

                DataTable dtgoodsSql = DatabaseOperationWeb.ExecuteSelectDS(goodsSql, "T").Tables[0];
                if (dtgoodsSql.Rows.Count > 0)
                {
                    for (int i = 0; i < dtgoodsSql.Rows.Count; i++)
                    {
                        ChangeGoods changeGoods = new ChangeGoods();
                        changeGoods.goodsName = dtgoodsSql.Rows[i]["goodsName"].ToString();
                        changeGoods.barcode = dtgoodsSql.Rows[i]["barcode"].ToString();
                        changeGoods.imgurl = dtgoodsSql.Rows[i]["slt"].ToString() + "?x-oss-process=image/resize,limit_0,m_fill,w_800,h_800/quality,q_100";

                        if (ifShowPrice)
                        {
                            changeGoods.price = "￥" + dtgoodsSql.Rows[i]["pprice"].ToString();
                        }
                        else
                        {
                            changeGoods.price = "登陆后显示价格";
                        }
                        homePageChangeGoodsItem.goodsList.Add(changeGoods);
                    }
                    homePageChangeGoodsItem.page = homePageParam.page + 1;

                }
            }

            //品牌
            string brandimgsSql = "select a.imgurl,a.advname "
                + " from t_base_adv a  "
                + " where  a.advtype='brands' and a.country='中国' and a.flag='1'  "
                + " limit 0,11";
            DataTable dtbrandimgsSql = DatabaseOperationWeb.ExecuteSelectDS(brandimgsSql, "T").Tables[0];
            if (dtbrandimgsSql.Rows.Count > 0)
            {
                for (int i = 0; i < dtbrandimgsSql.Rows.Count; i++)
                {
                    Brands brands = new Brands();
                    brands.imgurl = dtbrandimgsSql.Rows[i]["imgurl"].ToString();
                    brands.brandsName = dtbrandimgsSql.Rows[i]["advname"].ToString();
                    homePageChangeGoodsItem.brandimgs.Add(brands);//品牌图
                }

            }

            homePageChangeGoodsItem.type = "1";

            return homePageChangeGoodsItem;
        }


        /// <summary>
        /// 首页各馆换一批接口(韩国，日本)
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public HomePageChangeGoodsItem HomePageChangeGoods(HomePageParam homePageParam, string userId)
        {
            HomePageChangeGoodsItem homePageChangeGoodsItem = new HomePageChangeGoodsItem();
            homePageChangeGoodsItem.goodsList = new List<ChangeGoods>();
            homePageChangeGoodsItem.classification = new List<AllClassificationItem>();
            homePageChangeGoodsItem.brandimgs = new List<Brands>();
            if (homePageParam.country == "韩国馆")
            {
                homePageChangeGoodsItem.country = "韩国馆";
                homePageParam.country = "韩国";
            }
            if (homePageParam.country == "日本馆")
            {
                homePageChangeGoodsItem.country = "日本馆";
                homePageParam.country = "日本";
            }
            bool ifShowPrice = true;
            string user = "";
            string settingSql = ""
                   + "select settingValue from t_sys_setting where settingCode='B2CSHOWPRICE'";
            if (userId != null && userId != "" && userId != "undefined")
            {
                user = " and a.usercode='" + userId + "'";
                homePageChangeGoodsItem.ifOnload = "1";
                UserDao userDao = new UserDao();
                string userType = userDao.getUserType(userId);
                if (userType == "6" || userType == "7")
                {
                    DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                    if (dtsettingSql.Rows.Count > 0)
                    {
                        ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                    }
                }
            }
            else
            {
                DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                if (dtsettingSql.Rows.Count > 0)
                {
                    ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                }
            }
            //分类与adv图、页数限定
            string classificationSql = " select c.name,b.catelog1,a.barcode"
                + " from t_goods_distributor_price a,t_goods_list b  left join t_goods_category c  on c.id=b.catelog1"
                + " where a.barcode=b.barcode and b.recom='1'  and b.country='" + homePageParam.country + "'   and a.pprice>'0' and a.show='1' "
                + " GROUP BY a.barcode";

            DataTable alldtSql = DatabaseOperationWeb.ExecuteSelectDS(classificationSql, "T").Tables[0];
            DataView dv = new DataView(alldtSql);
            DataTable dtclassificationSql = dv.ToTable(true, "name", "catelog1");
            if (dtclassificationSql.Rows.Count > 0)
            {
                for (int i = 0; i < dtclassificationSql.Rows.Count; i++)
                {
                    AllClassificationItem allClassificationItem = new AllClassificationItem();
                    allClassificationItem.allclassification = dtclassificationSql.Rows[i]["name"].ToString();
                    allClassificationItem.classificationST = dtclassificationSql.Rows[i]["catelog1"].ToString();
                    allClassificationItem.country = homePageParam.country;
                    homePageChangeGoodsItem.classification.Add(allClassificationItem);//分类

                }

            }

            DataTable dtallGoodsSql = dv.ToTable(true, "barcode");
            if (dtallGoodsSql.Rows.Count > 0)
            {
                if (homePageParam.page * 12 > dtallGoodsSql.Rows.Count)
                {
                    homePageParam.page = 0;
                }

                //商品信息
                string goodsSql = ""
                + " select b.goodsName,b.barcode,b.slt,min(a.pprice) pprice "
                + " from t_goods_distributor_price a,t_goods_list b "
                + " where a.barcode=b.barcode and b.recom='1' and b.country='" + homePageParam.country + "'   and a.pprice>'0'  and a.show='1'"
                + " GROUP BY a.barcode ORDER BY a.id DESC  LIMIT " + homePageParam.page * 12 + "," + 12;

                DataTable dtgoodsSql = DatabaseOperationWeb.ExecuteSelectDS(goodsSql, "T").Tables[0];
                if (dtgoodsSql.Rows.Count > 0)
                {
                    for (int i = 0; i < dtgoodsSql.Rows.Count; i++)
                    {
                        ChangeGoods changeGoods = new ChangeGoods();
                        changeGoods.goodsName = dtgoodsSql.Rows[i]["goodsName"].ToString();
                        changeGoods.barcode = dtgoodsSql.Rows[i]["barcode"].ToString();
                        changeGoods.imgurl = dtgoodsSql.Rows[i]["slt"].ToString() + "?x-oss-process=image/resize,limit_0,m_fill,w_800,h_800/quality,q_100";

                        if (ifShowPrice)
                        {
                            changeGoods.price = "￥" + dtgoodsSql.Rows[i]["pprice"].ToString();
                        }
                        else
                        {
                            changeGoods.price = "登陆后显示价格";
                        }
                        homePageChangeGoodsItem.goodsList.Add(changeGoods);
                    }
                    homePageChangeGoodsItem.page = homePageParam.page + 1;

                }
            }

            //品牌
            string brandimgsSql = "select imgurl,advname "
                + "from t_base_adv where advtype='brands' and country='" + homePageParam.country + "' and flag='1'"
                + "limit 0,11";
            DataTable dtbrandimgsSql = DatabaseOperationWeb.ExecuteSelectDS(brandimgsSql, "T").Tables[0];
            if (dtbrandimgsSql.Rows.Count > 0)
            {
                for (int i = 0; i < dtbrandimgsSql.Rows.Count; i++)
                {
                    Brands brands = new Brands();
                    brands.imgurl = dtbrandimgsSql.Rows[i]["imgurl"].ToString();
                    brands.brandsName = dtbrandimgsSql.Rows[i]["advname"].ToString();
                    homePageChangeGoodsItem.brandimgs.Add(brands);//品牌图
                }

            }

            homePageChangeGoodsItem.type = "1";

            return homePageChangeGoodsItem;

        }

        /// <summary>
        /// 品类页接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public CategoryGoodsItem CategoryGoods(CategoryGoodsParam categoryGoodsParam, string userId)
        {
            CategoryGoodsItem categoryGoodsItem = new CategoryGoodsItem();
            categoryGoodsItem.brands = new List<string>();
            categoryGoodsItem.classificationSED = new List<AllClassificationItem>();
            categoryGoodsItem.changeGoods = new List<ChangeGoods>();
            categoryGoodsItem.pagination = new Page(categoryGoodsParam.current, categoryGoodsParam.pageSize);
            categoryGoodsItem.categoryImg = new List<string>();
            string brand = "";
            string country = "";
            string country1 = "";
            string classificationSED = "";
            string classificationST = "";
            string classificationST1 = "";
            if (categoryGoodsParam.brand != null && categoryGoodsParam.brand != "" && categoryGoodsParam.brand != "全部")
            {
                brand = " and b.brand='" + categoryGoodsParam.brand + "'";
            }

            //判断价格按钮
            string settingSql = ""
                   + "select settingValue from t_sys_setting where settingCode='B2CSHOWPRICE'";
            //判断是否显示价格
            bool ifShowPrice = true;
            if (userId != null && userId != "" && userId != "undefined")
            {
                categoryGoodsItem.ifOnload = "1";
                UserDao userDao = new UserDao();
                string userType = userDao.getUserType(userId);
                if (userType == "6" || userType == "7")
                {
                    DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                    if (dtsettingSql.Rows.Count > 0)
                    {
                        ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                    }
                }
            }
            else
            {
                DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                if (dtsettingSql.Rows.Count > 0)
                {
                    ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                }
            }
            if (categoryGoodsParam.classificationST != null && categoryGoodsParam.classificationST != "")
            {
                classificationST = " and b.catelog1='" + categoryGoodsParam.classificationST + "'";
                classificationST1 = " and advname='" + categoryGoodsParam.classificationST + "'";
            }
            if (categoryGoodsParam.classificationSED != null && categoryGoodsParam.classificationSED != "")
            {
                classificationSED = " and b.catelog2='" + categoryGoodsParam.classificationSED + "'";
            }
            //banner图
            string imgCategorysql = ""
                + "select imgurl from t_base_adv where advtype='toCCategory' and flag='1' " + country1 + classificationST1;

            DataTable dtimgCategorysql = DatabaseOperationWeb.ExecuteSelectDS(imgCategorysql, "T").Tables[0];
            if (dtimgCategorysql.Rows.Count > 0)
            {
                categoryGoodsItem.categoryImg.Add(dtimgCategorysql.Rows[0][0].ToString());
            }


            //二级分类、品牌、商品信息
            string catelog2Sql = ""
                + " select c.name,b.catelog2,b.brand,b.goodsName,a.barcode,b.slt,max(a.pprice) pprice  "
                + " from t_goods_distributor_price a,t_goods_list b  left join t_goods_category c  on b.catelog2=c.id"
                + " where a.barcode=b.barcode   and a.pprice>'0' and a.show='1' " + classificationST + classificationSED + brand + country
                + " group by a.barcode "
                + " ORDER BY a.id DESC";

            DataTable dtcatelog2Sql = DatabaseOperationWeb.ExecuteSelectDS(catelog2Sql, "T").Tables[0];
            //二级分类
            DataView dv = dtcatelog2Sql.DefaultView;
            DataTable DistTable = dv.ToTable(true, "name", "catelog2");

            if (DistTable.Rows.Count > 0)
            {
                for (int i = 0; i < DistTable.Rows.Count + 1; i++)
                {
                    AllClassificationItem allClassificationItem = new AllClassificationItem();
                    if (i == 0)
                    {
                        allClassificationItem.allclassification = "全部";
                        allClassificationItem.classificationST = "";
                    }
                    else
                    {
                        allClassificationItem.allclassification = DistTable.Rows[i - 1]["name"].ToString();
                        allClassificationItem.classificationST = DistTable.Rows[i - 1]["catelog2"].ToString();
                    }
                    categoryGoodsItem.classificationSED.Add(allClassificationItem);
                }
            }
            else
            {
                AllClassificationItem allClassificationItem = new AllClassificationItem();
                allClassificationItem.allclassification = "全部";
                allClassificationItem.classificationST = "";
                categoryGoodsItem.classificationSED.Add(allClassificationItem);
            }

            //品牌、商品信息

            DataTable dtbrandSql = dv.ToTable(true, "brand");

            if (dtbrandSql.Rows.Count > 0)
            {
                categoryGoodsItem.brands.Add("全部");
                foreach (DataRow dr in dtbrandSql.Rows)
                {
                    categoryGoodsItem.brands.Add(dr["brand"].ToString());
                }
            }
            else
            {
                categoryGoodsItem.brands.Add("全部");
            }

            DataTable dtclassificationSEDSql = dv.ToTable(true, "goodsName", "barcode", "slt", "pprice");
            if (dtclassificationSEDSql.Rows.Count > 0)
            {
                for (int i = 0; i < dtclassificationSEDSql.Rows.Count; i++)
                {
                    ChangeGoods changeGoods = new ChangeGoods();
                    changeGoods.goodsName = dtclassificationSEDSql.Rows[i]["goodsName"].ToString();
                    changeGoods.barcode = dtclassificationSEDSql.Rows[i]["barcode"].ToString();
                    changeGoods.imgurl = dtclassificationSEDSql.Rows[i]["slt"].ToString() + "?x-oss-process=image/resize,limit_0,m_fill,w_800,h_800/quality,q_100";

                    if (ifShowPrice)
                    {
                        changeGoods.price = "￥" + dtclassificationSEDSql.Rows[i]["pprice"].ToString();
                    }
                    else
                    {
                        changeGoods.price = "登陆后显示价格";
                    }
                    categoryGoodsItem.changeGoods.Add(changeGoods);
                }
                categoryGoodsItem.pagination.total = dtclassificationSEDSql.Rows.Count;
            }
            categoryGoodsItem.type = "1";
            return categoryGoodsItem;
        }


        /// <summary>
        /// 搜索页接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public CategoryGoodsItem SelectGoods(CategoryGoodsParam categoryGoodsParam, string userId)
        {
            CategoryGoodsItem categoryGoodsItem = new CategoryGoodsItem();
            categoryGoodsItem.brands = new List<string>();
            categoryGoodsItem.classificationSED = new List<AllClassificationItem>();
            categoryGoodsItem.changeGoods = new List<ChangeGoods>();
            categoryGoodsItem.pagination = new Page(categoryGoodsParam.current, categoryGoodsParam.pageSize);
            string brand = "";
            string classificationSED = "";
            string selectall = "";
            string select = "";
            bool sec = false;
            if (categoryGoodsParam.select != null && categoryGoodsParam.select != "")
            {
                categoryGoodsParam.select = ReplaceSQLChar(categoryGoodsParam.select);
                select = " b.goodsName like '%" + categoryGoodsParam.select + "%'";
                sec = true;
                categoryGoodsItem.select = categoryGoodsParam.select;
            }
            if (categoryGoodsParam.classificationSED != null && categoryGoodsParam.classificationSED != "" && categoryGoodsParam.classificationSED != "全部")
            {
                classificationSED = " and c.name='" + categoryGoodsParam.classificationSED + "'";
            }
            else if (sec)
            {
                select += " or c.name like '%" + categoryGoodsParam.select + "%'";
            }
            if (categoryGoodsParam.brand != null && categoryGoodsParam.brand != "" && categoryGoodsParam.brand != "全部")
            {
                brand = " and b.brand='" + categoryGoodsParam.brand + "'";
            }
            else if (sec)
            {
                select += " or b.brand like '%" + categoryGoodsParam.select + "%' ";
            }
            if (select != null && select != "")
            {
                selectall = " and (" + select + ")";
            }

            //判断是否显示价格
            string settingSql = ""
                   + "select settingValue from t_sys_setting where settingCode='B2CSHOWPRICE'";
            bool ifShowPrice = true;
            if (userId != null && userId != "" && userId != "undefined")
            {
                categoryGoodsItem.ifOnload = "1";
                UserDao userDao = new UserDao();
                string userType = userDao.getUserType(userId);
                if (userType == "6" || userType == "7")
                {
                    DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                    if (dtsettingSql.Rows.Count > 0)
                    {
                        ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                    }
                }
            }
            else
            {
                DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                if (dtsettingSql.Rows.Count > 0)
                {
                    ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                }
            }


            //二级分类
            string catelog2Sql = ""
                + " select c.name,b.catelog2,b.goodsName,a.barcode,b.slt,max(a.pprice) pprice,b.brand  "
                + " from t_goods_distributor_price a,t_goods_list b ,t_goods_category c "
                + " where a.barcode=b.barcode and b.catelog2=c.id  and a.pprice>'0' and a.show='1' " + selectall + classificationSED + brand
                + " GROUP BY a.barcode ORDER BY a.id DESC ";

            DataTable dtallSql = DatabaseOperationWeb.ExecuteSelectDS(catelog2Sql, "T").Tables[0];
            DataView dv = new DataView(dtallSql);
            DataTable dtcatelog2Sql = dv.ToTable(true, "name", "catelog2");
            if (dtcatelog2Sql.Rows.Count > 0)
            {
                for (int i = 0; i < dtcatelog2Sql.Rows.Count + 1; i++)
                {
                    AllClassificationItem allClassificationItem = new AllClassificationItem();
                    if (i == 0)
                    {
                        allClassificationItem.allclassification = "全部";
                        allClassificationItem.classificationST = "";
                    }
                    else
                    {
                        allClassificationItem.allclassification = dtcatelog2Sql.Rows[i - 1]["name"].ToString();
                        allClassificationItem.classificationST = dtcatelog2Sql.Rows[i - 1]["catelog2"].ToString();
                    }
                    categoryGoodsItem.classificationSED.Add(allClassificationItem);
                }
            }
            else
            {
                AllClassificationItem allClassificationItem = new AllClassificationItem();
                allClassificationItem.allclassification = "全部";
                allClassificationItem.classificationST = "";
                categoryGoodsItem.classificationSED.Add(allClassificationItem);
            }




            //品牌、商品信息

            DataTable dtbrandSql = dv.ToTable(true, "brand");
            if (dtbrandSql.Rows.Count > 0)
            {
                categoryGoodsItem.brands.Add("全部");
                foreach (DataRow dr in dtbrandSql.Rows)
                {
                    categoryGoodsItem.brands.Add(dr["brand"].ToString());
                }
            }
            else
            {
                categoryGoodsItem.brands.Add("全部");
            }

            DataTable dtclassificationSEDSql = dv.ToTable(true, "goodsName", "barcode", "slt", "pprice");
            if (dtclassificationSEDSql.Rows.Count > 0)
            {
                for (int i = 0; i < dtclassificationSEDSql.Rows.Count; i++)
                {
                    ChangeGoods changeGoods = new ChangeGoods();
                    changeGoods.goodsName = dtclassificationSEDSql.Rows[i]["goodsName"].ToString();
                    changeGoods.barcode = dtclassificationSEDSql.Rows[i]["barcode"].ToString();
                    changeGoods.imgurl = dtclassificationSEDSql.Rows[i]["slt"].ToString() + "?x-oss-process=image/resize,limit_0,m_fill,w_800,h_800/quality,q_100";

                    if (ifShowPrice)
                    {
                        changeGoods.price = "￥" + dtclassificationSEDSql.Rows[i]["pprice"].ToString();
                    }
                    else
                    {
                        changeGoods.price = "登陆后显示价格";
                    }
                    categoryGoodsItem.changeGoods.Add(changeGoods);
                }
                categoryGoodsItem.pagination.total = dtclassificationSEDSql.Rows.Count;
            }
            categoryGoodsItem.type = "1";
            return categoryGoodsItem;
        }

        /// <summary>
        /// 国家馆接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public CountryBrandsGoodsItem CountryGoods(HomePageParam homePageParam, string userId)
        {
            CountryBrandsGoodsItem countryGoodsItem = new CountryBrandsGoodsItem();
            countryGoodsItem.brands = new List<Brands>();
            countryGoodsItem.goods = new List<ChangeGoods>();
            countryGoodsItem.banner = new List<string>();
            //品牌、banner图
            string brandSql = ""
                + " select advname,imgurl,advtype  "
                + " from t_base_adv  "
                + " where advtype in('brands','toCadv') and country='" + homePageParam.country + "' and flag='1' "
                + " group by advname ";
            DataTable dtallSql = DatabaseOperationWeb.ExecuteSelectDS(brandSql, "T").Tables[0];
            DataRow[] dtbrandSql = dtallSql.Select("advtype='brands'");
            if (dtbrandSql.Length > 0)
            {
                for (int i = 0; i < 18 && i < dtbrandSql.Length; i++)
                {
                    Brands brands = new Brands();
                    brands.brandsName = dtbrandSql[i]["advname"].ToString();
                    brands.imgurl = dtbrandSql[i]["imgurl"].ToString();
                    countryGoodsItem.brands.Add(brands);
                }
            }

            DataRow[] dtbannerSql = dtallSql.Select("advtype='toCadv'");
            if (dtbannerSql.Length > 0)
            {
                countryGoodsItem.banner.Add(dtbannerSql[0]["imgurl"].ToString());
            }

            //判断是否显示价格   
            string settingSql = ""
                   + "select settingValue from t_sys_setting where settingCode='B2CSHOWPRICE'";
            bool ifShowPrice = true;
            if (userId != null && userId != "" && userId != "undefined")
            {
                countryGoodsItem.ifOnload = "1";
                UserDao userDao = new UserDao();
                string userType = userDao.getUserType(userId);
                if (userType == "6" || userType == "7")
                {
                    DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                    if (dtsettingSql.Rows.Count > 0)
                    {
                        ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                    }
                }
            }
            else
            {
                DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                if (dtsettingSql.Rows.Count > 0)
                {
                    ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                }
            }
            //商品信息
            string goodsSql = ""
                + " select b.goodsName,a.barcode,b.slt,max(a.pprice) pprice "
                + " from t_goods_distributor_price a,t_goods_list b "
                + " where a.barcode=b.barcode  and b.country='" + homePageParam.country + "'   and a.pprice>'0' and a.show='1'"
                + " GROUP BY a.barcode ORDER BY b.recom DESC  LIMIT 0,30";
            if (homePageParam.country == "中国")
            {
                goodsSql = ""
                    + " select b.goodsName,b.barcode,b.slt,min(a.pprice) pprice "
                    + " from t_goods_distributor_price a,t_goods_list b ,t_base_warehouse d"
                    + " where a.barcode=b.barcode  and  d.id=a.wid  and  d.businessType='0'   and a.pprice>'0'  and a.show='1'"
                    + " GROUP BY a.barcode ORDER BY b.recom DESC  LIMIT 0,30";
            }


            DataTable dtclassificationSEDSql = DatabaseOperationWeb.ExecuteSelectDS(goodsSql, "T").Tables[0];
            if (dtclassificationSEDSql.Rows.Count > 0)
            {
                for (int i = 0; i < dtclassificationSEDSql.Rows.Count; i++)
                {
                    ChangeGoods changeGoods = new ChangeGoods();
                    changeGoods.goodsName = dtclassificationSEDSql.Rows[i]["goodsName"].ToString();
                    changeGoods.barcode = dtclassificationSEDSql.Rows[i]["barcode"].ToString();
                    changeGoods.imgurl = dtclassificationSEDSql.Rows[i]["slt"].ToString() + "?x-oss-process=image/resize,limit_0,m_fill,w_800,h_800/quality,q_100";

                    if (ifShowPrice)
                    {
                        changeGoods.price = "￥" + dtclassificationSEDSql.Rows[i]["pprice"].ToString();
                    }
                    else
                    {
                        changeGoods.price = "登陆后显示价格";
                    }
                    countryGoodsItem.goods.Add(changeGoods);
                }

            }
            countryGoodsItem.type = "1";
            return countryGoodsItem;
        }

        /// <summary>
        /// 品牌页接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public BrandsGoodsItem BrandsGoods(Brands brands, string userId)
        {
            BrandsGoodsItem brandsGoodsItem = new BrandsGoodsItem();
            brandsGoodsItem.goods = new List<ChangeGoods>();
            brandsGoodsItem.pagination = new Page(brands.current, brands.pageSize);
            brandsGoodsItem.advimg = new List<string>();

            //品牌信息
            string brandsSql = ""
                + " select imgurl,imgurl2,advname,remark from t_base_adv "
                + " where advtype='brands' and advname='" + brands.brandsName + "' and flag='1'"
                + " limit 0,1";
            DataTable dtbrandsSql = DatabaseOperationWeb.ExecuteSelectDS(brandsSql, "T").Tables[0];
            if (dtbrandsSql.Rows.Count > 0)
            {
                string[] list = dtbrandsSql.Rows[0]["imgurl2"].ToString().Split(",");
                for (int i = 0; i < list.Length; i++)
                {
                    brandsGoodsItem.advimg.Add(list[i]);
                }
                brandsGoodsItem.brandimg = dtbrandsSql.Rows[0]["imgurl"].ToString();
                brandsGoodsItem.brandName = dtbrandsSql.Rows[0]["advname"].ToString();
                brandsGoodsItem.description = dtbrandsSql.Rows[0]["remark"].ToString();
            }

            //判断是否显示价格
            string settingSql = ""
                    + "select settingValue from t_sys_setting where settingCode='B2CSHOWPRICE'";
            bool ifShowPrice = true;
            if (userId != null && userId != "" && userId != "undefined")
            {
                brandsGoodsItem.ifOnload = "1";
                //判断是否显示价格
                UserDao userDao = new UserDao();
                string userType = userDao.getUserType(userId);
                if (userType == "6" || userType == "7")
                {
                    DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                    if (dtsettingSql.Rows.Count > 0)
                    {
                        ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                    }
                }
                //判断是否关注
                string select = ""
                    + "select id from t_user_collection  where userCode='" + userId + "' and collectionValue='" + brands.brandsName + "' and collectionType='2'";
                DataTable dtselect = DatabaseOperationWeb.ExecuteSelectDS(select, "T").Tables[0];
                if (dtselect.Rows.Count > 0)
                {
                    brandsGoodsItem.attentionType = "1";
                }
            }
            else
            {
                DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                if (dtsettingSql.Rows.Count > 0)
                {
                    ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                }
            }
            //商品信息
            string goodsSql = ""
                + " select a.goodsName,a.barcode,b.slt,max(a.pprice) pprice "
                + " from t_goods_distributor_price a,t_goods_list b "
                + " where a.barcode=b.barcode  and b.brand='" + brands.brandsName + "'   and a.pprice>'0' and a.show='1' "
                + " GROUP BY a.barcode ORDER BY a.id DESC ";// LIMIT 0,30";
            DataTable dtclassificationSEDSql = DatabaseOperationWeb.ExecuteSelectDS(goodsSql, "T").Tables[0];
            if (dtclassificationSEDSql.Rows.Count > 0)
            {
                for (int i = 0; i < dtclassificationSEDSql.Rows.Count; i++)
                {
                    ChangeGoods changeGoods = new ChangeGoods();
                    changeGoods.goodsName = dtclassificationSEDSql.Rows[i]["goodsName"].ToString();
                    changeGoods.barcode = dtclassificationSEDSql.Rows[i]["barcode"].ToString();
                    changeGoods.imgurl = dtclassificationSEDSql.Rows[i]["slt"].ToString() + "?x-oss-process=image/resize,limit_0,m_fill,w_800,h_800/quality,q_100";

                    if (ifShowPrice)
                    {
                        changeGoods.price = "￥" + dtclassificationSEDSql.Rows[i]["pprice"].ToString();
                    }
                    else
                    {
                        changeGoods.price = "登陆后显示价格";
                    }
                    brandsGoodsItem.goods.Add(changeGoods);
                }
                brandsGoodsItem.pagination.total = dtclassificationSEDSql.Rows.Count;
            }
            brandsGoodsItem.type = "1";


            return brandsGoodsItem;
        }

        /// <summary>
        /// 商品详情页接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public NewGoodsDetailsItem GoodsDetails(NewGoodsParam goodsParam, string userId)
        {
            NewGoodsDetailsItem goodsDetailsItem = new NewGoodsDetailsItem();
            goodsDetailsItem.img = new List<string>();
            goodsDetailsItem.goodsParameters = new List<GoodsParameters>();
            goodsDetailsItem.goodsDetailImgArr = new List<string>();
            //判断是否显示价格    
            string settingSql = ""
                   + "select settingValue from t_sys_setting where settingCode='B2CSHOWPRICE'";
            bool ifShowPrice = true;
            if (userId != null && userId != "" && userId != "undefined")
            {
                goodsDetailsItem.ifOnload = "1";
                //判断是否显示价格
                UserDao userDao = new UserDao();
                string userType = userDao.getUserType(userId);
                if (userType == "6" || userType == "7")
                {
                    DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                    if (dtsettingSql.Rows.Count > 0)
                    {
                        ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                    }
                }
                //判断是否关注
                string select = ""
                    + "select id from t_user_collection  where userCode='" + userId + "' and collectionValue='" + goodsParam.barcode + "' and collectionType='1'";
                DataTable dtselect = DatabaseOperationWeb.ExecuteSelectDS(select, "T").Tables[0];
                if (dtselect.Rows.Count > 0)
                {
                    goodsDetailsItem.attentionType = "1";
                }
            }
            else
            {
                DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                if (dtsettingSql.Rows.Count > 0)
                {
                    ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                }
            }

            string goodsSql = ""
                + " select a.efficacy,a.goodsName,a.imgZipUrl,a.model,a.country,a.brand,a.thumb,a.brandTxt,a.content,max(b.pprice) pprice "
                + " from t_goods_list a,t_goods_distributor_price b "
                + " where a.barcode=b.barcode and a.barcode='" + goodsParam.barcode + "' ";
            DataTable dtgoodsSql = DatabaseOperationWeb.ExecuteSelectDS(goodsSql, "T").Tables[0];
            if (dtgoodsSql.Rows.Count > 0)
            {
                goodsDetailsItem.goodsName = dtgoodsSql.Rows[0]["goodsName"].ToString();
                goodsDetailsItem.barcode = goodsParam.barcode;
                if (ifShowPrice)
                {
                    goodsDetailsItem.price = "￥" + dtgoodsSql.Rows[0]["pprice"].ToString();
                    goodsDetailsItem.imgZipUrl = dtgoodsSql.Rows[0]["imgZipUrl"].ToString();
                }
                else
                {
                    goodsDetailsItem.price = "登陆后显示价格";
                }

                goodsDetailsItem.discription = dtgoodsSql.Rows[0]["brandTxt"].ToString(); ;
                for (int j = 0; j < 6; j++)
                {
                    GoodsParameters goodsParameters = new GoodsParameters();
                    goodsDetailsItem.goodsParameters.Add(goodsParameters);
                }
                goodsDetailsItem.goodsParameters[0].key = 1;
                goodsDetailsItem.goodsParameters[0].name = "商品名称(中文)";
                goodsDetailsItem.goodsParameters[0].content = goodsDetailsItem.goodsName;

                goodsDetailsItem.goodsParameters[1].key = 2;
                goodsDetailsItem.goodsParameters[1].name = "品牌";
                goodsDetailsItem.goodsParameters[1].content = dtgoodsSql.Rows[0]["brand"].ToString();

                goodsDetailsItem.goodsParameters[2].key = 3;
                goodsDetailsItem.goodsParameters[2].name = "进口国";
                goodsDetailsItem.goodsParameters[2].content = dtgoodsSql.Rows[0]["country"].ToString();
                goodsDetailsItem.goodsParameters[3].key = 4;
                goodsDetailsItem.goodsParameters[3].name = "规格";
                goodsDetailsItem.goodsParameters[3].content = dtgoodsSql.Rows[0]["model"].ToString();
                goodsDetailsItem.goodsParameters[4].key = 5;
                goodsDetailsItem.goodsParameters[4].name = "生产商";
                goodsDetailsItem.goodsParameters[4].content = dtgoodsSql.Rows[0]["brand"].ToString();
                goodsDetailsItem.goodsParameters[5].key = 6;
                goodsDetailsItem.goodsParameters[5].name = "产品功效";
                goodsDetailsItem.goodsParameters[5].content = dtgoodsSql.Rows[0]["efficacy"].ToString();

                string[] list = dtgoodsSql.Rows[0]["thumb"].ToString().Split(",");
                for (int i = 0; i < list.Length; i++)
                {
                    goodsDetailsItem.img.Add(list[i]);
                }
                string[] list1 = dtgoodsSql.Rows[0]["content"].ToString().Split(",");
                for (int i = 0; i < list1.Length; i++)
                {
                    goodsDetailsItem.goodsDetailImgArr.Add(list1[i]);
                }


                goodsDetailsItem.type = "1";
            }
            return goodsDetailsItem;
        }

        /// 过滤SQL字符。
        /// </summary>
        /// <param name="str">要过滤SQL字符的字符串。</param>
        /// <returns>已过滤掉SQL字符的字符串。</returns>
        public string ReplaceSQLChar(string str)
        {
            if (str == String.Empty)
                return String.Empty;
            str = str.Replace("'", "‘");
            str = str.Replace(";", "；");
            str = str.Replace(",", ",");
            str = str.Replace("?", "?");
            str = str.Replace("<", "＜");
            str = str.Replace(">", "＞");
            str = str.Replace("(", "(");
            str = str.Replace(")", ")");
            str = str.Replace("@", "＠");
            str = str.Replace("=", "＝");
            str = str.Replace("+", "＋");
            str = str.Replace("*", "＊");
            str = str.Replace("&", "＆");
            str = str.Replace("#", "＃");
            str = str.Replace("%", "％");
            str = str.Replace("$", "￥");
            return str;
        }


        /// <summary>
        /// 添加取消收藏接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public MsgResult UserCollection(UserCollectionParam userCollectionParam, string userId)
        {
            MsgResult msgResult = new MsgResult();

            if (userCollectionParam.type == "1")
            {
                string select = ""
                    + "select id from t_user_collection where userCode='" + userId + "' and collectionType='" + userCollectionParam.collectionType + "' and  collectionValue ='" + userCollectionParam.collectionValue + "'";
                DataTable dtselect = DatabaseOperationWeb.ExecuteSelectDS(select, "T").Tables[0];
                if (dtselect.Rows.Count > 0)
                {
                    if (userCollectionParam.collectionType == "1")
                    {
                        msgResult.msg = "已收藏";
                    }
                    else
                    {
                        msgResult.msg = "已关注";
                    }
                    return msgResult;
                }
                string insert = ""
                    + " insert into t_user_collection(userCode,collectionType,collectionValue) "
                    + " values('" + userId + "','" + userCollectionParam.collectionType + "','" + userCollectionParam.collectionValue + "')";
                if (DatabaseOperationWeb.ExecuteDML(insert))
                {
                    msgResult.type = "1";
                }
            }
            else if (userCollectionParam.type == "0")
            {
                string delete = ""
                    + " delete from t_user_collection "
                    + " where userCode='" + userId + "' and collectionType='" + userCollectionParam.collectionType + "' and  collectionValue ='" + userCollectionParam.collectionValue + "'";
                if (DatabaseOperationWeb.ExecuteDML(delete))
                {
                    msgResult.type = "1";
                }
            }
            return msgResult;
        }

        /// <summary>
        /// 收藏商品接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public UserCollectionGoodsItem UserCollectionGoods(UserCollectionGoodsParam userCollectionGoodsParam, string userId)
        {
            UserCollectionGoodsItem userCollectionGoodsItem = new UserCollectionGoodsItem();
            userCollectionGoodsItem.goodsList = new List<ChangeGoods>();
            userCollectionGoodsItem.pagination = new Page(userCollectionGoodsParam.current, userCollectionGoodsParam.pageSize);
            try
            {
                //判断是否显示价格   
                string settingSql = ""
                       + "select settingValue from t_sys_setting where settingCode='B2CSHOWPRICE'";
                bool ifShowPrice = true;
                if (userId != null && userId != "" && userId != "undefined")
                {
                    userCollectionGoodsItem.ifOnload = "1";
                    UserDao userDao = new UserDao();
                    string userType = userDao.getUserType(userId);
                    if (userType == "6" || userType == "7")
                    {
                        DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                        if (dtsettingSql.Rows.Count > 0)
                        {
                            ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                        }
                    }
                }
                else
                {
                    DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                    if (dtsettingSql.Rows.Count > 0)
                    {
                        ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                    }
                }

                string select = ""
                    + "select a.goodsName,a.slt,max(a.pprice) pprice,a.barcode "
                    + " from t_goods_distributor_price  a,t_user_collection b "
                    + " where a.barcode=b.collectionValue  and  b.collectionType='1' and b.userCode='" + userId + "' "
                    + " group by a.barcode "
                    + " order by b.id desc ";
                DataTable dtselect = DatabaseOperationWeb.ExecuteSelectDS(select, "T").Tables[0];
                if (dtselect.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtselect.Rows)
                    {
                        ChangeGoods changeGoods = new ChangeGoods();
                        changeGoods.goodsName = dr["goodsName"].ToString();
                        changeGoods.barcode = dr["barcode"].ToString();
                        if (ifShowPrice)
                        {
                            changeGoods.price = "￥" + dr["pprice"].ToString();
                        }
                        else
                        {
                            changeGoods.price = "登陆后显示价格";
                        }

                        changeGoods.imgurl = dr["slt"].ToString();
                        userCollectionGoodsItem.goodsList.Add(changeGoods);
                    }
                }
                userCollectionGoodsItem.type = "1";
                userCollectionGoodsItem.pagination.total = dtselect.Rows.Count;
            }
            catch (Exception ex)
            {
                userCollectionGoodsItem.type = "0";
            }
            return userCollectionGoodsItem;
        }

        /// <summary>
        /// 关注品牌展示接口
        /// </summary>
        /// <param name="param">查询条件</param>
        /// <returns></returns>
        public UserCollectionBrandsItem UserCollectionBrands(UserCollectionGoodsParam userCollectionGoodsParam, string userId)
        {
            UserCollectionBrandsItem userCollectionBrandsItem = new UserCollectionBrandsItem();
            userCollectionBrandsItem.brandsList = new List<UserCollectionBrandsList>();
            userCollectionBrandsItem.pagination = new Page(userCollectionGoodsParam.current, userCollectionGoodsParam.pageSize);
            try
            {
                //判断是否显示价格   
                string settingSql = ""
                       + "select settingValue from t_sys_setting where settingCode='B2CSHOWPRICE'";
                bool ifShowPrice = true;
                userCollectionBrandsItem.ifOnload = "1";
                UserDao userDao = new UserDao();
                string userType = userDao.getUserType(userId);
                if (userType == "6" || userType == "7")
                {
                    DataTable dtsettingSql = DatabaseOperationWeb.ExecuteSelectDS(settingSql, "T").Tables[0];
                    if (dtsettingSql.Rows.Count > 0)
                    {
                        ifShowPrice = (dtsettingSql.Rows[0][0].ToString() == "1");
                    }
                }
                //查询品牌
                string selectBrands = ""
                    + "select a.advname,a.imgurl from t_base_adv a ,t_user_collection b"
                    + " where a.advname=b.collectionValue and  a.advtype='brands' and b.userCode='" + userId + "' and b.collectionType='2'"
                    + " order by a.id desc";
                DataTable dtselectBrands = DatabaseOperationWeb.ExecuteSelectDS(selectBrands, "T").Tables[0];
                if (dtselectBrands.Rows.Count > 0)
                {
                    //查询商品
                    string selectGoods = ""
                        + "select a.goodsName,a.slt,max(a.pprice) pprice,a.barcode,b.brand "
                        + " from t_goods_distributor_price  a ,t_goods_list b"
                        + " where a.barcode=b.barcode and a.pprice >'0' "
                        + " group by a.barcode ";
                    DataTable dtselectGoods = DatabaseOperationWeb.ExecuteSelectDS(selectGoods, "T").Tables[0];
                    for (int i = 0; i < dtselectBrands.Rows.Count; i++)
                    {
                        UserCollectionBrandsList userCollectionBrandsList = new UserCollectionBrandsList();
                        userCollectionBrandsList.goodsList = new List<ChangeGoods>();
                        userCollectionBrandsList.slt = dtselectBrands.Rows[i]["imgurl"].ToString();
                        userCollectionBrandsList.brand = dtselectBrands.Rows[i]["advname"].ToString();
                        userCollectionBrandsItem.pagination.total = dtselectBrands.Rows.Count;
                        DataRow[] dataRows = dtselectGoods.Select("brand='" + userCollectionBrandsList.brand + "'");
                        if (dataRows.Length > 0)
                        {
                            for (int j = 0; j < 5 && j < dataRows.Length; j++)
                            {
                                ChangeGoods changeGoods = new ChangeGoods();
                                changeGoods.goodsName = dataRows[j]["goodsName"].ToString();
                                changeGoods.barcode = dataRows[j]["barcode"].ToString();
                                if (ifShowPrice)
                                {
                                    changeGoods.price = "￥" + dataRows[j]["pprice"].ToString();
                                }
                                else
                                {
                                    changeGoods.price = "登陆后显示价格";
                                }

                                changeGoods.imgurl = dataRows[j]["slt"].ToString();
                                userCollectionBrandsList.goodsList.Add(changeGoods);
                            }
                        }
                        userCollectionBrandsItem.brandsList.Add(userCollectionBrandsList);

                    }
                }
                userCollectionBrandsItem.type = "1";
            }
            catch (Exception ex)
            {
                userCollectionBrandsItem.type = "0";
            }
            return userCollectionBrandsItem;
        }
    }
}
