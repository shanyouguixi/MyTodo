﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMemo.Common.Model
{
    public class Memo
    {
        public int? id { get; set; }
        public int? tagId { get; set; }
        public int workspaceId { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string color { get; set; }
        public int createTime { get; set; }
        public int updateTime { get; set; }

        public int tagIndex { get; set; }
    }
}
