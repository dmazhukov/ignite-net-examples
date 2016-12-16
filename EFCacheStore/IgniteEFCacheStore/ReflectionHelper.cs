using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Tim.DataAccess;

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
                                //Indexes = new []{new QueryIndex (t.GetProperties().Where(p => p.Name.ToLower().EndsWith("id")).Select(p =>p.Name).ToArray()) { Name = "all_idx", IndexType = QueryIndexType.Sorted } }
                            }
                        }
                    }
            };

            var cache = gm.Invoke(ignite, param);
            Console.WriteLine($"Got cache for {t.Name} with {GetCacheSize(cache)} entries");
            return cache;
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

        public static Type[] GetTimTypes()
        {
            return typeof(Tim_DB_Entities).Assembly.GetTypes().Where(t => t.IsPublic && t.IsClass && !t.IsAbstract
            && !t.Name.StartsWith("Asp") && !t.Name.EndsWith("View") && !t.Name.EndsWith("Tim_DB_Entities")
            && !t.Name.EndsWith("Repository") && !t.Name.EndsWith("ComplexPK") && !t.Name.EndsWith("Wrapper")
            && !t.Name.EndsWith("Configuration") && !t.Name.EndsWith("_test") && !t.Name.EndsWith("Extension")
            && !t.Name.EndsWith("LogWorker") && !t.Name.EndsWith("Factory")&& !t.Name.EndsWith("Provider")
            && !t.Name.EndsWith("Policy")).ToArray();
            //return new Type[] { typeof(Month) };
            //return new Type[] {typeof(Contractor),typeof(Month),typeof(Payment),typeof(PaymentRequest),typeof(SalePoint),
            //    typeof(SellOut),typeof(Contract),typeof(ContractMonth),typeof(PaymentPlan)};
        }

        public static Type[] GetTimViewTypes()
        {
            return typeof(Tim_DB_Entities).Assembly.GetTypes().Where(t => t.IsPublic && t.IsClass && !t.IsAbstract
            && t.Name.EndsWith("View")).ToArray();
        }
    }
}
