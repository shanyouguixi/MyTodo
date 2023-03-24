using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTodo.Common.Model
{
    public class User: BindableBase
    {
        public int id { get; set; }
        public string userName { get; set; }
        public string avatar { get; set;}
        public string email { get; set; }
        public int loginFlag { get; set; }
        public int createTime { get; set; }
        public int updateTime { get; set; }

    }

    public class UserDto
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string newPassword { get; set; }
    }
}
