using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMemo.Common.service.request
{
    public class ResponseData<T>
    {
        public int pageNum { get; set; }
        public int pageSize { get; set; }
        public int size { get; set; }
        public int startRow { get; set; }
        public int endRow { get; set; }
        public int total { get; set; }
        public int pages { get; set; }
        public int prePage { get; set; }
        public int nextPage { get; set; }
        public Boolean isFirstPage { get; set; }
        public Boolean isLastPage { get; set; }
        public Boolean hasPreviousPage { get; set; }
        public Boolean hasNextPage { get; set; }
        public int navigatePages { get; set; }
        public List<int> navigatepageNums { get; set; }
        public int navigateFirstPage { get; set; }
        public int navigateLastPage { get; set; }
        public int lastPage { get; set; }
        public int firstPage { get; set; }

        public List<T> list { get; set; }
    }
}
