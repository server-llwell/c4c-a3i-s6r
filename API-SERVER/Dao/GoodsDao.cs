﻿using API_SERVER.Buss;
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
        #region 商品库存列表
        /// <summary>
        /// 获取品牌下拉框
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 获取品牌下拉框
        /// </summary>
        /// <returns></returns>
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
        /// 获商品列表
        /// </summary>
        /// <returns></returns>
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
            string sql = "select g.id,g.brand,g.goodsName,g.barcode,g.slt,g.source,w.wname,wh.goodsnum,wh.flag,wh.`status`,u.username " +
                        " from t_goods_list g ,t_goods_warehouse wh,t_base_warehouse w ,t_user_list u " +
                        " where g.id = wh.goodsid and  wh.wid = w.id and wh.suppliercode = u.usercode " + st +
                        " order by g.brand,g.barcode  LIMIT " + (goodsSeachParam.current - 1) * goodsSeachParam.pageSize + "," + goodsSeachParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GoodsListItem goodsListItem = new GoodsListItem();
                goodsListItem.id = dt.Rows[i]["id"].ToString();
                goodsListItem.brand = dt.Rows[i]["brand"].ToString();
                goodsListItem.supplier = dt.Rows[i]["username"].ToString();
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
                    "and g.barCode = '" + dt.Rows[i]["barcode"].ToString() + "'  " + st1;
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

        /// <summary>
        /// 获取单个商品
        /// </summary>
        /// <returns></returns>
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
            if (fm.saveFileByBase64String(fileUploadParam.byte64, fileName))
            {
                DataTable dt = fm.readExcelFileToDataTable(fileName);
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
                        string goodsid = "", barcode = dt.Rows[i]["商品条码"].ToString(), wid = "", wcode = "", wname = "", suppliercode = fileUploadParam.userId, status = "0";
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
                        string insql = "insert into t_goods_warehouse_bak(logCode,goodsid,barcode," +
                            "wid,wcode,wname,goodsnum,inprice,suppliercode,status) " +
                            "values('" + logCode + "','" + goodsid + "','" + barcode + "','" + wid + "','" + wcode + "'," +
                            "'" + wname + "'," + goodsnum + ",'" + inprice + "','" + suppliercode + "','" + status + "')";
                        al.Add(insql);
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
                                    DataSet ds = fm.readExcelToDataSet("商品信息模板.xlsx");
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
                                        fm.updateFileToOSS(errorFileName, Global.ossB2BGoodsNum);
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

        public MsgResult UploadGoods(FileUploadParam fileUploadParam)
        {
            MsgResult msg = new MsgResult();
            FileManager fm = new FileManager();
            if (fm.saveFileByBase64String(fileUploadParam.byte64Zip, fileUploadParam.logId + "_Img.zip"))
            {
                msg.type = "1";
                msg.msg = "上传成功";
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
            string sql = "select w.*,u.username from t_base_warehouse w ,t_user_list u where w.suppliercode = u.usercode " + st +
                " order by w.id desc  LIMIT " + (goodsUserParam.current - 1) * goodsUserParam.pageSize + "," + goodsUserParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_base_warehouse").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                WarehouseItem warehouseItem = new WarehouseItem();
                warehouseItem.wid = dt.Rows[i]["id"].ToString();
                warehouseItem.wcode = dt.Rows[i]["wcode"].ToString();
                warehouseItem.wname = dt.Rows[i]["wname"].ToString();
                warehouseItem.supplier = dt.Rows[i]["username"].ToString();
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
            string sqlw = "select count(*) from t_base_warehouse where wcode ='" + code + "'";
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
                              + "','" + warehouseItem.taxation2line + "','" + warehouseItem.freight + "','1',)";
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