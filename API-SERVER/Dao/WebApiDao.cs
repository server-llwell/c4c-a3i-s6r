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
    public class WebApiDao
    {
        private string path = System.Environment.CurrentDirectory;

        public WebApiDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        public WebApiGiveWaybillList getWaybillList(GiveWaybillListParam wparam)
        {
            WebApiGiveWaybillList waybillList = new WebApiGiveWaybillList();
            waybillList.userCode = wparam.userCode;
            waybillList.dateFrom = wparam.dateFrom;
            waybillList.dateTo = wparam.dateTo;
            waybillList.waybillList = new List<WebApiGiveWaybill>();
            string sql = "select merchantOrderId,waybillno,waybilltime,expressName " +
                         "from t_webapi_order_list o ,t_base_express e " +
                         "where o.expressId = e.expressId and o.purchaserCode = '"+wparam.userCode+"' " +
                               "and o.tradeTime BETWEEN str_to_date('" + wparam.dateFrom + "', '%Y%m%d%H%i%s')  " +
                                   "AND str_to_date('" + wparam.dateTo + "', '%Y%m%d%H%i%s') ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            if (dt.Rows.Count>0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    WebApiGiveWaybill waybill = new WebApiGiveWaybill();
                    waybill.merchantOrderId = dt.Rows[i]["merchantOrderId"].ToString();
                    waybill.waybillTime = dt.Rows[i]["waybillTime"].ToString();
                    waybill.waybillNo = dt.Rows[i]["waybillNo"].ToString();
                    waybill.expressName = dt.Rows[i]["expressName"].ToString();
                    waybill.status = "1";
                    waybill.remark = "";
                    waybillList.waybillList.Add(waybill);
                }
            }
            return waybillList;
        }

        public ReturnItem sendOrderList(SendOrderListParam wparam)
        {
            ReturnItem returnItem = new ReturnItem();
            string error = "";
            for (int i = 0; i < wparam.orderList.Count; i++)
            {
                string orderError = "";
                //判断订单日期是否正确
                DateTime dtime = DateTime.Now;
                try
                {
                    dtime = Convert.ToDateTime(wparam.orderList[i].tradeTime).AddSeconds(1);
                }
                catch
                {
                    orderError += "订单时间日期格式填写错误，";
                }

                //判断订单是否已经存在
                string sqlno = "select id from t_webapi_order_list where merchantOrderId = '" + wparam.orderList[i].merchantOrderId + "' " +
                    "or  parentOrderId = '" + wparam.orderList[i].merchantOrderId + "'";
                DataTable dtno = DatabaseOperationWeb.ExecuteSelectDS(sqlno, "TABLE").Tables[0];
                if (dtno.Rows.Count > 0)
                {
                    orderError += "订单已存在,";
                }
                

                for (int j = 0; j < wparam.orderList[i].OrderGoods.Count; j++)
                {
                    //判断商品条码是否存在
                    string sqltm = "select id from t_goods_list where barcode = '" + wparam.orderList[i].OrderGoods[j].barCode + "'";
                    DataTable dttm = DatabaseOperationWeb.ExecuteSelectDS(sqltm, "TABLE").Tables[0];
                    if (dttm.Rows.Count == 0)
                    {
                        orderError += "订单商品" + wparam.orderList[i].OrderGoods[j].barCode + "的条码不存在，";
                    }

                    //判断商品数量是否为数字
                    if (wparam.orderList[i].OrderGoods[j].quantity<=0)
                    {
                        orderError += "订单商品" + wparam.orderList[i].OrderGoods[j].barCode + "的数量填写错误，";
                    }
                }
                if (orderError != "")
                {
                    error += "订单" + wparam.orderList[i].merchantOrderId + orderError+";";
                }
            }
            if (error=="")
            {
                returnItem.code = "1";
                returnItem.serviceTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            else
            {
                returnItem.code = "0";
                returnItem.message = error;
                returnItem.serviceTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            return returnItem;
        }

        public WebApiGetGoodsList getGoodsList(GetGoodsListParam wparam)
        {
            WebApiGetGoodsList getGoodsList = new WebApiGetGoodsList();
            getGoodsList.userCode = wparam.userCode;
            getGoodsList.date = wparam.date;
            getGoodsList.goodsList = new List<WebApiGetGoods>();
            string st = "";
            if (wparam.goodsInfo!=null&& wparam.goodsInfo.Length>0)
            {
                foreach (string barcode in wparam.goodsInfo)
                {
                    st = ",'" + barcode + "'";
                }
                if (st.Length>1)
                {
                    st = st.Substring(1);
                }
            }
            string sql = "select d.barcode,d.goodsName,brand,d.slt,pprice,rprice,w.goodsnum stock " +
                "from t_goods_distributor_price d,t_goods_list g ,t_goods_warehouse w " +
                "where d.barcode = g.barcode and w.barcode = d.barcode and w.wid = d.wid " +
                "and d.usercode = '" + wparam.userCode + "' and d.barcode in ("+st+")";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    WebApiGetGoods goods = new WebApiGetGoods();
                    goods.barcode = dt.Rows[i]["barcode"].ToString();
                    goods.goodsName = dt.Rows[i]["goodsName"].ToString();
                    goods.brand = dt.Rows[i]["brand"].ToString();
                    goods.slt = dt.Rows[i]["slt"].ToString();
                    goods.pprice = Convert.ToDouble( dt.Rows[i]["pprice"]);
                    goods.rprice = Convert.ToDouble(dt.Rows[i]["rprice"]);
                    goods.stock = Convert.ToDouble(dt.Rows[i]["stock"]);
                    getGoodsList.goodsList.Add(goods);
                }
            }
            return getGoodsList;
        }
        public string getSecurityKey(string userCode)
        {
            string sql = "select securityKey from t_user_list where usercode = '" + userCode + "'";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }


    }
}
