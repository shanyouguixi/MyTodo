using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTodo.Common.Model
{
    public class UserResource
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public string url { get; set; }
        public int typeId { get; set; }
        public int userId { get; set; }
    }
}
