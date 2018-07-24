using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace API_SERVER.Common
{
    public class PageResult
    {
        public object item;
        public List<Object> list;
        public Page pagination;
    }
    public class Page
    {
        public int current;
        public int total;
        public int pageSize;

        public Page(int current, int pageSize)
        {
            this.current = current;
            this.pageSize = pageSize;
        }
    }
}
