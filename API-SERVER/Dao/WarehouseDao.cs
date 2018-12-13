using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
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
        public PageResult CollectGoods(CollectGoodsIn cgi,string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(cgi.current, cgi.pageSize);
            pageResult.list = new List<Object>();
            string time = "";
            if (cgi.date!=null && cgi.date.Length==2)
            {
                time = "and sendTime between  str_to_date('" + cgi.date[0] + "', '%Y-%m-%d') " +
                               "AND DATE_ADD(str_to_date('" + cgi.date[1] + "', '%Y-%m-%d'),INTERVAL 1 DAY) ";
            }
            string sd = "";
            if (cgi.sendid != null && cgi.sendid != "")
            {
                sd = " and id='"+cgi.sendid+"' ";
            }
            string st = "";
            if (cgi.sendType!=null && cgi.sendType!="")
            {
                st = " and sendType='"+ cgi.sendType + "'";
            }
            string zt = "";
            if (cgi.status!=null && cgi.status!="")
            {
                zt = " and status='" + cgi.status + "'";
             }

            string sql = "select sendType,id,goodsTotal,sendTime,sendName,sendTel,`status` from t_warehouse_send where purchasersCode='" + userId+"' "+ sd + st + zt + time +
                "order by sendTime desc limit "+(cgi.current - 1) * cgi.pageSize + "," + cgi.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"table").Tables[0];
            if(dt.Rows.Count>0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CollectGoodsItem cgt = new CollectGoodsItem();
                    cgt.keyId = Convert.ToString((cgi.current - 1) * cgi.pageSize + i + 1);
                    cgt.sendid= dt.Rows[i]["id"].ToString();
                    cgt.sendName = dt.Rows[i]["sendName"].ToString();
                    cgt.sendTel= dt.Rows[i]["sendTel"].ToString();
                    cgt.sendTime = dt.Rows[i]["sendTime"].ToString();
                    cgt.sendType = dt.Rows[i]["sendType"].ToString();
                    cgt.status = dt.Rows[i]["status"].ToString();
                    cgt.goodsTotal = String.Format("{0:F}",Convert.ToDouble(dt.Rows[i]["goodsTotal"].ToString()) );
                    pageResult.list.Add(cgt);
                }
            }

            string sql1 = "select count(*) from t_warehouse_send where purchasersCode='" + userId + "' " + sd + st + zt + time;
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "table").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return pageResult;
        }

        public PageResult CollectGoodsList(CollectGoodsListIn cgi,string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(cgi.current, cgi.pageSize);
            pageResult.list = new List<object>();


            string sql = "select barcode,slt,a.id,goodsName,brand,supplyPrice,goodsNum,a.goodsTotal,waybillNo  from t_warehouse_send_goods a,t_warehouse_send  b  where a.sendId=b.id  and sendId='" +
                cgi.sendid+ "' limit " + (cgi.current - 1) * cgi.pageSize + "," + cgi.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "table").Tables[0];
            if (dt.Rows.Count>0)
            {
                CollectGoodsListSum cgs = new CollectGoodsListSum();
                for (int i=0;i<dt.Rows.Count;i++)
                {
                    CollectGoodsListItem collectGoodsListItem = new CollectGoodsListItem();
                    collectGoodsListItem.keyId = Convert.ToString((cgi.current-1)*cgi.pageSize+i+1);
                    collectGoodsListItem.id = dt.Rows[i]["id"].ToString();
                    collectGoodsListItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                    collectGoodsListItem.barcode= dt.Rows[i]["barcode"].ToString();
                    collectGoodsListItem.brand = dt.Rows[i]["brand"].ToString();
                    collectGoodsListItem.goodsNum = dt.Rows[i]["goodsNum"].ToString();
                    collectGoodsListItem.goodsTotal = String.Format("{0:F}", Convert.ToDouble(dt.Rows[i]["goodsTotal"].ToString()))  ;
                    collectGoodsListItem.slt = dt.Rows[i]["slt"].ToString();
                    collectGoodsListItem.supplyPrice = String.Format("{0:F}", Convert.ToDouble(dt.Rows[i]["supplyPrice"].ToString()));
                    cgs.money += Convert.ToDouble(dt.Rows[i]["goodsTotal"].ToString());

                    pageResult.list.Add(collectGoodsListItem);

                }
                cgs.money= Math.Round(cgs.money,2);
                cgs.waybillNo= dt.Rows[0]["waybillNo"].ToString();
                pageResult.item = cgs;


            }
            string sql1 = "select count(*) from t_warehouse_send_goods a,t_warehouse_send  b  where a.sendId=b.id  and sendId='" +
                cgi.sendid + "'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "table").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);

            return pageResult;
        }

        public MsgResult ConfirmGoods(ConfirmGoodsIn cgi,string userId)
        {
            string id = cgi.sendid;
            MsgResult msgResult = new MsgResult();
            msgResult.msg = "操作失败";
            msgResult.type = "0";
            if (cgi.waybillNo != null && cgi.waybillNo != "")
            {
                string sql = "update t_warehouse_send  SET `status`='0' , waybillNo='"+ cgi.waybillNo + "' WHERE id='" + id + "'";
                if (DatabaseOperationWeb.ExecuteDML(sql))
                {
                    msgResult.msg = "成功";
                    msgResult.type = "1";
                }



                return msgResult;
            }
            else
            {
                string time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                string sql = "update t_warehouse_send  SET `status`='1',confirmTime='"+ time + "'  WHERE id='" + id + "'";
                if (DatabaseOperationWeb.ExecuteDML(sql))
                {
                    msgResult.msg = "成功";
                    msgResult.type = "1";
                }

;

                return msgResult;
            }
            
        }
        public MsgResult exportSendGoods(ExportSendGoodsParam param,string userId)
        {
            MsgResult msg = new MsgResult();
            string fileName = "Export_SendGoods_" + userId + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx";
            string sql = "select w.barcode as '商品条码',g.goodsName as '*商品名称',w.rprice as '*售价（元）',c.`name` as '分类','' as '单位',pprice as '进价（元）','' as '会员价（元）',goodsNum as '库存' ,'' as '保质期（天）','' as '产地' from t_warehouse_send_goods w ,t_goods_list g,t_goods_category c where   w.barcode = g.barcode and g.catelog1 = c.id and sendId= '" + param.sendid+"'";
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
