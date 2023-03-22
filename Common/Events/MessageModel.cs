using MyMemo.Common.Model;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MyMemo.Common.Events
{
    public class MessageModel
    {
        public string Filter { get; set; }
        public string Message { get; set; }
    }
    public class MessageEvent : PubSubEvent<MessageModel>
    {
    }

    public class GlobalData<T>
    {
        public T Value { get; set; }
    }

    public class WorkspaceEvent : PubSubEvent<GlobalData<Workspace>>
    {
    }

    public class MemoEvent : PubSubEvent<GlobalData<Memo>>
    {
    }

    public class FlashModel
    {
        public string Name { get; set; }
    }
    public class FlashEvent : PubSubEvent<FlashModel>
    {
    }
}
