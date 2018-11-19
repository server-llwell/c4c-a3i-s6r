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
        public ImportOrderResult importOrder(ImportOrderParam importOrderParam,string json)
        {
            ImportOrderResult importOrderResult = new ImportOrderResult();
            #region 检查项
            Dictionary<string, string> errorDictionary = new Dictionary<string, string>();
            List<OrderItem> OrderItemList = importOrderParam.OrderList;
            foreach(OrderItem orderItem in OrderItemList)
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
            if (msg.type=="1")
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
                "values('importOrder',now(),'"+json.Replace("\r\n","").Replace(" ","")+ "','" + json1 + "')";
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
                    goodsListResultItem.price = Convert.ToDouble( dt.Rows[i]["rprice"]);
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
                    if (dt1.Rows.Count>0)
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
                string sql = "insert into t_wxapp_pagent_member(appId,openId,pagentCode,createTime) " +
                    "values('" + wXAPPParam.appId + "','" + wXAPPParam.openId + "','" + wXAPPParam.pagentCode + "',now())";
                if (DatabaseOperationWeb.ExecuteDML(sql))
                {
                    msgResult.msg = "绑定成功";
                    msgResult.type = "1";
                }
            }
            catch (Exception ex)
            {
                msgResult.msg = "数据库操作失败，请联系管理员！";
            }
            return msgResult;
        }
    }
}
