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
                return "";
            }
        }

        public Ticket GetTicket(TicketParam ticketParam)
        {
            Ticket ticket = new Ticket();
            string sql1 = "select * from t_daigou_ticket where ticketCode  = '" + ticketParam.ticketCode + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_daigou_ticket").Tables[0];
            if (dt.Rows.Count > 0)
            {
                ticket.id = dt.Rows[0]["id"].ToString();
                ticket.img = dt.Rows[0]["img"].ToString();
                ticket.openId = dt.Rows[0]["openId"].ToString();
                ticket.remark = dt.Rows[0]["remark"].ToString();
                ticket.shopName = dt.Rows[0]["shopName"].ToString();
                ticket.status = dt.Rows[0]["status"].ToString();
                ticket.createTime = dt.Rows[0]["createTime"].ToString();
                ticket.ticketCode = dt.Rows[0]["ticketCode"].ToString();
                ticket.ticketModList = new List<TicketBrand>();
                string sql2 = "select * from t_daigou_brand where ticketCode  = '" + ticketParam.ticketCode + "'";
                DataTable dt2 = DatabaseOperationWeb.ExecuteSelectDS(sql2, "t_daigou_ticket").Tables[0];
                if (dt2.Rows.Count > 0)
                {
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        TicketBrand ticketBrand = new TicketBrand();
                        ticketBrand.id = dt2.Rows[i]["id"].ToString();
                        ticketBrand.ticketCode = dt2.Rows[i]["ticketCode"].ToString();
                        ticketBrand.brand = dt2.Rows[i]["brand"].ToString();
                        ticketBrand.price = dt2.Rows[i]["price"].ToString();
                        ticket.ticketModList.Add(ticketBrand);
                    }
                }
            }
            return ticket;
        }

        public bool UpdateStatus(TicketParam ticketParam)
        {
            string sql = "UPDATE t_daigou_ticket set remark ='"+ ticketParam.remark1 + "', status ='"+ ticketParam.status1 + "'" +
                "  WHERE ticketCode ='" + ticketParam.ticketCode + "'";
            return DatabaseOperationWeb.ExecuteDML(sql);
        }
    }
}
