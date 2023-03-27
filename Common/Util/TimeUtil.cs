using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTodo.Common.Util
{
    public class TimeUtil
    {
        public static int getSecond(DateTime date)
        {
            TimeSpan ts = date - new DateTime(1970, 1, 1, 8, 0, 0, 0);
            return Convert.ToInt32(ts.TotalSeconds);
        }
    }
}
