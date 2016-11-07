using System;
using System.Collections.Generic;
using System.Text;

namespace ORM.render
{

    public class ListPagingData
    {
        public string url;
        public string urlParams;
        public int total;
        public int limit;
        public int offset;
        public ListPagingData(string url, string urlParams, int total, int limit, int offset)
        {
            this.url = url;
            this.total = total;
            this.offset = offset;
            this.limit = limit;
            this.urlParams = urlParams;
        }
    }

}
