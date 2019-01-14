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
    }
}
