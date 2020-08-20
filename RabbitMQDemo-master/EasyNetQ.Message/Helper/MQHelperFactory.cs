using EasyNetQ.Message.Helperr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyNetQ.Message.Helper
{
    public class MQHelperFactory
    {
        public static MQHelper Default() =>
            new MQHelper("default");
    }
}
