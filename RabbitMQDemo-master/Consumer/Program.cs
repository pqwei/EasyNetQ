﻿using ConsoleTables;
using EasyNetQ.Message.Helper;
using EasyNetQ.Message.Message;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using Topshelf;

namespace Consumer
{
    public class TownCrier
    {
        private Timer _timer = null;
        public readonly ILog _log = LogManager.GetLogger(typeof(TownCrier));
        public TownCrier()
        {
            _timer = new Timer(1000) { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) => _log.Info(DateTime.Now);
        }
        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            LogManager.GetLogger(typeof(Program)).Info("sdfsd");
            XmlConfigurator.ConfigureAndWatch(logCfg);
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            MQHelperFactory.Default().FanoutConsume(Hanlder());

            HostFactory.Run(x =>
            {
                x.Service<TownCrier>(s =>
                {
                    s.ConstructUsing(name => new TownCrier());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Sample Topshelf Host服务的描述");
                x.SetDisplayName("Stuff显示名称");
                x.SetServiceName("Stuff服务名称");
            });
            ////开头文
            //var styleSheet = new Colorful.StyleSheet(Color.LightGoldenrodYellow);
            //styleSheet.AddStyle("SFLYQ", Color.Yellow);
            //Colorful.Console.WriteLineStyled("rabbitmq test demo from author SFLYQ", styleSheet);
            //Console.WriteLine();
            //Colorful.Console.WriteLineStyled("命令列表：", new Colorful.StyleSheet(Color.PeachPuff));
            //Console.WriteLine();

            ////表格
            //var table = new ConsoleTable("cmd", "exchange model", "type", "advanced", "report http api");
            //table.AddRow("fanout-string", "fanout", "string", "no", "/home/fanout/?handle=string")
            //     .AddRow("fanout-obj", "fanout", "object", "no", "/home/fanout/?handle=obj")
            //     .AddRow("direct-string", "direct", "string", "no", "/home/direct/?handle=push-string")
            //     .AddRow("direct-obj", "direct", "object", "no", "/home/direct/?handle=obj")
            //     .AddRow("direct-consume-string", "direct", "string", "yes", "/home/direct/?handle=string")
            //     .AddRow("topic-string", "topic", "string", "no", "/home/topic/?handle=string")
            //     .AddRow("topic-obj", "topic", "object", "no", "/home/topic/?handle=obj")
            //     .AddRow("topic-consume-string", "topic", "string", "yes", "/home/topic/?handle=sub-string")
            //     .AddRow("fanout-obj2", "fanout", "object", "no", "/home/fanout/?handle=obj");
            //table.Write(Format.MarkDown);

            ////提示
            //Console.WriteLine();
            //Console.WriteLine("输入操作命令:");
            //Console.WriteLine();

            ////接收命令 Test
            //while (true)
            //{
            //    var cmd = Console.ReadLine();
            //    try
            //    {
            //        switch (cmd)
            //        {
            //            //Fanout Exchange 消息接收测试
            //            case "fanout-string": Fanout_Test_String(); break;
            //            case "fanout-obj": Fanout_Test_Obj(); break;
            //            //Direct Exchange 消息接收测试
            //            case "direct-string": Direct_Receive_Test_String(); break;
            //            case "direct-obj": Direct_Receive_Test_Obj(); break;
            //            case "direct-consume-obj": Direct_Consume_Test_Obj(); break;
            //            //Topic Exchange 消息接收测试
            //            case "topic-string": Topic_Test_String(); break;
            //            case "topic-obj": Topic_Test_String(); break;
            //            case "topic-consume-string": Topic_Consume_String(); break;
            //            case "fanout-obj2": MQHelperFactory.Default().FanoutConsume<TestMessage>(o => Console.WriteLine(o.Name)); break;
            //            default: Console.WriteLine($"what ?  "); continue;
            //        }
            //        Console.WriteLine($"{cmd},start testing ... ... ");
            //        Console.WriteLine();
            //        Console.WriteLine("可以继续输入:");
            //        Console.WriteLine();
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"{cmd},err:{ex.ToString()} ");
            //    }
            //}
        }

        private static Action<TestMessage> Hanlder()
        {
            return o =>
            {
                Console.WriteLine(o.Name);
                LogManager.GetLogger(typeof(Program)).Info(o.Name);
            };
        }

        #region "Fanout Exchange 消息消耗 Test"
        /// <summary>
        /// Fount Exchange Test object
        /// </summary>
        public static void Fanout_Test_String()
        {
            MQHelperFactory.Default().FanoutConsume<string>((mqStr) =>
            {
                Console.WriteLine($"[fanout-string]=> {mqStr}");
            });

        }
        /// <summary>
        /// Fount Exchange Test string
        /// </summary>
        public static void Fanout_Test_Obj()
        {
            MQHelperFactory.Default().FanoutConsume<MSObject>((mqObj) =>
            {
                Console.WriteLine($"[fanout-object]=> Name={mqObj.Name},Age={mqObj.Age},AddTime={mqObj.CreateTime.ToString()}");
            });

        }
        #endregion

        #region "Direct Exchange 消息消耗 Test"
        /// <summary>
        /// Direct Exchange Test string
        /// </summary>
        public static void Direct_Receive_Test_String()
        {
            var msg = string.Empty;
            var isSc = MQHelperFactory.Default().DirectReceive<string>("direct_mq", (str) =>
            {
                Console.WriteLine($"[direct-string]=> {str}");
            }, out msg);
            if (!string.IsNullOrWhiteSpace(msg)) Console.WriteLine(msg);

        }

        /// <summary>
        /// Direct Exchange Test object
        /// </summary>
        public static void Direct_Receive_Test_Obj()
        {
            var msg = string.Empty;
            var isSc = MQHelperFactory.Default().DirectReceive<MSObject>("direct_mq", (mqObj) =>
            {
                Console.WriteLine($"[direct-object]=> Name={mqObj.Name},Age={mqObj.Age},AddTime={mqObj.CreateTime.ToString()}");
            }, out msg);
            if (!string.IsNullOrWhiteSpace(msg)) Console.WriteLine(msg);

        }

        /// <summary>
        /// Direct Exchange Test object
        /// </summary>
        public static void Direct_Consume_Test_Obj()
        {
            var msg = string.Empty;
            MQHelperFactory.Default().DirectConsume<MSObject>((mqObj) =>
            {
                Console.WriteLine($"[direct-consume-object]=> Name={mqObj.Name},Age={mqObj.Age},AddTime={mqObj.CreateTime.ToString()}");
            }, out msg);
            if (!string.IsNullOrWhiteSpace(msg)) Console.WriteLine(msg);

        }

        #endregion

        #region "Topic Exchange 消息消耗 Test"
        public static void Topic_Test_String()
        {
            MQHelperFactory.Default().TopicSubscribe<string>("MyTopic", (info) =>
            {
                Console.WriteLine($"[topic-string]=> {info}");
            }, "A.*", "B.*", "C.*");

        }

        public static void Topic_Test_Obj()
        {
            MQHelperFactory.Default().TopicSubscribe<MSObject>("MyTopic", (mqObj) =>
            {
                Console.WriteLine($"[topic-object]=> Name={mqObj.Name},Age={mqObj.Age},AddTime={mqObj.CreateTime.ToString()}");
            }, "A.*", "B.*", "C.*");

        }

        public static void Topic_Consume_String()
        {
            MQHelperFactory.Default().TopicConsume<string>((mqStr) =>
            {
                Console.WriteLine($"[topic-consume-string]=> {mqStr}");
            }, topics: new string[] { "A.*", "B.*", "X.*" });
        }
        #endregion

    }
}
