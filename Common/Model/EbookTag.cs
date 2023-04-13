using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTodo.Common.Model
{
    public class EbookTag
    {
        public int id { get; set; }
        public string tagName { get; set; }

        public int userId { get; set; }
    }
}
