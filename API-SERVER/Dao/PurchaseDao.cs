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
    public class PurchaseDao
    {
        private string path = System.Environment.CurrentDirectory;

        public PurchaseDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        /// <summary>
        /// 导入询价商品
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult OnLoadGoodsList(OnLoadGoodsListParam onLoadGoodsListParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            Msg msg = new Msg();
            GoodspaginationParam goodspaginationParam = new GoodspaginationParam();
            msg.purchasesn = onLoadGoodsListParam.purchasesn;

            string logCode = userId + DateTime.Now.ToString("yyyyMMddHHmmssff");
            string fileName = logCode + ".xlsx";
            FileManager fm = new FileManager();
            string purchasesn = "";

            if (fm.fileCopy(onLoadGoodsListParam.fileTemp, fileName))
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
                    if (!dt.Columns.Contains("序号"))
                    {
                        msg.msg = "缺少“序号”列，";
                    }
                    if (!dt.Columns.Contains("询价商品名称"))
                    {
                        msg.msg += "缺少“询价商品名称”列，";
                    }
                    if (!dt.Columns.Contains("询价商品条码"))
                    {
                        msg.msg += "缺少“询价商品条码”列，";
                    }
                    if (!dt.Columns.Contains("询价数量"))
                    {
                        msg.msg += "缺少“询价数量”列，";
                    }
                    if (msg.msg != null && msg.msg != "")
                    {
                        pageResult.item = msg;
                        return pageResult;
                    }
                    #endregion
                    ArrayList list = new ArrayList();
                    if (onLoadGoodsListParam.purchasesn != null && onLoadGoodsListParam.purchasesn != "")
                    {
                        string sql3 = ""
                            + "select `status` "
                            + "from t_purchase_list "
                            + "where purchasesn='" + onLoadGoodsListParam.purchasesn + "' and usercode='" + userId + "'";

                        DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "t").Tables[0];
                        if (dt1.Rows.Count < 1)
                        {
                            msg.msg = "询价单号与用户不匹配";
                            pageResult.item = msg;
                            return pageResult;
                        }
                        string sql = ""
                            + "UPDATE t_purchase_goods "
                            + " set flag='0'"
                            + " WHERE purchasesn='" + onLoadGoodsListParam.purchasesn + "' ";
                        list.Add(sql);

                        purchasesn = onLoadGoodsListParam.purchasesn;
                    }
                    else
                    {
                        purchasesn = DateTime.Now.ToString("yyyyMMddHHmmssff");
                        string sql1 = ""
                            + "INSERT into t_purchase_list(purchasesn,usercode,status)"
                            + " VALUES('" + purchasesn + "','" + userId + "','0')  ";
                        list.Add(sql1);

                    }
                    int p = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        int i = 0;
                        if (dr["询价商品名称"].ToString()==null || dr["询价商品名称"].ToString()=="")
                        {
                            msg.msg = "询价商品名称不能为空";
                            i += 1;
                        }
                        if (dr["询价商品条码"].ToString() == null || dr["询价商品条码"].ToString() == "")
                        {
                            msg.msg = "询价商品条码不能为空";
                            i += 1;
                        }
                        if (dr["询价数量"].ToString() == null || dr["询价数量"].ToString() == "")
                        {
                            msg.msg = "询价数量不能为空";
                            i += 1;
                        }
                        else if(!int.TryParse(dr["询价数量"].ToString(),out int a))
                        {
                            msg.msg = "询价数量不能为非数字";                           
                        }
                        if (i!=3)
                        {
                            if (msg.msg != null && msg.msg != "")
                            {
                                pageResult.item = msg;
                                return pageResult;
                            }
                            string sql2 = ""
                            + "insert into t_purchase_goods(purchasesn,goodsname,barcode,oldtotal,flag) "
                            + " VALUES('" + purchasesn + "','" + dr["询价商品名称"].ToString() + "','" + dr["询价商品条码"].ToString() + "','" + dr["询价数量"].ToString() + "','1')";
                            list.Add(sql2);
                            p += 1;
                        }

                    }
                    if (p != 0)
                    {
                        if (!DatabaseOperationWeb.ExecuteDML(list))
                        {
                            msg.msg = "导入询价商品错误";
                            pageResult.item = msg;
                            return pageResult;
                        }
                    }
                    else
                    {
                        msg.msg = "不能导入空表";
                        pageResult.item = msg;
                        return pageResult;
                    }
                    
                }
            }
            else
            {
                msg.msg = "文件名错误";
                pageResult.item = msg;
                return pageResult;
            }
            goodspaginationParam.current = 1;
            goodspaginationParam.pageSize = 10;
            goodspaginationParam.purchasesn = purchasesn;
            pageResult = Goodspagination(goodspaginationParam, userId);
            msg.msg = "成功";
            msg.type = "1";
            msg.purchasesn = purchasesn;
            pageResult.item = msg;
            return pageResult;
        }


        /// <summary>
        /// 询价、询价中、待提交状态的商品分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult Goodspagination(GoodspaginationParam onLoadGoodsListParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(onLoadGoodsListParam.current, onLoadGoodsListParam.pageSize);
            pageResult.list = new List<object>();

            string sql = ""
                + "select a.purchasesn,a.goodsname,a.brand,a.barcode,a.oldtotal "
                + " from t_purchase_goods a,t_purchase_list b "
                + " where a.purchasesn=b.purchasesn and a.purchasesn = '" + onLoadGoodsListParam.purchasesn + "' and usercode='" + userId + "' and a.flag!='0' "
                + " limit " + (onLoadGoodsListParam.current - 1) * onLoadGoodsListParam.pageSize + "," + onLoadGoodsListParam.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    OnLoadGoodsListItem onLoadGoodsListItem = new OnLoadGoodsListItem();
                    onLoadGoodsListItem.goodsName = dt.Rows[i]["goodsname"].ToString();
                    onLoadGoodsListItem.keyId = Convert.ToString((onLoadGoodsListParam.current - 1) * onLoadGoodsListParam.pageSize + i + 1);
                    onLoadGoodsListItem.barcode = dt.Rows[i]["barcode"].ToString();
                    onLoadGoodsListItem.brand = dt.Rows[i]["brand"].ToString();
                    onLoadGoodsListItem.total = dt.Rows[i]["oldtotal"].ToString();
                    onLoadGoodsListItem.purchasesn = onLoadGoodsListParam.purchasesn;
                    pageResult.list.Add(onLoadGoodsListItem);
                }
            }
            string sql1 = ""
                + "select count(*) "
                + " from t_purchase_goods a,t_purchase_list b "
                + " where  a.purchasesn=b.purchasesn  and  a.purchasesn='" + onLoadGoodsListParam.purchasesn + "' and usercode='" + userId + "'  and a.flag!='0' ";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);

            return pageResult;

        }

        /// <summary>
        /// 询价商品删除接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult GoodsDelete(GoodsDeleteParam goodsDeleteParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            MsgResult msg = new MsgResult();
            GoodspaginationParam goodspaginationParam = new GoodspaginationParam();

            string sql = ""
                + "update t_purchase_goods "
                + " set flag='0' "
                + " where purchasesn='" + goodsDeleteParam.purchasesn + "' "
                + " and barcode='" + goodsDeleteParam.barcode + "' ";
            if (!DatabaseOperationWeb.ExecuteDML(sql))
            {
                msg.msg = "删除失败";
                pageResult.item = msg;
                return pageResult;
            }

            goodspaginationParam.current = 1;
            goodspaginationParam.pageSize = 10;
            goodspaginationParam.purchasesn = goodsDeleteParam.purchasesn;
            pageResult = Goodspagination(goodspaginationParam, userId);
            msg.type = "1";
            pageResult.item = msg;

            return pageResult;
        }

        /// <summary>
        /// 询价表保存接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult InquiryPreservation(InquiryPreservationParam ipp, string userId)
        {
            MsgResult msg = new MsgResult();
            string deliveryTime = Convert.ToDateTime(ipp.deliveryTime).ToString("yyyy-MM-dd");
            string createtime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            if (ipp.purchasesn != null && ipp.purchasesn != "")
            {
                string sql = ""
                + "update t_purchase_list "
                + " set sendtype='" + ipp.sendType + "' , contacts='" + ipp.contacts + "' , sex='" + ipp.sex + "' , tel='" + ipp.tel + "' , deliveryTime='" + deliveryTime + "' , remark='" + ipp.remark + "' , `status`='7' ,createtime='" + createtime + "'"
                + " where purchasesn='" + ipp.purchasesn + "' and usercode='" + userId + "'";
                if (!DatabaseOperationWeb.ExecuteDML(sql))
                {
                    msg.msg = "update询价表错误";
                }
                else
                {
                    msg.msg = "成功";
                    msg.type = "1";
                }
            }
            else
            {
                string purchasesn = DateTime.Now.ToString("yyyyMMddHHmmssff");
                string sql1 = ""
                    + "insert into t_purchase_list(purchasesn,sendtype,contacts,sex,tel,deliveryTime,remark,`status`,usercode,createtime)"
                    + " values('" + purchasesn + "','" + ipp.sendType + "','" + ipp.contacts + "','" + ipp.sex + "','" + ipp.tel + "','" + deliveryTime + "','" + ipp.remark + "','7','" + userId + "','" + createtime + "')";
                if (!DatabaseOperationWeb.ExecuteDML(sql1))
                {
                    msg.msg = "insert询价表错误";
                }
                else
                {
                    msg.msg = "成功";
                    msg.type = "1";
                }

            }
            return msg;

        }

        /// <summary>
        /// 询价表提交接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult InquirySubmission(InquiryPreservationParam ipp, string userId)
        {
            MsgResult msg = new MsgResult();           
            string deliveryTime = Convert.ToDateTime(ipp.deliveryTime).ToString("yyyy-MM-dd");
            string purchasesn = ipp.purchasesn;
            string createtime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            if (ipp.purchasesn != null && ipp.purchasesn != "")
            {
                string sql1 = ""
                + "update t_purchase_list "
                + " set sendtype='" + ipp.sendType + "' , contacts='" + ipp.contacts + "' , sex='" + ipp.sex + "' , tel='" + ipp.tel + "' , deliveryTime='" + deliveryTime + "' , remark='" + ipp.remark + "' , `status`='1' ,createtime='" + createtime + "',offerstatus='1'"
                + " where purchasesn='" + ipp.purchasesn + "' and usercode='" + userId + "'";
               
                //创建t_purchase_inquiry             
                string insert = ""
                    + "insert into t_purchase_inquiry(purchasesn,usercode,barcode,flag,createtime,goodsname) "
                    + " (select a.purchasesn,b.usercode,a.barcode,'1' as flag,'" + createtime + "' as createtime,a.goodsname from t_purchase_goods a,t_goods_offer b where a.barcode=b.barcode  and a.flag='1' and a.purchasesn='" + ipp.purchasesn + "')";

                string insert1 = ""
                   + "insert into t_purchase_inquiry_bak(purchasesn,usercode,barcode,flag,createtime,goodsname) "
                   + " (select a.purchasesn,b.usercode,a.barcode,'1' as flag,'" + createtime + "' as createtime,a.goodsname from t_purchase_goods a,t_goods_offer b where a.barcode=b.barcode  and a.flag='1' and a.purchasesn='" + ipp.purchasesn + "')";
                if (!DatabaseOperationWeb.ExecuteDML(sql1) || !DatabaseOperationWeb.ExecuteDML(insert) || !DatabaseOperationWeb.ExecuteDML(insert1))
                {
                    msg.msg = "update询价表错误";                    
                }
                else
                {
                    msg.type = "1";
                }
            }
                       
            return msg;
        }


        /// <summary>
        /// 询价列表接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult InquiryList(InquiryListParam ilp, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(ilp.current, ilp.pageSize);
            pageResult.list = new List<object>();
            DateTime createtime = DateTime.Now;
            string select = "";
            string date = "";
            string status = " and `status`!='0' ";
            if (ilp.select != null && ilp.select != "")
            {
                select = " and ( purchasesn like '%" + ilp.select + "%'"
                    + " or remark like '%" + ilp.select + "%' ) ";
            }
            if (ilp.status != null && ilp.status != "")
            {
                status = " and `status`='" + ilp.status + "' ";
            }
            if (ilp.date != null && ilp.date.Length == 2)
            {
                date = " and createtime between str_to_date('" + ilp.date[0] + "' , '%Y-%m-%d')"
                    + " AND DATE_ADD(str_to_date('" + ilp.date[1] + "','%Y-%m-%d') ,INTERVAL 1 DAY) ";
            }
            ArrayList arrayList = new ArrayList();
            string sql2 = ""
                + " update t_purchase_list "
                + " set `status`='6' "
                + " where  STR_TO_DATE(deliverytime,'%Y-%m-%d') <'"+DateTime.Now.ToString("yyyy-MM-dd")+"' ";
            arrayList.Add(sql2);
            string updateList = ""
                    + " update t_purchase_list set offerstatus='2',status='2'"
                    + " where offerstatus='1' and status='1' and purchasesn in " 
                    +"(select * from (select purchasesn from t_purchase_list where day('"+ createtime + "' - createtime)>3) b )";
            arrayList.Add(updateList);
            DatabaseOperationWeb.ExecuteDML(arrayList);
            
            string sql = ""
                + "select purchasesn,createtime,remark,`status` "
                + " from t_purchase_list "
                + " where usercode='" + userId + "' " + select + status + date + " order by createtime desc  limit " + (ilp.current - 1) * ilp.pageSize + "," + ilp.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    InquiryListItem inquiryListItem = new InquiryListItem();
                    inquiryListItem.keyId = Convert.ToString((ilp.current - 1) * ilp.pageSize + i + 1);
                    inquiryListItem.purchasesn = dt.Rows[i]["purchasesn"].ToString();
                    inquiryListItem.remark = dt.Rows[i]["remark"].ToString();
                    inquiryListItem.status = dt.Rows[i]["status"].ToString();
                    inquiryListItem.createtime = Convert.ToDateTime(dt.Rows[i]["createtime"]).ToString("yyyy.MM.dd");
                    pageResult.list.Add(inquiryListItem);
                }
            }
            string sql1 = ""
                + "select count(*) "
                + " from t_purchase_list "
                + " where usercode='" + userId + "' "
                + select + status + date;
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
            return pageResult;
        }


        /// <summary>
        /// 询价表查看接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult InquiryListDetailed(GoodspaginationParam gdp, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            InquiryListDetailedItem inquiryListDetailedItem = new InquiryListDetailedItem();
            string sql = ""
                + "select a.`status`,a.sendtype,a.contacts,a.sex,a.tel,a.deliveryTime,a.remark,a.tax,a.waybillfee,a.purchasePrice,b.typename "
                + " from t_purchase_list a,t_base_sendtype b  "
                + " where a.sendtype=b.id and a.purchasesn='" + gdp.purchasesn + "' and a.usercode='" + userId + "'";
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            string sql1 = ""
                + "select a.`status`,a.sendtype,a.contacts,a.sex,a.tel,a.deliveryTime,a.remark,a.tax,a.waybillfee,a.purchasePrice,b.typename "
                + " from t_purchase_list_bak a,t_base_sendtype b "
                + " where a.sendtype=b.id and a.purchasesn='" + gdp.purchasesn + "' and a.usercode='" + userId + "'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];


            DataTable dt = dt2;
            if (dt1.Rows.Count > 0)
            {
                dt = dt1;
            }
            if (dt.Rows.Count > 0)
            {
                inquiryListDetailedItem.purchasePrice = "0.00";
                inquiryListDetailedItem.tax = "0.00";
                inquiryListDetailedItem.waybillfee = "0.00";
                inquiryListDetailedItem.status = dt.Rows[0]["status"].ToString();
                inquiryListDetailedItem.contacts = dt.Rows[0]["contacts"].ToString();
                inquiryListDetailedItem.deliveryTime = Convert.ToDateTime(dt.Rows[0]["deliveryTime"]).ToString("yyyy.MM.dd");
                inquiryListDetailedItem.remark = dt.Rows[0]["remark"].ToString();
                inquiryListDetailedItem.sendType = dt.Rows[0]["sendType"].ToString();
                inquiryListDetailedItem.typeName = dt.Rows[0]["typename"].ToString();
                inquiryListDetailedItem.sex = dt.Rows[0]["sex"].ToString();
                inquiryListDetailedItem.tel = dt.Rows[0]["tel"].ToString();
                inquiryListDetailedItem.purchasesn = gdp.purchasesn;

                if (dt.Rows[0]["tax"].ToString() != null && dt.Rows[0]["tax"].ToString() != "")
                    inquiryListDetailedItem.tax = Math.Round(Convert.ToDouble(dt.Rows[0]["tax"]), 2).ToString();
                if (dt.Rows[0]["waybillfee"].ToString() != null && dt.Rows[0]["waybillfee"].ToString() != "")
                    inquiryListDetailedItem.waybillfee = Math.Round(Convert.ToDouble(dt.Rows[0]["waybillfee"]), 2).ToString();
                if (dt.Rows[0]["purchasePrice"].ToString() != null && dt.Rows[0]["purchasePrice"].ToString() != "")
                    inquiryListDetailedItem.purchasePrice = Math.Round(Convert.ToDouble(dt.Rows[0]["purchasePrice"]), 2).ToString();

                GoodspaginationParam goodspaginationParam = new GoodspaginationParam();
                goodspaginationParam.current = gdp.current;
                goodspaginationParam.pageSize = gdp.pageSize;
                goodspaginationParam.purchasesn = gdp.purchasesn;
                if (gdp.status == "1" || gdp.status == "7")
                {
                    pageResult = Goodspagination(goodspaginationParam, userId);

                }
                else if (gdp.status == "2")
                {
                    pageResult = InquiryPagesn(goodspaginationParam, userId);
                }
                else
                {
                    pageResult = OtherInquiryPagesn(goodspaginationParam, userId);
                }
                pageResult.item = inquiryListDetailedItem;

            }

            return pageResult;
        }


        /// <summary>
        /// 表单删除接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult InquiryListDelete(GoodsDeleteParam gdp, string userId)
        {
            MsgResult msg = new MsgResult();
            msg.msg = "失败";
            string sql = ""
                + " update t_purchase_list "
                + " set `status`='0' "
                + " where purchasesn='" + gdp.purchasesn + "' and usercode='" + userId + "'";
            if (DatabaseOperationWeb.ExecuteDML(sql))
            {
                msg.msg = "已删除";
                msg.type = "1";
            }
            return msg;
        }


        /// <summary>
        /// 已报价状态的商品分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult InquiryPagesn(GoodspaginationParam onLoadGoodsListParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(onLoadGoodsListParam.current, onLoadGoodsListParam.pageSize);
            pageResult.list = new List<object>();
            string sql4 = ""
                + "select *"
                + " from t_purchase_goods_bak"
                + " where purchasesn = '" + onLoadGoodsListParam.purchasesn + "' ";
            DataTable dt4 = DatabaseOperationWeb.ExecuteSelectDS(sql4, "T").Tables[0];
            if (dt4.Rows.Count == 0)
            {
                string sql5 = ""
                    + "insert into t_purchase_goods_bak(purchasesn,goodsid,brand,barcode,goodsname,price,deliverytype,expectprice,realprice,oldtotal,total,costprice,otherprice,totalPrice,supplierid,flag) "
                    + " (select purchasesn,goodsid,brand,barcode,goodsname,price,deliverytype,expectprice,realprice,oldtotal,total,costprice,otherprice,totalPrice,supplierid,flag from t_purchase_goods"
                    + " where  purchasesn = '" + onLoadGoodsListParam.purchasesn + "' )";
                DatabaseOperationWeb.ExecuteDML(sql5);
            }

            string sql = ""
                + "select a.purchasesn,a.goodsname,a.brand,a.barcode,a.oldtotal "
                + " from t_purchase_goods_bak a,t_purchase_list b"
                + " where a.purchasesn=b.purchasesn  and a.purchasesn = '" + onLoadGoodsListParam.purchasesn + "' and usercode='" + userId + "' and a.flag!='0'  "
                + " limit " + (onLoadGoodsListParam.current - 1) * onLoadGoodsListParam.pageSize + "," + onLoadGoodsListParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            string sql3 = ""
                + "select purchasesn,barcode,usercode,demand,price,minProvide,maxProvide,flag"
                + " from t_purchase_inquiry"
                + " where purchasesn='" + onLoadGoodsListParam.purchasesn + "' and flag='2'";
            DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "T").Tables[0];

            string sql2 = ""
                + " select purchasesn,barcode,usercode,demand,price,minProvide,maxProvide,flag"
                + " from t_purchase_inquiry_bak"
                + " where purchasesn='" + onLoadGoodsListParam.purchasesn + "' and flag='2' ";
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "T").Tables[0];

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    OnLoadGoodsListItem onLoadGoodsListItem = new OnLoadGoodsListItem();
                    onLoadGoodsListItem.goodsName = dt.Rows[i]["goodsname"].ToString();
                    onLoadGoodsListItem.keyId = Convert.ToString((onLoadGoodsListParam.current - 1) * onLoadGoodsListParam.pageSize + i + 1);
                    onLoadGoodsListItem.barcode = dt.Rows[i]["barcode"].ToString();
                    onLoadGoodsListItem.brand = dt.Rows[i]["brand"].ToString();
                    onLoadGoodsListItem.total = dt.Rows[i]["oldtotal"].ToString();
                    DataRow[] dr1 = dt3.Select("barcode= '" + onLoadGoodsListItem.barcode + "'");
                    DataRow[] dr2 = dt2.Select("barcode= '" + onLoadGoodsListItem.barcode + "'");
                    DataRow[] dr3 = dr1;
                    if (dr2.Length > 0)
                    {
                        dr3 = dr2;
                    }
                    if (dr3.Count() == 0)
                    {
                        onLoadGoodsListItem.supplierNumType = "0";
                    }
                    else if (dr3.Count() == 1)
                    {
                        onLoadGoodsListItem.supplierNumType = "1";
                    }
                    else
                    {
                        onLoadGoodsListItem.supplierNumType = "2";
                    }
                    int sumTotal = 0;//采购数量
                    int sumMax = 0;//最大供货数量
                    int sumMin = 0;//最小供货数量
                    double avgSupplyPrice = 0;
                    double minSupplyPrice = 0;
                    double maxSupplyPrice = 0;
                    foreach (DataRow dr in dr3)
                    {
                        int total = 0;
                        int max = 0;
                        int min = 0;
                        if (dr["demand"] != System.DBNull.Value)
                            total = Convert.ToInt16(dr["demand"]);
                        if (dr["maxProvide"] != System.DBNull.Value)
                            max = Convert.ToInt16(dr["maxProvide"]);
                        if (dr["minProvide"] != System.DBNull.Value)
                            min = Convert.ToInt16(dr["minProvide"]);
                        sumTotal += total;
                        sumMax += max;
                        sumMin += min;
                        minSupplyPrice += Math.Round(min * Convert.ToDouble(dr["price"]), 2);
                        maxSupplyPrice += Math.Round(max * Convert.ToDouble(dr["price"]), 2);
                        avgSupplyPrice += Math.Round(total * Convert.ToDouble(dr["price"]), 2);
                    }
                    if (sumTotal > 0)
                    {
                        onLoadGoodsListItem.purchaseNum = Convert.ToString(sumTotal);
                        onLoadGoodsListItem.supplyPrice = string.Format("{0:N2}", avgSupplyPrice / sumTotal);
                        onLoadGoodsListItem.totalPrice = Convert.ToString(avgSupplyPrice);
                    }
                    else if(dr3.Length == 1)
                    {
                        onLoadGoodsListItem.supplyPrice = Math.Round(maxSupplyPrice/ sumMax,2).ToString();
                    }
                    else if (sumMin > 0 && sumMax > 0  )
                    {
                        onLoadGoodsListItem.supplyPrice = string.Format("{0:N2}", minSupplyPrice / sumMin) + "~" + string.Format("{0:N2}", maxSupplyPrice / sumMax);
                    }
                    else if (sumMin == 0 && sumMax > 0 )
                    {
                        onLoadGoodsListItem.supplyPrice = "0~" + string.Format("{0:N2}", maxSupplyPrice / sumMax);
                    }
                   
                    onLoadGoodsListItem.maxAvailableNum = Convert.ToString(sumMax);
                    onLoadGoodsListItem.minAvailableNum = Convert.ToString(sumMin);
                    onLoadGoodsListItem.purchasesn = onLoadGoodsListParam.purchasesn;
                    pageResult.list.Add(onLoadGoodsListItem);

                }
            }
            string sql1 = ""
                + "select count(*) "
                + " from t_purchase_goods_bak a,t_purchase_list b "
                + " where  a.purchasesn=b.purchasesn  and  a.purchasesn='" + onLoadGoodsListParam.purchasesn + "' and usercode='" + userId + "'  and a.flag!='0' ";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);

            return pageResult;

        }


        /// <summary>
        /// 已报价商品详情接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object GoodsDetails(GoodsDetailsParam goodspaginationParam, string userId)
        {
            List<GoodsDetailsItem> list = new List<GoodsDetailsItem>();
            string sql = ""
                + "select usercode,demand,price,minProvide,maxProvide,totalPrice "
                + " from t_purchase_inquiry "
                + " where purchasesn='" + goodspaginationParam.purchasesn + "' and  barcode='" + goodspaginationParam.barcode + "' and flag='2'";
               
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            string sql1 = ""
               + "select usercode,demand,price,minProvide,maxProvide,totalPrice "
               + " from t_purchase_inquiry_bak "
               + " where purchasesn='" + goodspaginationParam.purchasesn + "' and  barcode='" + goodspaginationParam.barcode + "' and flag='2'";
              
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            DataTable dt2 = dt;
            if (dt1.Rows.Count > 0)
            {
                dt2 = dt1;
            }


            if (dt2.Rows.Count > 0)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    GoodsDetailsItem goodsDetailsItem = new GoodsDetailsItem();
                    goodsDetailsItem.demand = "0";
                    goodsDetailsItem.id = dt2.Rows[i]["usercode"].ToString();
                    goodsDetailsItem.purchaseAmount = "0";
                    goodsDetailsItem.keyId = Convert.ToString(i + 1);
                    goodsDetailsItem.minOfferNum = dt2.Rows[i]["minProvide"].ToString();
                    goodsDetailsItem.maxOfferNum = dt2.Rows[i]["maxProvide"].ToString();
                    goodsDetailsItem.supplyId = "GH" + goodsDetailsItem.keyId;
                    goodsDetailsItem.price = dt2.Rows[i]["price"].ToString();
                    if (dt2.Rows[i]["demand"] != System.DBNull.Value)
                        goodsDetailsItem.demand = dt2.Rows[i]["demand"].ToString();
                    if (dt2.Rows[i]["totalPrice"] != System.DBNull.Value)
                        goodsDetailsItem.purchaseAmount = dt2.Rows[i]["totalPrice"].ToString();
                    list.Add(goodsDetailsItem);
                   
                }
            }
           

            return list;
        }


        /// <summary>
        /// 已报价商品详情确定接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public GoodsDetailsDetermineItem GoodsDetailsDetermine(GoodsDetailsDetermineParam gddp, string userId)
        {
            GoodsDetailsDetermineItem gddi = new GoodsDetailsDetermineItem();
            int purchaseNum = 0;

            #region inquiry表操作
            string sql = ""
                + "select *"
                + " from t_purchase_inquiry_bak"
                + " where purchasesn='" + gddp.purchasesn + "' and  barcode='" + gddp.barcode + "' and flag!='0'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            double purchasePrice = 0;
            ArrayList insert = new ArrayList();
            if (Convert.ToInt16(dt.Rows.Count) == 0)
            {
                string sql2 = ""
                   + "insert into t_purchase_inquiry_bak(purchasesn,usercode,goodsid,goodsname,barcode,demand,price,minProvide,maxProvide,total,totalPrice,flag,remark,createtime) "
                   + " (SELECT purchasesn,usercode,goodsid,goodsname,barcode,demand,price,minProvide,maxProvide,total,totalPrice,flag,remark,createtime  FROM t_purchase_inquiry"
                   + " where purchasesn='" + gddp.purchasesn + "' and  barcode='" + gddp.barcode + "' and flag='2')";
                insert.Add(sql2);

            }
            for (int i = 0; i < gddp.list.Count; i++)
            {
                double totalPrice = Math.Round(Convert.ToDouble(gddp.list[i].price) * Convert.ToInt16(gddp.list[i].demand), 2);//采购金额
                string flag = " ,flag='3'";
                string st = " purchasesn='" + gddp.purchasesn + "'";
                if (gddp.list.Count == 1)
                {
                    st = st + "  and flag!=0  and barcode='" + gddp.barcode + "'";
                    string sql1 = ""
                         + "update t_purchase_inquiry_bak"
                         + " set demand='" + gddp.list[i].demand + "', totalPrice='" + totalPrice + "'" + flag
                         + " where " + st;
                    insert.Add(sql1);
                }
                else if (gddp.list[i].id!=null && gddp.list[i].id != "" )
                {
                    st = st + " and usercode='" + gddp.list[i].id + "' and flag!=0  and barcode='" + gddp.barcode + "'";                   
                    if (Convert.ToInt16(gddp.list[i].demand) == 0)
                    {
                        flag = " ,flag='2'";
                    }
                    string sql1 = ""
                          + "update t_purchase_inquiry_bak"
                          + " set demand='" + gddp.list[i].demand + "', totalPrice='" + totalPrice + "'" + flag
                          + " where " + st;
                    insert.Add(sql1);
                }
                
                purchasePrice += totalPrice;
                purchaseNum += Convert.ToInt16(gddp.list[i].demand);
            }
            #endregion

            gddi.supplyPrice = string.Format("{0:N2}", purchasePrice / purchaseNum);
            #region goods表操作
            string sql6 = ""
                + " select *"
                + " from t_purchase_goods_bak"
                + " where purchasesn='" + gddp.purchasesn + "' and  barcode='" + gddp.barcode + "'";
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql6, "T").Tables[0];

            if (dt2.Rows.Count == 0)
            {
                string sql7 = ""
                 + "insert into t_purchase_goods_bak(purchasesn,goodsid,brand,barcode,goodsname,price,deliverytype,expectprice,realprice,oldtotal,total,costprice,otherprice,totalPrice,supplierid,flag) "
                 + " (SELECT purchasesn,goodsid,brand,barcode,goodsname,price,deliverytype,expectprice,realprice,oldtotal,total,costprice,otherprice,totalPrice,supplierid,flag FROM t_purchase_goods"
                 + " where purchasesn='" + gddp.purchasesn + "' and  barcode='" + gddp.barcode + "' and flag!='0')";
                insert.Add(sql7);
            }
            string sql8 = ""
               + "update t_purchase_goods_bak"
               + " set totalPrice='" + purchasePrice + "' ,total='" + purchaseNum + "' ,price= '" + gddi.supplyPrice + "'"
               + " where purchasesn='" + gddp.purchasesn + "' and  barcode='" + gddp.barcode + "' and flag!=0";
            insert.Add(sql8);
            #endregion

            #region list表操作
            string sql9 = ""
                + " select *"
                + " from t_purchase_goods_bak"
                + " where purchasesn='" + gddp.purchasesn + "' and flag!='0' and barcode!='"+ gddp.barcode + "'";


            string sql3 = ""
                + " select * "
                + " from t_purchase_list_bak"
                + " where purchasesn='" + gddp.purchasesn + "'";
            DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "T").Tables[0];
            if (dt3.Rows.Count == 0)
            {
                string sql4 = ""
                  + "insert into t_purchase_list_bak(purchasesn,usercode,stage,status,contacts,sex,tel,goodsnames,sendtype,address,deliverytime,currency,purchasePrice,waybillno,waybillremark,waybillfee,tax,createtime,purchasetime,remark) "
                  + " (SELECT purchasesn,usercode,stage,status,contacts,sex,tel,goodsnames,sendtype,address,deliverytime,currency,purchasePrice,waybillno,waybillremark,waybillfee,tax,createtime,purchasetime,remark FROM t_purchase_list"
                  + " where purchasesn='" + gddp.purchasesn + "')";
                insert.Add(sql4);
            }
            DatabaseOperationWeb.ExecuteDML(insert);

            DataTable dt4 = DatabaseOperationWeb.ExecuteSelectDS(sql9, "T").Tables[0];
            double total = 0;
            for (int i=0;i< dt4.Rows.Count;i++)
            {                            
                if (dt4.Rows[i]["totalPrice"].ToString()!=null && dt4.Rows[i]["totalPrice"].ToString()!="")
                {
                    total = Convert.ToDouble(dt4.Rows[i]["totalPrice"]);
                }
                gddi.allPrice += Math.Round(total, 2);
            }
            gddi.allPrice = Math.Round((gddi.allPrice + purchasePrice), 2);
            string sql5 = ""
               + "update t_purchase_list_bak"
               + " set purchasePrice='" + gddi.allPrice + "'"
               + " where purchasesn='" + gddp.purchasesn + "'";
            DatabaseOperationWeb.ExecuteDML(sql5);
            #endregion        
            gddi.purchasesn = gddp.purchasesn;
            gddi.barcode = gddp.barcode;
            gddi.purchaseNum = Convert.ToString(purchaseNum);
            gddi.totalPrice = Convert.ToString(purchasePrice);




            return gddi;
        }

        /// <summary>
        /// 已报价商品删除接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult InquiryGoodsDelete(GoodsDeleteParam goodsDeleteParam, string userId)
        {

            MsgResult msg = new MsgResult();
            GoodspaginationParam goodspaginationParam = new GoodspaginationParam();

            string sql = ""
                + "update t_purchase_goods_bak "
                + " set flag='0' "
                + " where purchasesn='" + goodsDeleteParam.purchasesn + "' "
                + " and barcode='" + goodsDeleteParam.barcode + "' ";

            if (!DatabaseOperationWeb.ExecuteDML(sql))
            {
                msg.msg = "删除失败";

                return msg;
            }
            msg.type = "1";
            return msg;
        }

        /// <summary>
        /// 已报价表取消接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult OfferCancel(GoodsDeleteParam goodsDeleteParam, string userId)
        {
            MsgResult msg = new MsgResult();
            ArrayList list = new ArrayList();
            string sql = ""
                + " delete from t_purchase_list_bak "
                + " where purchasesn='" + goodsDeleteParam.purchasesn + "'";
            list.Add(sql);
            string sql1 = ""
                + " delete from t_purchase_goods_bak"
                + " where purchasesn='" + goodsDeleteParam.purchasesn + "'";
            list.Add(sql1);
            string sql2 = ""
               + " delete from t_purchase_inquiry_bak"
               + " where purchasesn='" + goodsDeleteParam.purchasesn + "'";
            list.Add(sql2);
            if (DatabaseOperationWeb.ExecuteDML(list))
            {
                msg.type = "1";
            }

            return msg;

        }

        /// <summary>
        /// 已报价表提交接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult OfferSub(GoodsDeleteParam goodsDeleteParam, string userId)
        {
            MsgResult msg = new MsgResult();

            string sql = ""
                + "select usercode,barcode,demand,totalPrice"
                + " from t_purchase_inquiry_bak"
                + " where purchasesn='" + goodsDeleteParam.purchasesn + "'  and flag='3'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            ArrayList list = new ArrayList();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (Convert.ToInt16(dr["demand"]) > 0)
                    {
                        string sql1 = ""
                        + " update t_purchase_inquiry"
                        + " set demand='" + dr["demand"].ToString() + "' , totalPrice='" + dr["totalPrice"].ToString() + "' ,flag='3'"
                        + " where barcode='" + dr["barcode"].ToString() + "' and usercode='"+ dr["usercode"].ToString() + "' and purchasesn='" + goodsDeleteParam.purchasesn + "'";
                        list.Add(sql1);
                    }
                }
            }
            string sql2 = ""
                + " select barcode,totalPrice,price,total"
                + " from t_purchase_goods_bak"
                + " where purchasesn='" + goodsDeleteParam.purchasesn + "' and flag='1'";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "T").Tables[0];
            if (dt1.Rows.Count > 0)
            {
                foreach (DataRow dr in dt1.Rows)
                {
                    string sql3 = ""
                        + "update t_purchase_goods"
                        + " set totalPrice='" + dr["totalPrice"].ToString() + "' , price='" + dr["price"].ToString() + "' , total='" + dr["total"].ToString() + "'"
                        + " where barcode='" + dr["barcode"].ToString() + "' and  purchasesn='" + goodsDeleteParam.purchasesn + "'  and flag='1'";
                    list.Add(sql3);
                }
            }
            string sql4 = ""
                + "update t_purchase_list "
                + " set purchasePrice=(select purchasePrice from t_purchase_list_bak where purchasesn='" + goodsDeleteParam.purchasesn + "'),`status`='3',offerstatus='3' "
                + " where purchasesn='" + goodsDeleteParam.purchasesn + "'";
            list.Add(sql4);

            string update = ""
                + "update t_purchase_suppliermessage set status='1' where purchasesn='" + goodsDeleteParam.purchasesn + "'";
            list.Add(update);
            if (DatabaseOperationWeb.ExecuteDML(list))
            {
                msg = OfferCancel(goodsDeleteParam, userId);
            }
            return msg;
        }

        /// <summary>
        /// 报价中、已报价（二次）、已完成状态的商品分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult OtherInquiryPagesn(GoodspaginationParam onLoadGoodsListParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(onLoadGoodsListParam.current, onLoadGoodsListParam.pageSize);
            pageResult.list = new List<object>();

            string sql = ""
                + "select a.purchasesn,a.goodsname,a.brand,a.barcode,a.oldtotal,a.total,a.price,a.totalPrice,sum(c.minProvide) min,sum(c.maxProvide) max"
                + " from t_purchase_goods a,t_purchase_list b, t_purchase_inquiry c"
                + " where a.barcode=c.barcode and  a.purchasesn=b.purchasesn  and a.purchasesn = '" + onLoadGoodsListParam.purchasesn + "' and b.usercode='" + userId + "' and a.flag!='0' and a.total>0 "
                + " group by a.barcode"
                + " limit " + (onLoadGoodsListParam.current - 1) * onLoadGoodsListParam.pageSize + "," + onLoadGoodsListParam.pageSize;
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            string sql2 = ""
                + " select a.barcode,count(b.id) id "
                + " from t_purchase_inquiry b,t_purchase_goods a"
                + " where a.purchasesn=b.purchasesn and a.barcode=b.barcode and a.purchasesn = '" + onLoadGoodsListParam.purchasesn + "' and b.flag='3' and a.flag='1' "
                + " group by a.barcode";
            DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "T").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convert.ToInt16(dt.Rows[i]["total"]) > 0)
                    {
                        OnLoadGoodsListItem onLoadGoodsListItem = new OnLoadGoodsListItem();
                        onLoadGoodsListItem.keyId = Convert.ToString((onLoadGoodsListParam.current - 1) * onLoadGoodsListParam.pageSize + i + 1);
                        onLoadGoodsListItem.purchasesn = dt.Rows[i]["purchasesn"].ToString();
                        onLoadGoodsListItem.barcode = dt.Rows[i]["barcode"].ToString();
                        onLoadGoodsListItem.brand = dt.Rows[i]["brand"].ToString();
                        onLoadGoodsListItem.goodsName = dt.Rows[i]["goodsname"].ToString();
                        onLoadGoodsListItem.maxAvailableNum = dt.Rows[i]["max"].ToString();
                        onLoadGoodsListItem.minAvailableNum = dt.Rows[i]["min"].ToString();
                        onLoadGoodsListItem.purchaseNum = dt.Rows[i]["total"].ToString();
                        onLoadGoodsListItem.supplyPrice = dt.Rows[i]["price"].ToString();
                        onLoadGoodsListItem.total = dt.Rows[i]["oldtotal"].ToString();
                        onLoadGoodsListItem.totalPrice = dt.Rows[i]["totalPrice"].ToString();
                        if (Convert.ToInt16(dt2.Select("barcode='" + dt.Rows[i]["barcode"] + "'")[0]["id"]) > 1)
                            onLoadGoodsListItem.supplierNumType = "2";
                        else
                            onLoadGoodsListItem.supplierNumType = "1";

                        pageResult.list.Add(onLoadGoodsListItem);
                    }

                }
            }

            string sql1 = ""
                + "select a.purchasesn,a.goodsname,a.brand,a.barcode,a.oldtotal,a.total,a.price,a.totalPrice,sum(c.minProvide) min,sum(c.maxProvide) max"
                + " from t_purchase_goods a,t_purchase_list b, t_purchase_inquiry c"
                + " where a.barcode=c.barcode and  a.purchasesn=b.purchasesn  and a.purchasesn = '" + onLoadGoodsListParam.purchasesn + "' and b.usercode='" + userId + "' and a.flag!='0' and a.total>0 "
                + " group by a.barcode";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows.Count);

            return pageResult;

        }

        /// <summary>
        /// 报价中、已报价（二次）、已完成状态的商品详情接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object OtherGoodsDetails(GoodsDetailsParam goodspaginationParam, string userId)
        {

            List<GoodsDetailsItem> list = new List<GoodsDetailsItem>();
            string sql = ""
               + "select usercode,demand,price,minProvide,maxProvide,totalPrice "
               + " from t_purchase_inquiry "
               + " where purchasesn='" + goodspaginationParam.purchasesn + "' and  barcode='" + goodspaginationParam.barcode + "' and flag='3'";
              
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    GoodsDetailsItem goodsDetailsItem = new GoodsDetailsItem();
                    goodsDetailsItem.demand = "0";
                    goodsDetailsItem.purchaseAmount = "0";
                    goodsDetailsItem.id = dt.Rows[i]["usercode"].ToString();
                    goodsDetailsItem.keyId = Convert.ToString(i + 1);
                    goodsDetailsItem.minOfferNum = dt.Rows[i]["minProvide"].ToString();
                    goodsDetailsItem.maxOfferNum = dt.Rows[i]["maxProvide"].ToString();
                    goodsDetailsItem.supplyId = "GH" + goodsDetailsItem.keyId;
                    goodsDetailsItem.price = dt.Rows[i]["price"].ToString();
                    if (dt.Rows[i]["demand"] != System.DBNull.Value)
                        goodsDetailsItem.demand = dt.Rows[i]["demand"].ToString();
                    if (dt.Rows[i]["totalPrice"] != System.DBNull.Value)
                        goodsDetailsItem.purchaseAmount = dt.Rows[i]["totalPrice"].ToString();
                    list.Add(goodsDetailsItem);
                   
                }
            }
           

            return list;

        }


        /// <summary>
        /// 已报价（二次）提交接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult OfferedSub(GoodsDeleteParam goodsDeleteParam, string userId)
        {
            MsgResult msg = new MsgResult();
            string sql1 = "select `status` "
                + " from t_purchase_list a "
                + " where purchasesn = '" + goodsDeleteParam.purchasesn + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            if (goodsDeleteParam.status == dt.Rows[0]["status"].ToString())
            {
                string purchaseTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                string stage = "";
                if (goodsDeleteParam.status == "4")
                {
                    stage = "purchasetime='" + purchaseTime + "', stage='1' ,";
                }
                string sql = ""
                + "update t_purchase_list"
                + " set " + stage + " offerstatus='4', `status`= case '" + goodsDeleteParam.status + "'"
                + " when '3' then '4'"
                + " when '4' then '5'"
                + " end"
                + " where purchasesn='" + goodsDeleteParam.purchasesn + "'";
                if (DatabaseOperationWeb.ExecuteDML(sql))
                {
                    msg.type = "1";
                }
            }

            return msg;
        }


        /// <summary>
        /// 采购列表接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult PurchaseList(InquiryListParam ilp, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(ilp.current, ilp.pageSize);
            pageResult.list = new List<object>();
            string select = "";
            string date = "";
            string stage = " and stage!=''";
            if (ilp.select != null && ilp.select != "")
            {
                select = " and  a.purchasesn like '%" + ilp.select + "%'";
            }
            if (ilp.stage != null && ilp.stage != "")
            {
                stage = " and stage='" + ilp.stage + "' ";
            }
            if (ilp.date != null && ilp.date.Length == 2)
            {
                date = " and a.purchasetime between str_to_date('" + ilp.date[0] + "' , '%Y-%m-%d')"
                    + " AND DATE_ADD(str_to_date('" + ilp.date[1] + "','%Y-%m-%d') ,INTERVAL 1 DAY) ";
            }

            string sql = ""
                + "select a.purchasesn,a.purchasetime,a.stage,a.purchasePrice,a.waybillfee,a.tax,sum(b.total) total "
                + " from t_purchase_list a,t_purchase_goods b"
                + " where a.purchasesn=b.purchasesn and b.flag!='0' and usercode='" + userId + "' " + select + stage + date + " group by a.purchasesn order by purchasetime desc  limit " + (ilp.current - 1) * ilp.pageSize + "," + ilp.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    double tax = 0;
                    double waybillfee = 0;
                    if (dt.Rows[i]["tax"] != DBNull.Value)
                        tax = Convert.ToDouble(dt.Rows[i]["tax"]);
                    if (dt.Rows[i]["waybillfee"] != DBNull.Value)
                        waybillfee = Convert.ToDouble(dt.Rows[i]["waybillfee"]);
                    PurchaseListItem inquiryListItem = new PurchaseListItem();
                    inquiryListItem.keyId = Convert.ToString((ilp.current - 1) * ilp.pageSize + i + 1);
                    inquiryListItem.purchasesn = dt.Rows[i]["purchasesn"].ToString();
                    inquiryListItem.money = string.Format("{0:N2}", (Convert.ToDouble(dt.Rows[i]["purchasePrice"]) + tax + waybillfee));
                    inquiryListItem.stage = dt.Rows[i]["stage"].ToString();
                    if (dt.Rows[i]["purchasetime"] != DBNull.Value)
                        inquiryListItem.purchaseTime = Convert.ToDateTime(dt.Rows[i]["purchasetime"]).ToString("yyyy.MM.dd");
                    inquiryListItem.num = dt.Rows[i]["total"].ToString();
                    pageResult.list.Add(inquiryListItem);
                }
            }
            string sql1 = ""
                + " select a.purchasesn,a.purchasetime,a.stage,a.purchasePrice,a.waybillfee,a.tax,sum(b.total) total "
                + " from t_purchase_list a,t_purchase_goods b "
                + " where a.purchasesn=b.purchasesn and b.flag!='0' and usercode='" + userId + "' "
                + select + stage + date + " group by a.purchasesn";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "T").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows.Count);
            return pageResult;
        }


        /// <summary>
        /// 询价表查看接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult PurchaseDetails(GoodspaginationParam gdp, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            InquiryListDetailedItem inquiryListDetailedItem = new InquiryListDetailedItem();
            string sql = ""
                + "select a.stage,a.sendtype,a.contacts,a.sex,a.tel,a.deliveryTime,a.remark,a.tax,a.waybillfee,a.purchasePrice,b.typename "
                + " from t_purchase_list a,t_base_sendtype b  "
                + " where a.sendtype=b.id and a.purchasesn='" + gdp.purchasesn + "' and a.usercode='" + userId + "' and a.stage='" + gdp.stage + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            if (dt.Rows.Count > 0)
            {
                inquiryListDetailedItem.purchasePrice = "0.00";
                inquiryListDetailedItem.tax = "0.00";
                inquiryListDetailedItem.waybillfee = "0.00";
                inquiryListDetailedItem.stage = dt.Rows[0]["stage"].ToString();
                inquiryListDetailedItem.contacts = dt.Rows[0]["contacts"].ToString();
                inquiryListDetailedItem.deliveryTime = Convert.ToDateTime(dt.Rows[0]["deliveryTime"]).ToString("yyyy.MM.dd");
                inquiryListDetailedItem.remark = dt.Rows[0]["remark"].ToString();
                inquiryListDetailedItem.sendType = dt.Rows[0]["sendType"].ToString();
                inquiryListDetailedItem.typeName = dt.Rows[0]["typename"].ToString();
                inquiryListDetailedItem.sex = dt.Rows[0]["sex"].ToString();
                inquiryListDetailedItem.tel = dt.Rows[0]["tel"].ToString();

                if (dt.Rows[0]["tax"].ToString() != null && dt.Rows[0]["tax"].ToString() != "")
                    inquiryListDetailedItem.tax = Math.Round(Convert.ToDouble(dt.Rows[0]["tax"]), 2).ToString();
                if (dt.Rows[0]["waybillfee"].ToString() != null && dt.Rows[0]["waybillfee"].ToString() != "")
                    inquiryListDetailedItem.waybillfee = Math.Round(Convert.ToDouble(dt.Rows[0]["waybillfee"]), 2).ToString();
                if (dt.Rows[0]["purchasePrice"].ToString() != null && dt.Rows[0]["purchasePrice"].ToString() != "")
                    inquiryListDetailedItem.purchasePrice = Math.Round(Convert.ToDouble(dt.Rows[0]["purchasePrice"]), 2).ToString();

                GoodspaginationParam goodspaginationParam = new GoodspaginationParam();
                goodspaginationParam.current = gdp.current;
                goodspaginationParam.pageSize = gdp.pageSize;
                goodspaginationParam.purchasesn = gdp.purchasesn;
                pageResult = OtherInquiryPagesn(goodspaginationParam, userId);
                pageResult.item = inquiryListDetailedItem;
            }
            return pageResult;
        }


        /// <summary>
        /// 报价列表接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult OfferOrderList(OfferOrderListParam oop, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            pageResult.pagination = new Page(oop.current,oop.pageSize);
            DateTime createtime = DateTime.Now;            
            string st = " and (b.flag='1' and a.offerstatus = '1')";
            switch (oop.offerstatus)
            {
                case "已关闭"://0已关闭、1待报价、2已报价、3待确认、4已成交                   
                    st = " and (b.flag='0' or a.offerstatus = '0')";
                    break;
                case "待报价":                   
                    st = " and (b.flag='1' and a.offerstatus = '1')";
                    break;
                case "已报价":
                    st = " and ((b.flag='2' and a.offerstatus = '2') or (b.flag='2' and a.offerstatus = '1')) ";
                    break;
                case "待确认":
                    st = " and (b.flag='3' and a.offerstatus = '3')";
                    break;
                case "已成交":
                    st = " and (b.flag='3' and a.offerstatus = '4')";
                    break;
            }
            if (oop.offerstatus == "待报价" || oop.offerstatus == "" || oop.offerstatus == null)
            {
                string update = ""
                    + "update t_purchase_inquiry set flag='0' "
                    + "where  flag ='1'   and purchasesn in( select * from ("
                    + " select purchasesn "
                    + " from t_purchase_list "
                    + " where offerstatus='1'  and  day('" + createtime + "'-createtime)>3 "
                    + " group by purchasesn) b)";

                DatabaseOperationWeb.ExecuteDML(update);               
            }

            string selectSql = ""
                + " select a.purchasesn,a.offerstatus,a.deliverytime,a.createtime,b.flag "
                + " from t_purchase_list a,t_purchase_inquiry b "
                + " where a.purchasesn=b.purchasesn   and b.usercode='" + userId + "' "+st
                + " GROUP BY a.purchasesn ORDER BY a.deliverytime ASC ";               
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(selectSql, "T").Tables[0];
            if (dt.Rows.Count>0)
            {
                for (int i= (oop.current - 1) * oop.pageSize; i< oop.current * oop.pageSize && i< dt.Rows.Count;i++)
                {
                    OfferOrderListItem offerOrderListItem = new OfferOrderListItem();
                    offerOrderListItem.keyId = Convert.ToString((oop.current - 1) * oop.pageSize + i + 1);
                    offerOrderListItem.purchasesn = dt.Rows[i]["purchasesn"].ToString();
                    offerOrderListItem.offerstatus = oop.offerstatus;                   
                    offerOrderListItem.deliverytime= Convert.ToDateTime(dt.Rows[i]["deliverytime"]).ToString("yyyy-MM-dd");
                    offerOrderListItem.createtime = Convert.ToDateTime(dt.Rows[i]["createtime"]).ToString("yyyy-MM-dd");
                    pageResult.list.Add(offerOrderListItem);
                }              
            }
            pageResult.pagination.total = dt.Rows.Count;
            return pageResult;
        }

        /// <summary>
        /// 查看报价单详情接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public OfferOrderDetailsItem OfferOrderDetails(GoodsDetailsParam gdp, string userId)
        {
            OfferOrderDetailsItem oodi = new OfferOrderDetailsItem(); 
            string sql = ""
                + " select a.purchasesn,a.offerstatus,a.contacts,a.tel,a.address,a.deliverytime,a.remark,b.tax,b.waybillfee,b.otherprice,b.offerlisturl,b.purchasegoodsurl,b.message,b.status "
                + " from t_purchase_list a left join (select purchasesn,tax,waybillfee,otherprice,offerlisturl,purchasegoodsurl,message,status   from t_purchase_suppliermessage where usercode='" + userId+"') b  on  a.purchasesn=b.purchasesn"
                + " where  a.purchasesn='"+ gdp.purchasesn + "'";
            DataTable dtsql = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];
            if (dtsql.Rows.Count>0)
            {
                oodi.purchasesn = dtsql.Rows[0]["purchasesn"].ToString();
                oodi.contacts = dtsql.Rows[0]["contacts"].ToString();
                oodi.tel = dtsql.Rows[0]["tel"].ToString();
                oodi.address = dtsql.Rows[0]["address"].ToString();
                oodi.deliverytime = dtsql.Rows[0]["deliverytime"].ToString();
                oodi.goodslisturl = dtsql.Rows[0]["purchasegoodsurl"].ToString();
                oodi.message= dtsql.Rows[0]["message"].ToString();
                oodi.status = "1";
                if (dtsql.Rows[0]["status"].ToString()=="0" || dtsql.Rows[0]["status"].ToString() =="2" || dtsql.Rows[0]["status"].ToString() == ""  || dtsql.Rows[0]["status"].ToString() == null)
                {
                    oodi.status = "2";
                }
                if (oodi.goodslisturl=="" || oodi.goodslisturl==null)
                {
                    string select = ""
                        + " select a.barcode as 商品条码,a.goodsname as 商品名称,a.oldtotal as 预购数量,'' as 报价,'' as 最低供货数,'' as 最大供货数"
                        + " from t_purchase_goods a,t_goods_offer b"
                        + " where a.barcode=b.barcode and a.purchasesn='" + gdp.purchasesn + "' and a.flag='1' and b.flag='1' and b.usercode='"+ userId + "' ";
                    DataTable dtselect = DatabaseOperationWeb.ExecuteSelectDS(select,"T").Tables[0];
                    if (dtselect.Rows.Count>0)
                    {
                        FileManager file = new FileManager();
                        if (file.writeDataTableToExcel(dtselect, userId + gdp.purchasesn + ".xlsx"))
                        {
                            if (file.updateFileToOSS(userId + gdp.purchasesn+".xlsx", Global.OssDirFiles, userId + gdp.purchasesn + ".xlsx"))
                            {
                                oodi.goodslisturl = Global.OssUrl + Global.OssDirFiles + userId + gdp.purchasesn + ".xlsx";
                                string insert = "insert into t_purchase_suppliermessage(purchasesn,usercode,purchasegoodsurl)"
                                    + "values('"+ gdp.purchasesn + "','"+ userId +"','"+ oodi.goodslisturl + "')";
                                DatabaseOperationWeb.ExecuteDML(insert);                                
                            }
                        }                                             
                    }
                }
                oodi.remark = dtsql.Rows[0]["remark"].ToString();
                oodi.tax = dtsql.Rows[0]["tax"].ToString();
                oodi.waybillfee = dtsql.Rows[0]["waybillfee"].ToString();
                oodi.otherprice = dtsql.Rows[0]["otherprice"].ToString();
                oodi.offerlisturl = dtsql.Rows[0]["offerlisturl"].ToString();
                
            }
            oodi.offerstatus = gdp.purchasesn;
            return oodi;
        }

        /// <summary>
        /// 上传报价单接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult UploadOfferOrder(UploadOfferOrderParam uop, string userId)
        {
            MsgResult msg = new MsgResult();
            DateTime time = DateTime.Now;
            string logCode = userId + time.ToString("yyyyMMddHHmmssff");
            string fileName = logCode + ".xlsx";
            FileManager fm = new FileManager();
            if (fm.fileCopy(uop.file, fileName))
            {
                DataTable dt = fm.readExcelFileToDataTable(fileName);
                if (dt == null)
                {
                    msg.msg = "导入文档错误，请确认excel里的列是否正确，是否有相同名称的列。";
                    return msg;
                }
                if (dt.Rows.Count > 0)
                {
                    if (!dt.Columns.Contains("商品条码"))
                    {
                        msg.msg += "缺少“商品条码”列，";
                    }
                    if (!dt.Columns.Contains("商品名称"))
                    {
                        msg.msg += "缺少“商品名称”列，";
                    }
                    if (!dt.Columns.Contains("预购数量"))
                    {
                        msg.msg += "缺少“预购数量”列，";
                    }
                    if (!dt.Columns.Contains("报价"))
                    {
                        msg.msg += "缺少“报价”列，";
                    }
                    if (!dt.Columns.Contains("最低供货数"))
                    {
                        msg.msg += "缺少“最低供货数”列，";
                    }
                    if (!dt.Columns.Contains("最大供货数"))
                    {
                        msg.msg += "缺少“最大供货数”列，";
                    }
                    if (msg.msg != null && msg.msg != "")
                    {
                        return msg;
                    }
                    ArrayList arrayList = new ArrayList();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int count=0;
                        if (dt.Rows[i]["报价"].ToString()==null && dt.Rows[i]["报价"].ToString() =="")
                        {
                            count += 1;
                            msg.msg += "上传文件缺少报价";
                        }
                        if (dt.Rows[i]["商品条码"].ToString() == null && dt.Rows[i]["商品条码"].ToString() == "")
                        {
                            count += 1;
                            msg.msg += ",上传文件缺少商品条码";
                        }
                        if (dt.Rows[i]["商品名称"].ToString() == null && dt.Rows[i]["商品名称"].ToString() == "")
                        {
                            count += 1;
                            msg.msg += ",上传文件缺少商品名称";
                        }
                        if (dt.Rows[i]["最大供货数"].ToString() == null && dt.Rows[i]["最大供货数"].ToString() == "")
                        {
                            count += 1;
                            msg.msg += ",上传文件缺少最大供货数";
                        }
                        if (0<count && 4 > count)
                        {
                            return msg;
                        }
                        if (dt.Rows[i]["最低供货数"].ToString() == null && dt.Rows[i]["最低供货数"].ToString() == "")
                        {
                            dt.Rows[i]["最低供货数"]="0";
                        }
                        string update = ""
                        + "update  t_purchase_inquiry_bak set price='" + dt.Rows[i]["报价"].ToString() + "',minProvide='" + dt.Rows[i]["最低供货数"].ToString() + "',maxProvide='" + dt.Rows[i]["最大供货数"].ToString() + "',flag='2',createtime='" + time.ToString() + "'"
                        + " where purchasesn='" + uop.purchasesn + "' and  usercode='" + userId + "' and  barcode='"+ dt.Rows[i]["商品条码"].ToString() + "'";
                        arrayList.Add(update);
                    }
                    if (DatabaseOperationWeb.ExecuteDML(arrayList))
                    {
                        msg.type = "1";
                    }
                }
                else
                {
                    msg.msg = "上传文件无内容";
                }
            }
            else
            {
                msg.msg = "找不到文件";
            }
            return msg;
        }

        /// <summary>
        /// 报价单提交接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult UploadOfferOrderSubmit(UploadOfferOrderParam uop, string userId)
        {
            MsgResult msg = new MsgResult(); 
            DateTime time = DateTime.Now;
            string logCode = userId + time.ToString("yyyyMMddHHmmssff");
            string fileName = logCode + ".xlsx";
            FileManager fm = new FileManager();

            string select = ""
                + "select a.barcode 商品条码,a.goodsname 商品名称,a.price 报价,a.minProvide 最低供货数,a.maxProvide 最大供货数,a.createtime,b.oldtotal 预购数量"
                + " from t_purchase_inquiry_bak a,t_purchase_goods b"
                + " where a.purchasesn=b.purchasesn and a.purchasesn='" + uop.purchasesn + "' and  a.usercode='" + userId + "' and a.flag='2' "
                + " group by a.barcode";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(select,"T").Tables[0];            
            if (dt.Rows.Count > 0)
            {
                ArrayList arrayList = new ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string update = ""
                        + "update  t_purchase_inquiry set price='" + dt.Rows[i]["报价"].ToString() + "',minProvide='" + dt.Rows[i]["最低供货数"].ToString() + "',maxProvide='" + dt.Rows[i]["最大供货数"].ToString() + "',flag='2',createtime='" + dt.Rows[i]["createtime"].ToString() + "'"
                        + " where purchasesn='" + uop.purchasesn + "' and  usercode='" + userId + "' and barcode='"+ dt.Rows[i]["商品条码"].ToString() + "'";
                    arrayList.Add(update);
                }
                DatabaseOperationWeb.ExecuteDML(arrayList);
                ArrayList arrayList1 = new ArrayList();
                string update1 = ""
                    + " update t_purchase_inquiry set flag='0'"
                    + " where flag!='2' and purchasesn='" + uop.purchasesn + "' and  usercode='" + userId + "'";
                arrayList1.Add(update1);
                DataView dv = new DataView(dt);//虚拟视图吧，我这么认为
                DataTable dt2 = dv.ToTable(true, "商品条码", "商品名称", "预购数量", "报价", "最低供货数", "最大供货数");
                if (fm.writeDataTableToExcel1(dt2, fileName) == "true")
                {
                    if (fm.updateFileToOSS(fileName, Global.OssDirFiles, fileName))
                    {
                        string update2 = "update  t_purchase_suppliermessage set offerlisturl='" + Global.OssUrl + Global.OssDirFiles + fileName + "'"
                            + " where purchasesn='" + uop.purchasesn + "' and  usercode='" + userId + "'";
                        arrayList1.Add(update2);
                    }
                }                
                if (DatabaseOperationWeb.ExecuteDML(arrayList1))
                {
                    msg.type = "1";
                }
            }
            else
            {
                msg.msg = "请上传报价";
            }
            string selectall = ""
                + "select * from t_purchase_inquiry "
                + " where purchasesn='" + uop.purchasesn + "' and flag='1'";
            DataTable dtselectall = DatabaseOperationWeb.ExecuteSelectDS(selectall,"T").Tables[0];
            if (dtselectall.Rows.Count==0)
            {
                string updateList = ""
                    + " update t_purchase_list set offerstatus='2',status='2'"
                    + " where purchasesn='" + uop.purchasesn + "'";
                DatabaseOperationWeb.ExecuteDML(updateList);
            }
            return msg;
        }

        /// <summary>
        /// 待确认提交接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult WaitingSubmit(WaitingSubmitParam wsp, string userId)
        {
            MsgResult msg = new MsgResult(); 
            ArrayList arrayList = new ArrayList();
            string update = ""
                + "update t_purchase_suppliermessage set tax='"+ wsp.tax + "',waybillfee='"+ wsp.waybillfee + "',otherprice='"+ wsp.otherprice + "',status='2' "
                + " where purchasesn='" + wsp.purchasesn + "' and usercode='" + userId + "' ";
            arrayList.Add(update);
            if (DatabaseOperationWeb.ExecuteDML(arrayList))
            {
                msg.type = "1";
            }
            return msg;
        }

    }
}
