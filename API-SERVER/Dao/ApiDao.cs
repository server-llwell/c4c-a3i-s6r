using API_SERVER.Buss;
using API_SERVER.Common;
using Com.ACBC.Framework.Database;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API_SERVER.Dao;
using StackExchange.Redis;
using System.Net;
using System.IO;
using System.Text;

namespace API_SERVER.Dao
{
    public class ApiDao
    {
        public ApiDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        /// <summary>
        /// 获取运营的结算列表
        /// </summary>
        /// <param name="searchBalanceParam"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ImportOrderResult importOrder(ImportOrderParam importOrderParam, string json)
        {
            ImportOrderResult importOrderResult = new ImportOrderResult();
            #region 检查项
            Dictionary<string, string> errorDictionary = new Dictionary<string, string>();
            List<OrderItem> OrderItemList = importOrderParam.OrderList;
            foreach (OrderItem orderItem in OrderItemList)
            {
                string error = "";
                //判断订单是否已经存在
                string sqlno = "select id from t_order_list where merchantOrderId = '" + orderItem.merchantOrderId + "' or  parentOrderId = '" + orderItem.merchantOrderId + "'";
                DataTable dtno = DatabaseOperationWeb.ExecuteSelectDS(sqlno, "TABLE").Tables[0];
                if (dtno.Rows.Count > 0)
                {
                    errorDictionary.Add(orderItem.merchantOrderId, "订单已存在!");
                }
                else
                {
                    //判断订单日期是否正确
                    DateTime dtime = DateTime.Now;
                    try
                    {
                        dtime = Convert.ToDateTime(orderItem.tradeTime);
                    }
                    catch
                    {
                        error += "创建时间日期格式填写错误,";
                    }
                    //判断地址是否正确
                    string sqlp = "select provinceid from t_base_provinces where province like '" + orderItem.addrProvince + "%'";
                    DataTable dtp = DatabaseOperationWeb.ExecuteSelectDS(sqlp, "TABLE").Tables[0];
                    if (dtp.Rows.Count > 0)
                    {
                        string provinceid = dtp.Rows[0][0].ToString();
                        string sqlc = "select cityid from t_base_cities  " +
                            "where city like '" + orderItem.addrCity + "%' and provinceid=" + provinceid + "";
                        DataTable dtc = DatabaseOperationWeb.ExecuteSelectDS(sqlc, "TABLE").Tables[0];
                        if (dtc.Rows.Count > 0)
                        {
                            string cityid = dtc.Rows[0][0].ToString();
                            string sqla = "select id from t_base_areas " +
                                "where area ='" + orderItem.addrDistrict + "' and cityid=" + cityid + "";
                            DataTable dta = DatabaseOperationWeb.ExecuteSelectDS(sqla, "TABLE").Tables[0];
                            if (dta.Rows.Count == 0)
                            {
                                error += "收货人区填写错误,";
                            }
                        }
                        else
                        {
                            error += "收货人市填写错误,";
                        }
                    }
                    else
                    {
                        error += "收货人省填写错误,";
                    }
                    //判断商品
                    foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
                    {
                        //判断条码是否已经存在
                        string sqltm = "select id,goodsName from t_goods_list where barcode = '" + orderGoodsItem.barCode + "'";
                        DataTable dttm = DatabaseOperationWeb.ExecuteSelectDS(sqltm, "TABLE").Tables[0];
                        if (dttm.Rows.Count == 0)
                        {
                            error += orderGoodsItem.barCode + ":商品条码不存在";
                        }
                        ////判断商品数量,商品申报单价是否为数字
                        //double d = 0;
                        //if (!double.TryParse(orderGoodsItem.quantity, out d))
                        //{
                        //    error += "序号为" + dt.Rows[i]["序号"].ToString() + "行商品数量填写错误，请核对\r\n";
                        //}
                        //if (!double.TryParse(dt.Rows[i]["商品申报单价"].ToString(), out d))
                        //{
                        //    error += "序号为" + dt.Rows[i]["序号"].ToString() + "行商品申报单价填写错误，请核对\r\n";
                        //}
                    }
                    errorDictionary.Add(orderItem.merchantOrderId, error);
                }
            }
            #endregion

            #region 改为调用公共方法处理 20181104 -韩明
            //#region 处理因仓库分单
            //List<OrderItem> newOrderItemList = new List<OrderItem>();
            //foreach (var orderItem in OrderItemList)
            //{
            //    if (errorDictionary[orderItem.merchantOrderId]!="")
            //    {
            //        continue;
            //    }

            //    string error = "";
            //    Dictionary<int, List<OrderGoodsItem>> myDictionary = new Dictionary<int, List<OrderGoodsItem>>();
            //    foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
            //    {
            //        string wsql = "select d.platformId,u.id as userId,u.platformCost,u.platformCostType,u.priceType," +
            //                      "d.pprice,d.profitPlatform,d.profitAgent,d.profitDealer,d.profitOther1," +
            //                      "d.profitOther1Name,d.profitOther2,d.profitOther2Name,d.profitOther3," +
            //                      "d.profitOther3Name,w.id as goodsWarehouseId,w.wid,w.wcode,w.goodsnum,w.inprice,bw.taxation,bw.taxation2," +
            //                      "bw.taxation2type,bw.taxation2line,bw.freight,w.suppliercode,g.NW,g.slt " +
            //                      "from t_goods_distributor_price d ,t_goods_warehouse w,t_base_warehouse bw," +
            //                      "t_goods_list g,t_user_list u   " +
            //                      "where u.usercode = d.usercode and g.barcode = d.barcode and w.wid = bw.id " +
            //                      "and d.barcode = w.barcode and w.supplierid = d.supplierid and d.wid = bw.id " +
            //                      "and d.usercode = '" + importOrderParam.userCode + "' " +
            //                      "and d.barcode = '" + orderGoodsItem.barCode + "' and w.goodsnum >=" + orderGoodsItem.quantity +
            //                      " order by w.goodsnum asc";
            //        DataTable wdt = DatabaseOperationWeb.ExecuteSelectDS(wsql, "TABLE").Tables[0];
            //        int wid = 0;
            //        if (wdt.Rows.Count == 1)
            //        {
            //            wid = Convert.ToInt16(wdt.Rows[0]["wid"]);
            //            orderGoodsItem.dr = wdt.Rows[0];
            //        }
            //        else if (wdt.Rows.Count > 1)
            //        {
            //            wid = Convert.ToInt16(wdt.Rows[0]["wid"]);
            //            orderGoodsItem.dr = wdt.Rows[0];
            //            for (int i = 0; i < wdt.Rows.Count; i++)
            //            {
            //                if (myDictionary.ContainsKey(Convert.ToInt16(wdt.Rows[i]["wid"])))
            //                {
            //                    wid = Convert.ToInt16(wdt.Rows[i]["wid"]);
            //                    orderGoodsItem.dr = wdt.Rows[i];
            //                    break;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            error += orderGoodsItem.barCode + "没找到对应默认供货信息，";
            //            continue;
            //        }
            //        if (!myDictionary.ContainsKey(wid))
            //        {
            //            myDictionary.Add(wid, new List<OrderGoodsItem>());
            //        }
            //        myDictionary[wid].Add(orderGoodsItem);
            //    }
            //    if (error!="")
            //    {
            //        errorDictionary[orderItem.merchantOrderId] += error;
            //        continue;
            //    }
            //    if (myDictionary.Count() > 1)//一个订单有一个以上仓库和供应商的商品
            //    {
            //        int num = 0;
            //        foreach (var kvp in myDictionary)
            //        {
            //            if (num == 0)//第一个仓库的订单修改部分字段
            //            {
            //                orderItem.parentOrderId = orderItem.merchantOrderId;
            //                orderItem.merchantOrderId += kvp.Key;
            //                orderItem.warehouseId = kvp.Key.ToString();
            //                orderItem.OrderGoods = new List<OrderGoodsItem>();
            //                double tradeAmount = 0;
            //                foreach (var item in kvp.Value)
            //                {
            //                    tradeAmount += Convert.ToDouble(item.skuUnitPrice) * Convert.ToDouble(item.quantity);
            //                    orderItem.OrderGoods.Add(item);
            //                }
            //                orderItem.tradeAmount = tradeAmount.ToString();
            //                newOrderItemList.Add(orderItem);
            //            }
            //            else//其他仓库的订单新建子订单
            //            {
            //                OrderItem orderItemNew = new OrderItem();
            //                orderItemNew.parentOrderId = orderItem.parentOrderId;
            //                orderItemNew.merchantOrderId = orderItem.parentOrderId + kvp.Key;
            //                orderItemNew.tradeTime = orderItem.tradeTime;
            //                orderItemNew.consigneeName = orderItem.consigneeName;
            //                orderItemNew.consigneeMobile = orderItem.consigneeMobile;
            //                orderItemNew.idNumber = orderItem.idNumber;
            //                orderItemNew.addrCountry = orderItem.addrCountry;
            //                orderItemNew.addrProvince = orderItem.addrProvince;
            //                orderItemNew.addrCity = orderItem.addrCity;
            //                orderItemNew.addrDistrict = orderItem.addrDistrict;
            //                orderItemNew.addrDetail = orderItem.addrDetail;
            //                orderItemNew.consignorName = orderItem.consignorName;
            //                orderItemNew.consignorMobile = orderItem.consignorMobile;
            //                orderItemNew.consignorAddr = orderItem.consignorAddr;
            //                orderItemNew.OrderGoods = new List<OrderGoodsItem>();
            //                double tradeAmount = 0;
            //                foreach (var item in kvp.Value)
            //                {
            //                    tradeAmount += Convert.ToDouble(item.skuUnitPrice) * Convert.ToDouble(item.quantity);
            //                    orderItemNew.OrderGoods.Add(item);
            //                }
            //                orderItemNew.tradeAmount = tradeAmount.ToString();
            //                newOrderItemList.Add(orderItemNew);
            //            }
            //            num++;
            //        }
            //    }
            //    else
            //    {
            //        double tradeAmount = 0;
            //        foreach (OrderGoodsItem orderGoodsItem in orderItem.OrderGoods)
            //        {
            //            tradeAmount += Convert.ToDouble(orderGoodsItem.skuUnitPrice) * Convert.ToDouble(orderGoodsItem.quantity);
            //        }
            //        orderItem.parentOrderId = orderItem.merchantOrderId;
            //        orderItem.tradeAmount = tradeAmount.ToString();
            //        newOrderItemList.Add(orderItem);
            //    }
            //}
            //#endregion

            //#region 价格分拆

            //ArrayList al = new ArrayList();
            //ArrayList goodsNumAl = new ArrayList();
            //foreach (var orderItem in newOrderItemList)
            //{
            //    if (errorDictionary[orderItem.parentOrderId] != "")
            //    {
            //        continue;
            //    }
            //    double freight = 0, tradeAmount = 1;
            //    double.TryParse(orderItem.OrderGoods[0].dr["freight"].ToString(), out freight);
            //    double.TryParse(orderItem.tradeAmount, out tradeAmount);
            //    orderItem.freight = Math.Round(freight, 2);
            //    orderItem.platformId = orderItem.OrderGoods[0].dr["platformId"].ToString();
            //    orderItem.warehouseId = orderItem.OrderGoods[0].dr["wid"].ToString();
            //    orderItem.warehouseCode = orderItem.OrderGoods[0].dr["wcode"].ToString();
            //    orderItem.supplier = orderItem.OrderGoods[0].dr["suppliercode"].ToString();
            //    orderItem.purchaseId = orderItem.OrderGoods[0].dr["userId"].ToString();
            //    orderItem.purchase = importOrderParam.userCode;
            //    double fr = Math.Round(orderItem.freight / tradeAmount, 4);
            //    for (int i = 0; i < orderItem.OrderGoods.Count; i++)
            //    {
            //        OrderGoodsItem orderGoodsItem = orderItem.OrderGoods[i];
            //        //处理运费
            //        //if (i== orderItem.OrderGoods.Count-1)
            //        //{
            //        //    orderGoodsItem.waybillPrice = freight;
            //        //}
            //        //else
            //        //{
            //        //    orderGoodsItem.waybillPrice = Math.Round(fr * orderGoodsItem.totalPrice,2);
            //        //    freight -= orderGoodsItem.waybillPrice;
            //        //}
            //        //从运费平摊修改为运费都为全部运费。
            //        orderGoodsItem.waybillPrice = freight;


            //        //处理供货价和销售价和供货商code
            //        orderGoodsItem.supplyPrice = Math.Round(Convert.ToDouble(orderGoodsItem.dr["inprice"]), 2);
            //        orderGoodsItem.purchasePrice = Math.Round(Convert.ToDouble(orderGoodsItem.dr["pprice"]), 2);
            //        orderGoodsItem.suppliercode = orderGoodsItem.dr["suppliercode"].ToString();
            //        orderGoodsItem.slt = orderGoodsItem.dr["slt"].ToString();

            //        string goodsWarehouseId = orderGoodsItem.dr["goodsWarehouseId"].ToString();//库存id
            //                                                                                   //处理税
            //        double taxation = 0;
            //        double.TryParse(orderGoodsItem.dr["taxation"].ToString(), out taxation);
            //        if (taxation > 0)
            //        {
            //            double taxation2 = 0, taxation2type = 0, taxation2line = 0, nw = 0;
            //            double.TryParse(orderGoodsItem.dr["taxation2"].ToString(), out taxation2);
            //            double.TryParse(orderGoodsItem.dr["taxation2type"].ToString(), out taxation2type);
            //            double.TryParse(orderGoodsItem.dr["taxation2line"].ToString(), out taxation2line);
            //            double.TryParse(orderGoodsItem.dr["NW"].ToString(), out nw);
            //            if (taxation2 == 0)
            //            {
            //                orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
            //            }
            //            else
            //            {
            //                if (taxation2type == 1)//按总价提档
            //                {
            //                    if (orderGoodsItem.skuUnitPrice > taxation2line)//价格过线
            //                    {
            //                        orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation2 / 100;
            //                    }
            //                    else//价格没过线
            //                    {
            //                        orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
            //                    }
            //                }
            //                else if (taxation2type == 2)//按元/克提档
            //                {
            //                    if (nw == 0)//如果没有净重，按默认税档
            //                    {
            //                        orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
            //                    }
            //                    else
            //                    {
            //                        if (orderGoodsItem.skuUnitPrice / (nw * 1000) > taxation2line)//价格过线
            //                        {
            //                            orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation2 / 100;
            //                        }
            //                        else//价格没过线
            //                        {
            //                            orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
            //                        }
            //                    }
            //                    //还要考虑面膜的问题
            //                }
            //                else//都不是按初始税率走
            //                {
            //                    orderGoodsItem.tax = orderGoodsItem.totalPrice * taxation / 100;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            orderGoodsItem.tax = 0;
            //        }
            //        orderGoodsItem.tax = Math.Round(orderGoodsItem.tax, 2);
            //        //处理平台提点
            //        orderGoodsItem.platformPrice = 0;
            //        double platformCost = 0;
            //        double.TryParse(orderGoodsItem.dr["platformCost"].ToString(), out platformCost);
            //        if (platformCost > 0)
            //        {
            //            if (orderGoodsItem.dr["platformCostType"].ToString() == "1")//进价计算
            //            {
            //                orderGoodsItem.platformPrice = orderGoodsItem.supplyPrice * orderGoodsItem.quantity * platformCost / 100;
            //            }
            //            else if (orderGoodsItem.dr["platformCostType"].ToString() == "2")//售价计算
            //            {
            //                if (orderGoodsItem.dr["priceType"].ToString() == "1")//按订单售价计算
            //                {
            //                    orderGoodsItem.platformPrice = orderGoodsItem.totalPrice * platformCost / 100;
            //                }
            //                else if (orderGoodsItem.dr["priceType"].ToString() == "2")//按供货价计算
            //                {
            //                    orderGoodsItem.platformPrice = orderGoodsItem.purchasePrice * orderGoodsItem.quantity * platformCost / 100;
            //                }
            //            }
            //        }
            //        orderGoodsItem.platformPrice = Math.Round(orderGoodsItem.platformPrice, 2);
            //        //处理利润
            //        //利润为供货价-进价-提点-运费分成-税
            //        double profit = orderGoodsItem.purchasePrice * orderGoodsItem.quantity
            //            - orderGoodsItem.supplyPrice * orderGoodsItem.quantity
            //            - orderGoodsItem.platformPrice
            //            - orderGoodsItem.waybillPrice
            //            - orderGoodsItem.tax;
            //        double profitAgent = 0, profitDealer = 0, profitOther1 = 0, profitOther2 = 0, profitOther3 = 0;
            //        double.TryParse(orderGoodsItem.dr["profitAgent"].ToString(), out profitAgent);
            //        double.TryParse(orderGoodsItem.dr["profitDealer"].ToString(), out profitDealer);
            //        double.TryParse(orderGoodsItem.dr["profitOther1"].ToString(), out profitOther1);
            //        double.TryParse(orderGoodsItem.dr["profitOther2"].ToString(), out profitOther2);
            //        double.TryParse(orderGoodsItem.dr["profitOther3"].ToString(), out profitOther3);
            //        orderGoodsItem.profitAgent = Math.Round(profit * profitAgent / 100, 2);
            //        orderGoodsItem.profitDealer = Math.Round(profit * profitDealer / 100, 2);
            //        orderGoodsItem.profitOther1 = Math.Round(profit * profitOther1 / 100, 2);
            //        orderGoodsItem.profitOther2 = Math.Round(profit * profitOther2 / 100, 2);
            //        orderGoodsItem.profitOther3 = Math.Round(profit * profitOther3 / 100, 2);
            //        orderGoodsItem.profitPlatform = Math.Round(profit - orderGoodsItem.profitAgent
            //                                        - orderGoodsItem.profitDealer - orderGoodsItem.profitOther1
            //                                        - orderGoodsItem.profitOther2 - orderGoodsItem.profitOther3, 2);
            //        orderGoodsItem.other1Name = orderGoodsItem.dr["profitOther1Name"].ToString();
            //        orderGoodsItem.other2Name = orderGoodsItem.dr["profitOther2Name"].ToString();
            //        orderGoodsItem.other3Name = orderGoodsItem.dr["profitOther3Name"].ToString();
            //        string sqlgoods = "insert into t_order_goods(merchantOrderId,barCode,slt,skuUnitPrice," +
            //                      "quantity,skuBillName,batchNo,goodsName," +
            //                      "api,fqSkuID,sendType,status," +
            //                      "suppliercode,supplyPrice,purchasePrice,waybill," +
            //                      "waybillPrice,tax,platformPrice,profitPlatform," +
            //                      "profitAgent,profitDealer,profitOther1,other1Name," +
            //                      "profitOther2,other2Name,profitOther3,other3Name) " +
            //                      "values('" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "','" + orderGoodsItem.slt + "','" + orderGoodsItem.skuUnitPrice + "'" +
            //                      ",'" + orderGoodsItem.quantity + "','" + orderGoodsItem.skuBillName + "','','" + orderGoodsItem.skuBillName + "'" +
            //                      ",'','','','0'" +
            //                      ",'" + orderGoodsItem.suppliercode + "','" + orderGoodsItem.supplyPrice + "','" + orderGoodsItem.purchasePrice + "',''" +
            //                      ",'" + orderGoodsItem.waybillPrice + "','" + orderGoodsItem.tax + "','" + orderGoodsItem.platformPrice + "','" + orderGoodsItem.profitPlatform + "'" +
            //                      ",'" + orderGoodsItem.profitAgent + "','" + orderGoodsItem.profitDealer + "','" + orderGoodsItem.profitOther1 + "','" + orderGoodsItem.other1Name + "'" +
            //                      ",'" + orderGoodsItem.profitOther2 + "','" + orderGoodsItem.other2Name + "','" + orderGoodsItem.profitOther3 + "','" + orderGoodsItem.other3Name + "'" +
            //                      ")";
            //        al.Add(sqlgoods);
            //        string upsql = "update t_goods_warehouse set goodsnum = goodsnum-" + orderGoodsItem.quantity + " where id = " + goodsWarehouseId;
            //        goodsNumAl.Add(upsql);
            //        string logsql = "insert into t_log_goodsnum(inputType,createtime,wid,wcode,orderid,barcode,goodsnum,state) " +
            //                        "values('',now(),'" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "'," +
            //                        "'" + orderItem.merchantOrderId + "','" + orderGoodsItem.barCode + "'," +
            //                        "" + orderGoodsItem.quantity + ",'" + orderItem.status + "')";
            //        goodsNumAl.Add(logsql);
            //    }
            //    string sqlorder = "insert into t_order_list(warehouseId,warehouseCode,customerCode,actionType," +
            //        "orderType,serviceType,parentOrderId,merchantOrderId," +
            //        "payType,payNo,tradeTime," +
            //        "tradeAmount,goodsTotalAmount,consigneeName,consigneeMobile," +
            //        "addrCountry,addrProvince,addrCity,addrDistrict," +
            //        "addrDetail,zipCode,idType,idNumber," +
            //        "idFountImgUrl,idBackImgUrl,status,purchaserCode," +
            //        "purchaserId,distributionCode,apitype,waybillno," +
            //        "expressId,inputTime,fqID," +
            //        "operate_status,sendapi,platformId,consignorName," +
            //        "consignorMobile,consignorAddr,batchid,outNo,waybillOutNo," +
            //        "accountsStatus,accountsNo,prePayId,ifPrint,printNo) " +
            //        "values('" + orderItem.warehouseId + "','" + orderItem.warehouseCode + "','" + orderItem.supplier + "',''" +
            //        ",'','','" + orderItem.parentOrderId + "','" + orderItem.merchantOrderId + "'" +
            //        ",'','','" + orderItem.tradeTime + "'" +
            //        "," + orderItem.tradeAmount + ",'" + orderItem.tradeAmount + "','" + orderItem.consigneeName + "','" + orderItem.consigneeMobile + "'" +
            //        ",'" + orderItem.addrCountry + "','" + orderItem.addrProvince + "','" + orderItem.addrCity + "','" + orderItem.addrDistrict + "'" +
            //        ",'" + orderItem.addrDetail + "','','1','" + orderItem.idNumber + "'" +
            //        ",'','','1','" + orderItem.purchase + "'" +
            //        ",'" + orderItem.purchaseId + "','','',''" +
            //        ",'',now(),''" +
            //        ",'0','','" + orderItem.platformId + "','" + orderItem.consignorName + "'" +
            //        ",'" + orderItem.consignorMobile + "','" + orderItem.consignorAddr + "','','',''" +
            //        ",'0','','','0','') ";
            //    al.Add(sqlorder);
            //}

            //#endregion
            //if (DatabaseOperationWeb.ExecuteDML(al))
            //{
            //    DatabaseOperationWeb.ExecuteDML(goodsNumAl);
            //    importOrderResult.code = "1";
            //    importOrderResult.content = new List<ImportOrderResultItem>();
            //    int errNum = 0;
            //    foreach (var kvp in errorDictionary)
            //    {
            //        ImportOrderResultItem importOrderResultItem = new ImportOrderResultItem();
            //        importOrderResultItem.merchantOrderId = kvp.Key;
            //        if (kvp.Value != "")
            //        {
            //            importOrderResultItem.code = "0";
            //            importOrderResultItem.message = kvp.Value;
            //            errNum++;
            //        }
            //        else
            //        {
            //            importOrderResultItem.code = "1";
            //            importOrderResultItem.message = "订单导入成功";
            //        }
            //        importOrderResult.content.Add(importOrderResultItem);
            //    }
            //    if (errNum > 0)
            //    {
            //        importOrderResult.message = "有" + errNum + "个订单错误";
            //    }
            //    else
            //    {
            //        importOrderResult.message = "订单全部导入成功";
            //    }
            //}
            //else
            //{
            //    importOrderResult.message = "订单导入错误，请联系技术人员！";
            //}
            #endregion

            OrderDao orderDao = new OrderDao();

            MsgResult msg = orderDao.orderHandle(OrderItemList, importOrderParam.userCode, "1", ref errorDictionary);
            if (msg.type == "1")
            {
                importOrderResult.code = "1";
                importOrderResult.content = new List<ImportOrderResultItem>();
                int errNum = 0;
                foreach (var kvp in errorDictionary)
                {
                    ImportOrderResultItem importOrderResultItem = new ImportOrderResultItem();
                    importOrderResultItem.merchantOrderId = kvp.Key;
                    if (kvp.Value != "")
                    {
                        importOrderResultItem.code = "0";
                        importOrderResultItem.message = kvp.Value;
                        errNum++;
                    }
                    else
                    {
                        importOrderResultItem.code = "1";
                        importOrderResultItem.message = "订单导入成功";
                    }
                    importOrderResult.content.Add(importOrderResultItem);
                }
                if (errNum > 0)
                {
                    importOrderResult.message = "有" + errNum + "个订单错误";
                }
                else
                {
                    importOrderResult.message = "订单全部导入成功";
                }
            }
            else
            {
                importOrderResult.message = "订单导入错误，请联系技术人员！";
            }
            string json1 = JsonConvert.SerializeObject(importOrderResult);
            string logsql1 = "insert into t_log_api(apiType,inputTime,inputValue,resultValue) " +
                "values('importOrder',now(),'" + json.Replace("\r\n", "").Replace(" ", "") + "','" + json1 + "')";
            DatabaseOperationWeb.ExecuteDML(logsql1);
            return importOrderResult;
        }

        public GoodsListResult getGoodsList(ImportOrderParam importOrderParam)
        {
            GoodsListResult goodsListResult = new GoodsListResult();
            goodsListResult.goodsList = new List<GoodsListResultItem>();
            string sql = "select g.barcode,g.brand,(select `name` from t_goods_category where id = g.catelog1 ) as catelog1," +
                         "(select `name` from t_goods_category where id=g.catelog2) as catelog2,g.goodsName, g.slt,g.thumb,g.content," +
                         "g.country,g.model,g.ifXG,g.ifRB,g.ifHW,g.ifbs,g.ifmy,d.rprice,d.pNum,d.supplierid,d.wid,p.platformType,g.GW,w.taxation,w.businessType,w.customClearType,w.taxType " +
                         "from t_goods_list g ,t_goods_distributor_price d,t_base_platform p,t_base_warehouse w " +
                         "where w.id = d.wid and g.barcode = d.barcode and d.platformId = p.platformId " +
                         "and d.usercode = '" + importOrderParam.userCode + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    GoodsListResultItem goodsListResultItem = new GoodsListResultItem();
                    goodsListResultItem.barcode = dt.Rows[i]["barcode"].ToString();
                    goodsListResultItem.brand = dt.Rows[i]["brand"].ToString();
                    goodsListResultItem.goodsName = dt.Rows[i]["goodsName"].ToString();
                    goodsListResultItem.catelog1 = dt.Rows[i]["catelog1"].ToString();
                    goodsListResultItem.catelog2 = dt.Rows[i]["catelog2"].ToString();
                    goodsListResultItem.slt = dt.Rows[i]["slt"].ToString();
                    goodsListResultItem.country = dt.Rows[i]["country"].ToString();
                    goodsListResultItem.model = dt.Rows[i]["model"].ToString();
                    goodsListResultItem.sendType = dt.Rows[i]["platformType"].ToString();
                    goodsListResultItem.price = Convert.ToDouble(dt.Rows[i]["rprice"]);
                    goodsListResultItem.weight = Convert.ToDouble(dt.Rows[i]["GW"]);
                    goodsListResultItem.taxRate = Convert.ToDouble(dt.Rows[i]["taxation"]) / 100;
                    goodsListResultItem.businessType = dt.Rows[i]["businessType"].ToString();
                    goodsListResultItem.customClearType = dt.Rows[i]["customClearType"].ToString();
                    goodsListResultItem.taxType = dt.Rows[i]["taxType"].ToString();

                    if (dt.Rows[i]["thumb"].ToString() != "")
                    {
                        goodsListResultItem.thumb = dt.Rows[i]["thumb"].ToString().Split(",");
                    }
                    if (dt.Rows[i]["content"].ToString() != "")
                    {
                        goodsListResultItem.content = dt.Rows[i]["content"].ToString().Split(",");
                    }

                    string sql1 = "select sum(goodsnum) goodsnum from t_goods_warehouse w " +
                              "where w.wid = '" + dt.Rows[i]["wid"].ToString() + "' " +
                              "and w.supplierid = '" + dt.Rows[i]["supplierid"].ToString() + "' " +
                              "and w.barcode ='" + dt.Rows[i]["barcode"].ToString() + "'";
                    DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "TABLE").Tables[0];
                    if (dt1.Rows.Count > 0)
                    {
                        goodsListResultItem.stock = dt1.Rows[0][0].ToString();
                    }
                    else
                    {
                        goodsListResultItem.stock = "0";
                    }
                    goodsListResult.goodsList.Add(goodsListResultItem);
                }
                goodsListResult.code = "1";
            }
            else
            {
                goodsListResult.message = "账号下没有商品";
            }
            return goodsListResult;
        }


        public MsgResult bindingWXAPP(WXAPPParam wXAPPParam)
        {
            MsgResult msgResult = new MsgResult();
            try
            {
                string sql = "select * from t_wxapp_pagent_member " +
                    "where appId='" + wXAPPParam.appId + "' " +
                    "and openId='" + wXAPPParam.openId + "' " +
                    "and pagentCode='" + wXAPPParam.pagentCode + "'";
                DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
                if (dt.Rows.Count==0)
                {
                    string sqlx = "select * from t_user_list " +
                    "where openId='" + wXAPPParam.openId + "' ";
                    DataTable dtx = DatabaseOperationWeb.ExecuteSelectDS(sqlx, "TABLE").Tables[0];
                    if (dtx.Rows.Count>0)
                    {
                        msgResult.msg = "已经是分销商！";
                        return msgResult;
                    }
                    string purchasersCode="", supplierCode= "";
                    string sql1 = "select * from t_wxapp_app where appId='" + wXAPPParam.appId + "'";
                    DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "TABLE").Tables[0];
                    if (dt1.Rows.Count > 0)
                    {
                        purchasersCode = dt1.Rows[0]["purchasersCode"].ToString();
                    }
                    else
                    {
                        msgResult.msg = "小程序没有绑定采购商账号！";
                        return msgResult;
                    }
                    string sql2 = "select * from t_wxapp_pagent where pagentCode='" + wXAPPParam.pagentCode + "'";
                    DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "TABLE").Tables[0];
                    if (dt2.Rows.Count > 0)
                    {
                        supplierCode = dt2.Rows[0]["supplierCode"].ToString();
                    }
                    else
                    {
                        msgResult.msg = "中介没有绑定供应商账号！";
                        return msgResult;
                    }
                    
                    string insql = "insert into t_wxapp_pagent_member(appId,openId,pagentCode,purchasersCode,supplierCode,createTime) " +
                    "values('" + wXAPPParam.appId + "','" + wXAPPParam.openId + "','" + wXAPPParam.pagentCode + "','" + purchasersCode + "','" + supplierCode + "',now())";
                    if (DatabaseOperationWeb.ExecuteDML(insql))
                    {
                        msgResult.msg = "绑定成功";
                        msgResult.type = "1";
                    }
                }
                else
                {
                    msgResult.msg = "已存在绑定数据";
                    msgResult.type = "1";
                }
                
            }
            catch (Exception ex)
            {
                msgResult.msg = "数据库操作失败，请联系管理员！";
            }
            return msgResult;
        }
        public MsgResult bindingWXB2B(WXAPPParam wXAPPParam)
        {
            MsgResult msgResult = new MsgResult();
            try
            {
                string purchasersCode = "";
                string sql1 = "select * from t_wxapp_app where appId='" + wXAPPParam.appId + "'";
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "TABLE").Tables[0];
                if (dt1.Rows.Count > 0)
                {
                    purchasersCode = dt1.Rows[0]["purchasersCode"].ToString();
                }
                else
                {
                    msgResult.msg = "小程序没有绑定采购商账号！";
                    return msgResult;
                }


                string sql = "select * from t_user_list " +
                    "where openId='" + wXAPPParam.openId + "'";
                DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
                if (dt.Rows.Count == 0)
                {
                    string sql2 = "select nextval('BBCAGENT')";
                    DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "TABLE").Tables[0];
                    string userCode = "BBC" + dt2.Rows[0][0].ToString();
                    string userName = "分销商"+ dt2.Rows[0][0].ToString();
                    string insql = "insert into t_user_list(usercode,pwd,usertype,openId," +
                        "username,createtime,verifycode,flag,ofAgent) " +
                    "values('" + userCode + "','e10adc3949ba59abbe56e057f20f883e','4','" + wXAPPParam.openId + "'," +
                    "'" + userName + "',now(),'4','1','"+ purchasersCode + "')";
                    if (DatabaseOperationWeb.ExecuteDML(insql))
                    {
                        string sql3 = "select id from t_user_list where usercode = '"+ userCode + "'";
                        DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "TABLE").Tables[0];
                        if (dt3.Rows.Count>0)
                        {
                            ArrayList al = new ArrayList();
                            string insql1 = "insert into t_user_role(user_id,role_id) values("+dt3.Rows[0][0].ToString()+",9)";
                            al.Add(insql1);
                            string insql2 = "insert into t_wxapp_pagent(pagentCode,supplierCode,flag) " +
                                "values('" + wXAPPParam.openId + "','" + userCode + "',9)";
                            al.Add(insql2);
                            string desql = "delete from t_wxapp_pagent_member where openId ='" + wXAPPParam.openId + "' ";
                            al.Add(desql);
                            if (DatabaseOperationWeb.ExecuteDML(al))
                            {
                                msgResult.msg = "绑定成功";
                                msgResult.type = "1";
                            }
                        }
                    }
                }
                else
                {
                    msgResult.msg = "已存在绑定数据";
                    msgResult.type = "1";
                }

            }
            catch (Exception ex)
            {
                msgResult.msg = "数据库操作失败，请联系管理员！";
            }
            return msgResult;
        }
        public MsgResult getTypeByOpenId(WXAPPParam wXAPPParam)
        {
            MsgResult msgResult = new MsgResult();
            string sql = "select * from t_user_list where openId='" + wXAPPParam.openId + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            if (dt.Rows.Count > 0)
            {
                msgResult.msg = "2";
                msgResult.type = "1";
            }
            else
            {
                msgResult.msg = "3";
                msgResult.type = "1";
            }
            return msgResult;
        }
        public ProfitData getProfitByOpenId(WXAPPParam wXAPPParam)
        {
            ProfitData profitData = new ProfitData();
            ProfitItem profitItem = new ProfitItem();
            try
            {
                string sql = "select * from t_user_list where openId='" + wXAPPParam.openId + "'";
                DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
                if (dt.Rows.Count > 0)
                {
                    string userCode = dt.Rows[0]["usercode"].ToString();
                    //获取账户余额
                    string sql1 = "SELECT sum(price) from t_account_list where usercode = '" + userCode + "'";
                    DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "TABLE").Tables[0];
                    if (dt1.Rows.Count > 0)
                    {
                        double.TryParse(dt1.Rows[0][0].ToString(), out profitItem.accountMoney);
                    }
                    //获取收益明细
                    string sql2 = "SELECT SUM(G.profitDealer) ACTUAL_AMOUNT " +
                        "FROM T_ORDER_LIST O,T_ORDER_GOODS G " +
                        "WHERE O.MERCHANTORDERID = G.MERCHANTORDERID AND O.distributionCode = '" + userCode + "' " +
                          "and DATE_FORMAT(TRADETIME,'%Y-%m')='" + DateTime.Now.ToString("yyyy-MM") + "' " +
                        "GROUP BY DATE_FORMAT(TRADETIME,'%Y-%m') ";
                    DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "TABLE").Tables[0];
                    if (dt2.Rows.Count > 0)
                    {
                        double.TryParse(dt2.Rows[0][0].ToString(), out profitItem.monthProfit);
                    }

                    //获取上月结算
                    string sql3 = "select sum(price) from t_account_list " +
                        "where usercode = '" + userCode + "' and accountType='1' " +
                          "and DATE_FORMAT(dateTo,'%Y-%m')='" + DateTime.Now.AddMonths(-1).ToString("yyyy-MM") + "'";
                    DataTable dt3 = DatabaseOperationWeb.ExecuteSelectDS(sql3, "TABLE").Tables[0];
                    if (dt3.Rows.Count > 0)
                    {
                        double.TryParse(dt3.Rows[0][0].ToString(), out profitItem.lastMonthProfit);
                    }
                    //收益明细
                    string sql4 = "SELECT DATE_FORMAT(TRADETIME,'%Y-%m') MONTH,SUM(G.profitDealer) ACTUAL_AMOUNT " +
                        "FROM T_ORDER_LIST O,T_ORDER_GOODS G " +
                        "WHERE O.MERCHANTORDERID = G.MERCHANTORDERID AND O.distributionCode = '" + userCode + "' " +
                        "GROUP BY DATE_FORMAT(TRADETIME,'%Y-%m') " +
                        "ORDER BY MONTH DESC";
                    DataTable dt4 = DatabaseOperationWeb.ExecuteSelectDS(sql4, "TABLE").Tables[0];
                    if (dt4.Rows.Count > 0)
                    {
                        string sql5 = "select g.goodsName,g.profitDealer,o.tradeTime,o.tradeAmount,DATE_FORMAT(o.tradeTime,'%Y-%m') date1 " +
                                "from t_order_list o ,t_order_goods g " +
                                "where o.merchantOrderId = g.merchantOrderId and o.distributionCode = '" + userCode + "' " +
                                "order by tradeTime desc";
                        DataTable dt5 = DatabaseOperationWeb.ExecuteSelectDS(sql5, "TABLE").Tables[0];
                        for (int i = 0; i < dt4.Rows.Count; i++)
                        {
                            MonthGoodsProfit monthGoodsProfit = new MonthGoodsProfit();
                            monthGoodsProfit.month = dt4.Rows[i]["MONTH"].ToString();
                            monthGoodsProfit.monthTotal = dt4.Rows[i]["ACTUAL_AMOUNT"].ToString();
                            DataRow[] drs = dt5.Select("date1='" + dt4.Rows[i]["MONTH"].ToString() + "'");
                            for (int j = 0; j < drs.Length; j++)
                            {
                                GoodsProfit goodsProfit = new GoodsProfit();
                                goodsProfit.goodsName = drs[j]["goodsName"].ToString();
                                goodsProfit.profit = drs[j]["profitDealer"].ToString();
                                goodsProfit.tradeTime = drs[j]["tradeTime"].ToString();
                                goodsProfit.accountTime = Convert.ToDateTime(drs[j]["tradeTime"].ToString()).AddMonths(1).ToString("yyyy-MM-01");
                                goodsProfit.tradeAmount = drs[j]["tradeAmount"].ToString();
                                monthGoodsProfit.goodsProfitList.Add(goodsProfit);
                            }
                            profitItem.monthGoodsProfitList.Add(monthGoodsProfit);
                        }
                    }
                    //结算记录
                    string sql6 = "select i.orderId,l.createTime,i.price " +
                        "from t_account_list l,t_account_info i " +
                        "where l.accountCode = i.accountCode and l.usercode = '" + userCode + "' " +
                        "order by i.id desc";
                    DataTable dt6 = DatabaseOperationWeb.ExecuteSelectDS(sql6, "TABLE").Tables[0];
                    for (int i = 0; i < dt6.Rows.Count; i++)
                    {
                        AccountInfo accountInfo = new AccountInfo();
                        accountInfo.merchantOrderId = dt6.Rows[i]["orderId"].ToString();
                        accountInfo.accountTime = dt6.Rows[i]["createTime"].ToString();
                        accountInfo.profit = dt6.Rows[i]["price"].ToString();
                        profitItem.accountInfoList.Add(accountInfo);
                    }
                }
            }
            catch 
            {
                profitData.success = false;
                profitData.errorMessage = "数据处理错误";
            }
            profitData.data = profitItem;
            return profitData;
        }
        public MsgResult getQrcode(WXAPPParam wXAPPParam)
        {
            MsgResult msgResult = new MsgResult();
            try
            {
                string secret = "";
                string sql1 = "select * from t_wxapp_app where appId='" + wXAPPParam.appId + "'";
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "TABLE").Tables[0];
                if (dt1.Rows.Count > 0)
                {
                    secret = dt1.Rows[0]["secret"].ToString();
                    string token = Request_Url(wXAPPParam.appId,secret);
                    Demo demo = new Demo
                    {
                        path = "pages/index/index?agent="+ wXAPPParam.openId,
                        width = 1000,
                        is_hyaline = false,
                    };

                    string body= JsonConvert.SerializeObject(demo);
                    byte[] byte1 = PostMoths("https://api.weixin.qq.com/wxa/getwxacode?access_token="+token, body);
                    FileManager fileManager = new FileManager();
                    fileManager.saveImgByByte(byte1, wXAPPParam.appId+wXAPPParam.openId + ".jpg");
                    fileManager.updateFileToOSS(wXAPPParam.appId + wXAPPParam.openId + ".jpg", Global.OssDirOrder, wXAPPParam.appId + wXAPPParam.openId + ".jpg");
                    msgResult.msg = Global.OssUrl + Global.OssDirOrder + wXAPPParam.appId + wXAPPParam.openId + ".jpg";
                    msgResult.type = "1";
                }
            }
            catch (Exception ex)
            {
                msgResult.msg = "数据库操作失败，请联系管理员！";
            }
            return msgResult;
        }
        //获取AccessToken
        public string Request_Url(string _appid,string _appsecret)
        {
            using (var client = ConnectionMultiplexer.Connect(Global.REDIS))
            {
                try
                {
                    var db = client.GetDatabase(0);
                    var tokenRedis = db.StringGet("WXToken"+ _appid);
                    if (!tokenRedis.IsNull)
                    {
                        return tokenRedis;
                    }
                    else
                    {
                        // 设置参数
                        string _url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + _appid + "&secret=" + _appsecret;
                        string method = "GET";
                        HttpWebRequest request = WebRequest.Create(_url) as HttpWebRequest;
                        CookieContainer cookieContainer = new CookieContainer();
                        request.CookieContainer = cookieContainer;
                        request.AllowAutoRedirect = true;
                        request.Method = method;
                        request.ContentType = "text/html";
                        request.Headers.Add("charset", "utf-8");

                        //发送请求并获取相应回应数据
                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        //直到request.GetResponse()程序才开始向目标网页发送Post请求
                        Stream responseStream = response.GetResponseStream();
                        StreamReader sr = new StreamReader(responseStream, Encoding.UTF8);
                        //返回结果网页（html）代码
                        string content = sr.ReadToEnd();
                        //由于微信服务器返回的JSON串中包含了很多信息，我们只需要将AccessToken获取就可以了，需要将JSON拆分
                        string[] str = content.Split('"');
                        content = str[3];
                        db.StringSet("WXToken"+ _appid, content,new TimeSpan(1,0,0));
                        return content;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    return "";
                }
            }
        }
        
        public byte[] PostMoths(string _url,string _jso)
        {
            string strURL = _url;
            System.Net.HttpWebRequest request;
            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";

            //string paraUrlCoded = param;
            byte[] payload;
            //payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            payload = System.Text.Encoding.UTF8.GetBytes(_jso);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream s;
            s = response.GetResponseStream();
            byte[] tt = StreamToBytes(s);
            return tt;
        }
        ///将数据流转为byte[]
        public static byte[] StreamToBytes(Stream stream)
        {
            List<byte> bytes = new List<byte>();
            int temp = stream.ReadByte();
            while (temp != -1)
            {
                bytes.Add((byte)temp);
                temp = stream.ReadByte();
            }
            return bytes.ToArray();
        }

        public MsgResult addBunkCode(BankParam bankParam)
        {
            MsgResult msg = new MsgResult();
            string sql = "select id from t_user_list where openId = '"+bankParam.openId+"' and userType='4'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            if (dt.Rows.Count>0)
            {
                string upsql = "update t_user_list set bank = '" + bankParam.bank + "'," +
                                                      "bankName = '" + bankParam.bankName + "'," +
                                                      "bankCardCode = '" + bankParam.bankCardCode + "'," +
                                                      "bankTel = '" + bankParam.bankTel + "'," +
                                                      "bankOperator = '" + bankParam.bankOperator + "' " +
                                                "where id = " + dt.Rows[0][0].ToString();
                DatabaseOperationWeb.ExecuteDML(upsql);
                msg.msg = "添加完成";
                msg.type = "1";
            }
            else
            {
                msg.msg = "未找到openid对应的代理！";
            }
            return msg;
        }
        public BankParam getBunkCode(string openId)
        {
            BankParam bankParam = new BankParam();
            string sql = "select bank,bankName,bankCardCode,bankTel,bankOperator " +
                         "from t_user_list where openId = '" + openId + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            if (dt.Rows.Count > 0)
            {
                bankParam.openId = openId;
                bankParam.bank = dt.Rows[0]["bank"].ToString();
                bankParam.bankName = dt.Rows[0]["bankName"].ToString();
                bankParam.bankCardCode = dt.Rows[0]["bankCardCode"].ToString();
                bankParam.bankTel = dt.Rows[0]["bankTel"].ToString();
                bankParam.bankOperator = dt.Rows[0]["bankOperator"].ToString();
            }
            return bankParam;
        }
    }
    public class Demo
    {
        public string path;
        public int width;
        public bool is_hyaline;
    }
}
