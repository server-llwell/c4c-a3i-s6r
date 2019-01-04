using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace API_SERVER.Dao
{
    public class DistributorDao
    {
        private string path = System.Environment.CurrentDirectory;

        public DistributorDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        #region 渠道商费用
        /// <summary>
        /// 获取渠道商类型下拉框
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<PlatformItem> getPlatform()
        {
            List<PlatformItem> lp = new List<PlatformItem>();
            string sql = "select platformId,platformType from t_base_platform";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PlatformItem platformItem = new PlatformItem();
                    platformItem.platformId = dt.Rows[i]["platformId"].ToString();
                    platformItem.platformType = dt.Rows[i]["platformType"].ToString();
                    lp.Add(platformItem);
                }
            }
            return lp;
        }

        /// <summary>
        /// 修改渠道商费用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult updateDistributor(DistributorItem distributorItem)
        {
            MsgResult msg = new MsgResult();
            try
            {
                string sql = "update t_user_list set priceType='" + distributorItem.priceType + "'," +
                             "platformCost=" + distributorItem.platformCost + "," +
                             "platformCostType='" + distributorItem.platformCostType + "' " +
                             "where id=" + distributorItem.id + " and usertype='2'";
                if (DatabaseOperationWeb.ExecuteDML(sql))
                {
                    msg.type = "1";
                    msg.msg = "修改成功";
                }
            }
            catch (Exception)
            {
                msg.msg = "数据库处理失败";
            }
            return msg;
        }

        /// <summary>
        /// 获取渠道商费用列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult getDistributorList(DistributorParam distributorParam)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(distributorParam.current, distributorParam.pageSize);
            pageResult.list = new List<Object>();
            string sql = "select id,usercode,username,u.platformId,p.platformType,priceType,platformCost,platformCostType " +
                         "from t_user_list u LEFT JOIN t_base_platform p on u.platformId = p.platformId where u.usertype='2' " +
                         " ORDER BY id asc LIMIT " + (distributorParam.current - 1) * distributorParam.pageSize + "," + distributorParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql1 = "select count(*) from t_user_list u LEFT JOIN t_base_platform p on u.platformId = p.platformId " +
                              "where u.usertype='2' ";

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DistributorItem distributorItem = new DistributorItem();
                    distributorItem.id = dt.Rows[i]["id"].ToString();
                    distributorItem.usercode = dt.Rows[0]["usercode"].ToString();
                    distributorItem.username = dt.Rows[i]["username"].ToString();
                    if (!double.TryParse(dt.Rows[i]["platformCost"].ToString(), out distributorItem.platformCost))
                    {
                        distributorItem.platformCost = 0;
                    }
                    //try
                    //{
                    //    distributorItem.platformCost = Convert.ToDouble(dt.Rows[i]["platformCost"]);
                    //}
                    //catch (Exception)
                    //{
                    //}
                    distributorItem.platformId = dt.Rows[i]["platformId"].ToString();
                    distributorItem.platformType = dt.Rows[i]["platformType"].ToString();
                    distributorItem.priceType = dt.Rows[i]["priceType"].ToString();
                    distributorItem.platformCostType = dt.Rows[i]["platformCostType"].ToString();

                    pageResult.list.Add(distributorItem);
                }
            }
            return pageResult;
        }
        #endregion

        #region 渠道商商品

        /// <summary>
        /// 获取渠道商商品列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PageResult getDGoodsList(DistributorParam distributorParam)
        {
            PageResult pageResult = new PageResult();
            pageResult.pagination = new Page(distributorParam.current, distributorParam.pageSize);
            pageResult.list = new List<Object>();

            string st = "";
            if (distributorParam.purchase != null && distributorParam.purchase != "")
            {
                st = " and g.usercode='" + distributorParam.purchase + "' ";
            }
            string sql = "select g.*, p.platformType,u.username as suppliername,ul.username as purchase " +
                         "from t_user_list ul ,t_base_platform p, t_goods_distributor_price g LEFT JOIN t_user_list u on g.supplierid = u.id " +
                         "where g.platformId = p.platformId  and ul.usercode = g.usercode " + st +
                         " ORDER BY g.id asc LIMIT " + (distributorParam.current - 1) * distributorParam.pageSize + "," + distributorParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "Table").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string sql1 = "select count(*)  from t_goods_distributor_price g where 1=1 " + st;

                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "Table").Tables[0];
                pageResult.pagination.total = Convert.ToInt16(dt1.Rows[0][0]);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DistributorGoodsItem distributorGoodsItem = new DistributorGoodsItem();
                    distributorGoodsItem.id = dt.Rows[i]["id"].ToString();
                    distributorGoodsItem.usercode = dt.Rows[i]["usercode"].ToString();
                    distributorGoodsItem.purchase = dt.Rows[i]["purchase"].ToString();
                    distributorGoodsItem.goodsid = dt.Rows[i]["goodsid"].ToString();
                    distributorGoodsItem.barcode = dt.Rows[i]["barcode"].ToString();
                    distributorGoodsItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                    distributorGoodsItem.slt = dt.Rows[i]["slt"].ToString();
                    distributorGoodsItem.platformId = dt.Rows[i]["platformId"].ToString();
                    distributorGoodsItem.platformType = dt.Rows[i]["platformType"].ToString();
                    distributorGoodsItem.pprice = Convert.ToDouble(dt.Rows[i]["pprice"].ToString());
                    distributorGoodsItem.pNum = Convert.ToDouble(dt.Rows[i]["pNum"]);
                    distributorGoodsItem.supplierid = dt.Rows[i]["supplierid"].ToString();
                    distributorGoodsItem.suppliername = dt.Rows[i]["suppliername"].ToString();
                    distributorGoodsItem.profitPlatform = Convert.ToDouble(dt.Rows[i]["profitPlatform"]);
                    distributorGoodsItem.profitAgent = Convert.ToDouble(dt.Rows[i]["profitAgent"]);
                    distributorGoodsItem.profitDealer = Convert.ToDouble(dt.Rows[i]["profitDealer"]);
                    distributorGoodsItem.profitOther1 = Convert.ToDouble(dt.Rows[i]["profitOther1"]);
                    distributorGoodsItem.profitOther1Name = dt.Rows[i]["profitOther1Name"].ToString();
                    distributorGoodsItem.profitOther2 = Convert.ToDouble(dt.Rows[i]["profitOther2"]);
                    distributorGoodsItem.profitOther2Name = dt.Rows[i]["profitOther2Name"].ToString();
                    distributorGoodsItem.profitOther3 = Convert.ToDouble(dt.Rows[i]["profitOther3"]);
                    distributorGoodsItem.profitOther3Name = dt.Rows[i]["profitOther3Name"].ToString();

                    pageResult.list.Add(distributorGoodsItem);
                }
            }
            return pageResult;
        }

        /// <summary>
        /// 修改渠道商商品费用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public MsgResult updateDGoods(DistributorGoodsItem distributorGoodsItem)
        {
            MsgResult msg = new MsgResult();
            try
            {
                double sum = distributorGoodsItem.profitPlatform + distributorGoodsItem.profitAgent + distributorGoodsItem.profitDealer
                           + distributorGoodsItem.profitOther1 + distributorGoodsItem.profitOther2 + distributorGoodsItem.profitOther3;
                if (sum != 100)
                {
                    msg.msg = "利润分成总和不是100";
                    return msg;
                }
                if (distributorGoodsItem.supplierid == null)
                {
                    distributorGoodsItem.supplierid = "";
                }
                if (distributorGoodsItem.profitOther1Name == null)
                {
                    distributorGoodsItem.profitOther1Name = "";
                }
                if (distributorGoodsItem.profitOther2Name == null)
                {
                    distributorGoodsItem.profitOther2Name = "";
                }
                if (distributorGoodsItem.profitOther3Name == null)
                {
                    distributorGoodsItem.profitOther3Name = "";
                }

                string sql = "update t_goods_distributor_price set pprice=" + distributorGoodsItem.pprice + "," +
                             "pNum='" + distributorGoodsItem.pNum + "'," +
                             "supplierid='" + distributorGoodsItem.supplierid + "'," +
                             "profitPlatform=" + distributorGoodsItem.profitPlatform + "," +
                             "profitAgent=" + distributorGoodsItem.profitAgent + "," +
                             "profitDealer=" + distributorGoodsItem.profitDealer + "," +
                             "profitOther1=" + distributorGoodsItem.profitOther1 + "," +
                             "profitOther1Name='" + distributorGoodsItem.profitOther1Name + "'," +
                             "profitOther2=" + distributorGoodsItem.profitOther2 + "," +
                             "profitOther2Name='" + distributorGoodsItem.profitOther2Name + "'," +
                             "profitOther3=" + distributorGoodsItem.profitOther3 + "," +
                             "profitOther3Name='" + distributorGoodsItem.profitOther3Name + "' " +
                             "where id=" + distributorGoodsItem.id;
                if (DatabaseOperationWeb.ExecuteDML(sql))
                {
                    msg.type = "1";
                    msg.msg = "修改成功";
                }
            }
            catch (Exception)
            {
                msg.msg = "数据库处理失败";
            }
            return msg;
        }

        public MsgResult uploadDGoods(FileUploadParam uploadParam)
        {
            string selsql = "select * from t_goods_distributor_price";
            DataTable distributorDt = DatabaseOperationWeb.ExecuteSelectDS(selsql, "TABLE").Tables[0];
            MsgResult msg = new MsgResult();
            FileManager fm = new FileManager();
            DataTable dt = fm.readExcelFileToDataTable(uploadParam.fileTemp);
            if (dt==null)
            {
                msg.msg = "导入文档错误，请确认excel里的列是否正确，是否有相同名称的列。";
                return msg;
            }
            if (dt.Rows.Count > 0)
            {
                #region 校验导入文档的列
                if (!dt.Columns.Contains("商品条码"))
                {
                    msg.msg += "缺少“商品条码”列，";
                }
                if (!dt.Columns.Contains("商品名称(中文)"))
                {
                    msg.msg += "缺少“商品名称(中文)”列，";
                }
                if (!dt.Columns.Contains("采购类型"))
                {
                    msg.msg += "缺少“采购类型”列，";
                }
                if (!dt.Columns.Contains("采购商"))
                {
                    msg.msg += "缺少“采购商”列，";
                }
                if (!dt.Columns.Contains("采购单价"))
                {
                    msg.msg += "缺少“采购单价”列，";
                }
                if (!dt.Columns.Contains("零售价"))
                {
                    msg.msg += "缺少“零售价”列，";
                }   
                //if (!dt.Columns.Contains("采购数量"))
                //{
                //    msg.msg += "缺少“采购数量”列，";
                //}
                if (!dt.Columns.Contains("默认供应商"))
                {
                    msg.msg += "缺少“默认供应商”列，";
                }
                if (!dt.Columns.Contains("默认仓库"))
                {
                    msg.msg += "缺少“默认仓库”列，";
                }
                if (!dt.Columns.Contains("利润分成百分比（平台）"))
                {
                    msg.msg += "缺少“利润分成百分比（平台）”列，";
                }
                if (!dt.Columns.Contains("利润分成百分比（代理）"))
                {
                    msg.msg += "缺少“利润分成百分比（代理）”列，";
                }
                if (!dt.Columns.Contains("利润分成百分比（分销商）"))
                {
                    msg.msg += "缺少“利润分成百分比（分销商）”列，";
                }
                if (!dt.Columns.Contains("利润分成百分比（其他1）"))
                {
                    msg.msg += "缺少“利润分成百分比（其他1）”列，";
                }
                if (!dt.Columns.Contains("利润分成百分比（其他2）"))
                {
                    msg.msg += "缺少“利润分成百分比（其他2）”列，";
                }
                if (!dt.Columns.Contains("利润分成百分比（其他3）"))
                {
                    msg.msg += "缺少“利润分成百分比（其他3）”列，";
                }
                if (!dt.Columns.Contains("其他1命名"))
                {
                    msg.msg += "缺少“其他1命名”列，";
                }
                if (!dt.Columns.Contains("其他2命名"))
                {
                    msg.msg += "缺少“其他2命名”列，";
                }
                if (!dt.Columns.Contains("其他3命名"))
                {
                    msg.msg += "缺少“其他3命名”列，";
                }
                if (!dt.Columns.Contains("一般中介商"))
                {
                    msg.msg += "缺少“一般中介商”列，";
                }
                if (!dt.Columns.Contains("城市代理中介商"))
                {
                    msg.msg += "缺少“城市代理中介商”列，";
                }
                //判断是否有商品条码重复
                DataView dv = new DataView(dt);
                if (dv.Count != dv.ToTable(true, "商品条码").Rows.Count)
                {
                    msg.msg += "表格中有重复的商品条码，";
                }
                if (msg.msg != null && msg.msg != "")
                {
                    return msg;
                }

                #endregion
                ArrayList al = new ArrayList();
                ArrayList delal = new ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    #region 检查项
                    string error="";
                    string supplierId = "", supplierCode = "",wid="";
                    //判断条码是否已经存在
                    string sqltm = "select id,goodsName,slt,supplierId,supplierCode,wid from t_goods_list where barcode = '" + dt.Rows[i]["商品条码"].ToString() + "'";
                    DataTable dttm = DatabaseOperationWeb.ExecuteSelectDS(sqltm, "TABLE").Tables[0];
                    if (dttm.Rows.Count > 0)
                    {
                        supplierId = dttm.Rows[0]["supplierId"].ToString();
                        supplierCode = dttm.Rows[0]["supplierCode"].ToString();
                        wid = dttm.Rows[0]["wid"].ToString();
                    }
                    else
                    {
                        error += (i + 1) + "行商品条码不存在，请核对\r\n";
                    }
                    //判断采购类型是否已经存在
                    string sqlt = "select platformId from t_base_platform where platformType = '" + dt.Rows[i]["采购类型"].ToString() + "'";
                    DataTable dtt = DatabaseOperationWeb.ExecuteSelectDS(sqlt, "TABLE").Tables[0];
                    if (dtt.Rows.Count == 0)
                    {
                        error += (i + 1) + "行采购类型不存在，请核对\r\n";
                    }
                    //判断渠道商是否已经存在
                    string sqlp = "select id,usercode from t_user_list where username = '" + dt.Rows[i]["采购商"].ToString() + "'";
                    DataTable dtp = DatabaseOperationWeb.ExecuteSelectDS(sqlp, "TABLE").Tables[0];
                    if (dtp.Rows.Count == 0)
                    {
                        error +=  (i + 1) + "行渠道商不存在，请核对\r\n";
                    }
                    //判断供应商是否已经存在,如果存在则覆盖商品的默认供应商
                    if (dt.Rows[i]["默认供应商"].ToString() != ""&& dt.Rows[i]["默认仓库"].ToString() != "")
                    {
                        string sqls = "select u.id,u.usercode,gw.wid " +
                            "from t_goods_warehouse gw,t_base_warehouse w,t_user_list u " +
                            "where gw.wid = w.id and gw.supplierid = u.id and u.username = '" + dt.Rows[i]["默认供应商"].ToString() + "'" +
                            " and w.wname = '" + dt.Rows[i]["默认仓库"].ToString() + "'" +
                            " and gw.barcode = '" + dt.Rows[i]["商品条码"].ToString() + "' ";
                        DataTable dts = DatabaseOperationWeb.ExecuteSelectDS(sqls, "TABLE").Tables[0];
                        if (dts.Rows.Count > 0)
                        {
                            supplierId = dts.Rows[0]["id"].ToString();
                            supplierCode = dts.Rows[0]["usercode"].ToString();
                            wid = dts.Rows[0]["wid"].ToString();
                        }
                    }
                    //如果默认供应商都不在，则报错
                    if (supplierId == "" || supplierCode == ""|| wid == "")
                    {
                        error += (i + 1) + "行默认供应商或默认仓库不存在，请核对\r\n";
                    }
                    //判断商品数量,商品申报单价是否为数字
                    double d1 = 0, d2 = 0,d3=0;
                    if (!double.TryParse(dt.Rows[i]["采购单价"].ToString(), out d1))
                    {
                        error +=  (i + 1) + "行采购单价填写错误，请核对\r\n";
                    }
                    //if (!double.TryParse(dt.Rows[i]["采购数量"].ToString(), out d2))
                    //{
                    //    error += (i + 1) + "行采购数量填写错误，请核对\r\n";
                    //}
                    if (!double.TryParse(dt.Rows[i]["零售价"].ToString(), out d3))
                    {
                        error += (i + 1) + "行零售价填写错误，请核对\r\n";
                    }
                    double p1 = 0, p2 = 0, p3 = 0, p4 = 0, p5 = 0, p6 = 0;
                    if (!double.TryParse(dt.Rows[i]["利润分成百分比（平台）"].ToString(), out p1))
                    {
                        error += (i + 1) + "行利润分成百分比（平台）填写错误，请核对\r\n";
                    }
                    if (!double.TryParse(dt.Rows[i]["利润分成百分比（代理）"].ToString(), out p2))
                    {
                        error += (i + 1) + "行利润分成百分比（代理）填写错误，请核对\r\n";
                    }
                    if (!double.TryParse(dt.Rows[i]["利润分成百分比（分销商）"].ToString(), out p3))
                    {
                        error +=  (i + 1) + "行利润分成百分比（分销商）填写错误，请核对\r\n";
                    }
                    if (!double.TryParse(dt.Rows[i]["利润分成百分比（其他1）"].ToString(), out p4))
                    {
                        error += (i + 1) + "行利润分成百分比（其他1）填写错误，请核对\r\n";
                    }
                    if (!double.TryParse(dt.Rows[i]["利润分成百分比（其他2）"].ToString(), out p5))
                    {
                        error +=(i + 1) + "行利润分成百分比（其他2）填写错误，请核对\r\n";
                    }
                    if (!double.TryParse(dt.Rows[i]["利润分成百分比（其他3）"].ToString(), out p6))
                    {
                        error +=  (i + 1) + "行利润分成百分比（其他3）填写错误，请核对\r\n";
                    }
                    if (error != "")
                    {
                        msg.msg += error;
                        continue;
                    }
                    //判断几个利润分成的和是否是100
                    if (p1+p2+p3+p4+p5+p6!=100)
                    {
                        msg.msg += (i + 1) + "行利润分成的和不是100，请核对\r\n";
                        continue;
                    }
                    #endregion

                    DataRow[] drs = distributorDt.Select(" barcode='" + dt.Rows[i]["商品条码"].ToString() + "' and usercode='" + dtp.Rows[0]["usercode"].ToString() + "' and platformId='" + dtt.Rows[0]["platformId"].ToString() + "'");
                    if (drs.Length>0)
                    {
                        string delSql = "update t_goods_distributor_price set " +
                            "pprice=" + d1 + ",rprice=" + d3 + ",goodsName='" + dt.Rows[i]["商品名称(中文)"].ToString() + "'," +
                            "profitPlatform=" + p1 + ",profitAgent=" + p2 + ",profitDealer=" + p3 + "," +
                            "profitOther1=" + p4 + ",profitOther2=" + p5 + ",profitOther3=" + p6 + " " +
                            "where id='" + drs[0]["id"].ToString() + "'";
                        delal.Add(delSql);
                    }
                    else
                    {
                        string sql = "insert into t_goods_distributor_price(" +
                            "usercode,goodsid,barcode,goodsName," +
                            "slt,platformId,pprice,rprice,pNum," +
                            "supplierid,wid,profitPlatform,profitAgent,profitDealer," +
                            "profitOther1,profitOther2,profitOther3," +
                            "profitOther1Name,profitOther2Name,profitOther3Name) " +
                            "values('" + dtp.Rows[0]["usercode"].ToString() + "','" + dttm.Rows[0]["id"].ToString() + "','" + dt.Rows[i]["商品条码"].ToString() + "','" + dttm.Rows[0]["goodsName"].ToString() + "'" +
                            ",'" + dttm.Rows[0]["slt"].ToString() + "','" + dtt.Rows[0]["platformId"].ToString() + "'," + d1 + "," + d3 + "," + d2 +
                            ",'" + supplierId + "','" + wid + "'," + p1 + "," + p2 + "," + p3 + "," + p4 + "," + p5 + "," + p5 +
                            ",'" + dt.Rows[i]["其他1命名"].ToString() + "','" + dt.Rows[i]["其他2命名"].ToString() + "','" + dt.Rows[i]["其他3命名"].ToString() + "')";
                        al.Add(sql);
                    }

                    
                }
                if (msg.msg!="")
                {
                    return msg;
                }
                if (DatabaseOperationWeb.ExecuteDML(delal))
                {
                    if (DatabaseOperationWeb.ExecuteDML(al))
                    {
                        msg.msg = "导入成功";
                        msg.type = "1";
                    }
                }
            }
            else
            {
                msg.msg = "导入数据为空";
            }
            return msg;
        }

        #endregion
    }
}
