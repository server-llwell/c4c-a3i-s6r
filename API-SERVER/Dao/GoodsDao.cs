using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Dao
{
    public class GoodsDao
    {
        private string path = System.Environment.CurrentDirectory;

        public GoodsDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        public List<BrandItem> GetBrand()
        {
            List<BrandItem> brandList = new List<BrandItem>();
            string sql1 = "select brand from t_goods_list where brand is not null and brand <>  ''  GROUP BY brand ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_daigou_ticket").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BrandItem brandItem = new BrandItem();
                brandItem.brand = dt.Rows[i]["brand"].ToString();
                brandList.Add(brandItem);
            }
            return brandList;
        }

        public List<BrandItem> GetBrand(string userId)
        {
            List<BrandItem> brandList = new List<BrandItem>();
            string sql1 = "select brand from t_goods_list where brand is not null and brand <>  '' and supplierCode = '" + userId + "' GROUP BY brand ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_daigou_ticket").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BrandItem brandItem = new BrandItem();
                brandItem.brand = dt.Rows[i]["brand"].ToString();
                brandList.Add(brandItem);
            }
            return brandList;
        }

        public bool UpdateStatus(TicketParam ticketParam)
        {
            string sql = "UPDATE t_daigou_ticket set remark ='"+ ticketParam.remark1 + "', status ='"+ ticketParam.status1 + "'" +
                "  WHERE ticketCode ='" + ticketParam.ticketCode + "'";
            return DatabaseOperationWeb.ExecuteDML(sql);
        }
    }
}
