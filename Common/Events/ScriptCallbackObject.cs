using MyTodo.Common.Extendsions;
using MyTodo.Common.Model;
using MyTodo.Common.service;
using MyTodo.Common.service.request;
using MyTodo.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace MyTodo.Common.Events
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class ScriptCallbackObject
    {
        private readonly IEventAggregator aggregator;

        private MemoService memoService;

        public ScriptCallbackObject(IEventAggregator aggregator)
        {
            this.aggregator = aggregator;
            memoService = new MemoService();
        }

        public async void SaveMemo(string arg)
        {
            Memo model = JsonConvert.DeserializeObject<Memo>(arg);
            //string model = JsonConvert.SerializeObject(model);
            JsonObject memo = JsonObject.Parse(arg).AsObject();
            ApiResponse res;
            if (model.id ==null )
            {
                res = await memoService.SaveMemo(memo);

            }
            else
            {
                res = await memoService.UpdateMemo(memo);
            }
            if (res.code == 0)
            {
                MessageBox.Show("保存成功");
                MemosViewModel memoModel = new MemosViewModel(aggregator);
                memoModel.flashMemo();
            }
            else
            {
                MessageBox.Show("保存失败");
            }
        }
    }
}
