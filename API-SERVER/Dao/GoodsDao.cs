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
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
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
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BrandItem brandItem = new BrandItem();
                brandItem.brand = dt.Rows[i]["brand"].ToString();
                brandList.Add(brandItem);
            }
            return brandList;
        }

        public List<WarehouseItem> GetWarehouse()
        {
            List<WarehouseItem> warehouseList = new List<WarehouseItem>();
            string sql1 = "select w.id,W.wcode,w.wname from t_goods_list g ,t_goods_warehouse gw,t_base_warehouse w where g.barcode = gw.barcode  and GW.wid = w.id   group by w.id,W.wcode,w.wname ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                WarehouseItem warehouseItem = new WarehouseItem();
                warehouseItem.wid = dt.Rows[i]["id"].ToString();
                warehouseItem.wcode = dt.Rows[i]["wcode"].ToString();
                warehouseItem.wname = dt.Rows[i]["wname"].ToString();
                warehouseList.Add(warehouseItem);
            }
            return warehouseList;
        }

        public List<WarehouseItem> GetWarehouse(string userId)
        {
            List<WarehouseItem> warehouseList = new List<WarehouseItem>();
            string sql1 = "select w.id,W.wcode,w.wname from t_goods_list g ,t_goods_warehouse gw,t_base_warehouse w where g.barcode = gw.barcode  and GW.wid = w.id and gw.supplierCode = '" + userId + "'  group by w.id,W.wcode,w.wname ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                WarehouseItem warehouseItem = new WarehouseItem();
                warehouseItem.wid = dt.Rows[i]["id"].ToString();
                warehouseItem.wcode = dt.Rows[i]["wcode"].ToString();
                warehouseItem.wname = dt.Rows[i]["wname"].ToString();
                warehouseList.Add(warehouseItem);
            }
            return warehouseList;
        }

        public PageResult GetGoodsList(GoodsSeachParam goodsSeachParam)
        {
            PageResult goodsResult = new PageResult();
            goodsResult.pagination = new Page(goodsSeachParam.current, goodsSeachParam.pageSize);
            goodsResult.list = new List<Object>();
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(goodsSeachParam.userId);
            string st = "";
            if (userType == "1")//供应商 
            {
                st += " and wh.suppliercode ='" + goodsSeachParam.userId + "' ";
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {

            }
            else
            {
                return goodsResult;
            }
            //状态
            if (goodsSeachParam.status == "上架")
            {
                st += " and wh.flag='1' ";
            }
            else if (goodsSeachParam.status == "下架")
            {
                st += " and wh.flag='0' ";
            }
            else if (goodsSeachParam.status == "申请中")
            {
                st += " and wh.status='1' ";
            }
            else if (goodsSeachParam.status == "已驳回")
            {
                st += " and wh.status='2' ";
            }
            if (goodsSeachParam.wid != null && goodsSeachParam.wid != "")
            {
                st += " and wh.wid='" + goodsSeachParam.wid + "' ";
            }
            if (goodsSeachParam.goodsName != null && goodsSeachParam.goodsName != "")
            {
                st += " and g.goodsName like '%" + goodsSeachParam.goodsName + "%' ";
            }
            if (goodsSeachParam.brand != null && goodsSeachParam.brand != "")
            {
                st += " and g.brand='" + goodsSeachParam.brand + "' ";
            }
            if (goodsSeachParam.barcode != null && goodsSeachParam.barcode != "")
            {
                st += " and wh.barcode like '%" + goodsSeachParam.barcode + "%' ";
            }
            string sql = "select g.id,g.brand,g.goodsName,g.barcode,g.slt,g.source,w.wname,wh.goodsnum,wh.flag,wh.`status`,wh.suppliercode " +
                "from t_goods_list g ,t_goods_warehouse wh,t_base_warehouse w " +
                "where g.id = wh.goodsid and  wh.wid = w.id  order by g.brand,g.barcode  LIMIT " + (goodsSeachParam.current - 1) * goodsSeachParam.pageSize + "," + goodsSeachParam.pageSize ;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GoodsListItem goodsListItem = new GoodsListItem();
                goodsListItem.id = dt.Rows[i]["id"].ToString();
                goodsListItem.brand = dt.Rows[i]["brand"].ToString();
                goodsListItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                goodsListItem.barcode = dt.Rows[i]["barcode"].ToString();
                goodsListItem.slt = dt.Rows[i]["slt"].ToString();
                goodsListItem.source = dt.Rows[i]["source"].ToString();
                goodsListItem.flag = dt.Rows[i]["flag"].ToString();
                goodsListItem.wname = dt.Rows[i]["wname"].ToString();
                goodsListItem.goodsnum = dt.Rows[i]["goodsnum"].ToString();
                goodsListItem.status = dt.Rows[i]["status"].ToString();
                string st1 = "";
                if (userType == "1")//供应商 
                {
                    st1 = " and g.suppliercode ='" + goodsSeachParam.userId + "' ";
                }
                string sqlWeek = "select ifnull(sum(g.quantity),0) from t_order_list l,t_order_goods g " +
                    "where l.merchantOrderId = g.merchantOrderId and l.tradeTime BETWEEN  DATE_ADD(now(),INTERVAL -7 DAY) AND now() " +
                    "and g.barCode = '"+ dt.Rows[i]["barcode"].ToString() + "'  " + st1;
                DataTable dtWeek = DatabaseOperationWeb.ExecuteSelectDS(sqlWeek, "t_goods_list").Tables[0];
                goodsListItem.week = Convert.ToInt16(dtWeek.Rows[0][0]);

                string sqlMonth = "select ifnull(sum(g.quantity),0) from t_order_list l,t_order_goods g " +
                    "where l.merchantOrderId = g.merchantOrderId and l.tradeTime BETWEEN  DATE_ADD(now(),INTERVAL -30 DAY) AND now() " +
                    "and g.barCode = '" + dt.Rows[i]["barcode"].ToString() + "'  " + st1;
                DataTable dtMonth = DatabaseOperationWeb.ExecuteSelectDS(sqlMonth, "t_goods_list").Tables[0];
                goodsListItem.month = Convert.ToInt16(dtMonth.Rows[0][0]);

                goodsResult.list.Add(goodsListItem);
            }
            return goodsResult;
        }
    }
}
