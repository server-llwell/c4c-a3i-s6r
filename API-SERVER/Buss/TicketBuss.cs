using API_SERVER.Common;
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

        public object Do_TestAbc(object param)
        {
            TicketTestParam ticketTestParam = JsonConvert.DeserializeObject<TicketTestParam>(param.ToString());
            if (ticketTestParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            return new { test = ticketTestParam.test };
        }
    }

    public class TicketTestParam
    {
        public string test;
    }
}
