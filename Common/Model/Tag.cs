using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTodo.Common.Model
{
    public class Tag
    {
        public int id { get; set; }
        public string name { get; set; }

        public string color { get; set; }
        public int sort { get; set; }
        public int createTime { get; set; }
        public int updateTime { get; set; }
    }
}
