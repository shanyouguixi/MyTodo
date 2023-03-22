using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMemo.Common.Extendsions
{
    static class UserControlData<T>
    {
        public delegate void PropertyChangedEventHandler(T e);
        static public event PropertyChangedEventHandler propertyChangedHandler;

        static private T data;

        public static T Data { get => data; set => data = value; }
    }
}
