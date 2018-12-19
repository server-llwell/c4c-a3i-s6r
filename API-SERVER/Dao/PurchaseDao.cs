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
        public PageResult OnLoadGoodsList(OnLoadGoodsListParam  onLoadGoodsListParam,string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            MsgResult msg = new MsgResult();
            GoodspaginationParam goodspaginationParam = new GoodspaginationParam();


            string logCode = userId + DateTime.Now.ToString("yyyyMMddHHmmssff");
            string fileName = logCode + ".xlsx";
            FileManager fm = new FileManager();
            string purchasesn="";

            if (fm.fileCopy(onLoadGoodsListParam.fileTemp, fileName))
            {
                DataTable dt = fm.readExcelFileToDataTable(fileName);

                if (dt == null)
                {
                    msg.msg = "导入文档错误，请确认excel里的列是否正确，是否有相同名称的列。";
                    pageResult.item = msg;
                    return pageResult;
                }
                if (dt.Rows.Count>0)
                {
                    if (onLoadGoodsListParam.purchasesn != null && onLoadGoodsListParam.purchasesn != "")
                    {
                        string sql3 = ""
                            + "select `status` "
                            + "from t_purchase_list "
                            + "where purchasesn='"+ onLoadGoodsListParam.purchasesn + "' and usercode='"+userId+"'";

                        DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql3,"t").Tables[0];
                        if (dt1.Rows.Count<1)
                        {
                            msg.msg = "询价单号与用户不匹配";
                            pageResult.item = msg;
                            return pageResult;
                        }
                        string sql = ""
                            + "UPDATE t_purchase_goods "
                            + " set flag='0'"
                            + " WHERE purchasesn='" + onLoadGoodsListParam.purchasesn + "' ";                     
                        if (!DatabaseOperationWeb.ExecuteDML(sql))
                        {
                            msg.msg = "删除询价商品错误";
                            pageResult.item = msg;
                            return pageResult;
                        }
                        purchasesn = onLoadGoodsListParam.purchasesn;
                    }
                    else
                    {
                        purchasesn = DateTime.Now.ToString("yyyyMMddHHmmssff");
                        string sql1 = ""
                            + "INSERT into t_purchase_list(purchasesn,usercode,status)"
                            + " VALUES('"+ purchasesn + "','"+userId+"','0')  ";

                        if (!DatabaseOperationWeb.ExecuteDML(sql1))
                        {
                            msg.msg = "创建询价表错误";
                            pageResult.item = msg;
                            return pageResult;
                        }
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        string sql2 = ""
                            + "insert into t_purchase_goods(purchasesn,goodsname,barcode,oldtotal,flag) "
                            + " VALUES('"+ purchasesn + "','"+dr["询价商品名称"].ToString()+"','"+dr["询价商品条码"].ToString()+"','"+dr["询价数量"].ToString()+"','1')";
                        if (!DatabaseOperationWeb.ExecuteDML(sql2))
                        {
                            msg.msg = "导入询价商品错误";
                            pageResult.item = msg;
                            return pageResult;
                        } 
                        
                    }

                    
                }
            }
            goodspaginationParam.current = 1;
            goodspaginationParam.pageSize = 10;
            goodspaginationParam.purchasesn = purchasesn;
            pageResult= Goodspagination(goodspaginationParam, userId);
            msg.msg = "成功";
            msg.type = "1";
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
           
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];
            if (dt.Rows.Count>0)
            {
                for (int i=0;i<dt.Rows.Count;i++)
                {
                    OnLoadGoodsListItem onLoadGoodsListItem = new OnLoadGoodsListItem();
                    onLoadGoodsListItem.goodsName = dt.Rows[i]["goodsname"].ToString();
                    onLoadGoodsListItem.keyId = Convert.ToString((onLoadGoodsListParam.current - 1) * onLoadGoodsListParam.pageSize + i + 1);
                    onLoadGoodsListItem.barcode= dt.Rows[i]["barcode"].ToString();
                    onLoadGoodsListItem.brand= dt.Rows[i]["brand"].ToString();
                    onLoadGoodsListItem.total= dt.Rows[i]["oldtotal"].ToString();
                    onLoadGoodsListItem.purchasesn = onLoadGoodsListParam.purchasesn;
                    pageResult.list.Add(onLoadGoodsListItem);
                }              
            }
            string sql1 = ""
                + "select count(*) "
                + " from t_purchase_goods a,t_purchase_list b "
                + " where  a.purchasesn=b.purchasesn  and  a.purchasesn='" + onLoadGoodsListParam.purchasesn + "' and usercode='" + userId + "'  and a.flag!='0' ";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1,"T").Tables[0];
            pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]) ;

            return pageResult;

        }

        /// <summary>
        /// 询价商品删除接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult GoodsDelete(GoodsDeleteParam  goodsDeleteParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            MsgResult msg = new MsgResult();
            GoodspaginationParam goodspaginationParam = new GoodspaginationParam();

            string sql = ""
                + "update t_purchase_goods "
                + " set flag='0' "
                + " where purchasesn='"+ goodsDeleteParam.purchasesn+ "' "
                + " and barcode='"+ goodsDeleteParam .barcode+ "' ";
            if (!DatabaseOperationWeb.ExecuteDML(sql))
            {
                msg.msg = "删除失败";
                pageResult.item = msg;
                return pageResult;
            }
            
            goodspaginationParam.current = 1;
            goodspaginationParam.pageSize = 10;
            goodspaginationParam.purchasesn = goodsDeleteParam.purchasesn;


            return Goodspagination(goodspaginationParam, userId);
        }

        /// <summary>
        /// 询价表保存接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult InquiryPreservation(InquiryPreservationParam ipp, string userId)
        {
            MsgResult msg = new MsgResult();
            
            if (ipp.purchasesn != null && ipp.purchasesn != "")
            {               
                string sql = ""
                + "update t_purchase_list "
                + " set sendtype='" + ipp.sendType + "' , contacts='" + ipp.contacts + "' , sex='" + ipp.sex + "' , tel='" + ipp.tel + "' , deliveryTime='" + ipp.deliveryTime + "' , remark='" + ipp.remark + "' , `status`='7' "
                + " where purchasesn='" + ipp.purchasesn + "' and usercode='"+userId+"'";
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
                    + "insert into t_purchase_list(purchasesn,sendtype,contacts,sex,tel,deliveryTime,remark,`status`,usercode)"
                    + " values('"+ purchasesn + "','"+ipp.sendType+"','"+ipp.contacts+"','"+ipp.sex+"','"+ipp.tel+"','"+ipp.deliveryTime+"','"+ipp.remark+"','7','"+userId+"')";
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
            msg.msg = "失败";
            string purchasesn=ipp.purchasesn;
            string createtime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            if (ipp.purchasesn != null && ipp.purchasesn != "")
            {
                string sql1 = ""
                + "update t_purchase_list "
                + " set sendtype='" + ipp.sendType + "' , contacts='" + ipp.contacts + "' , sex='" + ipp.sex + "' , tel='" + ipp.tel + "' , deliveryTime='" + ipp.deliveryTime + "' , remark='" + ipp.remark + "' , `status`='1' ,createtime='"+ createtime + "'"
                + " where purchasesn='" + ipp.purchasesn + "' and usercode='" + userId + "'";
                if (!DatabaseOperationWeb.ExecuteDML(sql1))
                {
                    msg.msg = "update询价表错误";
                    return msg;
                }              
            }
            else
            {
                purchasesn = DateTime.Now.ToString("yyyyMMddHHmmssff");        
                string sql1 = ""
                    + "insert into t_purchase_list(purchasesn,sendtype,contacts,sex,tel,deliveryTime,remark,`status`,usercode,createtime)"
                    + " values('" + purchasesn + "','" + ipp.sendType + "','" + ipp.contacts + "','" + ipp.sex + "','" + ipp.tel + "','" + ipp.deliveryTime + "','" + ipp.remark + "','1','" + userId + "','"+ createtime + "')";
                if (!DatabaseOperationWeb.ExecuteDML(sql1))
                {
                    msg.msg = "insert询价表错误";
                    return msg;
                }
            }
            msg.msg = "成功";
            msg.type = "1";
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
            string select = "";
            string date = "";
            string status = " and `status`!='0' ";
            if (ilp.select!=null && ilp.select!="")
            {
                select = " and purchasesn like '%"+ ilp.select + "%'"
                    + " and remark like '%"+ ilp.select + "%' ";
            }
            if (ilp.status!=null && ilp.status !="")
            {
                status = " and `status`='"+ ilp.status + "' ";
            }
            if (ilp.date!=null && ilp.date.Length==2)
            {
                date = " and createtime between str_to_date('" + ilp.date[0]+ "' , '%Y-%m-%d')"
                    + " AND DATE_ADD(str_to_date('"+ ilp.date[1] + "','%Y-%m-%d') ,INTERVAL 1 DAY) ";
            }

            string sql = ""
                + "select purchasesn,createtime,remark,`status` "
                + " from t_purchase_list "
                + " where usercode='"+ userId + "' "+ select + status + date  + " order by createtime desc  limit " + (ilp.current - 1) * ilp.pageSize + "," + ilp.pageSize;
           
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];
            
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    InquiryListItem inquiryListItem = new InquiryListItem();
                    inquiryListItem.keyId= Convert.ToString((ilp.current - 1) * ilp.pageSize + i + 1);
                    inquiryListItem.purchasesn = dt.Rows[i]["purchasesn"].ToString();
                    inquiryListItem.remark= dt.Rows[i]["remark"].ToString();
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
            pageResult.pagination.total =Convert.ToInt16(dt1.Rows[0][0]) ;
            return pageResult;
        }


        /// <summary>
        /// 询价表查看接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult InquiryListDetailed(GoodsDeleteParam gdp, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.list = new List<object>();
            InquiryListDetailedItem inquiryListDetailedItem = new InquiryListDetailedItem();
            string sql = ""
                + "select sendtype,contacts,sex,tel,deliveryTime,remark,tax,waybillfee,purchasePrice "
                + " from t_purchase_list "
                + " where purchasesn='"+ gdp.purchasesn + "' and usercode='"+ userId + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql,"T").Tables[0];

            if (dt.Rows.Count>0)
            {
                inquiryListDetailedItem.purchasePrice = "0.00";
                inquiryListDetailedItem.tax = "0.00";
                inquiryListDetailedItem.waybillfee = "0.00";
                inquiryListDetailedItem.contacts = dt.Rows[0]["contacts"].ToString();
                inquiryListDetailedItem.deliveryTime =Convert.ToDateTime(dt.Rows[0]["deliveryTime"]).ToString("yyyy.MM.dd");              
                inquiryListDetailedItem.remark = dt.Rows[0]["remark"].ToString();
                inquiryListDetailedItem.sendType = dt.Rows[0]["sendType"].ToString();
                inquiryListDetailedItem.sex = dt.Rows[0]["sex"].ToString();
                inquiryListDetailedItem.tel = dt.Rows[0]["tel"].ToString();

                if (dt.Rows[0]["tax"].ToString() != null && dt.Rows[0]["tax"].ToString() != "")
                    inquiryListDetailedItem.tax = Math.Round(Convert.ToDouble(dt.Rows[0]["tax"]), 2).ToString();
                if (dt.Rows[0]["waybillfee"].ToString() != null && dt.Rows[0]["waybillfee"].ToString() != "")
                    inquiryListDetailedItem.waybillfee = Math.Round(Convert.ToDouble(dt.Rows[0]["waybillfee"]), 2).ToString();
                if (dt.Rows[0]["purchasePrice"].ToString() != null && dt.Rows[0]["purchasePrice"].ToString() != "")
                    inquiryListDetailedItem.purchasePrice = Math.Round(Convert.ToDouble(dt.Rows[0]["purchasePrice"]), 2).ToString();

                GoodspaginationParam goodspaginationParam = new GoodspaginationParam();
                goodspaginationParam.current = 1;
                goodspaginationParam.pageSize = 10;
                goodspaginationParam.purchasesn = gdp.purchasesn;
                if (gdp.status != "1" || gdp.status != "7")
                {
                    pageResult = InquiryPagesn(goodspaginationParam, userId);
                }
                else
                {
                    pageResult = Goodspagination(goodspaginationParam, userId);
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
                + " where purchasesn='" + gdp.purchasesn +"' and usercode='"+userId+"'";
            if (DatabaseOperationWeb.ExecuteDML(sql))
            {
                msg.msg = "已删除";
                msg.type = "1";
            }
            return msg;
        }


        /// <summary>
        /// 已报价、报价中、已报价（二次）、已完成状态的商品分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult InquiryPagesn(GoodspaginationParam onLoadGoodsListParam, string userId)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(onLoadGoodsListParam.current, onLoadGoodsListParam.pageSize);
            pageResult.list = new List<object>();
                    
            string sql = ""
                + "select a.purchasesn,a.goodsname,a.brand,a.barcode,a.oldtotal "
                + " from t_purchase_goods a,t_purchase_list b"
                + " where a.purchasesn=b.purchasesn  and a.purchasesn = '" + onLoadGoodsListParam.purchasesn + "' and usercode='" + userId + "' and a.flag!='0'  "
                + " limit " + (onLoadGoodsListParam.current - 1) * onLoadGoodsListParam.pageSize + "," + onLoadGoodsListParam.pageSize;

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "T").Tables[0];

            string sql3 = ""
                + "select purchasesn,barcode,usercode,demand,price,minProvide,maxProvide,flag"
                + " from t_purchase_inquiry"
                + " where purchasesn='" + onLoadGoodsListParam.purchasesn + "' ";
            DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "T").Tables[0];


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
                    DataRow[] dr1= dt3.Select("barcode= '" + onLoadGoodsListItem.barcode+"'");
                    
                    int sumTotal = 0;
                    int sumMax = 0;
                    int sumMin = 0;
                    double avgSupplyPrice=0;
                    double minSupplyPrice = 0;
                    double maxSupplyPrice = 0;
                    foreach (DataRow dr in dr1)
                    {
                        int total = 0;
                        int max = 0;
                        int min = 0;
                        if (dr["demand"]!= System.DBNull.Value)
                            total =Convert.ToInt16(dr["demand"]);
                        if (dr["maxProvide"] != System.DBNull.Value)
                            max = Convert.ToInt16(dr["maxProvide"]);
                        if (dr["minProvide"] != System.DBNull.Value)
                            min = Convert.ToInt16(dr["minProvide"]);
                        sumTotal += total;
                        sumMax += max;
                        sumMin += min;
                        minSupplyPrice +=Math.Round(min * Convert.ToDouble(dr["price"]),2) ;                        
                        maxSupplyPrice +=Math.Round(max * Convert.ToDouble(dr["price"]),2) ;                      
                        avgSupplyPrice +=Math.Round(total * Convert.ToDouble(dr["price"]),2);
                    }
                    if (sumTotal > 0)
                    {
                        onLoadGoodsListItem.purchaseNum = Convert.ToString(sumTotal);
                        onLoadGoodsListItem.supplyPrice = string.Format("{0:N2}", avgSupplyPrice / sumTotal);
                        onLoadGoodsListItem.totalPrice = Convert.ToString(avgSupplyPrice);
                    }
                    else if (sumMin > 0 && sumMax > 0)
                    {
                        onLoadGoodsListItem.supplyPrice = string.Format("{0:N2}", minSupplyPrice / sumMin) + "~" + string.Format("{0:N2}", maxSupplyPrice / sumMax);
                    }
                    else if (sumMin == 0 && sumMax > 0)
                    {
                        onLoadGoodsListItem.supplyPrice = "0~" + string.Format("{0:N2}", maxSupplyPrice / sumMax);
                    }
                    onLoadGoodsListItem.availableNum =Convert.ToString(sumMax) ;

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
    }
}
