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
                string sql1 = "select id,usercode,username,u.platformId,p.platformType,priceType,platformCost,platformCostType " +
                         "from t_user_list u LEFT JOIN t_base_platform p on u.platformId = p.platformId where u.usertype='2' ";

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DistributorItem distributorItem = new DistributorItem();
                    distributorItem.id = dt.Rows[i]["id"].ToString();
                    distributorItem.usercode = dt.Rows[0]["usercode"].ToString();
                    distributorItem.username = dt.Rows[i]["username"].ToString();
                    try
                    {
                        distributorItem.platformCost = Convert.ToDouble(dt.Rows[i]["platformCost"]);
                    }
                    catch (Exception)
                    {
                    }
                    distributorItem.platformId = dt.Rows[i]["platformId"].ToString();
                    distributorItem.platformType = dt.Rows[i]["platformType"].ToString();
                    distributorItem.priceType = dt.Rows[i]["priceType"].ToString();
                    distributorItem.platformCostType = dt.Rows[i]["platformCostType"].ToString();

                    pageResult.list.Add(distributorItem);
                }
            }
            return pageResult;
        }
    }
}
