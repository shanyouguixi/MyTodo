using MyTodo.Common.DialogUtils;
using MyTodo.Common.Events;
using MyTodo.Common.Model;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTodo.Common.Extendsions
{
    public static class DialogExtensions
    {
        /// <summary>
        /// 询问窗口
        /// </summary>
        /// <param name="dialogHost">指定的DialogHost会话主机</param>
        /// <param name="title">标题</param>
        /// <param name="content">询问内容</param>
        /// <param name="dialogHostName">会话主机名称(唯一)</param>
        /// <returns></returns>
        public static async Task<IDialogResult> Question(this IDialogHostService dialogHost,
            string title, string content, string dialogHostName = "Root"
            )
        {
            DialogParameters param = new DialogParameters();
            param.Add("Title", title);
            param.Add("Content", content);
            param.Add("dialogHostName", dialogHostName);
            var dialogResult = await dialogHost.ShowDialog("MsgView", param, dialogHostName);
            return dialogResult;
        }

        /// <summary>
        /// 推送 加载等待消息
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="model"></param>
        public static void UpdateLoading(this IEventAggregator aggregator, UpdateModel model)
        {
            aggregator.GetEvent<UpdateLoadingEvent>().Publish(model);
        }

        /// <summary>
        /// 注册 加载等待消息
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="action"></param>
        public static void Resgiter(this IEventAggregator aggregator, Action<UpdateModel> action)
        {
            aggregator.GetEvent<UpdateLoadingEvent>().Subscribe(action);
        }

        /// <summary>
        /// 注册提示消息 
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="action"></param>
        public static void ResgiterMessage(this IEventAggregator aggregator,
            Action<MessageModel> action, string filterName = "Main")
        {
            aggregator.GetEvent<MessageEvent>().Subscribe(action,
                ThreadOption.PublisherThread, true, (m) =>
                {
                    return m.Filter.Equals(filterName);
                });
        }

        /// <summary>
        /// 发送提示消息
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="message"></param>
        public static void SendMessage(this IEventAggregator aggregator, string message, string filterName = "Main")
        {
            aggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = filterName,
                Message = message,
            });
        }
        /// <summary>
        /// 设置workspace
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="workspace"></param>
        public static void SetWorkspace(this IEventAggregator aggregator, Workspace workspace)
        {
            aggregator.GetEvent<WorkspaceEvent>().Publish(new GlobalData<Workspace>() { Value = workspace});
        }

        /// <summary>
        /// 获取workspace
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="action"></param>
        public static void ResgiterWorkspace(this IEventAggregator aggregator,
           Action<GlobalData<Workspace>> action)
        {
            aggregator.GetEvent<WorkspaceEvent>().Subscribe(action,
                ThreadOption.PublisherThread, true, (m) =>
                {
                    return true;
                });
        }

        /// <summary>
        /// 设置memo
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="memo"></param>
        public static void SetMemo(this IEventAggregator aggregator, Memo memo)
        {
            aggregator.GetEvent<MemoEvent>().Publish(new GlobalData<Memo>() { Value = memo });
        }
        /// <summary>
        /// 获取memo
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="action"></param>
        public static void ResgiterMemo(this IEventAggregator aggregator,
           Action<GlobalData<Memo>> action)
        {
            aggregator.GetEvent<MemoEvent>().Subscribe(action,
                ThreadOption.PublisherThread, true, (m) =>
                {
                    return true;
                });
        }


        public static void SetFlash(this IEventAggregator aggregator, string name)
        {
            aggregator.GetEvent<FlashEvent>().Publish(new FlashModel() { Name = name });
        }

        public static void ResgiterFlash(this IEventAggregator aggregator,
           Action<FlashModel> action)
        {
            aggregator.GetEvent<FlashEvent>().Subscribe(action,
                ThreadOption.PublisherThread, true, (m) =>
                {
                    return true;
                });
        }
    }
}
