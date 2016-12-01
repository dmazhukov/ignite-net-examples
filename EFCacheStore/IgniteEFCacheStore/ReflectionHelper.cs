using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;

namespace IgniteEFCacheStore
{
    [Serializable]
    static class ReflectionHelper
    {
        public static object GetOrCreateCache(IIgnite ignite, Type t)
        {
            Console.WriteLine($"GetOrCreateCache for {t.Name} called in PID {Process.GetCurrentProcess().Id}");
            var method = typeof(IIgnite).GetMethods().FirstOrDefault(m => m.Name == "GetOrCreateCache"
&& m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(CacheConfiguration));
            var gm = method.MakeGenericMethod(typeof(int), t);
            var param = new object[]
            {
                    new CacheConfiguration(t.Name, t)
                    {
                        CacheStoreFactory = new DynamicEntityFrameworkCacheStoreFactory<TimDbContext>(t),
                        ReadThrough = true,
                        WriteThrough = true,
                        KeepBinaryInStore = false, // Store works with concrete classes.
                        QueryEntities = new []
                        {
                            new QueryEntity
                            {
                                KeyType = typeof(int),
                                ValueType = t,
                                Fields = t.GetProperties().Select(p=>new QueryField(p.Name,p.PropertyType)).ToArray(),
                                Indexes = t.GetProperties().Where(p=>p.Name.ToLower().EndsWith("id")).Select(p=>new QueryIndex(p.Name)).ToArray()
                            }
                        }
                    }
            };

            var cache = gm.Invoke(ignite, param);
            Console.WriteLine($"Got cache for {t.Name} with {GetCacheSize(cache)} entries");
            return  cache;
        }

        public static object GetCache(IIgnite ignite, Type t)
        {
            //Console.WriteLine($"GetCache for {t.Name} called in PID {Process.GetCurrentProcess().Id}");
            var method = typeof(IIgnite).GetMethods().FirstOrDefault(m => m.Name == "GetCache"
&& m.GetParameters().Length == 1);
            var gm = method.MakeGenericMethod(typeof(int), t);
            var param = new object[] { t.Name };

            var cache = gm.Invoke(ignite, param);
            Console.WriteLine($"Got cache {cache} for {t.Name}");
            return cache;
        }

        public static int GetCacheSize(object cache)
        {
            return (int)cache.GetType().GetMethod("GetSize").Invoke(cache, new object[] { null });
        }

        public static void LoadCache(object cache)
        {
            Console.WriteLine($"LoadCache for {cache.GetType().Name} called in PID {Process.GetCurrentProcess().Id}");
            cache.GetType().GetMethod("LoadCache").Invoke(cache, new object[] { null, null });
        }
    }
}
