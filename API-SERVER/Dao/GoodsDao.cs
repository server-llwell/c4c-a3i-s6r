﻿using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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
        #region 商品库存列表
        /// <summary>
        /// 获取品牌下拉框
        /// </summary>
        /// <returns></returns>
        public List<BrandItem> GetBrand()
        {
            List<BrandItem> brandList = new List<BrandItem>();
            string sql1 = "select a.brand from t_goods_list a,t_goods_warehouse b where  b.barcode=a.barcode and a.brand is not null and a.brand <>''  GROUP BY brand ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BrandItem brandItem = new BrandItem();
                brandItem.brand = dt.Rows[i]["brand"].ToString();
                brandList.Add(brandItem);
            }
            return brandList;
        }

        /// <summary>
        /// 获取品牌下拉框
        /// </summary>
        /// <returns></returns>
        public List<BrandItem> GetBrand(string userId)
        {
            List<BrandItem> brandList = new List<BrandItem>();
            string sql1 = "select a.brand from t_goods_list a,t_goods_warehouse b where  b.barcode=a.barcode and a.brand is not null and a.brand <>''  and b.supplierCode = '" + userId + "' GROUP BY brand ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BrandItem brandItem = new BrandItem();
                brandItem.brand = dt.Rows[i]["brand"].ToString();
                brandList.Add(brandItem);
            }
            return brandList;
        }

        /// <summary>
        /// 获仓库下拉框
        /// </summary>
        /// <returns></returns>
        public List<WarehouseItem> GetWarehouse()
        {
            List<WarehouseItem> warehouseList = new List<WarehouseItem>();
            string sql1 = "select w.id,W.wcode,w.wname from t_base_warehouse w ";
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

        /// <summary>
        /// 获仓库下拉框
        /// </summary>
        /// <returns></returns>
        public List<WarehouseItem> GetWarehouse(string userId)
        {
            List<WarehouseItem> warehouseList = new List<WarehouseItem>();
            string sql1 = "select w.id,W.wcode,w.wname from t_base_warehouse w where w.supplierCode = '" + userId + "'";
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

        /// <summary>
        /// 获商品列表-- 供应商
        /// </summary>
        /// <returns></returns>
        public PageResult GetGoodsListForSupplier(GoodsSeachParam goodsSeachParam)
        {
            PageResult goodsResult = new PageResult();
            goodsResult.pagination = new Page(goodsSeachParam.current, goodsSeachParam.pageSize);
            goodsResult.list = new List<Object>();

            string st = "";
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
            if (goodsSeachParam.ifph != null && goodsSeachParam.ifph != "")
            {
                st += " and wh.ifph='" + goodsSeachParam.ifph + "'";
            }
            string sql = "select wh.ifph,g.id,g.brand,wh.inprice,g.goodsName,g.barcode,g.slt,g.source,w.wname,wh.goodsnum,wh.flag,wh.`status`,u.username " +
                    " from t_goods_list g ,t_goods_warehouse wh,t_base_warehouse w ,t_user_list u " +
                    " where g.id = wh.goodsid and  wh.wid = w.id and wh.suppliercode = u.usercode  " +
                    " and wh.suppliercode ='" + goodsSeachParam.userId + "' " + st +
                    " order by g.brand,g.barcode  LIMIT " + (goodsSeachParam.current - 1) * goodsSeachParam.pageSize + "," + goodsSeachParam.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GoodsListItem goodsListItem = new GoodsListItem();
                goodsListItem.keyId = Convert.ToString((goodsSeachParam.current - 1) * goodsSeachParam.pageSize + i + 1);
                goodsListItem.id = dt.Rows[i]["id"].ToString();
                goodsListItem.brand = dt.Rows[i]["brand"].ToString();
                goodsListItem.supplier = dt.Rows[i]["username"].ToString();
                goodsListItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                goodsListItem.barcode = dt.Rows[i]["barcode"].ToString();
                goodsListItem.slt = dt.Rows[i]["slt"].ToString();
                goodsListItem.wname = dt.Rows[i]["wname"].ToString();
                goodsListItem.goodsnum = dt.Rows[i]["goodsnum"].ToString();
                goodsListItem.price = dt.Rows[i]["inprice"].ToString();
                goodsListItem.ifph= dt.Rows[i]["ifph"].ToString();
                goodsResult.list.Add(goodsListItem);
            }
            string sql1 = "select count(*) from t_goods_list g ,t_goods_warehouse wh,t_base_warehouse w " +
                "where g.id = wh.goodsid and  wh.wid = w.id and wh.suppliercode ='" + goodsSeachParam.userId + "' " + st;

            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
            goodsResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return goodsResult;
        }
        /// <summary>
        /// 获取商品列表--运营
        /// </summary>
        /// <param name="goodsSeachParam"></param>
        /// <returns></returns>
        public PageResult GetGoodsListForOperator(GoodsSeachParam goodsSeachParam)
        {
            PageResult goodsResult = new PageResult();
            goodsResult.pagination = new Page(goodsSeachParam.current, goodsSeachParam.pageSize);
            goodsResult.list = new List<Object>();

            string st = "";
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
            string sql = "select g.id,g.brand,g.goodsName,g.barcode,g.slt,g.supplierId,g.supplierCode,sum(wh.goodsnum) as goodsnum " +
                    " from t_goods_list g ,t_goods_warehouse wh,t_base_warehouse w ,t_user_list u " +
                    " where g.id = wh.goodsid and  wh.wid = w.id and wh.suppliercode = u.usercode " + st +
                    " group by g.id,g.brand,g.goodsName,g.barcode,g.slt,g.supplierId,g.supplierCode" +
                    " order by g.brand,g.barcode  LIMIT " + (goodsSeachParam.current - 1) * goodsSeachParam.pageSize + "," + goodsSeachParam.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GoodsListItem goodsListItem = new GoodsListItem();
                goodsListItem.keyId = Convert.ToString((goodsSeachParam.current - 1) * goodsSeachParam.pageSize + i + 1);
                goodsListItem.id = dt.Rows[i]["id"].ToString();
                goodsListItem.brand = dt.Rows[i]["brand"].ToString();
                //goodsListItem.supplier = dt.Rows[i]["username"].ToString();
                goodsListItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                goodsListItem.barcode = dt.Rows[i]["barcode"].ToString();
                goodsListItem.slt = dt.Rows[i]["slt"].ToString();
                //goodsListItem.source = dt.Rows[i]["source"].ToString();
                //goodsListItem.flag = dt.Rows[i]["flag"].ToString();
                //goodsListItem.wname = dt.Rows[i]["wname"].ToString();
                goodsListItem.goodsnum = dt.Rows[i]["goodsnum"].ToString();
                //goodsListItem.status = dt.Rows[i]["status"].ToString();
                if (dt.Rows[i]["supplierId"].ToString()!="")
                {
                    //获取默认供应商等信息
                    string selSql = "select w.inprice,w.goodsnum,u.username " +
                                    "from t_goods_warehouse w LEFT JOIN t_user_list u on u.id = w.supplierid " +
                                    "where w.supplierId ='" + dt.Rows[i]["supplierId"].ToString() + "' " +
                                    "and w.barcode = '" + dt.Rows[i]["barcode"].ToString() + "' and w.goodsnum >0 " +
                                    "order by w.inprice asc";
                    DataTable selDt = DatabaseOperationWeb.ExecuteSelectDS(selSql, "t_goods_list").Tables[0];
                    if (selDt.Rows.Count > 0)
                    {
                        goodsListItem.selGoodsNum = selDt.Rows[0]["goodsnum"].ToString();
                        goodsListItem.selPrice = selDt.Rows[0]["inprice"].ToString();
                        goodsListItem.selSupplier = selDt.Rows[0]["username"].ToString();
                    }
                }
                
                goodsResult.list.Add(goodsListItem);
            }
            string sql1 = "select count(*) from (select g.id from t_goods_list g ,t_goods_warehouse wh,t_base_warehouse w " +
                "where g.id = wh.goodsid and  wh.wid = w.id  " + st +" group by g.id) x";

            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
            goodsResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return goodsResult;
        }

        /// <summary>
        /// 获商品列表 - 代理和渠道商
        /// </summary>
        /// <returns></returns>
        public PageResult GetGoodsListForAgent(GoodsSeachParam goodsSeachParam)
        {
            PageResult goodsResult = new PageResult();
            goodsResult.pagination = new Page(goodsSeachParam.current, goodsSeachParam.pageSize);
            goodsResult.list = new List<Object>();

            string st = "";
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
                st += " and g.barcode like '%" + goodsSeachParam.barcode + "%' ";
            }              
            if (goodsSeachParam.businessType != null && goodsSeachParam.businessType != "")
            {
                st += " and a.businessType like '%" + goodsSeachParam.businessType + "%' ";
            }
            string sql = "select g.id,g.brand,g.goodsName,g.barcode,g.slt,g.supplierId,g.supplierCode,p.pprice,sum(IFNULL(w.goodsnum,0)) goodsnum ,a.businessType" +
                         " from t_base_warehouse a,t_goods_list g ,t_goods_distributor_price p LEFT JOIN t_goods_warehouse w on w.barcode = p.barcode  and w.wid=p.wid " +
                         " where a.id=p.wid and  g.barcode = p.barcode and p.usercode ='" + goodsSeachParam.userId + "' " + st +
                         " group by g.id,g.brand,g.goodsName,g.barcode,g.slt,g.supplierId,g.supplierCode,p.pprice " +
                         " order by g.brand,g.barcode  LIMIT " + (goodsSeachParam.current - 1) * goodsSeachParam.pageSize + "," + goodsSeachParam.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            if (dt.Rows.Count>0&&dt.Rows[0]["id"].ToString()!=null && dt.Rows[0]["id"].ToString() !="")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    GoodsListItem goodsListItem = new GoodsListItem();
                    goodsListItem.keyId = Convert.ToString((goodsSeachParam.current - 1) * goodsSeachParam.pageSize + i + 1);
                    goodsListItem.id = dt.Rows[i]["id"].ToString();
                    goodsListItem.brand = dt.Rows[i]["brand"].ToString();
                    goodsListItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                    goodsListItem.barcode = dt.Rows[i]["barcode"].ToString();
                    goodsListItem.slt = dt.Rows[i]["slt"].ToString();
                    goodsListItem.goodsnum = dt.Rows[i]["goodsnum"].ToString();
                    goodsListItem.price = dt.Rows[i]["pprice"].ToString();
                    if (dt.Rows[i]["businessType"].ToString() == "0")
                    {
                        goodsListItem.businessType = "国内现货";
                    }
                    else if (dt.Rows[i]["businessType"].ToString() == "1")
                    {
                        goodsListItem.businessType = "海外直邮";
                    }
                    else if (dt.Rows[i]["businessType"].ToString() == "2")
                    {
                        goodsListItem.businessType = "保税";
                    }
                    else
                    {
                        goodsListItem.businessType = "";
                    }
                    goodsResult.list.Add(goodsListItem);
                }
            }
            
            string sql1 = "select count(*) " +
                         "from t_base_warehouse a,t_goods_list g ,t_goods_distributor_price p  " +
                         "where a.id=p.wid and g.barcode = p.barcode and p.usercode ='" + goodsSeachParam.userId + "' " + st;

            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
            goodsResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return goodsResult;
        }

        /// <summary>
        /// 获商品列表 - 零售
        /// </summary>
        /// <returns></returns>
        public PageResult GetGoodsListForRetail(GoodsSeachParam goodsSeachParam)
        {
            PageResult goodsResult = new PageResult();
            goodsResult.pagination = new Page(goodsSeachParam.current, goodsSeachParam.pageSize);
            goodsResult.list = new List<Object>();

            string st = "";
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
                st += " and g.barcode like '%" + goodsSeachParam.barcode + "%' ";
            }
            if (goodsSeachParam.businessType != null && goodsSeachParam.businessType != "")
            {
                st += " and a.businessType like '%" + goodsSeachParam.businessType + "%' ";
            }
            goodsSeachParam.userId = "bbcagent@llwell.net";
            string sql = "select g.id,g.brand,g.goodsName,g.barcode,g.slt,g.supplierId,g.supplierCode,p.pprice,sum(IFNULL(w.goodsnum,0)) goodsnum ,a.businessType" +
                         " from t_base_warehouse a,t_goods_list g ,t_goods_distributor_price p LEFT JOIN t_goods_warehouse w on w.barcode = p.barcode  and w.wid=p.wid " +
                         " where a.id=p.wid and  g.barcode = p.barcode and p.usercode ='" + goodsSeachParam.userId + "' " + st +
                         " group by g.id,g.brand,g.goodsName,g.barcode,g.slt,g.supplierId,g.supplierCode,p.pprice " +
                         " order by g.brand,g.barcode  LIMIT " + (goodsSeachParam.current - 1) * goodsSeachParam.pageSize + "," + goodsSeachParam.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            if (dt.Rows.Count > 0 && dt.Rows[0]["id"].ToString() != null && dt.Rows[0]["id"].ToString() != "")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    GoodsListItem goodsListItem = new GoodsListItem();
                    goodsListItem.keyId = Convert.ToString((goodsSeachParam.current - 1) * goodsSeachParam.pageSize + i + 1);
                    goodsListItem.id = dt.Rows[i]["id"].ToString();
                    goodsListItem.brand = dt.Rows[i]["brand"].ToString();
                    goodsListItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                    goodsListItem.barcode = dt.Rows[i]["barcode"].ToString();
                    goodsListItem.slt = dt.Rows[i]["slt"].ToString();
                    goodsListItem.goodsnum = dt.Rows[i]["goodsnum"].ToString();
                    goodsListItem.price = dt.Rows[i]["pprice"].ToString();
                    if (dt.Rows[i]["businessType"].ToString() == "0")
                    {
                        goodsListItem.businessType = "国内现货";
                    }
                    else if (dt.Rows[i]["businessType"].ToString() == "1")
                    {
                        goodsListItem.businessType = "海外直邮";
                    }
                    else if (dt.Rows[i]["businessType"].ToString() == "2")
                    {
                        goodsListItem.businessType = "保税";
                    }
                    else
                    {
                        goodsListItem.businessType = "";
                    }
                    goodsResult.list.Add(goodsListItem);
                }
            }

            string sql1 = "select count(*) " +
                         "from t_base_warehouse a,t_goods_list g ,t_goods_distributor_price p  " +
                         "where a.id=p.wid and g.barcode = p.barcode and p.usercode ='" + goodsSeachParam.userId + "' " + st;

            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
            goodsResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return goodsResult;
        }

        /// <summary>
        /// 获商品列表 - 分销商
        /// </summary>
        /// <returns></returns>
        public PageResult GetGoodsListForDistribution(GoodsSeachParam goodsSeachParam,string agent)
        {
            PageResult goodsResult = new PageResult();
            goodsResult.pagination = new Page(goodsSeachParam.current, goodsSeachParam.pageSize);
            goodsResult.list = new List<Object>();

            string st = "";
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
                st += " and g.barcode like '%" + goodsSeachParam.barcode + "%' ";
            }
            string sql = "select g.id,g.brand,g.goodsName,g.barcode,g.slt,g.supplierId,g.supplierCode,p.pprice,sum(IFNULL(w.goodsnum,0)) goodsnum " +
                         "from t_goods_list g ,t_goods_distributor_price p LEFT JOIN t_goods_warehouse w on w.barcode = p.barcode  and w.wid=p.wid " +
                         "where g.barcode = p.barcode and p.usercode ='" + agent + "' " + st +
                         " group by g.id,g.brand,g.goodsName,g.barcode,g.slt,g.supplierId,g.supplierCode,p.pprice " +
                         " order by g.brand,g.barcode  LIMIT " + (goodsSeachParam.current - 1) * goodsSeachParam.pageSize + "," + goodsSeachParam.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            if (dt.Rows.Count > 0 && dt.Rows[0]["id"].ToString() != null && dt.Rows[0]["id"].ToString() != "")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    GoodsListItem goodsListItem = new GoodsListItem();
                    goodsListItem.keyId = Convert.ToString((goodsSeachParam.current - 1) * goodsSeachParam.pageSize + i + 1);
                    goodsListItem.id = dt.Rows[i]["id"].ToString();
                    goodsListItem.brand = dt.Rows[i]["brand"].ToString();
                    goodsListItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                    goodsListItem.barcode = dt.Rows[i]["barcode"].ToString();
                    goodsListItem.slt = dt.Rows[i]["slt"].ToString();
                    goodsListItem.goodsnum = dt.Rows[i]["goodsnum"].ToString();
                    goodsListItem.price = dt.Rows[i]["pprice"].ToString();

                    goodsResult.list.Add(goodsListItem);
                }
            }

            string sql1 = "select count(*) " +
                         "from t_goods_list g ,t_goods_distributor_price p LEFT JOIN t_goods_warehouse w on w.barcode = p.barcode  and w.wid=p.wid " +
                         "where g.barcode = p.barcode and p.usercode ='" + agent + "' " + st;

            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
            goodsResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return goodsResult;
        }
        /// <summary>
        /// 获取单个商品
        /// </summary>
        /// <returns></returns>
        public GoodsItem GetGoodsById(GoodsSeachParam goodsSeachParam)
        {
            GoodsItem goodsItem = new GoodsItem();
            string sql = "select g.id,g.brand,g.goodsName,g.barcode,c.`name` as catelog3,g.slt,g.source,g.model,g.applicable," +
                "g.thumb,g.content,g.formula,g.shelflife,g.storage,w.wname,wh.goodsnum,wh.inprice " +
                "from t_goods_list g ,t_goods_warehouse wh, t_base_warehouse w ,t_goods_category c " +
                "where g.id = wh.goodsid and wh.wid = w.id and g.catelog2 = c.id and g.id= " + goodsSeachParam.goodsId;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                goodsItem.id = dt.Rows[0]["id"].ToString();
                goodsItem.brand = dt.Rows[0]["brand"].ToString();
                goodsItem.goodsName = dt.Rows[0]["goodsName"].ToString();
                goodsItem.barcode = dt.Rows[0]["barcode"].ToString();
                goodsItem.catelog3 = dt.Rows[0]["catelog3"].ToString();
                goodsItem.slt = dt.Rows[0]["slt"].ToString();
                goodsItem.source = dt.Rows[0]["source"].ToString();
                goodsItem.model = dt.Rows[0]["model"].ToString();
                goodsItem.applicable = dt.Rows[0]["applicable"].ToString();
                goodsItem.formula = dt.Rows[0]["formula"].ToString();
                goodsItem.shelfLife = dt.Rows[0]["shelflife"].ToString();
                goodsItem.storage = dt.Rows[0]["storage"].ToString();
                goodsItem.wname = dt.Rows[0]["wname"].ToString();
                goodsItem.goodsnum = dt.Rows[0]["goodsnum"].ToString();
                goodsItem.inprice = dt.Rows[0]["inprice"].ToString();
                if (dt.Rows[0]["thumb"].ToString() != "")
                {
                    goodsItem.thumb = dt.Rows[0]["thumb"].ToString().Split(',');
                }
                if (dt.Rows[0]["content"].ToString() != "")
                {
                    goodsItem.content = dt.Rows[0]["content"].ToString().Split(',');
                }
            }
            return goodsItem;
        }

        /// <summary>
        /// 获取单个商品- 运营
        /// </summary>
        /// <returns></returns>
        public GoodsItem GetGoodsByIdForOperator(GoodsSeachParam goodsSeachParam)
        {
            GoodsItem goodsItem = new GoodsItem();
            string sql = "select g.id,g.brand,g.goodsName,g.barcode,c.`name` as catelog3,g.slt,g.source,g.model,g.applicable," +
                "g.thumb,g.content,g.formula,g.shelflife,g.storage,w.wname,wh.goodsnum,wh.inprice,g.supplierCode,g.wid " +
                "from t_goods_list g ,t_goods_warehouse wh, t_base_warehouse w ,t_goods_category c " +
                "where g.id = wh.goodsid and wh.wid = w.id and g.catelog2 = c.id and g.id= " + goodsSeachParam.goodsId;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                goodsItem.id = dt.Rows[0]["id"].ToString();
                goodsItem.brand = dt.Rows[0]["brand"].ToString();
                goodsItem.goodsName = dt.Rows[0]["goodsName"].ToString();
                goodsItem.barcode = dt.Rows[0]["barcode"].ToString();
                goodsItem.catelog3 = dt.Rows[0]["catelog3"].ToString();
                goodsItem.slt = dt.Rows[0]["slt"].ToString();
                goodsItem.source = dt.Rows[0]["source"].ToString();
                goodsItem.model = dt.Rows[0]["model"].ToString();
                goodsItem.applicable = dt.Rows[0]["applicable"].ToString();
                goodsItem.formula = dt.Rows[0]["formula"].ToString();
                goodsItem.shelfLife = dt.Rows[0]["shelflife"].ToString();
                goodsItem.storage = dt.Rows[0]["storage"].ToString();
                goodsItem.wname = dt.Rows[0]["wname"].ToString();
                goodsItem.goodsnum = dt.Rows[0]["goodsnum"].ToString();
                goodsItem.inprice = dt.Rows[0]["inprice"].ToString();
                if (dt.Rows[0]["thumb"].ToString() != "")
                {
                    goodsItem.thumb = dt.Rows[0]["thumb"].ToString().Split(',');
                }
                if (dt.Rows[0]["content"].ToString() != "")
                {
                    goodsItem.content = dt.Rows[0]["content"].ToString().Split(',');
                }
                goodsItem.goodsSelectSupplierList = new List<GoodsSelectSupplier>();
                string sql1 = "select min(g.id) id,w.id wid,u.usercode,w.wname,u.username,min(g.inprice) inprice,sum(IFNULL(g.goodsnum,0)) goodsnum " +
                    "from t_goods_warehouse g,t_user_list u,t_base_warehouse w " +
                    "where g.supplierid = u.id and g.wid = w.id and g.barcode = '" + dt.Rows[0]["barcode"].ToString() + "' " +
                    "group by u.usercode,w.id,w.wname,u.username";
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    GoodsSelectSupplier goodsSelectSupplier = new GoodsSelectSupplier();
                    goodsSelectSupplier.id = dt1.Rows[i]["id"].ToString();
                    goodsSelectSupplier.supplierName = dt1.Rows[i]["username"].ToString();
                    goodsSelectSupplier.wname = dt1.Rows[i]["wname"].ToString();
                    goodsSelectSupplier.goodsnum = dt1.Rows[i]["goodsnum"].ToString();
                    goodsSelectSupplier.inprice = dt1.Rows[i]["inprice"].ToString();
                    if (dt1.Rows[i]["wid"].ToString()== dt.Rows[0]["wid"].ToString()&& dt1.Rows[i]["usercode"].ToString() == dt.Rows[0]["supplierCode"].ToString())
                    {
                        goodsSelectSupplier.ifSel = "1";
                    }
                    else
                    {
                        goodsSelectSupplier.ifSel = "0";
                    }

                    goodsItem.goodsSelectSupplierList.Add(goodsSelectSupplier);
                }
            }
            return goodsItem;
        }
        /// <summary>
        /// 获取单个商品- 代理
        /// </summary>
        /// <returns></returns>
        public GoodsItem GetGoodsByIdForAgent(GoodsSeachParam goodsSeachParam)
        {
            GoodsItem goodsItem = new GoodsItem();
            string sql = "select g.id,g.brand,g.goodsName,g.barcode,c.`name` as catelog3,g.slt,g.source,g.model,g.applicable," +
                "g.thumb,g.content,g.formula,g.shelflife,g.storage,w.wname,wh.goodsnum,p.pprice " +
                "from t_goods_list g ,t_goods_warehouse wh, t_base_warehouse w ,t_goods_category c, t_goods_distributor_price p " +
                "where g.barcode = p.barcode and g.id = wh.goodsid and wh.wid = w.id and g.catelog2 = c.id" +
                " and g.id= " + goodsSeachParam.goodsId+" and p.userCode= '"+ goodsSeachParam.userId + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                goodsItem.id = dt.Rows[0]["id"].ToString();
                goodsItem.brand = dt.Rows[0]["brand"].ToString();
                goodsItem.goodsName = dt.Rows[0]["goodsName"].ToString();
                goodsItem.barcode = dt.Rows[0]["barcode"].ToString();
                goodsItem.catelog3 = dt.Rows[0]["catelog3"].ToString();
                goodsItem.slt = dt.Rows[0]["slt"].ToString();
                goodsItem.source = dt.Rows[0]["source"].ToString();
                goodsItem.model = dt.Rows[0]["model"].ToString();
                goodsItem.applicable = dt.Rows[0]["applicable"].ToString();
                goodsItem.formula = dt.Rows[0]["formula"].ToString();
                goodsItem.shelfLife = dt.Rows[0]["shelflife"].ToString();
                goodsItem.storage = dt.Rows[0]["storage"].ToString();
                goodsItem.wname = dt.Rows[0]["wname"].ToString();
                goodsItem.goodsnum = dt.Rows[0]["goodsnum"].ToString();
                goodsItem.inprice = dt.Rows[0]["pprice"].ToString();
                if (dt.Rows[0]["thumb"].ToString() != "")
                {
                    goodsItem.thumb = dt.Rows[0]["thumb"].ToString().Split(',');
                }
                if (dt.Rows[0]["content"].ToString() != "")
                {
                    goodsItem.content = dt.Rows[0]["content"].ToString().Split(',');
                }
            }
            return goodsItem;
        }

        public MsgResult UpdateGoodsSelect(GoodsSelParam goodsSelParam)
        {
            MsgResult msg = new MsgResult();
            string sql = "select * from t_goods_warehouse where id = "+ goodsSelParam.id;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string upsql = "update t_goods_list set supplierId="+ dt.Rows[0]["supplierId"].ToString() + "," +
                    "supplierCode='"+ dt.Rows[0]["supplierCode"].ToString() + "'," +
                    "wId="+ dt.Rows[0]["wid"].ToString() + " " +
                    "where barcode = '"+ dt.Rows[0]["barcode"].ToString() + "'";
                if (DatabaseOperationWeb.ExecuteDML(upsql))
                {
                    msg.msg = "修改成功";
                    msg.type = "1";
                }
                else
                {
                    msg.msg = "修改失败";
                }
            }
            else
            {
                msg.msg = "未查询到默认信息";
            }
            return msg;
        }

        /// <summary>
        /// 获商品库存 - 代销
        /// </summary>
        /// <returns></returns>
        public PageResult SelectGoodsList(GoodsSales goodsSales, string agent)
        {
            PageResult pr = new PageResult();
            pr.pagination = new Page(goodsSales.current, goodsSales.pageSize);
            pr.list = new List<Object>();

            string st = "";
            if (goodsSales.information != null && goodsSales.information != "")
            {
                st += " and (a.goodsName like '%" + goodsSales.information + "%' "+
                    "  or b.brand like '%"+ goodsSales .information+ "%' "+
                     " or a.barcode like '%" + goodsSales.information + "%') ";
            }
           
            string ar = "order by createTime desc";
            if (goodsSales.shelfLife == "1")
            {
                ar = "order by shelfLife desc ";
            }
            else if (goodsSales.shelfLife == "0")
            {
                ar = "order by shelfLife asc ";
            }
            else if (goodsSales.confirmTime == "1")
            {
                ar = "order by confirmTime desc ";
            }
            else if (goodsSales.confirmTime == "0")
            {
                ar = "order by confirmTime asc ";
            }
            else if (goodsSales.pNum == "1")
            {
                ar = "order by a.pNum desc ";
            }
            else if (goodsSales.pNum == "0")
            {
                ar = "order by a.pNum asc ";
            }
            else if (goodsSales.pprice == "1")
            {
                ar = "order by pprice desc ";
            }
            else if(goodsSales.pprice == "0")
            {
                ar = "order by pprice asc ";
            }
           
            
            string sql = "select a.id,a.goodsName,a.slt,c.sendTime,b.brand,a.createTime,c.confirmTime,a.barcode,a.pprice,a.pNum,b.shelfLife,c.goodsNum  " +
                         "FROM  t_goods_list b ,t_goods_distributor_price a LEFT JOIN ( select s1.sendTime,g1.goodsNum,g1.barcode,s1.confirmTime " +
                                                                              "from t_warehouse_send s1 ,t_warehouse_send_goods g1 " +
                                                                              "where  s1.id = g1.sendId and g1.id in (select max(g.id) id from t_warehouse_send s ,t_warehouse_send_goods g " +
                                                                                                                     "where s.id = g.sendId and s.purchasersCode ='"+ agent + "' and s.sendType = '1' and s.`status` = '1' GROUP BY barcode )) c " +
                                                                 " on  a.barcode=c.barcode " +
                         "WHERE a.barcode=b.barcode   and a.usercode='" + agent + "' "+ st + ar+"  LIMIT "  + (goodsSales.current - 1) * goodsSales.pageSize + "," + goodsSales.pageSize;

           
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            if (dt.Rows.Count > 0 )
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    GoodsSalesItem goodsSalesItem = new GoodsSalesItem();
                    goodsSalesItem.keyId = Convert.ToString((goodsSales.current - 1) * goodsSales.pageSize + i + 1);
                    goodsSalesItem.id = dt.Rows[i]["id"].ToString();
                    goodsSalesItem.slt = dt.Rows[i]["slt"].ToString();
                    goodsSalesItem.brand = dt.Rows[i]["brand"].ToString();
                    goodsSalesItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                    goodsSalesItem.barcode = dt.Rows[i]["barcode"].ToString();
                    goodsSalesItem.shelfLife = dt.Rows[i]["shelfLife"].ToString();
                    goodsSalesItem.pprice = String.Format("{0:F}",Convert.ToDouble(dt.Rows[i]["pprice"].ToString()) ) ;
                    if (dt.Rows[i]["confirmTime"].ToString() != null && dt.Rows[i]["confirmTime"].ToString() != "")
                    {
                        goodsSalesItem.confirmTime = Convert.ToDateTime(dt.Rows[i]["confirmTime"].ToString()).ToString("yyyy-MM-dd");
                    }
                    
                    if (dt.Rows[i]["createTime"].ToString()!=null && dt.Rows[i]["createTime"].ToString()!="")
                    {
                        goodsSalesItem.createTime = Convert.ToDateTime(dt.Rows[i]["createTime"].ToString()).ToString("yyyy-MM-dd");
                    }                  
                    goodsSalesItem.pNum = dt.Rows[i]["pNum"].ToString();
                    goodsSalesItem.goodsNum= dt.Rows[i]["goodsNum"].ToString();
                    if (dt.Rows[i]["sendTime"].ToString() != null && dt.Rows[i]["sendTime"].ToString() != "")
                    {
                        goodsSalesItem.sendTime = Convert.ToDateTime(dt.Rows[i]["sendTime"].ToString()).ToString("yyyy-MM-dd");
                    }
                    
                    pr.list.Add(goodsSalesItem);
                }
            }

            string sql1 = "select count(*) " +
                          "FROM  t_goods_list b ,t_goods_distributor_price a LEFT JOIN ( select s1.sendTime,g1.goodsNum,g1.barcode,s1.confirmTime " +
                          "from t_warehouse_send s1 ,t_warehouse_send_goods g1 " +
                          "where  s1.id = g1.sendId and g1.id in (select max(g.id) id from t_warehouse_send s ,t_warehouse_send_goods g " +
                          "where s.id = g.sendId and s.purchasersCode ='" + agent + "' and s.sendType = '1' and s.`status` = '1' GROUP BY barcode )) c " +
                          " on  a.barcode=c.barcode " +
                          "WHERE a.barcode=b.barcode   and a.usercode='" + agent + "' " + st;

            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
            pr.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);

            return pr;
        }


        /// <summary>
        /// 获取查看上传商品列表 - 供应商
        /// </summary>
        /// <returns></returns>
        public PageResult SelectOnloadGoodsList(SelectOnloadGoodsListParam ep, string userId)
        {
            
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(ep.current,ep.pageSize);
            pageResult.list = new List<object>();
            string sql = "select g.id,g.brand,wh.inprice,g.goodsName,g.barcode,g.slt,g.source,w.wname,wh.goodsnum,u.username " +
                    " from t_goods_list g ,t_goods_warehouse_bak wh,t_base_warehouse w ,t_user_list u ,t_log_upload  a" +
                    " where g.id = wh.goodsid and  wh.wid = w.id and wh.suppliercode = u.usercode  and a.logCode=wh.logCode" +
                    " and wh.suppliercode ='" + userId + "' and a.id='"+ ep.logId + "'"  +
                    " order by g.brand,g.barcode  LIMIT " + (ep.current - 1) * ep.pageSize + "," + ep.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GoodsListItem goodsListItem = new GoodsListItem();
                goodsListItem.keyId = Convert.ToString((ep.current - 1) * ep.pageSize + i + 1);
                goodsListItem.id = dt.Rows[i]["id"].ToString();
                goodsListItem.brand = dt.Rows[i]["brand"].ToString();
                goodsListItem.supplier = dt.Rows[i]["username"].ToString();
                goodsListItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                goodsListItem.barcode = dt.Rows[i]["barcode"].ToString();
                goodsListItem.slt = dt.Rows[i]["slt"].ToString();
                goodsListItem.wname = dt.Rows[i]["wname"].ToString();
                goodsListItem.goodsnum = dt.Rows[i]["goodsnum"].ToString();
                goodsListItem.price = dt.Rows[i]["inprice"].ToString();
                pageResult.list.Add(goodsListItem);
            }
            string sql1 = "select count(*) " 
                    + " from t_goods_list g ,t_goods_warehouse_bak wh, t_base_warehouse w ,t_user_list u, t_log_upload  a" 
                    + " where g.id = wh.goodsid and  wh.wid = w.id and wh.suppliercode = u.usercode  and a.logCode=wh.logCode"
                    + " and wh.suppliercode ='" + userId + "' and a.id='" + ep.logId + "'";

            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return pageResult;
        }


        /// <summary>
        /// 获取查看批采可供商品列表 - 供应商
        /// </summary>
        /// <returns></returns>
        public SelectSupplyGoodsListItem SelectSupplyOfferGoodsList(SelectSupplyGoodsListParam ssgp, string userId)
        {
            SelectSupplyGoodsListItem ssgi = new SelectSupplyGoodsListItem();
            ssgi.pagination = new Page(ssgp.current, ssgp.pageSize);
            ssgi.catelog1 = new List<string>();
            ssgi.flag = new List<string>();
            ssgi.selectSupplyGoodsItems = new List<SelectSupplyGoodsItem>();
            ssgi.flag.Add("全部");           
            ssgi.type = ssgp.type;
            string st = "";
            if (ssgp.catelog1 !=null && ssgp.catelog1 !="" && ssgp.catelog1 != "全部")
            {
                st += " and c.name='"+ ssgp.catelog1 + "'";
            }
            if (ssgp.flag != null && ssgp.flag != "" && ssgp.flag != "全部")
            {
                st += (ssgp.flag == "已上架") ? " and a.flag='1'" : " and a.flag='0'";
                ssgi.flag.Add(ssgp.flag);
            }
            else
            {
                ssgi.flag.Add("已上架");
                ssgi.flag.Add("已下架");
            }
            if (ssgp.select != null && ssgp.select != "")
            {
                st += " and (a.goodsName like '%" + ssgp.select + "%' or a.barcode like '%"+ ssgp.select + "%' or b.brand like '%"+ ssgp.select + "%' )";
            }
            string select = ""
                + "select a.barcode,a.goodsName,a.slt,a.offer,c.name,b.brand"
                + " from t_goods_offer a,t_goods_list b, t_goods_category c "
                + " where a.barcode=b.barcode and b.catelog1=c.id and a.usercode='"+ userId + "' "+ st
                + " order by goodsid asc";
            DataTable dtselect = DatabaseOperationWeb.ExecuteSelectDS(select,"T").Tables[0];
            DataView dv = new DataView(dtselect);
            DataTable dtname = dv.ToTable(true, "name");
            DataTable dtGoods = dv.ToTable(true, "barcode", "goodsName", "brand", "slt", "offer");
            ssgi.pagination.total = dtGoods.Rows.Count;
            if (dtselect.Rows.Count > 0)
            {

                ssgi.catelog1.Add("全部");
                foreach (DataRow drname in dtname.Rows)
                {
                    ssgi.catelog1.Add(drname["name"].ToString());
                }

                for (int i = 0; i < dtGoods.Rows.Count; i++)
                {
                    SelectSupplyGoodsItem selectSupplyGoodsItem = new SelectSupplyGoodsItem();
                    selectSupplyGoodsItem.barcode = dtGoods.Rows[i]["barcode"].ToString();
                    selectSupplyGoodsItem.name = dtGoods.Rows[i]["goodsName"].ToString();
                    selectSupplyGoodsItem.brand = dtGoods.Rows[i]["brand"].ToString();
                    selectSupplyGoodsItem.slt = dtGoods.Rows[i]["slt"].ToString();
                    selectSupplyGoodsItem.price = "￥"+dtGoods.Rows[i]["offer"].ToString();
                    ssgi.selectSupplyGoodsItems.Add(selectSupplyGoodsItem);
                }
            }
            else
            {
                ssgi.catelog1.Add("全部");
                if (ssgp.catelog1 != null && ssgp.catelog1 != "" && ssgp.catelog1 != "全部")
                {
                    ssgi.catelog1.Add(ssgp.catelog1);
                }
            }
            return ssgi;
        }

        /// <summary>
        /// 获取查看一件代发、铺货可供商品列表 - 供应商
        /// </summary>
        /// <returns></returns>
        public SelectSupplyGoodsListItem SelectSupplyWarehouseGoodsList(SelectSupplyGoodsListParam ssgp, string userId)
        {
            SelectSupplyGoodsListItem ssgi = new SelectSupplyGoodsListItem();
            ssgi.pagination = new Page(ssgp.current, ssgp.pageSize);
            ssgi.catelog1 = new List<string>();
            ssgi.flag = new List<string>();
            ssgi.selectSupplyGoodsItems = new List<SelectSupplyGoodsItem>();
            ssgi.type = ssgp.type;
            ssgi.flag.Add("全部");                       
            string st = "  ";
            if (ssgp.type=="铺货")
            {
                st += " and a.ifph='1' ";
            }
            if (ssgp.catelog1 != null && ssgp.catelog1 != "" && ssgp.catelog1 != "全部")
            {
                st += " and c.name='" + ssgp.catelog1 + "'";
            }
            if (ssgp.flag == "已缺货")
            {
                st += " and a.goodsnum='0' ";
                ssgi.flag.Add("已缺货");
            }
            else if (ssgp.flag != null && ssgp.flag != "" && ssgp.flag != "全部")
            {
                st += (ssgp.flag == "已上架") ? " and a.flag='1'" : " and a.flag='0'";
                ssgi.flag.Add(ssgp.flag);
            }
            else
            {
                ssgi.flag.Add("已上架");
                ssgi.flag.Add("已下架");
                ssgi.flag.Add("已缺货");
            }
            if (ssgp.select != null && ssgp.select != "")
            {
                st += " and (b.goodsName like '%" + ssgp.select + "%' or a.barcode like '%" + ssgp.select + "%' or b.brand like '%" + ssgp.select + "%' )";
            }

            string select = ""
                + "select a.barcode,b.goodsName,b.slt,a.inprice,c.name,b.brand "
                + " from t_goods_warehouse a,t_goods_list b,t_goods_category c "
                + " where a.barcode=b.barcode and b.catelog1=c.id and a.suppliercode='" + userId + "' " + st
                + " order by goodsid asc";
            DataTable dtselect = DatabaseOperationWeb.ExecuteSelectDS(select,"T").Tables[0];
            DataView dv = new DataView(dtselect);
            DataTable dtname = dv.ToTable(true, "name");
            DataTable dtGoods = dv.ToTable(true, "barcode", "goodsName", "slt", "inprice", "brand");
            ssgi.pagination.total = dtGoods.Rows.Count;
            if (dtselect.Rows.Count>0)
            {                
                ssgi.catelog1.Add("全部");
                foreach (DataRow dr in dtname.Rows)
                {
                    ssgi.catelog1.Add(dr["name"].ToString());
                }               
                for (int i=0;i< dtGoods.Rows.Count ;i++)
                {
                    SelectSupplyGoodsItem selectSupplyGoodsItem = new SelectSupplyGoodsItem();
                    selectSupplyGoodsItem.barcode = dtGoods.Rows[i]["barcode"].ToString();
                    selectSupplyGoodsItem.name = dtGoods.Rows[i]["goodsName"].ToString();
                    selectSupplyGoodsItem.brand = dtGoods.Rows[i]["brand"].ToString();
                    selectSupplyGoodsItem.slt = dtGoods.Rows[i]["slt"].ToString();
                    selectSupplyGoodsItem.price = "￥" + dtGoods.Rows[i]["inprice"].ToString();
                    ssgi.selectSupplyGoodsItems.Add(selectSupplyGoodsItem);
                }
            }
            else
            {
                ssgi.catelog1.Add("全部");
                if (ssgp.catelog1!=null && ssgp.catelog1!=""&& ssgp.catelog1 != "全部")
                {
                    ssgi.catelog1.Add(ssgp.catelog1);
                }               
            }
            return ssgi;
        }

        /// <summary>
        /// 获取批采可供商品详情 - 供应商
        /// </summary>
        /// <returns></returns>
        public SelectSupplyGoodsDetailsItem SelectSupplyOfferGoodsDetails(SelectSupplyGoodsDetailsParam ssgdp, string userId)
        {
            SelectSupplyGoodsDetailsItem ssgdi = new SelectSupplyGoodsDetailsItem();
            ssgdi.slt = new List<string>();
            ssgdi.prices = new List<string>();
            ssgdi.num = new List<string>();
            ssgdi.goodsDetailImgArr = new List<string>();
            ssgdi.goodsParameters = new List<GoodsParameters>();
            ssgdi.type = ssgdp.type;
            string select = ""
                 + "select a.barcode,a.goodsName,b.thumb,c.goodsNum,c.multprice,b.brand,b.efficacy,b.content,b.country,b.model,a.flag"
                 + " from t_goods_list b,t_goods_offer a left join t_goods_price_mult c  on a.barcode=c.barcode and a.usercode=c.supplierCode and c.priceType='p'"
                 + " where a.barcode=b.barcode   and a.usercode='" + userId + "' and a.barcode='"+ssgdp.barcode+"' "
                 + " order by c.goodsNum asc";
            DataTable dtselect = DatabaseOperationWeb.ExecuteSelectDS(select,"T").Tables[0];
            if (dtselect.Rows.Count>0)
            {
                ssgdi.name = dtselect.Rows[0]["barcode"].ToString()+ dtselect.Rows[0]["brand"].ToString() + dtselect.Rows[0]["goodsName"].ToString();
                ssgdi.barcode= dtselect.Rows[0]["barcode"].ToString();
                ssgdi.efficacy = dtselect.Rows[0]["efficacy"].ToString();
                ssgdi.flag = dtselect.Rows[0]["flag"].ToString();
                string[] picture = dtselect.Rows[0]["thumb"].ToString().Split(",");
                for (int i =0;i< dtselect.Rows.Count;i++)
                {                   
                    if (i == 0)
                    {
                        ssgdi.num.Add("0~" + dtselect.Rows[i]["goodsNum"].ToString()) ;
                        ssgdi.prices.Add("￥" + dtselect.Rows[i]["multprice"].ToString()) ;                        
                    }
                    else
                    {
                        ssgdi.num.Add(dtselect.Rows[i - 1]["goodsNum"].ToString() + "~" + dtselect.Rows[i]["goodsNum"].ToString()) ;
                        ssgdi.prices.Add("￥" + dtselect.Rows[i]["multprice"].ToString()) ;                       
                    }                  
                }
                foreach (string p in picture)
                {
                    ssgdi.slt.Add(p);
                }
                string[] goodsDetailImgArr= dtselect.Rows[0]["content"].ToString().Split(",");
                foreach (string g in goodsDetailImgArr)
                {
                    ssgdi.goodsDetailImgArr.Add(g);
                }
                for (int j = 0; j < 6; j++)
                {
                    GoodsParameters goodsParameters = new GoodsParameters();
                    ssgdi.goodsParameters.Add(goodsParameters);
                }
                ssgdi.goodsParameters[0].key = 1;
                ssgdi.goodsParameters[0].name = "商品名称(中文)";
                ssgdi.goodsParameters[0].content = dtselect.Rows[0]["goodsName"].ToString();

                ssgdi.goodsParameters[1].key = 2;
                ssgdi.goodsParameters[1].name = "品牌";
                ssgdi.goodsParameters[1].content = dtselect.Rows[0]["brand"].ToString();

                ssgdi.goodsParameters[2].key = 3;
                ssgdi.goodsParameters[2].name = "进口国";
                ssgdi.goodsParameters[2].content = dtselect.Rows[0]["country"].ToString();
                ssgdi.goodsParameters[3].key = 4;
                ssgdi.goodsParameters[3].name = "规格";
                ssgdi.goodsParameters[3].content = dtselect.Rows[0]["model"].ToString();
                ssgdi.goodsParameters[4].key = 5;
                ssgdi.goodsParameters[4].name = "生产商";
                ssgdi.goodsParameters[4].content = dtselect.Rows[0]["brand"].ToString();
                ssgdi.goodsParameters[5].key = 6;
                ssgdi.goodsParameters[5].name = "产品功效";
                ssgdi.goodsParameters[5].content = dtselect.Rows[0]["efficacy"].ToString();
            }
            return ssgdi;
        }


        /// <summary>
        /// 获取一件代发、铺货可供商品详情 - 供应商
        /// </summary>
        /// <returns></returns>
        public SelectSupplyGoodsDetailsItem SelectSupplyWarehouseGoodsDetails(SelectSupplyGoodsDetailsParam ssgdp, string userId)
        {
            SelectSupplyGoodsDetailsItem ssgdi = new SelectSupplyGoodsDetailsItem();
            ssgdi.slt = new List<string>();
            ssgdi.num = new List<string>();
            ssgdi.prices = new List<string>();
            ssgdi.goodsDetailImgArr = new List<string>();
            ssgdi.goodsParameters = new List<GoodsParameters>();
            ssgdi.type = ssgdp.type;
            string st = "";
            string at = "";
            string bt = "";
            if (ssgdp.type=="一件代发")
            {
                at = ",c.goodsNum,c.multprice";
                st = " left join t_goods_price_mult c on a.barcode=c.barcode and a.suppliercode=c.supplierCode and c.priceType='o' ";
                bt = " order by c.goodsNum  asc";
            }
            string select = ""
                + "select a.barcode,b.goodsName,b.thumb,b.brand,b.efficacy,b.content,b.country,b.model,a.flag,a.inprice " + at
                + " from t_goods_list b ,t_goods_warehouse a " + st
                + " where a.barcode=b.barcode  and a.suppliercode='" + userId + "' and a.barcode='" + ssgdp.barcode + "' "+ bt;
                
            DataTable dtselect = DatabaseOperationWeb.ExecuteSelectDS(select,"T").Tables[0];
            if (dtselect.Rows.Count > 0)
            {
                ssgdi.name = dtselect.Rows[0]["barcode"].ToString() + dtselect.Rows[0]["brand"].ToString() + dtselect.Rows[0]["goodsName"].ToString();
                ssgdi.efficacy = dtselect.Rows[0]["efficacy"].ToString();
                ssgdi.flag = dtselect.Rows[0]["flag"].ToString();
                ssgdi.barcode = dtselect.Rows[0]["barcode"].ToString();
                string[] picture = dtselect.Rows[0]["thumb"].ToString().Split(",");
                if (ssgdp.type == "一件代发")
                {
                    for (int i = 0; i < dtselect.Rows.Count; i++)
                    {
                        if (i == 0)
                        {
                            ssgdi.num.Add("0~" + dtselect.Rows[i]["goodsNum"].ToString());
                            ssgdi.prices.Add("￥" + dtselect.Rows[i]["multprice"].ToString());
                        }
                        else
                        {
                            ssgdi.num.Add(dtselect.Rows[i - 1]["goodsNum"].ToString() + "~" + dtselect.Rows[i]["goodsNum"].ToString());
                            ssgdi.prices.Add("￥" + dtselect.Rows[i]["multprice"].ToString());
                        }
                    }
                }
                else
                {
                    ssgdi.inprice= "￥" + dtselect.Rows[0]["inprice"].ToString();
                }
                foreach (string p in picture)
                {
                    ssgdi.slt.Add(p);
                }
                string[] goodsDetailImgArr = dtselect.Rows[0]["content"].ToString().Split(",");
                foreach (string g in goodsDetailImgArr)
                {
                    ssgdi.goodsDetailImgArr.Add(g);
                }
                for (int j = 0; j < 6; j++)
                {
                    GoodsParameters goodsParameters = new GoodsParameters();
                    ssgdi.goodsParameters.Add(goodsParameters);
                }
                ssgdi.goodsParameters[0].key = 1;
                ssgdi.goodsParameters[0].name = "商品名称(中文)";
                ssgdi.goodsParameters[0].content = dtselect.Rows[0]["goodsName"].ToString();

                ssgdi.goodsParameters[1].key = 2;
                ssgdi.goodsParameters[1].name = "品牌";
                ssgdi.goodsParameters[1].content = dtselect.Rows[0]["brand"].ToString();

                ssgdi.goodsParameters[2].key = 3;
                ssgdi.goodsParameters[2].name = "进口国";
                ssgdi.goodsParameters[2].content = dtselect.Rows[0]["country"].ToString();
                ssgdi.goodsParameters[3].key = 4;
                ssgdi.goodsParameters[3].name = "规格";
                ssgdi.goodsParameters[3].content = dtselect.Rows[0]["model"].ToString();
                ssgdi.goodsParameters[4].key = 5;
                ssgdi.goodsParameters[4].name = "生产商";
                ssgdi.goodsParameters[4].content = dtselect.Rows[0]["brand"].ToString();
                ssgdi.goodsParameters[5].key = 6;
                ssgdi.goodsParameters[5].name = "产品功效";
                ssgdi.goodsParameters[5].content = dtselect.Rows[0]["efficacy"].ToString();
            }
            return ssgdi;

        }

        /// <summary>
        /// 一件代发、铺货供商品上架下架接口 - 供应商
        /// </summary>
        /// <returns></returns>
        public MsgResult WarehouseUpDownFlag(SelectSupplyGoodsDetailsParam ssgdp, string userId)
        {
            MsgResult msg = new MsgResult();
            string flag = (ssgdp.flag=="1")?"0":"1";
            string update = ""
                + "update t_goods_warehouse set flag='"+ flag + "'"
                + " where suppliercode='" + userId + "' and barcode='" + ssgdp.barcode + "' ";
            if (DatabaseOperationWeb.ExecuteDML(update))
            {
                msg.type = "1";
            }
            return msg;
        }

        /// <summary>
        /// 批采供商品上架下架接口 - 供应商
        /// </summary>
        /// <returns></returns>
        public MsgResult OfferUpDownFlag(SelectSupplyGoodsDetailsParam ssgdp, string userId)
        {
            MsgResult msg = new MsgResult();
            string flag = (ssgdp.flag == "1") ? "0" : "1";
            string update = ""
                + "update t_goods_offer set flag='" + flag + "'"
                + " where usercode='" + userId + "' and barcode='" + ssgdp.barcode + "' ";
            if (DatabaseOperationWeb.ExecuteDML(update))
            {
                msg.type = "1";
            }
            return msg;
        }

        #endregion

        #region 商品库存上传
        /// <summary>
        /// 获取商品入库列表
        /// </summary>
        /// <param name="goodsUserParam"></param>
        /// <returns></returns>
        public PageResult getUploadList(GoodsUserParam goodsUserParam)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(goodsUserParam.current, goodsUserParam.pageSize);
            pageResult.list = new List<Object>();
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(goodsUserParam.userId);
            string st = "";
            if (userType == "1")//供应商 
            {
                st += " and l.userCode ='" + goodsUserParam.userId + "' ";
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {

            }
            else
            {
                return pageResult;
            }
            string sql = "select l.*,u.username from t_log_upload l,t_user_list u where l.userCode = u.usercode " + st + " order by l.id desc  LIMIT " + (goodsUserParam.current - 1) * goodsUserParam.pageSize + "," + goodsUserParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_base_warehouse").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string uploadNum = "", statusText = "";
                if (dt.Rows[i]["status"].ToString() == "0")
                {
                    uploadNum = dt.Rows[i]["uploadNum"].ToString() + "个SKU （" + dt.Rows[i]["errorNum"].ToString() + "个新SKU等待上传商品图片）";
                    statusText = "等待补全图片信息";
                }
                else if (dt.Rows[i]["status"].ToString() == "1")
                {
                    uploadNum = dt.Rows[i]["uploadNum"].ToString() + "个SKU";
                    statusText = "正在审核中";
                }
                else if (dt.Rows[i]["status"].ToString() == "2")
                {
                    uploadNum = dt.Rows[i]["uploadNum"].ToString() + "个SKU";
                    statusText = "通过审核，成功入库";
                }
                else if (dt.Rows[i]["status"].ToString() == "3")
                {
                    uploadNum = dt.Rows[i]["uploadNum"].ToString() + "个SKU （" + dt.Rows[i]["errorNum"].ToString() + "个SKU审核未通过）";
                    statusText = "审核部分成功";
                }


                UploadItem uploadItem = new UploadItem();
                uploadItem.id = dt.Rows[i]["id"].ToString();
                uploadItem.username = dt.Rows[i]["username"].ToString();
                uploadItem.uploadTime = dt.Rows[i]["uploadTime"].ToString();
                uploadItem.uploadNum = uploadNum;
                uploadItem.statusText = statusText;
                uploadItem.status = dt.Rows[i]["status"].ToString();

                pageResult.list.Add(uploadItem);
            }
            string sql1 = "select count(*) from t_log_upload l,t_user_list u where l.userCode = u.usercode " + st;

            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "TABLE").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return pageResult;
        }

        /// <summary>
        /// 查询补充信息
        /// </summary>
        /// <returns></returns>
        public UploadLogItem getUploadStatus(FileUploadParam fileUploadParam)
        {
            UploadLogItem item = new UploadLogItem();
            string sql = "select * from t_log_upload where id= " + fileUploadParam.logId;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                item.id = dt.Rows[0]["id"].ToString();
                item.status = dt.Rows[0]["status"].ToString();
            }
            return item;
        }

        /// <summary>
        /// 查询补充信息
        /// </summary>
        /// <returns></returns>
        public UploadLogItem getUploadStatusOne(FileUploadParam fileUploadParam)
        {
            UploadLogItem item = new UploadLogItem();
            string sql = "select * from t_log_upload where id= " + fileUploadParam.logId;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                item.id = dt.Rows[0]["id"].ToString();
                item.log = "您共上传了" + dt.Rows[0]["uploadNum"].ToString() + "个SKU，" +
                            "已成功入库" + dt.Rows[0]["successNum"].ToString() + "个SKU，" +
                            "还有" + dt.Rows[0]["errorNum"].ToString() + "个新SKU需要上传图片。";
                item.url = dt.Rows[0]["errorFileUrl"].ToString();
                item.status = dt.Rows[0]["status"].ToString();
            }
            return item;
        }
        /// <summary>
        /// 查询等待审批信息
        /// </summary>
        /// <returns></returns>
        public UploadLogItem getUploadStatusTwo(FileUploadParam fileUploadParam)
        {
            UploadLogItem item = new UploadLogItem();
            string sql = "select * from t_log_upload where id= " + fileUploadParam.logId;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                item.id = dt.Rows[0]["id"].ToString();
                item.log = "您共上传了" + dt.Rows[0]["uploadNum"].ToString() + "个SKU，正在审核中 ...";
                item.status = dt.Rows[0]["status"].ToString();
            }
            return item;
        }
        /// <summary>
        /// 查询审批完成信息
        /// </summary>
        /// <returns></returns>
        public UploadLogItem getUploadStatusThree(FileUploadParam fileUploadParam)
        {
            UploadLogItem item = new UploadLogItem();
            string sql = "select * from t_log_upload where id= " + fileUploadParam.logId;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                item.id = dt.Rows[0]["id"].ToString();
                item.log = "恭喜您！您共上传了" + dt.Rows[0]["uploadNum"].ToString() + "个SKU，已审核成功。";
                item.status = dt.Rows[0]["status"].ToString();
            }
            return item;
        }
        /// <summary>
        /// 查询审批失败信息
        /// </summary>
        /// <returns></returns>
        public UploadLogItem getUploadStatusFour(FileUploadParam fileUploadParam)
        {
            UploadLogItem item = new UploadLogItem();
            string sql = "select * from t_log_upload where id= " + fileUploadParam.logId;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                item.id = dt.Rows[0]["id"].ToString();
                item.log = "您共上传了" + dt.Rows[0]["uploadNum"].ToString() + "个SKU，其中" + dt.Rows[0]["successNum"].ToString() + "个SKU已成功入库，" +
                    dt.Rows[0]["errorNum"].ToString() + "个SKU未成功入库的原因是：" + dt.Rows[0]["remark"].ToString() + "。";
                item.url = dt.Rows[0]["errorFileUrl"].ToString();
                item.status = dt.Rows[0]["status"].ToString();
            }
            return item;
        }
        /// <summary>
        /// 上传商品库存信息
        /// </summary>
        /// <param name="fileUploadParam"></param>
        /// <returns></returns>
        public UploadMsgItem UploadWarehouseGoods(FileUploadParam fileUploadParam)
        {
            UploadMsgItem msg = new UploadMsgItem();
            string logCode = fileUploadParam.userId + DateTime.Now.ToString("yyyyMMddHHmmssff");
            string fileName = logCode + ".xlsx";
            string errorFileName = "";
            FileManager fm = new FileManager();
            if (fm.fileCopy(fileUploadParam.fileTemp, fileName))
            {
                DataTable dt = fm.readExcelFileToDataTable(fileName);
                if (dt == null)
                {
                    msg.msg = "导入文档错误，请确认excel里的列是否正确，是否有相同名称的列。";
                    return msg;
                }
                if (dt.Rows.Count > 0)
                {
                    #region 校验导入文档的列
                    if (!dt.Columns.Contains("商品条码"))
                    {
                        msg.msg = "缺少“商品条码”列，";
                    }
                    if (!dt.Columns.Contains("商品名称(中文)"))
                    {
                        msg.msg += "缺少“商品名称(中文)”列，";
                    }
                    if (!dt.Columns.Contains("供货商"))
                    {
                        msg.msg += "缺少“供货商”列，";
                    }
                    if (!dt.Columns.Contains("所属仓库"))
                    {
                        msg.msg += "缺少“所属仓库”列，";
                    }
                    if (!dt.Columns.Contains("供货价"))
                    {
                        msg.msg += "缺少“供货价”列，";
                    }
                    if (!dt.Columns.Contains("供货数量"))
                    {
                        msg.msg += "缺少“供货数量”列，";
                    }
                    if (!dt.Columns.Contains("是否铺货"))
                    {
                        msg.msg += "缺少“是否铺货”列，";
                    }
                    if (msg.msg != null && msg.msg != "")
                    {
                        return msg;
                    }
                    #endregion


                    string error = "";
                    int successNum = 0, errorNum = 0;
                    ArrayList al = new ArrayList();
                    ArrayList errorAl = new ArrayList();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string goodsid = "", barcode = dt.Rows[i]["商品条码"].ToString(), supplierid = "", wid = "", wcode = "", wname = "", suppliercode = fileUploadParam.userId, status = "0",ifph="0";
                        double goodsnum = 0, inprice = 0;

                        try
                        {
                            goodsnum = Convert.ToDouble(dt.Rows[i]["供货数量"]);
                            inprice = Convert.ToDouble(dt.Rows[i]["供货价"]);
                        }
                        catch (Exception)
                        {
                            error += "第" + (i + 1).ToString() + "行条码为[" + barcode + "]的供货数量或供货价不是数字！";
                            continue;
                        }

                        //判断仓库信息
                        string whsql = "select id,wcode from t_base_warehouse where wname ='" + dt.Rows[i]["所属仓库"].ToString() + "' and suppliercode ='" + suppliercode + "'";
                        DataTable whdt = DatabaseOperationWeb.ExecuteSelectDS(whsql, "TABLE").Tables[0];
                        if (whdt.Rows.Count > 0)
                        {
                            wid = whdt.Rows[0]["id"].ToString();
                            wcode = whdt.Rows[0]["wcode"].ToString();
                        }
                        else
                        {
                            error += "第" + (i + 1).ToString() + "行条码为[" + barcode + "]的所属仓库不存在，请先联系运营添加仓库信息！";
                            continue;
                        }
                        //获取商品信息
                        string goodssql = "select id from t_goods_list where barcode ='" + barcode + "'";
                        DataTable goodsdt = DatabaseOperationWeb.ExecuteSelectDS(goodssql, "TABLE").Tables[0];
                        if (goodsdt.Rows.Count > 0)
                        {
                            successNum++;
                            goodsid = goodsdt.Rows[0][0].ToString();
                        }
                        else
                        {
                            errorAl.Add(barcode);
                            errorNum++;
                            status = "9";//商品表里没有的状态为9
                        }
                        //获取用户id
                        string userSql = "select id from t_user_list where usercode = '"+ suppliercode + "'";
                        DataTable userdt = DatabaseOperationWeb.ExecuteSelectDS(userSql, "TABLE").Tables[0];
                        if (userdt.Rows.Count > 0)
                        {
                            supplierid = userdt.Rows[0]["id"].ToString();
                        }
                        if (dt.Rows[i]["是否铺货"].ToString()=="是")
                        {
                            ifph = "1";
                        }
                        string insql = "insert into t_goods_warehouse_bak(logCode,goodsid,barcode," +
                            "wid,wcode,wname,goodsnum,inprice,supplierid,suppliercode,status,ifph) " +
                            "values('" + logCode + "','" + goodsid + "','" + barcode + "','" + wid + "','" + wcode + "'," +
                            "'" + wname + "'," + goodsnum + ",'" + inprice + "','" + supplierid + "','" + suppliercode + "','" + status + "','"+ ifph + "')";
                        al.Add(insql);
                    }
                    DataSet ds = null;
                    if (errorAl.Count > 0)
                    {
                        ds = fm.readGoodsTempletToDataSet();
                        if (ds ==null ||ds.Tables.Count == 0)
                        {
                            error = "商品信息模板找不到，请联系客服人员！";
                        }
                    }
                    
                    if (error != "")
                    {
                        msg.msg = error;
                        return msg;
                    }
                    else
                    {

                        try
                        {
                            if (DatabaseOperationWeb.ExecuteDML(al))
                            {
                                #region 处理没有信息的商品
                                string status = "1";//先设定为等待审核
                                if (errorAl.Count > 0)
                                {
                                    status = "0";//如果有不全的信息则状态为等待补充信息
                                    if (ds.Tables.Count > 0)
                                    {
                                        for (int i = 0; i < errorAl.Count; i++)
                                        {
                                            DataRow dr = ds.Tables[0].NewRow();
                                            dr[0] = errorAl[i];
                                            ds.Tables[0].Rows.Add(dr);
                                        }
                                        errorFileName = "err_" + fileName;
                                        fm.writeDataSetToExcel(ds, errorFileName);
                                        fm.updateFileToOSS(errorFileName, Global.ossB2BGoodsNum, errorFileName);
                                    }
                                }
                                #endregion

                                string inlogsql = "insert into t_log_upload(logCode,userCode,uploadTime,uploadType," +
                                "fileName,uploadNum,successNum,errorNum,errorFileUrl,status,remark) " +
                                "values('" + logCode + "','" + fileUploadParam.userId + "',now(),'warehouseGoodsNum'," +
                                " '" + fileName + "'," + dt.Rows.Count + "," + successNum + "," + errorNum + ",'" + Global.OssUrl + Global.ossB2BGoodsNum + errorFileName + "'," +
                                "'" + status + "','')";

                                if (DatabaseOperationWeb.ExecuteDML(inlogsql))
                                {
                                    string sqlid = "select id,status from t_log_upload where logCode = '" + logCode + "'";
                                    DataTable dtid = DatabaseOperationWeb.ExecuteSelectDS(sqlid, "TABLE").Tables[0];
                                    if (dtid.Rows.Count > 0)
                                    {
                                        msg.id = dtid.Rows[0]["id"].ToString();
                                        msg.status = dtid.Rows[0]["status"].ToString();
                                        msg.type = "1";
                                        msg.msg = "文件上传成功";
                                    }
                                    else
                                    {
                                        msg.msg = "文件存储失败！！";
                                    }
                                }
                                else
                                {
                                    msg.msg = "文件存储失败！！";
                                }
                            }
                            else
                            {
                                msg.msg = "文件存储失败！";
                            }
                        }
                        catch (Exception)
                        {
                            msg.msg = "文件存储出错！";
                        }
                    }
                }
                else
                {
                    msg.msg = "文件读取失败！";
                }
            }
            else
            {
                msg.msg = "文件上传失败！";
            }
            return msg;
        }
        /// <summary>
        /// 上传商品信息
        /// </summary>
        /// <param name="fileUploadParam"></param>
        /// <returns></returns>
        public MsgResult UploadGoods(FileUploadParam fileUploadParam)
        {
            MsgResult msg = new MsgResult();
            FileManager fm = new FileManager();

            if (fileUploadParam.fileTemp != null && fileUploadParam.fileTemp != "")
            {
                //图片zip保存到oss上
                fm.updateFileToOSS(fileUploadParam.fileTemp, Global.ossB2BGoods, fileUploadParam.logId + "_Img.zip");
            }
            if (fileUploadParam.fileTemp1 != null&& fileUploadParam.fileTemp1 != "")
            {
                fm.updateFileToOSS(fileUploadParam.fileTemp1, Global.ossB2BGoods, fileUploadParam.logId + "_Goods.xlsx");

                #region 去掉原来的处理商品信息的功能，改为操作人员处理 2019-4-29 han
                //string logCode = "";
                //double uploadNum = 0, errorNum = 0;
                //string sqlid = "select logCode,uploadNum,errorNum from t_log_upload where  id= '" + fileUploadParam.logId + "'";
                //DataTable dtid = DatabaseOperationWeb.ExecuteSelectDS(sqlid, "TABLE").Tables[0];
                //if (dtid.Rows.Count > 0)
                //{
                //    logCode = dtid.Rows[0]["logCode"].ToString();
                //    uploadNum = Convert.ToDouble(dtid.Rows[0]["uploadNum"]);
                //    errorNum = Convert.ToDouble(dtid.Rows[0]["errorNum"]);
                //}
                ////string st = "";
                ////DataSet ds = fm.readExcelToDataSet(fileUploadParam.fileTemp,out st);
                ////if (ds == null)
                ////{
                ////    msg.msg = st;
                ////    return msg;
                ////}
                ////DataTable dt = ds.Tables[0];
                //DataTable dt = fm.readExcelFileToDataTable(fileUploadParam.fileTemp1);

                //if (dt == null)
                //{
                //    msg.msg = "导入商品信息文档错误，请确认excel里的列是否正确，是否有相同名称的列。";
                //    return msg;
                //}
                //#region 判断列字段
                //if (!dt.Columns.Contains("商品条码"))
                //{
                //    msg.msg = "缺少“商品条码”列，";
                //}
                //if (!dt.Columns.Contains("品牌名称(中文)"))
                //{
                //    msg.msg = "缺少“品牌名称(中文)”列，";
                //}
                //if (!dt.Columns.Contains("品牌名称(外文)"))
                //{
                //    msg.msg = "缺少“品牌名称(外文)”列，";
                //}
                //if (!dt.Columns.Contains("商品名称(中文)"))
                //{
                //    msg.msg += "缺少“商品名称(中文)”列，";
                //}
                //if (!dt.Columns.Contains("商品名称(外文)"))
                //{
                //    msg.msg += "缺少“商品名称(外文)”列，";
                //}
                //if (!dt.Columns.Contains("一级分类"))
                //{
                //    msg.msg += "缺少“一级分类”列，";
                //}
                //if (!dt.Columns.Contains("二级分类"))
                //{
                //    msg.msg += "缺少“二级分类”列，";
                //}
                //if (!dt.Columns.Contains("三级分类"))
                //{
                //    msg.msg += "缺少“三级分类”列，";
                //}
                //if (!dt.Columns.Contains("原产国/地"))
                //{
                //    msg.msg += "缺少“原产国/地”列，";
                //}
                //if (!dt.Columns.Contains("货源国/地"))
                //{
                //    msg.msg += "缺少“货源国/地”列，";
                //}
                //if (!dt.Columns.Contains("型号"))
                //{
                //    msg.msg += "缺少“型号”列，";
                //}
                //if (!dt.Columns.Contains("颜色"))
                //{
                //    msg.msg += "缺少“颜色”列，";
                //}
                //if (!dt.Columns.Contains("口味"))
                //{
                //    msg.msg += "缺少“口味”列，";
                //}
                //if (!dt.Columns.Contains("毛重（kg)"))
                //{
                //    msg.msg += "缺少“毛重（kg)”列，";
                //}
                //if (!dt.Columns.Contains("净重(kg)"))
                //{
                //    msg.msg += "缺少“净重(kg)”列，";
                //}
                //if (!dt.Columns.Contains("计量单位"))
                //{
                //    msg.msg += "缺少“计量单位”列，";
                //}
                //if (!dt.Columns.Contains("商品规格CM:长*宽*高"))
                //{
                //    msg.msg += "缺少“商品规格CM:长*宽*高”列，";
                //}
                //if (!dt.Columns.Contains("包装规格CM:长*宽*高"))
                //{
                //    msg.msg += "缺少“包装规格CM:长*宽*高”列，";
                //}
                //if (!dt.Columns.Contains("适用人群"))
                //{
                //    msg.msg += "缺少“适用人群”列，";
                //}
                //if (!dt.Columns.Contains("使（食）用方法"))
                //{
                //    msg.msg += "缺少“使（食）用方法”列，";
                //}
                //if (!dt.Columns.Contains("用途/功效"))
                //{
                //    msg.msg += "缺少“用途/功效”列，";
                //}
                //if (!dt.Columns.Contains("卖点"))
                //{
                //    msg.msg += "缺少“卖点”列，";
                //}
                //if (!dt.Columns.Contains("配料成分含量"))
                //{
                //    msg.msg += "缺少“配料成分含量”列，";
                //}
                //if (!dt.Columns.Contains("保质期（天）"))
                //{
                //    msg.msg += "缺少“保质期（天）”列，";
                //}
                //if (!dt.Columns.Contains("贮存方式"))
                //{
                //    msg.msg += "缺少“贮存方式”列，";
                //}
                //if (!dt.Columns.Contains("注意事项"))
                //{
                //    msg.msg += "缺少“注意事项”列，";
                //}
                //if (!dt.Columns.Contains("指导零售价(RMB)"))
                //{
                //    msg.msg += "缺少“指导零售价(RMB)”列，";
                //}
                //if (msg.msg != null && msg.msg != "")
                //{
                //    return msg;
                //}
                //#endregion

                //if (dt.Rows.Count == 0)
                //{
                //    msg.msg = "没有数据，请核对";
                //    return msg;
                //}

                //string sqlc = "select id,name,parentid from t_goods_category";
                //DataTable dtc = DatabaseOperationWeb.ExecuteSelectDS(sqlc, "TABLE").Tables[0];

                //ArrayList al = new ArrayList();
                //ArrayList al1 = new ArrayList();//根据
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    string error = "";
                //    //判断条码是否已经存在
                //    string sqltm = "select id from t_goods_list where barcode = '" + dt.Rows[i]["商品条码"].ToString() + "'";
                //    DataTable dttm = DatabaseOperationWeb.ExecuteSelectDS(sqltm, "TABLE").Tables[0];
                //    if (dttm.Rows.Count > 0)
                //    {
                //        error += "第" + (i + 2).ToString() + "行条码已存在，请核对\r\n";
                //    }
                //    //判断净重毛重是否为数字
                //    double d = 0;
                //    if (!double.TryParse(dt.Rows[i]["净重(kg)"].ToString(), out d))
                //    {
                //        error += "第" + (i + 2).ToString() + "行净重(kg)填写错误，请核对\r\n";
                //    }
                //    if (!double.TryParse(dt.Rows[i]["毛重(kg)"].ToString(), out d))
                //    {
                //        error += "第" + (i + 2).ToString() + "行毛重(kg)填写错误，请核对\r\n";
                //    }
                //    if (error != "")
                //    {
                //        msg.msg += error;
                //        continue;
                //    }
                //    //处理商品分类
                //    string c1 = "", c2 = "", c3 = "0";
                //    //string sqlc1 = "select id from t_goods_category where name ='" + dt.Rows[i]["一级分类"].ToString() + "'";
                //    //DataTable dtc1 = DatabaseOperationWeb.ExecuteSelectDS(sqlc1, "TABLE").Tables[0];
                //    //if (dtc1.Rows.Count > 0)

                //    DataRow[] drs1 = dtc.Select("name ='" + dt.Rows[i]["一级分类"].ToString() + "'");
                //    if (drs1.Length>0)
                //    {
                //        c1 = drs1[0]["id"].ToString();
                //        //string sqlc2 = "select * from t_goods_category " +
                //        //    "where name ='" + dt.Rows[i]["二级分类"].ToString() + "' and parentid=" + c1 + "";
                //        //DataTable dtc2 = DatabaseOperationWeb.ExecuteSelectDS(sqlc2, "TABLE").Tables[0];
                //        //if (dtc2.Rows.Count > 0)
                //        DataRow[] drs2 = dtc.Select("name ='" + dt.Rows[i]["二级分类"].ToString() + "' and parentid=" + c1);
                //        if (drs2.Length>0)
                //        {
                //            c2 = drs2[0]["id"].ToString();
                //            ////string sqlc3 = "select * from t_goods_category " +
                //            ////    "where name ='" + dt.Rows[i]["三级分类"].ToString() + "' and parentid=" + c2 + "";
                //            ////DataTable dtc3 = DatabaseOperationWeb.ExecuteSelectDS(sqlc3, "TABLE").Tables[0];
                //            ////if (dtc3.Rows.Count > 0)
                //            //DataRow[] drs3 = dtc.Select("name = '" + dt.Rows[i]["三级分类"].ToString() + "' and parentid = " + c2);
                //            //if (drs3.Length > 0)
                //            //{
                //            //    c3 = drs3[0]["id"].ToString();
                //            //}
                //            //else
                //            //{
                //            //    error += "第" + (i + 2).ToString() + "行三级分类填写错误，请核对\r\n";
                //            //}
                //        }
                //        else
                //        {
                //            error += "第" + (i + 2).ToString() + "行二级分类填写错误，请核对\r\n";
                //        }
                //    }
                //    else
                //    {
                //        error += "第" + (i + 2).ToString() + "行一级分类填写错误，请核对\r\n";
                //    }
                //    if (error != "")
                //    {
                //        msg.msg += error;
                //        continue;
                //    }

                //    string insql = "insert into t_goods_list(barcode,brand,brandE,goodsName,goodsNameE,catelog1,catelog2,catelog3," +
                //                   "country,source,model,color,flavor,GW,NW,MEA,LWH,packLWH,applicable,useMethod,efficacy,USP," +
                //                   "formula,shelfLife,storage,needAttention,price,inputUserCode) " +
                //                   "values('" + dt.Rows[i]["商品条码"].ToString().Replace("'", "") + "','" + dt.Rows[i]["品牌名称(中文)"].ToString().Replace("'", "‘")
                //                   + "','" + dt.Rows[i]["品牌名称(外文)"].ToString().Replace("'","‘") + "','" + dt.Rows[i]["商品名称(中文)"].ToString().Replace("'", "‘")
                //                   + "','" + dt.Rows[i]["商品名称(外文)"].ToString().Replace("'", "‘") + "','" + c1
                //                   + "','" + c2 + "','" + c3
                //                   + "','" + dt.Rows[i]["原产国/地"].ToString().Replace("'", "‘") + "','" + dt.Rows[i]["货源国/地"].ToString().Replace("'", "‘")
                //                   + "','" + dt.Rows[i]["型号"].ToString() + "','" + dt.Rows[i]["颜色"].ToString().Replace("'", "‘")
                //                   + "','" + dt.Rows[i]["口味"].ToString().Replace("'", "‘") + "'," + dt.Rows[i]["毛重（kg)"].ToString()
                //                   + "," + dt.Rows[i]["净重(kg)"].ToString() + ",'" + dt.Rows[i]["计量单位"].ToString()
                //                   + "','" + dt.Rows[i]["商品规格CM:长*宽*高"].ToString().Replace("'", "‘") + "','" + dt.Rows[i]["包装规格CM:长*宽*高"].ToString().Replace("'", "‘")
                //                   + "','" + dt.Rows[i]["适用人群"].ToString().Replace("'", "‘") + "','" + dt.Rows[i]["使（食）用方法"].ToString().Replace("'", "‘")
                //                   + "','" + dt.Rows[i]["用途/功效"].ToString().Replace("'", "‘") + "','" + dt.Rows[i]["卖点"].ToString().Replace("'", "‘")
                //                   + "','" + dt.Rows[i]["配料成分含量"].ToString().Replace("'", "‘") + "','" + dt.Rows[i]["保质期（天）"].ToString().Replace("'", "‘")
                //                   + "','" + dt.Rows[i]["贮存方式"].ToString().Replace("'", "‘") + "','" + dt.Rows[i]["注意事项"].ToString().Replace("'", "‘")
                //                   + "','" + dt.Rows[i]["指导零售价(RMB)"].ToString() + "','" + fileUploadParam.userId + "')";
                //    al.Add(insql);
                //    string upsql = "update t_goods_warehouse_bak set status = '0' where logCode= '" + dtid.Rows[0][0].ToString() + "' and  barcode= '" + dt.Rows[i]["商品条码"].ToString().Replace("'", "") + "' ";
                //    al1.Add(upsql);

                //}
                //if (msg.msg != "")
                //{
                //    return msg;
                //}

                //if (DatabaseOperationWeb.ExecuteDML(al))
                //{
                //    if (DatabaseOperationWeb.ExecuteDML(al1))
                //    {
                //        string sql9 = "select count(*) from t_goods_warehouse_bak where logCode= '" + logCode + "' and status='9' ";
                //        DataTable dt9 = DatabaseOperationWeb.ExecuteSelectDS(sql9, "TABLE").Tables[0];
                //        int count = Convert.ToInt16(dt9.Rows[0][0]);
                //        if (count > 0)
                //        {
                //            string upsql = "update t_log_upload set successNum=" + (uploadNum - errorNum) + ",errorNum=" + errorNum +
                //                ",goodsFile='" + Global.OssUrl + Global.ossB2BGoods + fileUploadParam.logId + "_Goods.xlsx'" +
                //                ",goodsImgFile='" + Global.OssUrl + Global.ossB2BGoods + fileUploadParam.logId + "_Img.zip' " +
                //                " where logCode='" + logCode + "'";
                //            DatabaseOperationWeb.ExecuteDML(upsql);
                //        }
                //        else
                //        {
                //            string upsql = "update t_log_upload set successNum=" + uploadNum + ",errorNum=0,status='1'" +
                //                ",goodsFile='" + Global.OssUrl + Global.ossB2BGoods + fileUploadParam.logId + "_Goods.xlsx'" +
                //                ",goodsImgFile='" + Global.OssUrl + Global.ossB2BGoods + fileUploadParam.logId + "_Img.zip'" +
                //                " where logCode='" + logCode + "'";
                //            DatabaseOperationWeb.ExecuteDML(upsql);
                //        }
                //        ArrayList goodsAL = new ArrayList();
                //        //获取商品信息
                //        string goodssql = "select g.id,g.barcode from t_goods_warehouse_bak b ,t_goods_list g " +
                //            "where  b.barcode = g.barcode and b.logCode = '" + logCode + "' and b.goodsid = 0";
                //        DataTable goodsdt = DatabaseOperationWeb.ExecuteSelectDS(goodssql, "TABLE").Tables[0];
                //        for (int i = 0; i < goodsdt.Rows.Count; i++)
                //        {
                //            string sql = "update t_goods_warehouse_bak set goodsid = '"+ goodsdt.Rows[i]["id"].ToString() + "' " +
                //                "where logCode= '" + logCode + "' and barcode = '"+ goodsdt.Rows[i]["barcode"].ToString() + "' ";
                //            goodsAL.Add(sql);
                //        }
                //        DatabaseOperationWeb.ExecuteDML(goodsAL);
                //        msg.type = "1";
                //        msg.msg = "上传并保存成功";
                //    }
                //}
                //else
                //{
                //    msg.msg = "数据保存失败！";
                //}
                #endregion
            }
            if (msg.msg=="")
            {
                msg.type = "1";
                msg.msg = "上传成功";
            }
            return msg;
        }
        /// <summary>
        /// 商品上架详情
        /// </summary>
        /// <param name="fileUploadParam"></param>
        /// <returns></returns>
        public WarehouseGoodsListItem getWarehouseGoodsList(FileUploadParam fileUploadParam)
        {
            WarehouseGoodsListItem warehouseGoodsListItem = new WarehouseGoodsListItem();
            string sql1 = "select l.logCode,l.goodsFile,l.goodsImgFile,u.username from t_log_upload l,t_user_list u " +
                          "where l.userCode = u.usercode and l.id = " + fileUploadParam.logId;
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "TABLE").Tables[0];
            if (dt1.Rows.Count>0)
            {
                warehouseGoodsListItem.logId = fileUploadParam.logId;
                warehouseGoodsListItem.username = dt1.Rows[0]["username"].ToString();
                warehouseGoodsListItem.goodsUrl = dt1.Rows[0]["goodsFile"].ToString();

                warehouseGoodsListItem.goodsImgUrl = dt1.Rows[0]["goodsImgFile"].ToString().Replace(".zip", "");

                warehouseGoodsListItem.warehouseGoodsList = new List<WarehouseGoodsItem>();
                string sql2 = "select b.*,w.wname as name,g.goodsName " +
                    "from t_base_warehouse w,t_goods_warehouse_bak b LEFT JOIN t_goods_list g on g.barcode = b.barcode " +
                    "where b.wid = w.id and b.logCode= '" + dt1.Rows[0]["logCode"].ToString() + "'";
                DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "TABLE").Tables[0];
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    WarehouseGoodsItem WarehouseGoodsItem = new WarehouseGoodsItem();
                    WarehouseGoodsItem.id = dt2.Rows[i]["id"].ToString();
                    WarehouseGoodsItem.barcode = dt2.Rows[i]["barcode"].ToString();
                    WarehouseGoodsItem.goodsName = dt2.Rows[i]["goodsName"].ToString();
                    WarehouseGoodsItem.wname = dt2.Rows[i]["name"].ToString();
                    WarehouseGoodsItem.inprice = Convert.ToDouble( dt2.Rows[i]["inprice"]);
                    WarehouseGoodsItem.goodsnum = Convert.ToDouble(dt2.Rows[i]["goodsnum"]);
                    if (dt2.Rows[i]["status"].ToString() == "0")
                    {
                        WarehouseGoodsItem.status = "等待审核";
                    }
                    else if (dt2.Rows[i]["status"].ToString() == "1")
                    {
                        WarehouseGoodsItem.status = "审核成功";
                    }
                    else if (dt2.Rows[i]["status"].ToString() == "2")
                    {
                        WarehouseGoodsItem.status = "已驳回";
                    }
                    else if (dt2.Rows[i]["status"].ToString() == "9")
                    {
                        WarehouseGoodsItem.status = "等待补全信息";
                    }
                    warehouseGoodsListItem.warehouseGoodsList.Add(WarehouseGoodsItem);
                }
            }
            return warehouseGoodsListItem;
        }
        /// <summary>
        /// 商品上架审核操作
        /// </summary>
        /// <param name="examineParam"></param>
        /// <returns></returns>
        public MsgResult examineWarehouseGood(ExamineParam examineParam,string userId)
        {
            MsgResult msg = new MsgResult();
            string wsql = "select * from t_goods_warehouse where suppliercode = '"+userId+"'";
            DataTable wdt = DatabaseOperationWeb.ExecuteSelectDS(wsql, "TABLE").Tables[0];
            string sql = "select w.*,l.status from t_log_upload l ,t_goods_warehouse_bak w " +
                "where l.logCode = w.logCode and l.id =" + examineParam.logId;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];

            if (dt.Rows.Count>0)
            {
                if (dt.Rows[0]["status"].ToString()!="0"&&dt.Rows[0]["status"].ToString() != "1")
                {
                    msg.msg = "记录状态有误，请重试";
                    return msg;
                }
                ArrayList al = new ArrayList();
                ArrayList deleteAl = new ArrayList();
                ArrayList insqlAl= new ArrayList();
                int errorNum = 0, successNum = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bool isNot = true;
                    for (int j = 0; j < examineParam.logGoodsId.Length; j++)
                    {
                        if (dt.Rows[i]["id"].ToString()== examineParam.logGoodsId[j])
                        {
                            successNum++;
                            string upsql = "update t_goods_warehouse_bak set status='1' where id = "+ dt.Rows[i]["id"].ToString();
                            al.Add(upsql);

                            DataRow[] drs = wdt.Select("wid='" + dt.Rows[i]["wid"].ToString() + "' and barcode='" + dt.Rows[i]["barcode"].ToString() + "'");
                            if (drs.Length>0)
                            {
                                string upsql1 = "update t_goods_warehouse " +
                                    "set goodsnum=goodsnum+" + dt.Rows[i]["goodsnum"].ToString() + " ," +
                                        "inprice= " + dt.Rows[i]["inprice"].ToString() + " ,ifph='"+ dt.Rows[i]["ifph"].ToString() + "'" +
                                    "where id = "+drs[0]["id"].ToString();
                                insqlAl.Add(upsql1);
                            }
                            else
                            {
                                string insql = "insert into t_goods_warehouse(goodsid,barcode,wid,wcode," +
                                "goodsnum,inprice,supplierid,suppliercode,flag,status,ifph) " +
                                "values(" + dt.Rows[i]["goodsid"].ToString() + ",'" + dt.Rows[i]["barcode"].ToString() + "'," + dt.Rows[i]["wid"].ToString() + ",'" + dt.Rows[i]["wcode"].ToString() + "'" +
                                "," + dt.Rows[i]["goodsnum"].ToString() + "," + dt.Rows[i]["inprice"].ToString() + ",'" + dt.Rows[i]["supplierid"].ToString() + "','" + dt.Rows[i]["suppliercode"].ToString() + "','1','0','"+ dt.Rows[i]["ifph"].ToString() + "')";
                                insqlAl.Add(insql);
                            }

                            //新增判断：如果入库的商品没有默认供货商和仓库，则审核通过时自动添加默认供货商和仓库
                            string upsqlw = "update t_goods_list set supplierId='" + dt.Rows[i]["supplierid"].ToString() + "'," +
                                "supplierCode='" + dt.Rows[i]["suppliercode"].ToString() + "',wId=" + dt.Rows[i]["wid"].ToString()  +
                                " where wId is null and barcode = '" + dt.Rows[i]["barcode"].ToString() + "'";
                            insqlAl.Add(upsqlw);
                            isNot = false;
                            continue;
                        }
                    }
                    if (isNot)
                    {
                        errorNum++;
                        string upsql = "update t_goods_warehouse_bak set status='2' where id = " + dt.Rows[i]["id"].ToString();
                        al.Add(upsql);
                    }
                }
                if (DatabaseOperationWeb.ExecuteDML(al))
                {
                    string upsql = "";
                    if (errorNum==0)
                    {
                        upsql = "update t_log_upload set status='2',successNum="+ successNum + ",errorNum="+ errorNum  +
                                       " where id = " + examineParam.logId;
                    }
                    else
                    {
                        upsql = "update t_log_upload set status='3',successNum=" + successNum + "," +
                                       "errorNum=" + errorNum + ",remark='"+ examineParam.logText + "'" +
                                       " where id = " + examineParam.logId;
                    }
                    //DatabaseOperationWeb.ExecuteDML(deleteAl);
                    DatabaseOperationWeb.ExecuteDML(insqlAl);
                    if (DatabaseOperationWeb.ExecuteDML(upsql))
                    {
                        msg.msg = "审核完成";
                        msg.type = "1";
                    }
                }
                else
                {
                    msg.msg = "数据库保存失败";
                }
            }
            else
            {
                msg.msg = "未找到对应编号的记录";
            }
            return msg;
        }
        #endregion

        #region 仓库列表
        /// <summary>
        /// 获取仓库列表
        /// </summary>
        /// <returns></returns>
        public PageResult GetWarehouseList(GoodsUserParam goodsUserParam)
        {
            PageResult wareHouseResult = new PageResult();
            wareHouseResult.pagination = new Page(goodsUserParam.current, goodsUserParam.pageSize);
            wareHouseResult.list = new List<Object>();
            UserDao userDao = new UserDao();
            string userType = userDao.getUserType(goodsUserParam.userId);
            string st = "";
            if (userType == "1")//供应商 
            {
                st += " and w.suppliercode ='" + goodsUserParam.userId + "' ";
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {

            }
            else
            {
                return wareHouseResult;
            }
            string sql = "select w.*,u.id as userId,u.username from t_base_warehouse w ,t_user_list u where w.suppliercode = u.usercode " + st +
                " order by w.id desc  LIMIT " + (goodsUserParam.current - 1) * goodsUserParam.pageSize + "," + goodsUserParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_base_warehouse").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                WarehouseItem warehouseItem = new WarehouseItem();
                warehouseItem.wid = dt.Rows[i]["id"].ToString();
                warehouseItem.wcode = dt.Rows[i]["wcode"].ToString();
                warehouseItem.wname = dt.Rows[i]["wname"].ToString();
                warehouseItem.supplier = dt.Rows[i]["username"].ToString();
                warehouseItem.supplierId = dt.Rows[i]["userId"].ToString();
                warehouseItem.taxation = dt.Rows[i]["taxation"].ToString();
                warehouseItem.taxation2 = dt.Rows[i]["taxation2"].ToString();
                warehouseItem.taxation2type = dt.Rows[i]["taxation2type"].ToString();
                warehouseItem.taxation2line = dt.Rows[i]["taxation2line"].ToString();
                warehouseItem.freight = dt.Rows[i]["freight"].ToString();
                warehouseItem.orderCode = dt.Rows[i]["orderCode"].ToString();
                warehouseItem.if_send = dt.Rows[i]["if_send"].ToString();
                warehouseItem.if_CK = dt.Rows[i]["if_CK"].ToString();

                wareHouseResult.list.Add(warehouseItem);
            }
            string sql1 = "select count(*) from t_base_warehouse where 1=1 " + st;

            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "TABLE").Tables[0];
            wareHouseResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return wareHouseResult;
        }

        public MsgResult AddWareHouse(WarehouseItem warehouseItem)
        {
            MsgResult msg = new MsgResult();
            string code = GetPYChar(warehouseItem.wname).ToUpper();
            if (code=="*")
            {
                code = "CK";
            }
            string sqlw = "select count(*) from t_base_warehouse where wcode like '" + code + "%'";
            DataTable dtw = DatabaseOperationWeb.ExecuteSelectDS(sqlw, "TABLE").Tables[0];
            if (dtw.Rows[0][0].ToString() != "0")
            {
                code += (Convert.ToInt16(dtw.Rows[0][0]) + 1).ToString();
            }

            string sql1 = "select usercode from t_user_list where id = '" + warehouseItem.supplierId + "'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "TABLE").Tables[0];
            if (dt1.Rows.Count > 0)
            {
                string sql2 = "insert into t_base_warehouse(wcode,wname,supplierid,suppliercode,taxation," +
                              "taxation2,taxation2type,taxation2line,freight,flag) " +
                              "values('" + code + "','" + warehouseItem.wname + "','" + warehouseItem.supplierId
                              + "','" + dt1.Rows[0][0].ToString() + "','" + warehouseItem.taxation
                              + "','" + warehouseItem.taxation2 + "','" + warehouseItem.taxation2type
                              + "','" + warehouseItem.taxation2line + "','" + warehouseItem.freight + "','1')";
                if (DatabaseOperationWeb.ExecuteDML(sql2))
                {
                    msg.type = "1";
                    msg.msg = "保存成功";
                }
                else
                {
                    msg.msg = "保存失败";
                }
            }
            else
            {
                msg.msg = "供应商编号错误";
            }
            return msg;
        }

        public MsgResult UpdateWareHouse(WarehouseItem warehouseItem)
        {
            MsgResult msg = new MsgResult();
            //string code = GetPYChar(warehouseItem.wname).ToUpper();
            //string sqlw = "select count(*) from t_base_warehouse where wcode ='" + code + "'";
            //DataTable dtw = DatabaseOperationWeb.ExecuteSelectDS(sqlw, "TABLE").Tables[0];
            //if (dtw.Rows[0][0].ToString() != "0")
            //{
            //    code += (Convert.ToInt16(dtw.Rows[0][0]) + 1).ToString();
            //}

            string sql1 = "select usercode from t_user_list where id = '" + warehouseItem.supplierId + "'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "TABLE").Tables[0];
            if (dt1.Rows.Count > 0)
            {
                string sql2 = "update t_base_warehouse set wname='" + warehouseItem.wname + "'" +
                    ",supplierid='" + warehouseItem.supplierId + "',suppliercode='" + dt1.Rows[0][0].ToString() + "'" +
                    ",taxation='" + warehouseItem.taxation + "',taxation2='" + warehouseItem.taxation2 + "'" +
                    ",taxation2type='" + warehouseItem.taxation2type + "',taxation2line='" + warehouseItem.taxation2line + "'" +
                    ",freight='" + warehouseItem.freight + "' where id = '" + warehouseItem.wid + "'";
                if (DatabaseOperationWeb.ExecuteDML(sql2))
                {
                    msg.type = "1";
                    msg.msg = "修改成功";
                }
                else
                {
                    msg.msg = "修改失败";
                }
            }
            else
            {
                msg.msg = "供应商编号错误";
            }
            return msg;
        }

        public MsgResult DelWareHouse(WarehouseItem warehouseItem)
        {
            MsgResult msg = new MsgResult();
            string sql = "select * from t_order_list l ,t_base_warehouse w where l.warehouseCode = w.wcode and w.id = "+warehouseItem.wid;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            if (dt.Rows.Count>0)
            {
                msg.msg = "该仓库下已有数据，无法删除";
            }
            else
            {
                string sql2 = "delete from  t_base_warehouse where id = '" + warehouseItem.wid + "'";
                if (DatabaseOperationWeb.ExecuteDML(sql2))
                {
                    msg.type = "1";
                    msg.msg = "修改成功";
                }
                else
                {
                    msg.msg = "修改失败";
                }
            }
            

            return msg;
        }

        public List<SupplierItem> getSupplier(string userId)
        {
            List<SupplierItem> ls = new List<SupplierItem>();
            string sql = "select username,id from t_user_list where usertype='1' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SupplierItem supplierItem = new SupplierItem();
                supplierItem.supplier = dt.Rows[i]["username"].ToString();
                supplierItem.supplierId = dt.Rows[i]["id"].ToString();
                ls.Add(supplierItem);
            }
            return ls;
        }
        ///      
        /// 取单个字符的拼音声母     
        ///      
        /// 要转换的单个汉字     
        /// 拼音声母     
        public string GetPYChar(string c)
        {
            byte[] array = new byte[2];
            array = System.Text.Encoding.Default.GetBytes(c);
            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));
            if (i < 0xB0A1) return "*";
            if (i < 0xB0C5) return "a";
            if (i < 0xB2C1) return "b";
            if (i < 0xB4EE) return "c";
            if (i < 0xB6EA) return "d";
            if (i < 0xB7A2) return "e";
            if (i < 0xB8C1) return "f";
            if (i < 0xB9FE) return "g";
            if (i < 0xBBF7) return "h";
            if (i < 0xBFA6) return "j";
            if (i < 0xC0AC) return "k";
            if (i < 0xC2E8) return "l";
            if (i < 0xC4C3) return "m";
            if (i < 0xC5B6) return "n";
            if (i < 0xC5BE) return "o";
            if (i < 0xC6DA) return "p";
            if (i < 0xC8BB) return "q";
            if (i < 0xC8F6) return "r";
            if (i < 0xCBFA) return "s";
            if (i < 0xCDDA) return "t";
            if (i < 0xCEF4) return "w";
            if (i < 0xD1B9) return "x";
            if (i < 0xD4D1) return "y";
            if (i < 0xD7FA) return "z";
            return "*";
        }
        #endregion
    }
}
