using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;

namespace API_SERVER.Dao
{
    public class CallBackDao
    {
        public CallBackDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        public bool  checkOrderTotalPrice(string fundId ,double fundprice, string paytime, string payid,string openId)
        {
            StringBuilder selectBuilder = new StringBuilder();
            selectBuilder.AppendFormat("select count(*) from t_user_fund where fundId='{0}' and fundprice='{1}'", fundId,Math.Round(fundprice/100,2));
            string select = selectBuilder.ToString();
            DataTable dataTable = DatabaseOperationWeb.ExecuteSelectDS(select,"T").Tables[0];

            StringBuilder updateBuilder = new StringBuilder();
            updateBuilder.AppendFormat("update t_user_fund set paytime='{0}',payid='{1}',`status`='1' where fundId='{2}' ", paytime, payid, fundId);
            string updatet_user_fund = updateBuilder.ToString();
            ArrayList arrayList = new ArrayList();
            
            if (dataTable.Rows[0][0] != DBNull.Value && Convert.ToInt16(dataTable.Rows[0][0]) == 1 )
            {
                StringBuilder insertBuilder = new StringBuilder();
                insertBuilder.AppendFormat("insert into t_log_pay(orderId,payType,payNo,totalPrice,openid,createtime,status)"
                    + " values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", fundId, "微信支付", payid, Math.Round(fundprice / 100, 2), openId, paytime, "支付完成-成功");
                string insertt_log_pay = insertBuilder.ToString();
                arrayList.Add(updatet_user_fund);
                arrayList.Add(insertt_log_pay);
                DatabaseOperationWeb.ExecuteDML(arrayList);
                return true;
            }
            else
            {
                StringBuilder insertBuilder1 = new StringBuilder();
                insertBuilder1.AppendFormat("insert into t_log_pay(orderId,payType,payNo,totalPrice,openid,createtime,status)"
                    + " values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", fundId, "微信支付", payid, Math.Round(fundprice / 100, 2), openId, paytime, "支付完成-支付金额与订单总金额不符");
                string insertt_log_pay = insertBuilder1.ToString();
                DatabaseOperationWeb.ExecuteDML(insertt_log_pay);
                return false;
            }
        }

        public void updateOrderForPay( string paytime, string payid)
        {
            StringBuilder updateBuilder = new StringBuilder();
            updateBuilder.AppendFormat("update ");
            string select = updateBuilder.ToString();
            DataTable dataTable = DatabaseOperationWeb.ExecuteSelectDS(select, "T").Tables[0];
        }

    }
}
