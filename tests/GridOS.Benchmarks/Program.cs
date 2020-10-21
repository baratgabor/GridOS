using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IngameScript;

namespace GridOS.Benchmarks
{
    class Program
    {
        static void Main(string[] _)
        {
            BenchmarkRunner.Run<Benchmarks>();
            Console.ReadKey();
        }
    }

    public class Benchmarks
    {
        private readonly UpdateDispatcher_v1 v1 = new UpdateDispatcher_v1(new NullLogger(), () => UpdateFrequency.None, (_) => { });
        private readonly FastUpdateDispatcher fast = new FastUpdateDispatcher(new NullLogger(), () => UpdateFrequency.None, (_) => { });
        private readonly UpdateDispatcher_v2 v2 = new UpdateDispatcher_v2(new NullLogger(), () => UpdateFrequency.None, (_) => { });

        [GlobalSetup]
        public void GlobalSetup()
        {
            AddModules(v1);
            AddModules(fast);
            AddModules(v2);
        }

        private void AddModules(IUpdateDispatcher updateDispatcher)
        {
            AddSets(4);

            void AddSets(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    updateDispatcher.Add(new NullModule(UpdateFrequency.Update100));
                    updateDispatcher.Add(new NullModule(UpdateFrequency.Update10));
                    updateDispatcher.Add(new NullModule(UpdateFrequency.Update1));
                    updateDispatcher.Add(new NullModule(UpdateFrequency.Once));
                }
            }
        }

        [Benchmark]
        public void Test_UpdateDispatcher_V1()
        {
            v1.Dispatch(UpdateType.Update1);
        }

        [Benchmark]
        public void Test_UpdateDispatcher_Fast()
        {
            fast.Dispatch(UpdateType.Update1);
        }

        [Benchmark]
        public void Test_UpdateDispatcher_V2()
        {
            v2.Dispatch(UpdateType.Update1);
        }

        class NullLogger : IngameScript.ILogger
        {
            public void Log(LogLevel logLevel, string message) { }

            public void Log(LogLevel logLevel, string formatString, object param1, object param2 = null, object param3 = null) { }

            public void Log(LogLevel logLevel, StringBuilder message) { }

            public void Log(LogLevel logLevel, StringSegment message) { }
        }

        class NullModule : IModule, IUpdateSubscriber
        {
            public string ModuleDisplayName => string.Empty;
            public int I;

            public ObservableUpdateFrequency Frequency { get; }

            public NullModule(UpdateFrequency frequency)
            {
                Frequency = new ObservableUpdateFrequency(frequency, this);
            }

            public void Update(UpdateType updateType)
            {
                I++;
            }
        }
    }
}
