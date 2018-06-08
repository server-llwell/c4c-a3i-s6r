using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Common
{
    public class MsgResult
    {
        public string msg = "";
        public string type = "0";//0:错误； 1：正确 ；-1：跳出倒数
    }
}
