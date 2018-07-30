using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;

namespace API_SERVER.Dao
{
    public class DistributorDao
    {
        private string path = System.Environment.CurrentDirectory;

        public DistributorDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        #region 渠道商费用
        /// <summary>
        /// 获取渠道商类型下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<PlatformItem> getPlatform()
        {
            List<PlatformItem> lp = new List<PlatformItem>();
            string sql = "select platformId,platformType from t_base_platform";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PlatformItem platformItem = new PlatformItem();
                    platformItem.platformId = dt.Rows[i]["platformId"].ToString();
                    platformItem.platformType = dt.Rows[i]["platformType"].ToString();
                    lp.Add(platformItem);
                }
            }
            return lp;
        }

        /// <summary>
        /// 修改渠道商费用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult updateDistributor(DistributorItem distributorItem)
        {
            MsgResult msg = new MsgResult();
            try
            {
                string sql = "update t_user_list set platformId=" + distributorItem.platformId + "," +
                             "priceType='" + distributorItem.priceType + "'," +
                             "platformCost=" + distributorItem.platformCost + "," +
                             "platformCostType='" + distributorItem.platformCostType + "' " +
                             "where id=" + distributorItem.id+ " and usertype='2'";
                if (DatabaseOperationWeb.ExecuteDML(sql))
                {
                    msg.type = "1";
                    msg.msg = "修改成功";
                }
            }
            catch (Exception)
            {
                msg.msg = "数据库处理失败";
            }
            return msg;
        }

        /// <summary>
        /// 获取渠道商费用列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult getDistributorList(DistributorParam distributorParam)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(distributorParam.current, distributorParam.pageSize);
            pageResult.list = new List<Object>();
            string sql = "select id,usercode,username,u.platformId,p.platformType,priceType,platformCost,platformCostType " +
                         "from t_user_list u LEFT JOIN t_base_platform p on u.platformId = p.platformId where u.usertype='2' " +
                         " ORDER BY id asc LIMIT " + (distributorParam.current - 1) * distributorParam.pageSize + "," + distributorParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql1 = "select count(*) from t_user_list u LEFT JOIN t_base_platform p on u.platformId = p.platformId " +
                              "where u.usertype='2' ";

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DistributorItem distributorItem = new DistributorItem();
                    distributorItem.id = dt.Rows[i]["id"].ToString();
                    distributorItem.usercode = dt.Rows[0]["usercode"].ToString();
                    distributorItem.username = dt.Rows[i]["username"].ToString();
                    if (!double.TryParse(dt.Rows[i]["platformCost"].ToString(), out distributorItem.platformCost))
                    {
                        distributorItem.platformCost = 0;
                    }
                    //try
                    //{
                    //    distributorItem.platformCost = Convert.ToDouble(dt.Rows[i]["platformCost"]);
                    //}
                    //catch (Exception)
                    //{
                    //}
                    distributorItem.platformId = dt.Rows[i]["platformId"].ToString();
                    distributorItem.platformType = dt.Rows[i]["platformType"].ToString();
                    distributorItem.priceType = dt.Rows[i]["priceType"].ToString();
                    distributorItem.platformCostType = dt.Rows[i]["platformCostType"].ToString();

                    pageResult.list.Add(distributorItem);
                }
            }
            return pageResult;
        }
        #endregion

        #region 渠道商商品

        /// <summary>
        /// 获取渠道商商品列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult getDGoodsList(DistributorParam distributorParam)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(distributorParam.current, distributorParam.pageSize);
            pageResult.list = new List<Object>();

            string st = "";
            if (distributorParam.purchase!=null && distributorParam.purchase != "")
            {
                st = " and g.usercode='" + distributorParam.purchase + "' ";
            }
            string sql = "select g.*, p.platformType,u.username as suppliername,ul.username as purchase " +
                         "from t_user_list ul ,t_base_platform p, t_goods_distributor_price g LEFT JOIN t_user_list u on g.supplierid = u.id " +
                         "where g.platformId = p.platformId  and ul.usercode = g.usercode " + st+
                         " ORDER BY g.id asc LIMIT " + (distributorParam.current - 1) * distributorParam.pageSize + "," + distributorParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql1 = "select count(*)  from t_goods_distributor_price g where 1=1 " + st;

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DistributorGoodsItem distributorGoodsItem = new DistributorGoodsItem();
                    distributorGoodsItem.id = dt.Rows[i]["id"].ToString();
                    distributorGoodsItem.usercode = dt.Rows[i]["usercode"].ToString();
                    distributorGoodsItem.purchase = dt.Rows[i]["purchase"].ToString();
                    distributorGoodsItem.goodsid = dt.Rows[i]["goodsid"].ToString();
                    distributorGoodsItem.barcode = dt.Rows[i]["barcode"].ToString();
                    distributorGoodsItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                    distributorGoodsItem.slt = dt.Rows[i]["slt"].ToString();
                    distributorGoodsItem.platformId = dt.Rows[i]["platformId"].ToString();
                    distributorGoodsItem.platformType = dt.Rows[i]["platformType"].ToString();
                    distributorGoodsItem.pprice = Convert.ToDouble(dt.Rows[i]["pprice"].ToString());
                    distributorGoodsItem.pNum =Convert.ToDouble( dt.Rows[i]["pNum"]);
                    distributorGoodsItem.supplierid = dt.Rows[i]["supplierid"].ToString();
                    distributorGoodsItem.suppliername = dt.Rows[i]["suppliername"].ToString();
                    distributorGoodsItem.profitPlatform = Convert.ToDouble(dt.Rows[i]["profitPlatform"]);
                    distributorGoodsItem.profitAgent = Convert.ToDouble(dt.Rows[i]["profitAgent"]);
                    distributorGoodsItem.profitDealer = Convert.ToDouble(dt.Rows[i]["profitDealer"]);
                    distributorGoodsItem.profitOther1 = Convert.ToDouble(dt.Rows[i]["profitOther1"]);
                    distributorGoodsItem.profitOther1Name = dt.Rows[i]["profitOther1Name"].ToString();
                    distributorGoodsItem.profitOther2 = Convert.ToDouble(dt.Rows[i]["profitOther2"]);
                    distributorGoodsItem.profitOther2Name = dt.Rows[i]["profitOther2Name"].ToString();
                    distributorGoodsItem.profitOther3 = Convert.ToDouble(dt.Rows[i]["profitOther3"]);
                    distributorGoodsItem.profitOther3Name = dt.Rows[i]["profitOther3Name"].ToString();

                    pageResult.list.Add(distributorGoodsItem);
                }
            }
            return pageResult;
        }

        /// <summary>
        /// 修改渠道商商品费用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult updateDGoods(DistributorGoodsItem distributorGoodsItem)
        {
            MsgResult msg = new MsgResult();
            try
            {
                double sum = distributorGoodsItem.profitPlatform + distributorGoodsItem.profitAgent + distributorGoodsItem.profitDealer
                           + distributorGoodsItem.profitOther1 + distributorGoodsItem.profitOther2 + distributorGoodsItem.profitOther3;
                if (sum!=100)
                {
                    msg.msg = "利润分成总和不是100";
                    return msg;
                }
                if (distributorGoodsItem.supplierid == null)
                {
                    distributorGoodsItem.supplierid = "";
                }
                if (distributorGoodsItem.profitOther1Name == null)
                {
                    distributorGoodsItem.profitOther1Name = "";
                }
                if (distributorGoodsItem.profitOther2Name == null)
                {
                    distributorGoodsItem.profitOther2Name = "";
                }
                if (distributorGoodsItem.profitOther3Name == null)
                {
                    distributorGoodsItem.profitOther3Name = "";
                }

                string sql = "update t_goods_distributor_price set pprice=" + distributorGoodsItem.pprice + "," +
                             "pNum='" + distributorGoodsItem.pNum + "'," +
                             "supplierid='" + distributorGoodsItem.supplierid + "'," +
                             "profitPlatform=" + distributorGoodsItem.profitPlatform + "," +
                             "profitAgent=" + distributorGoodsItem.profitAgent + "," +
                             "profitDealer=" + distributorGoodsItem.profitDealer + "," +
                             "profitOther1=" + distributorGoodsItem.profitOther1 + "," +
                             "profitOther1Name='" + distributorGoodsItem.profitOther1Name + "'," +
                             "profitOther2=" + distributorGoodsItem.profitOther2 + "," +
                             "profitOther2Name='" + distributorGoodsItem.profitOther2Name + "'," +
                             "profitOther3=" + distributorGoodsItem.profitOther3 + "," +
                             "profitOther3Name='" + distributorGoodsItem.profitOther3Name + "' "+
                             "where id=" + distributorGoodsItem.id ;
                if (DatabaseOperationWeb.ExecuteDML(sql))
                {
                    msg.type = "1";
                    msg.msg = "修改成功";
                }
            }
            catch (Exception)
            {
                msg.msg = "数据库处理失败";
            }
            return msg;
        }

        #endregion
    }
}
