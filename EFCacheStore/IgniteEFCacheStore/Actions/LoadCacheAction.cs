using System;
using System.Diagnostics;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Compute;

namespace IgniteEFCacheStore.Actions
{
    public class LoadCacheAction:IComputeAction
    {
        public void Invoke()
        {
            Console.WriteLine($"Loading cache {Type.Name}");
            var ignite = Ignition.TryGetIgnite("timtest");
            ReflectionHelper.GetOrCreateCache(ignite, Type);
            var sw = Stopwatch.StartNew();
            //var cache = ignite.GetCache<object, object>(Type.Name);
            //cache.LoadCache(null);
            var cache = ReflectionHelper.GetCache(ignite, Type);
            ReflectionHelper.LoadCache(cache);
            Console.WriteLine($"{ReflectionHelper.GetCacheSize(cache)} {Type.Name}s loaded in {sw.Elapsed} in PID {Process.GetCurrentProcess().Id}");
        }

        public Type Type { get; set; }
    }
}