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
    public class ScanDao
    {
        private string path = System.Environment.CurrentDirectory;

        public ScanDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        public string getGoodsUrl(SCANParam scanParam)
        {
            string barcode = "";
            if (scanParam.barcode.IndexOf(",")>0)
            {
                string[] sts = scanParam.barcode.Split(",");
                if (sts.Count()>1)
                {
                    barcode = sts[1];
                }
                else
                {
                    barcode = sts[0];
                }
            }
            else
            {
                barcode = scanParam.barcode;
            }
            

            string sql = "select * from v_eshop_goods where uniacid = "+scanParam.code+" and merchid = 0 and productsn = '"+barcode+"'";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "TABLE").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string url = "http://eshop.llwell.net/app/index.php?i="+scanParam.code+"&c=entry&m=ewei_shopv2&do=mobile&r=goods.detail&id="+dt.Rows[0]["id"].ToString();
                return url;
            }
            else
            {
                return "http://eshop.llwell.net/app/index.php?i=" + scanParam.code + "&c=entry&m=ewei_shopv2&do=mobile";
            }
        }

       
    }
}
