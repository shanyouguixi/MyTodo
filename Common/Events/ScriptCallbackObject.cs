using MyMemo.Common.service;
using MyMemo.Common.Extendsions;
using MyMemo.Common.Model;
using MyMemo.Common.service;
using MyMemo.Common.service.request;
using MyMemo.ViewModels;
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

namespace MyMemo.Common.Events
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
            try
            {
                Memo model = JsonConvert.DeserializeObject<Memo>(arg);
                //string model = JsonConvert.SerializeObject(model);
                JsonObject memo = JsonObject.Parse(arg).AsObject();
                ApiResponse res;
                if (model.id == null)
                {
                    res = await memoService.SaveMemo(memo);

                }
                else
                {
                    res = await memoService.UpdateMemo(memo);
                }
                if (res.code == 0)
                {
                    aggregator.SendMessage("保存成功", "Main");
                    aggregator.SetFlash("Memo");
                }
                else
                {
                    aggregator.SendMessage("保存失败", "Main");
                }
            }
            catch(Exception ex)
            {
                aggregator.SendMessage("网络错误", "Main");
            }
        }
    }
}
