using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyNetQ.Message.Message
{
    public abstract class BaseMessage
    {
        /// <summary>
        /// 消息推送的模式
        /// 现在支持：订阅模式,推送模式,主题路由模式
        /// </summary>
        public SendEnum SendEnum { get; set; }

        /// <summary>
        /// 管道名称
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// 路由名称
        /// </summary>
        public string RouteName { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;
    }

    public enum SendEnum
    {
        订阅模式 = 1,
        推送模式 = 2,
        主题路由模式 = 3
    }
}
