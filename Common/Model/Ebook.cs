using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTodo.Common.Model
{
    public class Ebook
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string path { get; set; }
        public string image { get; set; }
        public string desc { get; set; }
        public int userId { get; set; }
        public int tagId { get; set; }
        public int createTime { get; set; }
        public int updateTime { get; set; }

        public int tagIndex { get; set; }
    }
}
