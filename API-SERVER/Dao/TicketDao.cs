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
    public class TicketDao
    {
        private string path = System.Environment.CurrentDirectory;

        public TicketDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        public PageResult getTicketList(TicketParam ticketParam)
        {
            PageResult ticketResult = new PageResult();
            ticketResult.pagination = new Page(ticketParam.current, ticketParam.pageSize);
            ticketResult.list = new List<Object>();
            string st = "";
            if (ticketParam.search != null && ticketParam.search != "")
            {
                st = "WHERE t.openId like '%" + ticketParam.search + "%' or t.ticketCode  like '%" + ticketParam.search + "%' or shopName like '%" + ticketParam.search + "%'";
            }
            string sql = "SELECT * FROM t_daigou_ticket t " + st + " ORDER BY status asc, id desc LIMIT " + (ticketParam.current - 1) * ticketParam.pageSize + "," + ticketParam.pageSize + ";";

            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_ticket").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Ticket ticket = new Ticket();
                    ticket.id = dt.Rows[i]["id"].ToString();
                    ticket.img = dt.Rows[i]["img"].ToString();
                    ticket.openId = dt.Rows[i]["openId"].ToString();
                    ticket.remark = dt.Rows[i]["remark"].ToString();
                    ticket.shopName = dt.Rows[i]["shopName"].ToString();
                    ticket.status = dt.Rows[i]["status"].ToString();
                    ticket.createTime = dt.Rows[i]["createTime"].ToString();
                    ticket.ticketCode = dt.Rows[i]["ticketCode"].ToString();
                    ticketResult.list.Add(ticket);
                }

            }
            string sql1 = "SELECT (count(*)/" + ticketParam.pageSize + ")+1 FROM t_daigou_ticket t " + st ;

            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_daigou_ticket").Tables[0];
            ticketResult.pagination.total =Convert.ToInt16( dt1.Rows[0][0]);

            return ticketResult;
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
