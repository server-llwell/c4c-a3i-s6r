using API_SERVER.Common;
using API_SERVER.Dao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Buss
{
    public class TicketBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.TicketApi;
        }

        public bool NeedCheckToken()
        {
            return true;
        }
        public object Do_TestAbc(object param,string userId)
        {
            TicketTestParam ticketTestParam = JsonConvert.DeserializeObject<TicketTestParam>(param.ToString());
            if (ticketTestParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            return new { test = ticketTestParam.test };
        }
        public object Do_TicketList(object param,string userId)
        {
            TicketParam ticketParam = JsonConvert.DeserializeObject<TicketParam>(param.ToString());
            if (ticketParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (ticketParam.pageSize == 0)
            {
                ticketParam.pageSize = 10;
            }
            if (ticketParam.current == 0)
            {
                ticketParam.current = 1;
            }
            TicketDao ticketDao = new TicketDao();
            return ticketDao.getTicketList(ticketParam);
        }
        public object Do_Ticket(object param,string userId)
        {
            TicketParam ticketParam = JsonConvert.DeserializeObject<TicketParam>(param.ToString());
            if (ticketParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (ticketParam.ticketCode==null|| ticketParam.ticketCode=="")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            TicketDao ticketDao = new TicketDao();
            return ticketDao.GetTicket(ticketParam);
        }
        public object Do_UpdateStatus(object param,string userId)
        {
            TicketParam ticketParam = JsonConvert.DeserializeObject<TicketParam>(param.ToString());
            if (ticketParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            if (ticketParam.ticketCode == null || ticketParam.ticketCode == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (ticketParam.status1 == null || ticketParam.status1 == "")
            {
                throw new ApiException(CodeMessage.InterfaceValueError, "InterfaceValueError");
            }
            if (ticketParam.remark1 == null)
            {
                ticketParam.remark1 = "";
            }
            MsgResult msg = new MsgResult();
            TicketDao ticketDao = new TicketDao();
            if(ticketDao.UpdateStatus(ticketParam))
            {
                msg.type = "1";
                msg.msg = "操作成功";
            }
            else
            {
                msg.msg = "操作失败";
            }
            return msg;
        }
    }

    public class TicketTestParam
    {
        public string test;
    }
    public class TicketParam
    {
        public string search;
        public string ticketCode;
        public string status1;
        public string remark1;
        public int current;//多少页
        public int pageSize;//页面显示多少个商品
    }
    
    public class Ticket
    {
        public string id;
        public string openId;
        public string createTime;
        public string img;
        public string ticketCode;
        public string shopName;
        public string status;
        public string remark;
        public List<TicketBrand> ticketModList;
    }
    public class TicketBrand
    {
        public string id;
        public string ticketCode;
        public string brand;
        public string price;
    }
}
