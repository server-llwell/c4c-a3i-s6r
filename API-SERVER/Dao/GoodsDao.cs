using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
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
                "where g.id = wh.goodsid and  wh.wid = w.id "+st+" order by g.brand,g.barcode  LIMIT " + (goodsSeachParam.current - 1) * goodsSeachParam.pageSize + "," + goodsSeachParam.pageSize ;
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
            string sql1 = "select count(*) from t_goods_list g ,t_goods_warehouse wh,t_base_warehouse w " +
                "where g.id = wh.goodsid and  wh.wid = w.id  " + st;

            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
            goodsResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return goodsResult;
        }

        public GoodsItem GetGoodsById(GoodsSeachParam goodsSeachParam)
        {
            GoodsItem goodsItem = new GoodsItem();
            string sql = "select g.id,g.brand,g.goodsName,g.barcode,c.`name` as catelog3,g.slt,g.source,g.model,g.applicable," +
                "g.formula,g.shelflife,g.storage,w.wname,wh.goodsnum,wh.inprice " +
                "from t_goods_list g ,t_goods_warehouse wh, t_base_warehouse w ,t_goods_category c " +
                "where g.id = wh.goodsid and wh.wid = w.id and g.catelog3 = c.id and g.id= " + goodsSeachParam.goodsId;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                goodsItem.id = dt.Rows[i]["id"].ToString();
                goodsItem.brand = dt.Rows[i]["brand"].ToString();
                goodsItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                goodsItem.barcode = dt.Rows[i]["barcode"].ToString();
                goodsItem.catelog3 = dt.Rows[i]["catelog3"].ToString();
                goodsItem.slt = dt.Rows[i]["slt"].ToString();
                goodsItem.source = dt.Rows[i]["source"].ToString();
                goodsItem.model = dt.Rows[i]["model"].ToString();
                goodsItem.applicable = dt.Rows[i]["applicable"].ToString();
                goodsItem.formula = dt.Rows[i]["formula"].ToString();
                goodsItem.shelfLife = dt.Rows[i]["shelflife"].ToString();
                goodsItem.storage = dt.Rows[i]["storage"].ToString();
                goodsItem.wname = dt.Rows[i]["wname"].ToString();
                goodsItem.goodsnum = dt.Rows[i]["goodsnum"].ToString();
                goodsItem.inprice = dt.Rows[i]["inprice"].ToString();
            }
            return goodsItem;
        }
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
                st += " and suppliercode ='" + goodsUserParam.userId + "' ";
            }
            else if (userType == "0" || userType == "5")//管理员或客服
            {

            }
            else
            {
                return wareHouseResult;
            }
            string sql = "select * from t_base_warehouse where 1=1 " + st + " order by id desc  LIMIT " + (goodsUserParam.current - 1) * goodsUserParam.pageSize + "," + goodsUserParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_base_warehouse").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                WarehouseItem warehouseItem = new WarehouseItem();
                warehouseItem.wid = dt.Rows[i]["id"].ToString();
                warehouseItem.wcode = dt.Rows[i]["wcode"].ToString();
                warehouseItem.wname = dt.Rows[i]["wname"].ToString();
                warehouseItem.supplierid = dt.Rows[i]["supplierid"].ToString();
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

        public MsgResult Do_UploadWarehouseGoods(FileUploadParam fileUploadParam)
        {
            MsgResult msg = new MsgResult();
            string logCode = fileUploadParam.userId + DateTime.Now.ToString("yyyyMMddHHmmssff");
            string fileName = logCode + ".xlsx";
            FileManager fm = new FileManager();
            if (fm.saveFileByBase64String(fileUploadParam.byte64, fileName))
            {
                DataTable dt = fm.readExcelFileToDataTable(fileName);
                if (dt.Rows.Count>0)
                {
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
                    if (msg.msg!="")
                    {
                        return msg;
                    }
                    string error = "";
                    int successNum = 0, errorNum = 0;
                    ArrayList al = new ArrayList();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string goodsid = "", barcode = dt.Rows[i]["商品条码"].ToString(), wid = "", wcode = "",wname="",  suppliercode = fileUploadParam.userId, status = "0";
                        double goodsnum = 0,inprice = 0;

                        try
                        {
                            goodsnum = Convert.ToDouble(dt.Rows[i]["供货数量"]);
                            inprice = Convert.ToDouble(dt.Rows[i]["供货价"]);
                        }
                        catch (Exception)
                        {
                            error += "第"+(i+1).ToString()+"行条码为["+barcode+"]的供货数量或供货价不是数字！";
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
                        string goodssql = "select id from t_goods_list where barcode ='"+dt.Rows[i]["商品条码"].ToString()+"'";
                        DataTable goodsdt = DatabaseOperationWeb.ExecuteSelectDS(goodssql, "TABLE").Tables[0];
                        if (goodsdt.Rows.Count>0)
                        {
                            successNum++;
                            goodsid = goodsdt.Rows[0][0].ToString();
                        }
                        else
                        {
                            errorNum++;
                            status = "9";//商品表里没有的状态为9
                        }
                        string insql = "insert into t_goods_warehouse_bak(logCode,goodsid,barcode," +
                            "wid,wcode,wname,goodsnum,inprice,suppliercode,status) " +
                            "values('" + logCode + "','" + goodsid + "','" + barcode + "','" + wid + "','" + wcode + "'," +
                            "'" + wname + "'," + goodsnum + ",'" + inprice + "','" + suppliercode + "','" + status + "')";
                        al.Add(insql);
                    }
                    if (error!="")
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
                                string inlogsql = "insert into t_log_upload(logCode,userCode,uploadTime,uploadType," +
                                    "fileName,uploadNum,successNum,errorNum,status,remark) " +
                                    "values('" + logCode + "','" + fileUploadParam.userId + "',now(),'warehouseGoodsNum'," +
                                    "'" + fileName + "'," + dt.Rows.Count + "," + dt.Rows.Count + "," + dt.Rows.Count + ",'0','')";

                                if (DatabaseOperationWeb.ExecuteDML(inlogsql))
                                {
                                    msg.type = "1";
                                    msg.msg = "上传成功";
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
    }
}
