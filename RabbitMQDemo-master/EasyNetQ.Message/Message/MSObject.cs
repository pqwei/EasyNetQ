﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyNetQ.Message.Message
{
    public class MSObject : BaseMessage
    {
        public string Name { get; set; }
        public int Age { get; set; } = 0;
    }
}
