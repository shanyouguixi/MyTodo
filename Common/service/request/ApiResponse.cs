using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTodo.Common.service.request
{
    public class ApiResponse<T>
    {
        public string msg { get; set; }

        public int code { get; set; }

        public T data { get; set; }

        
    }

    public class ApiResponse
    {
        public string msg { get; set; }

        public int code { get; set; }

        public Object data { get; set; }


    }
}
