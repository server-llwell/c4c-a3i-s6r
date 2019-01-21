using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static API_SERVER.Buss.WarehouseBuss;

namespace API_SERVER.Dao
{
    public class WarehouseDao
    {
        private string path = System.Environment.CurrentDirectory;

        public WarehouseDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }
        public PageResult CollectGoods(CollectGoodsIn cgi, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(cgi.current, cgi.pageSize);
            pageResult.list = new List<Object>();
            string time = "";
            if (cgi.date != null && cgi.date.Length == 2)
            {
                time = "and sendTime between  str_to_date('" + cgi.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + cgi.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            string sd = "";
            if (cgi.sendid != null && cgi.sendid != "")
            {
                sd = " and id='" + cgi.sendid + "' ";
            }
            string st = "";
            if (cgi.sendType != null && cgi.sendType != "")
            {
                st = " and sendType='" + cgi.sendType + "'";
            }
            string zt = "";
            if (cgi.status != null && cgi.status != "")
            {
                zt = " and status='" + cgi.status + "'";
            }

            string sql = "select sendType,id,goodsTotal,sendTime,sendName,sendTel,`status` from t_warehouse_send where purchasersCode='" + userId + "' " + sd + st + zt + time +
                "order by sendTime desc limit " + (cgi.current - 1) * cgi.pageSize + "," + cgi.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CollectGoodsItem cgt = new CollectGoodsItem();
                    cgt.keyId = Convert.ToString((cgi.current - 1) * cgi.pageSize + i + 1);
                    cgt.sendid = dt.Rows[i]["id"].ToString();
                    cgt.sendName = dt.Rows[i]["sendName"].ToString();
                    cgt.sendTel = dt.Rows[i]["sendTel"].ToString();
                    cgt.sendTime = Convert.ToDateTime(dt.Rows[i]["sendTime"].ToString()).ToString("yyyy-MM-dd");
                    cgt.sendType = dt.Rows[i]["sendType"].ToString();
                    cgt.status = dt.Rows[i]["status"].ToString();
                    cgt.goodsTotal = String.Format("{0:F}", Convert.ToDouble(dt.Rows[i]["goodsTotal"].ToString()));
                    pageResult.list.Add(cgt);
                }
            }

            string sql1 = "select count(*) from t_warehouse_send where purchasersCode='" + userId + "' " + sd + st + zt + time;
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "table").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return pageResult;
        }

        public PageResult CollectGoodsList(CollectGoodsListIn cgi, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(cgi.current, cgi.pageSize);
            pageResult.list = new List<object>();


            string sql = "select barcode,slt,a.id,goodsName,brand,supplyPrice,goodsNum,a.goodsTotal,waybillNo  from t_warehouse_send_goods a,t_warehouse_send  b  where a.sendId=b.id  and sendId='" +
                cgi.sendid + "' limit " + (cgi.current - 1) * cgi.pageSize + "," + cgi.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                CollectGoodsListSum cgs = new CollectGoodsListSum();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CollectGoodsListItem collectGoodsListItem = new CollectGoodsListItem();
                    collectGoodsListItem.keyId = Convert.ToString((cgi.current - 1) * cgi.pageSize + i + 1);
                    collectGoodsListItem.id = dt.Rows[i]["id"].ToString();
                    collectGoodsListItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                    collectGoodsListItem.barcode = dt.Rows[i]["barcode"].ToString();
                    collectGoodsListItem.brand = dt.Rows[i]["brand"].ToString();
                    collectGoodsListItem.goodsNum = dt.Rows[i]["goodsNum"].ToString();
                    collectGoodsListItem.goodsTotal = String.Format("{0:F}", Convert.ToDouble(dt.Rows[i]["goodsTotal"].ToString()));
                    collectGoodsListItem.slt = dt.Rows[i]["slt"].ToString();
                    collectGoodsListItem.supplyPrice = String.Format("{0:F}", Convert.ToDouble(dt.Rows[i]["supplyPrice"].ToString()));
                    cgs.money += Convert.ToDouble(dt.Rows[i]["goodsTotal"].ToString());

                    pageResult.list.Add(collectGoodsListItem);

                }
                cgs.money = Math.Round(cgs.money, 2);
                cgs.waybillNo = dt.Rows[0]["waybillNo"].ToString();
                pageResult.item = cgs;


            }
            string sql1 = "select count(*) from t_warehouse_send_goods a,t_warehouse_send  b  where a.sendId=b.id  and sendId='" +
                cgi.sendid + "'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "table").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);

            return pageResult;
        }

        public MsgResult ConfirmGoods(ConfirmGoodsIn cgi, string userId)
        {
            string id = cgi.sendid;
            MsgResult msgResult = new MsgResult();
            msgResult.msg = "操作失败";
            msgResult.type = "0";
            if (cgi.waybillNo != null && cgi.waybillNo != "")
            {
                string sql = "update t_warehouse_send  SET `status`='0' , waybillNo='" + cgi.waybillNo + "' WHERE id='" + id + "'";
                if (DatabaseOperationWeb.ExecuteDML(sql))
                {
                    msgResult.msg = "成功";
                    msgResult.type = "1";
                }
                return msgResult;
            }
            else
            {
                string error;
                string time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                string sql1 = "select s.purchasersCode,s.sendType,g.barcode,g.goodsNum,g.suppliercode,g.wid,g.wcode,g.goodsName " +
                        "from t_warehouse_send_goods g,t_warehouse_send s " +
                        "where g.sendId = s.id  and s.id ='" + id+"'";
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "table").Tables[0];
                ArrayList al = new ArrayList();
                if (dt1.Rows.Count > 0)
                {
                    string sql2 = "select barcode " +
                           "from t_goods_distributor_price " +
                           "where usercode ='" + dt1.Rows[0]["purchasersCode"].ToString() + "'";
                    DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "table").Tables[0];
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        if (dt1.Rows[i]["sendType"].ToString() == "1")//发货单 DN
                        {
                            DataRow[] drs = dt2.Select("barcode = '" + dt1.Rows[i]["barcode"].ToString() + "'");
                            if (drs.Length > 0)
                            {
                                string upSql1 = "update t_goods_distributor_price " +
                                            "set pnum=pnum+" + dt1.Rows[i]["goodsNum"].ToString() + " " +
                                            "where barcode = '" + dt1.Rows[i]["barcode"].ToString() + "' " +
                                            "and usercode = '" + dt1.Rows[i]["purchasersCode"].ToString() + "' ";
                                al.Add(upSql1);
                            }
                            else
                            {
                                msgResult.msg = "有商品未录入渠道，请联系运营！";
                                msgResult.type = "0";
                                return msgResult;
                            }


                            string upSql2 = "update t_goods_warehouse " +
                            "set goodsnum=goodsnum-" + dt1.Rows[i]["goodsNum"].ToString() + " " +
                            "where barcode = '" + dt1.Rows[i]["barcode"].ToString() + "' " +
                            "and suppliercode = '" + dt1.Rows[i]["suppliercode"].ToString() + "' " +
                            "and wid = '" + dt1.Rows[i]["wid"].ToString() + "'";
                            al.Add(upSql2);
                            string upSql3 = "insert into t_log_goodsnum(inputType,createtime,wid,wcode,orderid,barcode,goodsnum,state) " +
                                            "values('DN',now(),'" + dt1.Rows[i]["wid"].ToString() + "','" + dt1.Rows[i]["wcode"].ToString() + "'," +
                                            "'" + id + "','" + dt1.Rows[i]["barcode"].ToString() + "'," +
                                            "" + dt1.Rows[i]["goodsNum"].ToString() + ",'')";
                            al.Add(upSql3);
                        }
                        else//退货单 ASN
                        {
                            string upSql1 = "update t_goods_distributor_price " +
                            "set pnum=pnum-" + dt1.Rows[i]["goodsNum"].ToString() + " " +
                            "where barcode = '" + dt1.Rows[i]["barcode"].ToString() + "' " +
                            "and usercode = '" + dt1.Rows[i]["purchasersCode"].ToString() + "' ";
                            al.Add(upSql1);
                            string upSql2 = "update t_goods_warehouse " +
                            "set goodsnum=goodsnum+" + dt1.Rows[i]["goodsNum"].ToString() + " " +
                            "where barcode = '" + dt1.Rows[i]["barcode"].ToString() + "' " +
                            "and suppliercode = '" + dt1.Rows[i]["suppliercode"].ToString() + "' " +
                            "and wid = '" + dt1.Rows[i]["wid"].ToString() + "'";
                            al.Add(upSql2);
                            string upSql3 = "insert into t_log_goodsnum(inputType,createtime,wid,wcode,orderid,barcode,goodsnum,state) " +
                                            "values('ASN',now(),'" + dt1.Rows[i]["wid"].ToString() + "','" + dt1.Rows[i]["wcode"].ToString() + "'," +
                                            "'" + id + "','" + dt1.Rows[i]["barcode"].ToString() + "'," +
                                            "" + dt1.Rows[i]["goodsNum"].ToString() + ",'')";
                            al.Add(upSql3);
                        }
                    }
                    string sql = "update t_warehouse_send  SET `status`='1',confirmTime='" + time + "'  WHERE id='" + id + "'";
                    al.Add(sql);
                    if (DatabaseOperationWeb.ExecuteDML(al))
                    {
                        msgResult.msg = "成功";
                        msgResult.type = "1";
                    }
                }


                return msgResult;
            }

        }
        public MsgResult exportSendGoods(ExportSendGoodsParam param, string userId)
        {
            MsgResult msg = new MsgResult();
            string fileName = "Export_SendGoods_" + userId + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx";
            string sql = "select w.barcode as '商品条码',g.goodsName as '*商品名称',w.rprice as '*售价（元）',c.`name` as '分类','' as '单位',pprice as '进价（元）','' as '会员价（元）',goodsNum as '库存' ,'' as '保质期（天）','' as '产地' from t_warehouse_send_goods w ,t_goods_list g,t_goods_category c where   w.barcode = g.barcode and g.catelog1 = c.id and sendId= '" + param.sendid + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_ticket").Tables[0];
            if (dt.Rows.Count > 0)
            {
                FileManager fm = new FileManager();
                if (fm.writeDataTableToExcel(dt, fileName))
                {
                    if (fm.updateFileToOSS(fileName, Global.OssDirOrder, fileName))
                    {
                        msg.type = "1";
                        msg.msg = Global.OssUrl + Global.OssDirOrder + fileName;
                    }
                    else
                    {
                        msg.msg = "生成失败！！";
                    }
                }
                else
                {
                    msg.msg = "生成失败！";
                }
            }
            else
            {
                msg.msg = "没有需要导出的订单！";
            }
            return msg;
        }


        /// <summary>
        /// 我要发货-运营-导入
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult OperationDeliveryImport(OperationDeliveryImportParam ogp, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            Msg msg = new Msg();

            string logCode = userId + DateTime.Now.ToString("yyyyMMddHHmmssff");
            string fileName = logCode + ".xlsx";
            FileManager fm = new FileManager();
            ArrayList list = new ArrayList();
            if (fm.fileCopy(ogp.fileTemp, fileName))
            {
                DataTable dt = fm.readExcelFileToDataTable(fileName);

                if (dt == null)
                {
                    msg.msg = "导入文档错误，请确认excel里的列是否正确，是否有相同名称的列。";
                    pageResult.item = msg;
                    return pageResult;
                }
                if (dt.Rows.Count > 0)
                {
                    #region 校验导入文档的列


                    if (!dt.Columns.Contains("商品条码"))
                    {
                        msg.msg += "缺少“商品条码”列，";
                    }
                    if (!dt.Columns.Contains("发货数量"))
                    {
                        msg.msg += "缺少“发货数量”列，";
                    }
                    if (!dt.Columns.Contains("安全库存数"))
                    {
                        msg.msg += "缺少“安全库存数”列，";
                    }
                    if (msg.msg != null && msg.msg != "")
                    {
                        pageResult.item = msg;
                        return pageResult;
                    }
                    #endregion
                    if (dt.DefaultView.ToTable(true, "商品条码").Rows.Count < dt.Rows.Count)
                    {
                        msg.msg = "有重复的商品条码";
                        pageResult.item = msg;
                        return pageResult;
                    }
                    string sql1 = ""
                        + "select a.usercode,a.barcode,a.goodsName,a.slt,c.goodsnum,a.pprice,a.rprice,b.brand,b.country,b.model,c.wid,c.wcode,d.usercode supplierCode,c.inprice"
                        + " from t_goods_distributor_price a,t_goods_list b,t_goods_warehouse c ,t_user_list d"
                        + " where  a.wid= c.wid and a.barcode=b.barcode and c.barcode=a.barcode and d.id=a.supplierid  and c.goodsnum!='0' and  a.usercode='" + ogp.usercode + "'";
                    DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];


                    if (ogp.id != null && ogp.id != "")
                    {
                        string sql = "delete from t_warehouse_send_goods_bak where sendId='" + ogp.id + "'";
                        DatabaseOperationWeb.ExecuteDML(sql);
                        string sql5 = ""
                            + " update t_warehouse_send"
                            + " set ifupload='1'"
                            + " where id='" + ogp.id + "'";
                        list.Add(sql5);
                    }
                    else
                    {
                        ogp.id = "SEND" + DateTime.Now.ToString("yyyyMMddHHmmssff");
                        string sql2 = ""
                            + "INSERT into t_warehouse_send(id,sendType,status,purchasersCode,ifupload,inputOperator)"
                            + " VALUES('" + ogp.id + "','1','9','" + ogp.usercode + "','1','" + userId + "')  ";
                        list.Add(sql2);
                    }

                    string sql4 = ""
                                    + " select a.barcode,a.safeNum,max(b.sendTime)"
                                    + " from t_warehouse_send_goods a,t_warehouse_send b"
                                    + " where a.sendId=b.id and b.purchasersCode='" + ogp.usercode + "' and b.`status`=1 ";
                    DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql4, "T").Tables[0];

                    string message = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string goodsNum = dt.Rows[i]["发货数量"].ToString();
                        if (goodsNum == null || goodsNum == "")
                        {
                            goodsNum = "0";
                        }
                        DataRow[] dr = dt1.Select("barcode='" + dt.Rows[i]["商品条码"].ToString() + "'");
                        if (dr.Length == 0)
                        {
                            message += dt.Rows[i]["商品条码"].ToString() + " ";
                        }
                        else if (Convert.ToInt16(dr[0]["goodsnum"]) < Convert.ToInt16(goodsNum))
                        {
                            msg.msg = dt.Rows[i]["商品条码"].ToString() + " 商品发货数超出库存";
                            pageResult.item = msg;
                            return pageResult;
                        }
                        else
                        {
                            string safeNum = dt.Rows[i]["安全库存数"].ToString();
                            if (safeNum == null || safeNum == "")
                            {
                                DataRow[] dr1 = dt2.Select("barcode='" + dt.Rows[i]["商品条码"].ToString() + "'");
                                if (dt2.Rows.Count > 0)
                                {
                                    safeNum = dt2.Rows[0]["safeNum"].ToString();
                                }
                                else
                                    safeNum = "0";
                            }

                            string sql3 = ""
                                + "insert into t_warehouse_send_goods_bak(sendId,barcode,slt,goodsName,brand,suppliercode,supplyPrice,wid,wcode,goodsNum,goodsTotal,pprice,rprice,safeNum) "
                                + " values('" + ogp.id + "','" + dt.Rows[i]["商品条码"].ToString() + "','" + dr[0]["slt"].ToString() + "','" + dr[0]["goodsName"].ToString() + "','" + dr[0]["brand"].ToString() + "','" + dr[0]["supplierCode"].ToString() + "','" + dr[0]["inprice"].ToString() + "','" + dr[0]["wid"].ToString() + "','" + dr[0]["wcode"].ToString() + "','" + goodsNum + "','" + string.Format("{0:F}", Convert.ToDouble(dr[0]["pprice"]) * Convert.ToInt16(goodsNum)) + "','" + dr[0]["pprice"].ToString() + "','" + dr[0]["rprice"].ToString() + "','" + safeNum + "')";
                            list.Add(sql3);
                        }
                    }
                    if (message == "")
                    {
                        DatabaseOperationWeb.ExecuteDML(list);
                    }
                    else
                    {
                        msg.msg = message + " 上述条码不在供应范围或库存为0";
                        pageResult.item = msg;
                        return pageResult;
                    }
                }
            }
            else
            {
                msg.msg = "未找到该文件";
                pageResult.item = msg;
                return pageResult;
            }
            DeliverOrderListWithdrawParam dolw = new DeliverOrderListWithdrawParam();
            dolw.current = 1;
            dolw.pageSize = 10;
            dolw.id = ogp.id;
            pageResult = DeliverGoodsList(dolw, userId);
            msg.type = "1";
            msg.ifuploda = "1";
            pageResult.item = msg;
            return pageResult;

        }

        /// <summary>
        /// 发货列表-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult DeliverOrderList(DeliverOrderListParam dolp, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            pageResult.pagination = new Page(dolp.current, dolp.pageSize);

            string date = "";
            if (dolp.date != null && dolp.date.Length == 2)
            {
                date = " and sendTime between  str_to_date('" + dolp.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + dolp.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            string id = "";
            if (dolp.id != null && dolp.id != "")
            {
                id = " and a.id like '%" + dolp.id + "%'";
            }
            string status = " and a.`status`!='9'";
            if (dolp.status != null && dolp.status != "")
            {
                status = " and a.`status`='" + dolp.status + "'";
            }
            string name = "";
            if (dolp.purchasersName != null && dolp.purchasersName != "")
            {
                name = " and b.username='" + dolp.purchasersName + "'";
            }
            string sql = ""
                + "select a.id,a.purchasersCode,b.username,a.goodsTotal,a.sendTime,a.sendName,a.sendTel,a.`status`"
                + " from t_warehouse_send a,t_user_list b "
                + " where a.purchasersCode=b.usercode and a.sendType='1'" + date + id + status + name
                + " order by a.updateTime,a.id desc limit " + (dolp.current - 1) * dolp.pageSize + "," + dolp.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DeliverOrderListItem doli = new DeliverOrderListItem();
                    doli.keyId = Convert.ToString((dolp.current - 1) * dolp.pageSize + i + 1);
                    doli.id = dt.Rows[i]["id"].ToString();
                    doli.purchasersCode = dt.Rows[i]["purchasersCode"].ToString();
                    if (dt.Rows[i]["goodsTotal"].ToString() != DBNull.Value.ToString())
                        doli.goodsTotal = dt.Rows[i]["goodsTotal"].ToString();
                    doli.purchasersName = dt.Rows[i]["username"].ToString();
                    if (dt.Rows[i]["sendName"].ToString() != DBNull.Value.ToString())
                        doli.sendName = dt.Rows[i]["sendName"].ToString();
                    if (dt.Rows[i]["sendTel"].ToString() != DBNull.Value.ToString())
                        doli.sendTel = dt.Rows[i]["sendTel"].ToString();
                    if (dt.Rows[i]["sendTime"].ToString() != DBNull.Value.ToString())
                        doli.sendTime = Convert.ToDateTime(dt.Rows[i]["sendTime"]).ToString("yyyy-MM-dd");
                    doli.status = dt.Rows[i]["status"].ToString();
                    pageResult.list.Add(doli);
                }
            }
            string sql1 = ""
                + "select *"
                + " from t_warehouse_send a,t_user_list b "
                + " where a.purchasersCode=b.usercode and a.sendType='1'" + date + id + status + name;
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows.Count);
            return pageResult;

        }

        /// <summary>
        /// 发货列表撤回-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MsgResult DeliverOrderListWithdraw(DeliverOrderListWithdrawParam dolw, string userId)
        {
            MsgResult msg = new MsgResult();
            string sql1 = ""
                + "select `status`"
                + " from t_warehouse_send"
                + " where id='" + dolw.id + "' and sendType='1'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            ArrayList list = new ArrayList();
            if (dt.Rows.Count == 1 && dt.Rows[0]["status"].ToString() == "0")
            {
                string sql = ""
                    + " update t_warehouse_send "
                    + " set `status`='3' "
                    + " where id='" + dolw.id + "' ";
                list.Add(sql);

                string insert = ""
                    + "insert into t_warehouse_send_goods_bak"
                    + " (select * from t_warehouse_send_goods where sendId='"+ dolw.id + "')";
                list.Add(insert);

                string delete = ""
                    + "delete from t_warehouse_send_goods "
                    + " where sendId='" + dolw.id + "'";
                list.Add(delete);

                if (DatabaseOperationWeb.ExecuteDML(list))
                {
                    msg.type = "1";
                }
            }
            else
            {
                msg.msg = "该发货单状态不可撤回或该发货单不存在";
            }
            return msg;
        }

        /// <summary>
        /// 发货列表删除-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MsgResult DeliverOrderListDelete(DeliverOrderListWithdrawParam dolw, string userId)
        {
            MsgResult msg = new MsgResult();
            string sql1 = ""
                + "select `status`"
                + " from t_warehouse_send"
                + " where id='" + dolw.id + "' and sendType='1'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            if (dt.Rows.Count == 1 && dt.Rows[0]["status"].ToString() == "3")
            {
                string sql = ""
               + " update t_warehouse_send "
               + " set `status`='9'"
               + " where id='" + dolw.id + "' ";

                if (DatabaseOperationWeb.ExecuteDML(sql))
                {
                    msg.type = "1";
                }
            }
            else
            {
                msg.msg = "无此发货单或发货单不是待提交状态";
            }
            return msg;
        }

        /// <summary>
        /// 发货商品列表-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult DeliverGoodsList(DeliverOrderListWithdrawParam dolw, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(dolw.current, dolw.pageSize);
            pageResult.list = new List<object>();

            string sql = ""
                + "select a.goodsName,a.barcode,b.model,b.country,a.brand,a.rprice,a.pprice,c.goodsnum pNum,a.goodsNum,a.safeNum"
                + " FROM t_warehouse_send_goods_bak a,t_goods_list b ,t_goods_warehouse c"
                + " where a.barcode=b.barcode and c.barcode=a.barcode and c.wid=a.wid  and a.sendId='" + dolw.id + "'"
                + " limit " + (dolw.current - 1) * dolw.pageSize + "," + dolw.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            string sql1 = ""
                + "select a.goodsName,a.barcode,b.model,b.country,a.brand,a.rprice,a.pprice,c.goodsnum pNum,a.goodsNum,a.safeNum"
                + " FROM t_warehouse_send_goods a,t_goods_list b ,t_goods_warehouse c"
                + " where a.barcode=b.barcode and c.barcode=a.barcode and c.wid=a.wid  and a.sendId='" + dolw.id + "'"
                + " limit " + (dolw.current - 1) * dolw.pageSize + "," + dolw.pageSize;
            string t = " t_warehouse_send_goods_bak a";
            if (dt == null || dt.Rows.Count == 0)
            {
                dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
                t = " t_warehouse_send_goods a";
            }
            else
            {
                string sql3 = ""
                    + "select ifupload "
                    + " from t_warehouse_send"
                    + " where id='" + dolw.id + "'";
                DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "T").Tables[0];
                pageResult.item = dt2.Rows[0][0].ToString();
            }
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DeliverGoodsListItem dgli = new DeliverGoodsListItem();
                    dgli.keyId = Convert.ToString((dolw.current - 1) * dolw.pageSize + i + 1);
                    dgli.id = dolw.id;
                    dgli.goodsName = dt.Rows[i]["goodsName"].ToString();
                    dgli.goodsNum = dt.Rows[i]["goodsNum"].ToString();
                    dgli.model = dt.Rows[i]["model"].ToString();
                    dgli.pNum = Convert.ToString(Convert.ToInt16(dt.Rows[i]["pNum"]) - Convert.ToInt16(dgli.goodsNum));
                    dgli.pprice = dt.Rows[i]["pprice"].ToString();
                    dgli.rprice = dt.Rows[i]["rprice"].ToString();
                    dgli.safeNum = dt.Rows[i]["safeNum"].ToString();
                    dgli.barcode = dt.Rows[i]["barcode"].ToString();
                    dgli.brand = dt.Rows[i]["brand"].ToString();
                    dgli.country = dt.Rows[i]["country"].ToString();
                    pageResult.list.Add(dgli);

                }
            }
            string sql2 = ""
                + "select count(*)"
                + " FROM " + t + ",t_goods_list b ,t_goods_warehouse c"
                + " where a.barcode=b.barcode and c.barcode=a.barcode and c.wid=a.wid  and a.sendId='" + dolw.id + "'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "T").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);

            return pageResult;

        }

        /// <summary>
        /// 我要发货-运营-采购商下拉
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object DeliveryPurchasersList(string userId)
        {

            List<object> list = new List<object>();

            string sql = ""
                + " select username,usercode,contact,tel"
                + " from t_user_list"
                + " where usertype='2' and ifOnline='0' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DeliveryPurchasersListItem dpli = new DeliveryPurchasersListItem();
                    dpli.usercode = dt.Rows[i]["usercode"].ToString();
                    dpli.getName = dt.Rows[i]["username"].ToString();
                    dpli.contact = dt.Rows[i]["contact"].ToString();
                    dpli.getTel = dt.Rows[i]["tel"].ToString();
                    list.Add(dpli);
                }
            }
            return list;
        }

        /// <summary>
        /// 查看发货单-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult DeliverOrderDetails(DeliverOrderListWithdrawParam dolwp, string userId)
        {
            PageResult pageResult = new PageResult();


            string sql = ""
                + " select a.sendName,a.sendTel,a.express,a.waybillNo,a.getName,a.getTel,a.id,a.ifupload,b.username,a.purchasersCode"
                + " from t_warehouse_send a,t_user_list b"
                + " where a.purchasersCode=b.usercode and a.id='" + dolwp.id + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            if (dt.Rows.Count > 0)
            {
                pageResult = DeliverGoodsList(dolwp, userId);
                DeliverOrderDetailsItem dodi = new DeliverOrderDetailsItem();
                dodi.id = dt.Rows[0]["id"].ToString();
                dodi.express = dt.Rows[0]["express"].ToString();
                dodi.sendName = dt.Rows[0]["sendName"].ToString();
                dodi.sendTel = dt.Rows[0]["sendTel"].ToString();
                dodi.waybillNo = dt.Rows[0]["waybillNo"].ToString();
                dodi.contact = dt.Rows[0]["getName"].ToString();
                dodi.usercode= dt.Rows[0]["purchasersCode"].ToString();
                dodi.getName = dt.Rows[0]["username"].ToString();
                dodi.getTel = dt.Rows[0]["getTel"].ToString();
                dodi.ifupload = dt.Rows[0]["ifupload"].ToString();
                pageResult.item = dodi;
            }

            return pageResult;
        }

        /// <summary>
        /// 发货数量or安全数量修改-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object DeliverGoodsNum(DeliverGoodsNumParam dgnp, string userId)
        {
            DeliverGoodsNumItem dgni = new DeliverGoodsNumItem();

            string sql = ""
                + "select c.goodsnum pNum,a.pprice"
                + " FROM t_warehouse_send_goods_bak a,t_goods_list b ,t_goods_warehouse c"
                + " where a.barcode=b.barcode and c.barcode=a.barcode and c.wid=a.wid  and a.sendId='" + dgnp.id + "' and a.barcode='" + dgnp.barcode + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "2").Tables[0];
            if (dt.Rows.Count > 0)
            {

                string st = "";
                if (dgnp.goodsNum != null && dgnp.goodsNum != "")
                {
                    if (Convert.ToInt16(dt.Rows[0]["pNum"]) < Convert.ToInt16(dgnp.goodsNum))
                    {
                        MsgResult msg = new MsgResult();
                        msg.msg = "库存不够，请重新填写发货数";
                        return msg;
                    }
                    dgni.pNum = (Convert.ToInt16(dt.Rows[0]["pNum"]) - Convert.ToInt16(dgnp.goodsNum)).ToString();
                    st = " goodsNum='" + dgnp.goodsNum + "',goodsTotal='" + string.Format("{0:F}", Convert.ToInt16(dgnp.goodsNum) * Convert.ToDouble(dt.Rows[0]["pprice"])) + "'";
                }
                else
                {
                    st = " safeNum='" + dgnp.safeNum + "'";
                }
                string sql1 = ""
                    + "update t_warehouse_send_goods_bak"
                    + " set " + st
                    + " where barcode='" + dgnp.barcode + "' and  sendId='" + dgnp.id + "'";
                if (DatabaseOperationWeb.ExecuteDML(sql1))
                {
                    dgni.barcode = dgnp.barcode;
                    dgni.type = "1";
                }
            }
            return dgni;
        }


        /// <summary>
        /// 发货商品删除-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public object DeliverGoodsDelete(DeliverGoodsDeleteParam dgnp, string userId)
        {
            DeliverGoodsNumItem dgni = new DeliverGoodsNumItem();
            string sql = ""
                + "delete from t_warehouse_send_goods_bak"
                + " where sendId='" + dgnp.id + "' and barcode='" + dgnp.barcode + "'";
            if (DatabaseOperationWeb.ExecuteDML(sql))
            {
                dgni.barcode = dgnp.id;
                dgni.type = "1";
            }
            return dgni;
        }

        /// <summary>
        /// 查看选择发货商品列表-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult ChooseDeliverGoods(ChooseDeliverGoodsParam dgnp, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            pageResult.pagination = new Page(dgnp.current, dgnp.pageSize);
            if (dgnp.id == null || dgnp.id == "")
            {
                dgnp.id = "SEND" + DateTime.Now.ToString("yyyyMMddHHmmssff");
                string sql4 = ""
                   + "INSERT into t_warehouse_send(id,sendType,status,purchasersCode,ifupload,inputOperator)"
                   + " VALUES('" + dgnp.id + "','1','9','" + dgnp.usercode + "','0','" + userId + "')  ";
                DatabaseOperationWeb.ExecuteDML(sql4);
            }
            else if (dgnp.isDelete == "1")
            {
                string sql3 = "delete from t_warehouse_send_goods_bak"
                    + " where sendId='" + dgnp.id + "'";
                DatabaseOperationWeb.ExecuteDML(sql3);
            }
            string st = "";
            if (dgnp.id != null && dgnp.id != "")
            {
                st = dgnp.id;
            }
            
            string username = "";
            if (dgnp.supplierName != null && dgnp.supplierName != "")
            {
                username = " and d.username='" + dgnp.supplierName + "'";
            }

            string warehouse = "";
            if (dgnp.warehouse != null && dgnp.warehouse != "")
            {
                warehouse = " and e.wname='" + dgnp.warehouse + "'";
            }

            string select = "";
            if (dgnp.select != null && dgnp.select != "")
            {
                select = " and (a.goodsName like '%" + dgnp.select + "%' or a.barcode like '%" + dgnp.select + "%')";
            }

            string sql1 = "select *"
                        + " FROM (select a.barcode,a.goodsName,a.slt,c.goodsnum,a.pprice,a.rprice,b.brand,b.country,b.model,e.wname,d.username,c.inprice"                    
                        + " from t_goods_distributor_price a,t_goods_list b,t_goods_warehouse c,t_user_list d,t_base_warehouse e "
                        + " where  a.wid= c.wid and a.barcode=b.barcode and c.barcode=a.barcode and  a.supplierid=d.id and e.id=a.wid and c.goodsnum!='0' and  a.usercode='" + dgnp.usercode + "'" + username + warehouse + select + ")  f"
                        + " left join (select barcode  from t_warehouse_send_goods_bak  where sendId='" + st + "')  g  "
                        + " on f.barcode=g.barcode "
                        + " order by g.barcode desc  limit " + (dgnp.current - 1) * dgnp.pageSize + "," + dgnp.pageSize;
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            
            ChooseDeliverItem cdi = new ChooseDeliverItem();
            cdi.id = dgnp.id;

            string sql6 = ""
                + " select barcode "
                + " from t_warehouse_send_goods_bak "
                + " where sendId='" + st + "'";
            DataTable dt4 = DatabaseOperationWeb.ExecuteSelectDS(sql6, "T").Tables[0];
            int num = 0;
            if (dt4.Rows.Count > 0)
            {
                num = dt4.Rows.Count;
            }
            cdi.usercode = dgnp.usercode;
            cdi.list = new List<object>();
            string sql5 = ""
                + " select b.wname"
                + " from t_goods_warehouse a,t_base_warehouse b "
                + "  where a.wid=b.id GROUP BY b.wname";
            DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql5, "T").Tables[0];
            if (dt3.Rows.Count > 0)
            {
                for (int a = 0; a < dt3.Rows.Count; a++)
                {
                    cdi.list.Add(dt3.Rows[a]["wname"].ToString());
                }
            }
            cdi.num = num.ToString();
            if (dt1.Rows.Count > 0)
            {
                
                               
                for (int i = 0; i < dt1.Rows.Count; i++)
                {


                    ChooseDeliverGoodsItem cdgi = new ChooseDeliverGoodsItem();
                    cdgi.keyId = Convert.ToString((dgnp.current - 1) * dgnp.pageSize + i + 1);
                    cdgi.goodsName = dt1.Rows[i]["goodsName"].ToString();
                    cdgi.barcode = dt1.Rows[i]["barcode"].ToString();
                    cdgi.pNum = dt1.Rows[i]["goodsnum"].ToString();
                    cdgi.rprice = dt1.Rows[i]["rprice"].ToString();
                    cdgi.brand = dt1.Rows[i]["brand"].ToString();
                    cdgi.country = dt1.Rows[i]["country"].ToString();
                    cdgi.model = dt1.Rows[i]["model"].ToString();
                    cdgi.warehouse = dt1.Rows[i]["wname"].ToString();
                    cdgi.supplierName = dt1.Rows[i]["username"].ToString();
                    cdgi.inprice = dt1.Rows[i]["inprice"].ToString();
                    cdgi.time = "";
                    cdgi.ischoose = false;
                    if (dgnp.id != null && dgnp.id != "")
                    {
                        if (dt4.Select("barcode='"+ cdgi.barcode + "'").Count()==1)
                        {
                            cdgi.ischoose = true;                          
                        }                           
                    }
                    pageResult.list.Add(cdgi);
                }
                             
            }
            pageResult.item = cdi;
            string sql2 = ""
                       + "select count(*)"
                       + " from t_goods_distributor_price a,t_goods_list b,t_goods_warehouse c,t_user_list d,t_base_warehouse e "
                       + " where  a.wid= c.wid and a.barcode=b.barcode and c.barcode=a.barcode and  a.supplierid=d.id and e.id=a.wid and c.goodsnum!='0' and  a.usercode='" + dgnp.usercode + "'" + username + warehouse + select;
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "T").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt2.Rows[0][0]);
            return pageResult;

        }


        /// <summary>
        /// 选中发货商品-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MsgResult ChooseGoods(DeliverGoodsDeleteParam dgnp, string userId)
        {
            MsgResult msg = new MsgResult();
            List<object> list = new List<object>();


            string sql3 = "";
            if (dgnp.ischoose == true)
            {
                string sql = ""
                       + "select a.usercode,a.barcode,a.goodsName,a.slt,a.pNum,a.pprice,a.rprice,b.brand,b.country,b.model,c.wid,c.wcode,d.usercode supplierCode,c.inprice"
                       + " from t_goods_distributor_price a,t_goods_list b,t_goods_warehouse c ,t_user_list d"
                       + " where  a.wid= c.wid and a.barcode=b.barcode and c.barcode=a.barcode and d.id=a.supplierid  and a.barcode='" + dgnp.barcode + "' and  a.usercode='" + dgnp.usercode + "'";
                DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

                string sql1 = ""
                       + " select a.barcode,a.safeNum,max(b.sendTime)"
                       + " from t_warehouse_send_goods a,t_warehouse_send b"
                       + " where a.sendId=b.id and b.purchasersCode='" + dgnp.usercode + "' and b.`status`=1 and a.barcode='" + dgnp.barcode + "'";
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];

                string safeNum = "";
                if (dt1.Rows.Count == 0)
                {
                    if (dt1.Rows[0]["safeNum"].ToString() != null && dt1.Rows[0]["safeNum"].ToString() != "")
                    {
                        safeNum = dt1.Rows[0]["safeNum"].ToString();
                    }
                }
                sql3 = ""
                    + "insert into t_warehouse_send_goods_bak(sendId,barcode,slt,goodsName,brand,suppliercode,supplyPrice,wid,wcode,pprice,rprice,safeNum,goodsNum,goodsTotal) "
                    + " values('" + dgnp.id + "','" + dt.Rows[0]["barcode"].ToString() + "','" + dt.Rows[0]["slt"].ToString() + "','" + dt.Rows[0]["goodsName"].ToString() + "','" + dt.Rows[0]["brand"].ToString() + "','" + dt.Rows[0]["supplierCode"].ToString() + "','" + dt.Rows[0]["inprice"].ToString() + "','" + dt.Rows[0]["wid"].ToString() + "','" + dt.Rows[0]["wcode"].ToString() + "','" + dt.Rows[0]["pprice"].ToString() + "','" + dt.Rows[0]["rprice"].ToString() + "','" + safeNum + "','0','0')";
            }
            else
            {
                sql3 = ""
                    + "delete from t_warehouse_send_goods_bak"
                    + " where sendId='" + dgnp.id + "' and barcode='" + dgnp.barcode + "'";
            }

            string sql4 = ""
                + "select count(*)"
                + " from t_warehouse_send_goods_bak"
                + " where sendId='"+ dgnp.id + "'";

            if (DatabaseOperationWeb.ExecuteDML(sql3))
                msg.type = "1";
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql4,"T").Tables[0];
            if (dt2.Rows.Count > 0)
                msg.msg = dt2.Rows[0][0].ToString();
        
            return msg;

        }

        /// <summary>
        /// 发货单保存接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MsgResult DeliverOrderConserve(DeliverOrderConserveParam dgnp, string userId)
        {
            MsgResult msg = new MsgResult();

            string insert = "";
            string values = "";
            string update = "";
            if (dgnp.express != null && dgnp.express != "")
            {
                insert = ",express";
                values = ",'" + dgnp.express + "'";
                update = ",express='" + dgnp.express + "'";
            }
            if (dgnp.contact != null && dgnp.contact != "")
            {
                insert = insert + ",getName";
                values = values + ",'" + dgnp.contact + "'";
                update = update + ",getName='" + dgnp.contact + "'";
            }
            if (dgnp.getTel != null && dgnp.getTel != "")
            {
                insert = insert + ",getTel";
                values = values + ",'" + dgnp.getTel + "'";
                update = update + ",getTel='" + dgnp.getTel + "'";
            }
            if (dgnp.sendName != null && dgnp.sendName != "")
            {
                insert = insert + ",sendName";
                values = values + ",'" + dgnp.sendName + "'";
                update = update + ",sendName='" + dgnp.sendName + "'";
            }
            if (dgnp.sendTel != null && dgnp.sendTel != "")
            {
                insert = insert + ",sendTel";
                values = values + ",'" + dgnp.sendTel + "'";
                update = update + ",sendTel='" + dgnp.sendTel + "'";
            }
            if (dgnp.waybillNo != null && dgnp.waybillNo != "")
            {
                insert = insert + ",waybillNo";
                values = values + ",'" + dgnp.waybillNo + "'";
                update = update + ",waybillNo='" + dgnp.waybillNo + "'";
            }
            if (dgnp.usercode != null && dgnp.usercode != "")
            {
                insert = insert + ",purchasersCode";
                values = values + ",'" + dgnp.usercode + "'";
                update = update + ",purchasersCode='" + dgnp.usercode + "'";
            }
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string num = "";
            string sql = "";
            if (dgnp.id != null && dgnp.id != "")
            {
                string sql1 = ""
                    + "select sum(goodsNum)"
                    + " from t_warehouse_send_goods_bak"
                    + " where sendId='" + dgnp.id + "'";
                DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
                if (dt.Rows.Count > 0)
                {
                    num = ",goodsTotal='" + dt.Rows[0][0].ToString() + "'";
                }

                sql = ""
                    + "update t_warehouse_send"
                    + " set `status`='3',sendType='1',updateTime='" + date + "'" + update + num
                    + " where id='" + dgnp.id + "'";
            }
            else
            {
                dgnp.id = "SEND" + DateTime.Now.ToString("yyyyMMddHHmmssff");
                sql = ""
                + " insert into t_warehouse_send(id,sendType,`status`,inputOperator" + insert + ",ifupload,updateTime)"
                + " values('" + dgnp.id + "','1','3','" + userId + "'" + values + ",'0','"+ date + "')";
            }
            if (DatabaseOperationWeb.ExecuteDML(sql))
                msg.type = "1";
            return msg;

        }

        /// <summary>
        /// 发货单提交接口-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MsgResult DeliverOrderSubmission(DeliverOrderConserveParam dgnp, string userId)
        {
            MsgResult msg = new MsgResult();

            string num = "";
            string sql1 = ""
                   + "select sum(goodsNum)"
                   + " from t_warehouse_send_goods_bak"
                   + " where sendId='" + dgnp.id + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            if (dt.Rows[0][0] != DBNull.Value && Convert.ToInt16(dt.Rows[0][0]) > 0)
            {
                num = ",goodsTotal='" + dt.Rows[0][0].ToString() + "'";
            }
            else
            {
                msg.msg = "发货商品不能为空";
                return msg;
            }
            DateTime dateTime = DateTime.Now;
            string confirmTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string time = dateTime.ToString("yyyy-MM-dd");

            string sql = ""
                   + "update t_warehouse_send"
                   + " set updateTime='"+ confirmTime + "',`status`='0',sendType='1',purchasersCode='" + dgnp.usercode + "',sendTime='" + time + "',sendName='" + dgnp.sendName + "',sendTel='" + dgnp.sendTel + "',getName='" + dgnp.contact + "',getTel='" + dgnp.getTel + "',confirmTime='" + confirmTime + "',express='" + dgnp.express + "',waybillNo='" + dgnp.waybillNo + "'" + num
                   + " where id='" + dgnp.id + "'";
            ArrayList list = new ArrayList();
            list.Add(sql);

            string insert = ""
                + "INSERT into t_warehouse_send_goods (sendId,barcode,slt,goodsName,brand,suppliercode,supplyPrice,wid,wcode,pprice,rprice,goodsNum,goodsTotal,safeNum)"
                + " (select sendId,barcode,slt,goodsName,brand,suppliercode,supplyPrice,wid,wcode,pprice,rprice,goodsNum,goodsTotal,safeNum FROM t_warehouse_send_goods_bak where sendId='" + dgnp.id + "')"  ;
            list.Add(insert);

            string delete = " delete from t_warehouse_send_goods_bak"
                + " where sendId='" + dgnp.id + "'";
            list.Add(delete);

            if (DatabaseOperationWeb.ExecuteDML(list))
                msg.type = "1";
            return msg;
        }


        /// <summary>
        /// 库存管理-平台库存-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult PlatformInventory(PlatformInventoryParam dgnp, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            pageResult.pagination = new Page(dgnp.current, dgnp.pageSize);
                      
            string username = "";
            if (dgnp.supplierName != null && dgnp.supplierName != "")
            {
                username = " and d.username  like'%" + dgnp.supplierName + "%'";
            }

            string warehouse = "";
            if (dgnp.warehouse != null && dgnp.warehouse != "")
            {
                warehouse = " and e.wname='" + dgnp.warehouse + "'";
            }

            string select = "";
            if (dgnp.select != null && dgnp.select != "")
            {
                select = " and (a.goodsName like '%" + dgnp.select + "%' or a.barcode like '%" + dgnp.select + "%')";
            }

            string sql1 = "select a.barcode,a.goodsName,a.slt,c.goodsnum,a.pprice,a.rprice,b.brand,b.country,b.model,e.wname,d.username,c.inprice"
                        + " from t_goods_distributor_price a,t_goods_list b,t_goods_warehouse c,t_user_list d,t_base_warehouse e "                      
                        + " where  a.wid= c.wid and a.barcode=b.barcode and c.barcode=a.barcode and  a.supplierid=d.id and e.id=a.wid and c.goodsnum!='0' " + username + warehouse + select
                        + "  limit " + (dgnp.current - 1) * dgnp.pageSize + "," + dgnp.pageSize;
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            List<object> list = new List<object>();
            string sql5 = ""
                    + " select b.wname"
                    + " from t_goods_warehouse a,t_base_warehouse b "
                    + "  where a.wid=b.id GROUP BY b.wname";
            DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql5, "T").Tables[0];
            if (dt3.Rows.Count > 0)
            {
                for (int a = 0; a < dt3.Rows.Count; a++)
                {
                    list.Add(dt3.Rows[a]["wname"].ToString());
                }
            }
            if (dt1.Rows.Count > 0)
            {                            
                
                
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    ChooseDeliverGoodsItem cdgi = new ChooseDeliverGoodsItem();
                    cdgi.keyId = Convert.ToString((dgnp.current - 1) * dgnp.pageSize + i + 1);
                    cdgi.goodsName = dt1.Rows[i]["goodsName"].ToString();
                    cdgi.barcode = dt1.Rows[i]["barcode"].ToString();
                    cdgi.pNum = dt1.Rows[i]["goodsnum"].ToString();
                    cdgi.rprice = dt1.Rows[i]["rprice"].ToString();
                    cdgi.brand = dt1.Rows[i]["brand"].ToString();
                    cdgi.country = dt1.Rows[i]["country"].ToString();
                    cdgi.model = dt1.Rows[i]["model"].ToString();
                    cdgi.warehouse = dt1.Rows[i]["wname"].ToString();
                    cdgi.supplierName = dt1.Rows[i]["username"].ToString();
                    cdgi.inprice = dt1.Rows[i]["inprice"].ToString();
                    cdgi.time = "";
                                        
                    pageResult.list.Add(cdgi);
                }
                
            }
            pageResult.item = list;
            string sql2 = ""
                       + "select count(*)"
                       + " from t_goods_distributor_price a,t_goods_list b,t_goods_warehouse c,t_user_list d,t_base_warehouse e "
                       + " where  a.wid= c.wid and a.barcode=b.barcode and c.barcode=a.barcode and  a.supplierid=d.id and e.id=a.wid and c.goodsnum!='0' " + username + warehouse + select;
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "T").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt2.Rows[0][0]);
            return pageResult;

        }


        /// <summary>
        /// 我要发货-导入入库商品-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Object OnloadWarehousingGoods(OperationDeliveryImportParam ogp, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            MsgResult msg = new MsgResult();

            string logCode = userId + DateTime.Now.ToString("yyyyMMddHHmmssff");
            string fileName = logCode + ".xlsx";
            FileManager fm = new FileManager();
            ArrayList list = new ArrayList();
            if (fm.fileCopy(ogp.fileTemp, fileName))
            {
                DataTable dt = fm.readExcelFileToDataTable(fileName);

                if (dt == null)
                {
                    msg.msg = "导入文档错误，请确认excel里的列是否正确，是否有相同名称的列。";
                    pageResult.item = msg;
                    return pageResult;
                }
                if (dt.Rows.Count > 0)
                {
                    #region 校验导入文档的列

                    if (!dt.Columns.Contains("供货商"))
                    {
                        msg.msg += "缺少“供货商”列，";
                    }
                    if (!dt.Columns.Contains("仓库"))
                    {
                        msg.msg += "缺少“仓库”列，";
                    }
                    if (!dt.Columns.Contains("规格"))
                    {
                        msg.msg += "缺少“规格”列，";
                    }
                    if (!dt.Columns.Contains("原产地"))
                    {
                        msg.msg += "缺少“原产地”列，";
                    }
                    if (!dt.Columns.Contains("生产商"))
                    {
                        msg.msg += "缺少“生产商”列，";
                    }
                    if (!dt.Columns.Contains("平台采购价"))
                    {
                        msg.msg += "缺少“平台采购价”列，";
                    }
                    if (!dt.Columns.Contains("商品条码"))
                    {
                        msg.msg += "缺少“商品条码”列，";
                    }
                    if (!dt.Columns.Contains("入库数量"))
                    {
                        msg.msg += "缺少“入库数量”列，";
                    }
                    if (!dt.Columns.Contains("商品名称"))
                    {
                        msg.msg += "缺少“商品名称”列，";
                    }
                    if (!dt.Columns.Contains("零售价"))
                    {
                        msg.msg += "缺少“零售价”列，";
                    }
                    if (!dt.Columns.Contains("一级分类"))
                    {
                        msg.msg += "缺少“一级分类”列，";
                    }
                    if (!dt.Columns.Contains("二级分类"))
                    {
                        msg.msg += "缺少“二级分类”列，";
                    }
                    if (msg.msg != null && msg.msg != "")
                    {                       
                        return msg;
                    }
                    #endregion
                    if (dt.DefaultView.ToTable(true, "商品条码").Rows.Count < dt.Rows.Count)
                    {
                        msg.msg = "有重复的商品条码";                       
                        return msg;
                    }

                    string select = ""
                        + "select a.id,a.wcode,a.wname,a.supplierid,b.username,c.barcode,a.suppliercode,c.goodsnum,c.id cid"
                        + " from t_base_warehouse a,t_user_list b,t_goods_warehouse c"
                        + " where a.supplierid=b.id and a.id=c.wid and a.supplierid=c.supplierid";
                    DataTable dataTable = DatabaseOperationWeb.ExecuteSelectDS(select, "T").Tables[0];

                    string select1 = ""
                        + "select barcode "
                        + " from t_goods_list ";
                    DataTable  dataTable1= DatabaseOperationWeb.ExecuteSelectDS(select1, "T").Tables[0];

                   // string selectUserList = ""
                   //     + "select id,username"
                   //     + " from t_user_list";
                   // DataTable dtUserList= DatabaseOperationWeb.ExecuteSelectDS(selectUserList, "T").Tables[0];

                    string message = "";
                    for (int i=0;i< dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["仓库"].ToString() != null && dt.Rows[i]["仓库"].ToString() != "" && dt.Rows[i]["供货商"].ToString() != null && dt.Rows[i]["供货商"].ToString() != "")
                        {
                            DataRow[] dr = dataTable.Select("wname='" + dt.Rows[i]["仓库"].ToString() + "'");
                            if (dr.Length > 0)
                            {
                                if (dr[0]["username"].ToString() != dt.Rows[i]["供货商"].ToString())
                                {
                                    message += dt.Rows[i]["供货商"].ToString() + " ";
                                }
                            }
                            else
                            {
                                msg.msg = "仓库错误";
                                return msg;
                            }
                        }
                        else
                        {
                            msg.msg = "仓库与供货商不能为空";
                            return msg;
                        }
                        
                        
                    }
                    if(message!="")
                    {
                        msg.msg = message + "以上供应商与仓库不匹配";
                        return msg;
                    }

                    string select2 = ""
                        + " select id,name"
                        + " from t_goods_category";
                    DataTable dataTable2 = DatabaseOperationWeb.ExecuteSelectDS(select2,"T").Tables[0];
                    
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {                      
                        string barcode = dt.Rows[i]["商品条码"].ToString();
                        string brand = dt.Rows[i]["生产商"].ToString();
                        string goodsName = dt.Rows[i]["商品名称"].ToString();
                        string country = dt.Rows[i]["原产地"].ToString();
                        string model = dt.Rows[i]["规格"].ToString();
                        string c1 = "";
                        if (dt.Rows[i]["一级分类"].ToString() != "" && dt.Rows[i]["一级分类"].ToString() != null )
                        {
                            if (dataTable2.Select("name='" + dt.Rows[i]["一级分类"].ToString() + "'").Length > 0)
                                c1 = dataTable2.Select("name='" + dt.Rows[i]["一级分类"].ToString() + "'")[0][0].ToString();
                            else
                            {
                                msg.msg = dt.Rows[i]["商品条码"].ToString() + "条码的一级分类错误";
                                return msg;
                            }
                        }
                                                  
                        string c2 = "";
                        if (dt.Rows[i]["二级分类"].ToString() != "" && dt.Rows[i]["二级分类"].ToString() != null )
                        {
                            if( dataTable2.Select("name='" + dt.Rows[i]["二级分类"].ToString() + "'").Length > 0)
                                c2 = dataTable2.Select("name='" + dt.Rows[i]["二级分类"].ToString() + "'")[0][0].ToString();
                            else
                            {
                                msg.msg = dt.Rows[i]["商品条码"].ToString() + "条码的二级分类错误";
                                return msg;
                            }
                        }
                        

                        DataRow[] dr = dataTable.Select("username='" + dt.Rows[i]["供货商"].ToString() + "' and barcode='" + dt.Rows[i]["商品条码"].ToString() + "'");

                        if (dataTable1.Select("barcode='" + dt.Rows[i]["商品条码"].ToString() + "'").Count() == 0)
                        {
                            if (barcode != "" && barcode != null && brand != "" && brand != null && goodsName != "" && goodsName != null )
                            {
                                string slt = "http://ecc-product.oss-cn-beijing.aliyuncs.com/goodsuploads/" + barcode + ".jpg";
                                string insert = ""
                                + "insert into t_goods_list(brand,goodsName,catelog1,catelog2,slt,thumb,barcode,country,source,model,ifB2B)"
                                + " values('" + brand + "','" + goodsName + "','" + c1 + "','" + c2 + "','" + slt + "','" + slt + "','" + barcode + "','" + country + "','" + country + "','" + model + "','1')";
                                list.Add(insert);
                                
                                try
                                {
                                    if (dt.Rows[i]["入库数量"] != DBNull.Value && dt.Rows[i]["入库数量"].ToString() != "" && dt.Rows[i]["平台采购价"] != DBNull.Value && dt.Rows[i]["平台采购价"].ToString() != "")
                                    {
                                        string insert1 = ""
                                            + " insert into t_goods_warehouse(barcode,wid,wcode,goodsnum,inprice,supplierid,suppliercode)"
                                            + " values('" + dr[0]["barcode"].ToString() + "','" + dr[0]["id"].ToString() + "','" + dr[0]["wcode"].ToString() + "','" + dt.Rows[i]["入库数量"].ToString() + "','" + dt.Rows[i]["平台采购价"].ToString() + "','" + dr[0]["supplierid"].ToString() + "','" + dr[0]["suppliercode"].ToString() + "')";
                                        list.Add(insert1);
                                    }
                                    else
                                    {
                                        msg.msg = dr[0]["barcode"].ToString() + "上述条码的入库数量、平台采购价不能为空。 ";
                                        return msg;
                                    }
                                }
                                catch (Exception e){
                                    msg.msg = e.ToString();
                                    return msg;
                                }
                                
                            }
                            else
                            {
                                msg.msg = barcode + "上述条码所有值不能为空";
                                return msg;
                            }
                            
                        }
                        else
                        {
                            string gn = "";
                            if (goodsName!=null && goodsName !="")
                            {
                                gn = " ,goodsName='"+ goodsName + "'";
                            }
                            string br = "";
                            if (brand != null && brand != "")
                            {
                                br = " ,brand='" + brand + "'";
                            }
                            string co = "";
                            if (country != null && country != "")
                            {
                                co = " ,country='" + country + "'";
                            }
                            string mo = "";
                            if (model != null && model != "")
                            {
                                mo = " ,model='" + model + "'";
                            }
                            string c11 = "";
                            if (c1 != null && c1 != "")
                            {
                                c11 = " ,c1='" + c1 + "'";
                            }
                            string c22 = "";
                            if (c2 != null && c2 != "")
                            {
                                c22 = " ,c2='" + c2 + "'";
                            }
                            string update = ""
                                + " update t_goods_list "
                                + " set barcode='" + barcode + "'" + gn + br + co + mo + c11 + c22
                                + " where barcode='"+ barcode + "'";
                            list.Add(update);

                            if (dr.Length == 1)
                            {
                                string inp = "";
                                if (dt.Rows[i]["平台采购价"] != DBNull.Value && dt.Rows[i]["平台采购价"].ToString() != "")
                                {
                                    inp = ", inprice='" + dt.Rows[i]["平台采购价"].ToString() + "'";
                                }
                                string update1 = ""
                                    + " update t_goods_warehouse"
                                    + " set goodsnum='" + (Convert.ToInt16(dt.Rows[i]["入库数量"]) + Convert.ToInt16(dr[0]["goodsnum"])).ToString() + "'" + inp
                                    + " where id='" + dr[0]["cid"].ToString() + "'";
                                list.Add(update1);
                                if (dt.Rows[i]["零售价"] != DBNull.Value && dt.Rows[i]["零售价"].ToString() != "")
                                {
                                    string update2 = ""
                                        + " update t_goods_distributor_price"
                                        + " set rprice='" + dt.Rows[i]["零售价"].ToString() + "'"
                                        + " where usercode='" + dr[0]["suppliercode"].ToString() + "' and barcode='" + dr[0]["barcode"].ToString() + "'";
                                    list.Add(update2);
                                }
                            }
                            else if (dr.Length == 0)
                            {
                                try
                                {
                                    DataRow[] dataRows = dataTable.Select("username = '" + dt.Rows[i]["供货商"].ToString() + "'");
                                    if (dataRows.Length > 0)
                                    {
                                        if (dt.Rows[i]["入库数量"] != DBNull.Value && dt.Rows[i]["入库数量"].ToString() != "" && dt.Rows[i]["平台采购价"] != DBNull.Value && dt.Rows[i]["平台采购价"].ToString() != "")
                                        {
                                            string insert1 = ""
                                                + " insert into t_goods_warehouse(barcode,wid,wcode,goodsnum,inprice,supplierid,suppliercode)"
                                                + " values('" + dt.Rows[i]["商品条码"].ToString() + "','" + dataRows[0]["id"].ToString() + "','" + dataRows[0]["wcode"].ToString() + "','" + dt.Rows[i]["入库数量"].ToString() + "','" + dt.Rows[i]["平台采购价"].ToString() + "','" + dataRows[0]["supplierid"].ToString() + "','" + dataRows[0]["suppliercode"].ToString() + "')";
                                            list.Add(insert1);
                                        }
                                        else
                                        {
                                            msg.msg = dt.Rows[i]["barcode"].ToString() + "上述条码的入库数量、平台采购价不能为空。 ";
                                            return msg;
                                        }
                                    }
                                 
                                    
                                }
                                catch (Exception e)
                                {
                                    msg.msg = e.ToString();
                                    return msg;
                                }
                            }
                            else
                            {
                                msg.msg = "数据库中供应商："+ dt.Rows[i]["供货商"].ToString() + "与商品条码："+ dt.Rows[i]["商品条码"].ToString() + "有重复";
                                return msg;
                            }

                        }
                       
                    }
                                                  
                }
                if (!DatabaseOperationWeb.ExecuteDML(list))
                {
                    msg.msg = "sql错误";
                    return msg;
                }
                    
            }
            else
            {
                msg.msg = "未找到该文件";              
                return msg;
            }
            
           
            msg.type = "1";
           
            return msg;

        }


        /// <summary>
        /// 库存管理-门店库存-运营
        /// </summary>
        /// <param name="param"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PageResult StoreInventory(StoreInventoryParam dgnp, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            pageResult.pagination = new Page(dgnp.current, dgnp.pageSize);

            string purchasesnName = "";
            if (dgnp.purchasesnName != null && dgnp.purchasesnName != "")
            {
                purchasesnName = " x.username  like'%" + dgnp.purchasesnName + "%' and ";
            }
         

            string select = "";
            if (dgnp.select != null && dgnp.select != "")
            {
                select = " and (a.goodsName like '%" + dgnp.select + "%' or a.barcode like '%" + dgnp.select + "%')";
            }

            string sql1 = "select a.barcode,a.goodsName,a.slt,a.pNum,a.pprice,a.rprice,b.brand,b.country,b.model,e.wname,d.username supplier,c.inprice,(select x.username from t_user_list x where "+ purchasesnName + " x.usercode = a.usercode)  purchasesnName,( select z.safeNum from t_warehouse_send_goods z,t_warehouse_send h where h.`status`=1  and z.sendId=h.id and z.barcode=a.barcode and h.purchasersCode=a.usercode ORDER BY sendTime DESC LIMIT 0,1)  safeNum" 
                        + " from t_goods_distributor_price a,t_goods_list b,t_goods_warehouse c,t_user_list d,t_base_warehouse e "
                        + " where  c.goodsnum!='0'  and a.wid= c.wid and c.barcode=a.barcode and  a.supplierid=d.id and e.id=a.wid  and a.barcode=b.barcode "  + select
                        + "  limit " + (dgnp.current - 1) * dgnp.pageSize + "," + dgnp.pageSize;
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            if (dt1.Rows.Count > 0)
            {
                
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    StoreInventoryItem cdgi = new StoreInventoryItem();
                    cdgi.keyId = Convert.ToString((dgnp.current - 1) * dgnp.pageSize + i + 1);
                    cdgi.goodsName = dt1.Rows[i]["goodsName"].ToString();
                    cdgi.barcode = dt1.Rows[i]["barcode"].ToString();
                    cdgi.pNum = dt1.Rows[i]["pNum"].ToString();
                    cdgi.pprice = dt1.Rows[i]["pprice"].ToString();
                    cdgi.rprice = dt1.Rows[i]["rprice"].ToString();
                    cdgi.brand = dt1.Rows[i]["brand"].ToString();
                    cdgi.country = dt1.Rows[i]["country"].ToString();
                    cdgi.model = dt1.Rows[i]["model"].ToString();                
                    cdgi.supplierName = dt1.Rows[i]["supplier"].ToString();
                    cdgi.inprice = dt1.Rows[i]["inprice"].ToString();
                    cdgi.safeNum = dt1.Rows[i]["safeNum"].ToString();
                    cdgi.purchasesnName= dt1.Rows[i]["purchasesnName"].ToString();
                    cdgi.time = "";


                    pageResult.list.Add(cdgi);
                }

            }
        
            string sql2 = ""
                   + "select count(*)"
                   + " from t_goods_distributor_price a,t_goods_list b,t_goods_warehouse c,t_user_list d,t_base_warehouse e "
                   + " where  c.goodsnum!='0' and a.wid= c.wid and a.barcode=b.barcode and c.barcode=a.barcode and  a.supplierid=d.id and e.id=a.wid  "   + select;
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "T").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt2.Rows[0][0]);
            return pageResult;

        }




    }
}
