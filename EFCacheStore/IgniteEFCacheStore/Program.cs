using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Compute;
using Apache.Ignite.Linq;
using Remotion.Linq.Clauses;
using Tim.DataAccess;
using Tim.DataAccess.Abstract;

namespace IgniteEFCacheStore
{
    public static class Program
    {
        private static IIgnite _ignite;


        public static void Main(string[] args)
        {
            //InitializeDb();



            var td = new TimDbContext() { Configuration = { ProxyCreationEnabled = false } };
            Console.WriteLine(td.SalePoint.Count());
            Console.WriteLine(td.Month.Count());
            Console.WriteLine(td.PaymentRequest.Count());
            Console.WriteLine(td.Payment.Count());
            Console.WriteLine(td.SellOut.Count());
            Console.WriteLine(td.Contractor.Count());

            var arr = td.ContractMonth.Take(10).ToArray();


            //Environment.SetEnvironmentVariable("IGNITE_H2_DEBUG_CONSOLE", "true");
            var cfg = new IgniteConfiguration
            {
                BinaryConfiguration = new BinaryConfiguration(GetTimTypes())
                {
                    TypeConfigurations = GetTimTypes().Select(
                    t => new BinaryTypeConfiguration(t)
                    {
                        Serializer = new BinaryReflectiveSerializer()
                    }).ToArray()
                },

                GridName = "timtest",
            };
            _ignite = Ignition.TryGetIgnite("timtest");
            bool exists = true;
            if (_ignite == null)
            {
                exists = false;
                _ignite = Ignition.Start(cfg);
            }

            {
                CreateCaches(_ignite);
                Console.WriteLine("\n>>> Example started\n\n");

                if (_caches.All(c => (int)c.Value.GetType().GetMethod("GetSize").Invoke(c.Value, new object[] { null }) == 0))
                {
                    LoadCaches();
                }

                Console.WriteLine("Press Q to quit, L to reload caches, R to run query, any key to display local stats");
                while (true)
                {
                    switch (Console.ReadKey(true).KeyChar)
                    {
                        case 'q':
                            return;
                        case 'l':
                            LoadCaches();
                            break;
                        case 'r':
                            RunQuery2();
                            break;
                        default:
                            PrintStats();
                            break;
                    }
                }
            }
        }
        private static void RunQuery2()
        {
            var sw = Stopwatch.StartNew();

            var cms = GetCache<ContractMonth>().AsCacheQueryable(new QueryOptions { EnableDistributedJoins = true });
            var ms = GetCache<Month>().AsCacheQueryable(new QueryOptions { EnableDistributedJoins = true });
            var q = from cm in cms
                    from m in ms
                        //where cm.Value.MonthID == m.Value.ID 
                        //where m.Value.ActualYear == 2016
                    where m.Value.ID == cm.Value.MonthID && m.Value.ActualYear == 2016
                    //where cm.Value.MonthID == 26
                    select cm.Value;

            Console.WriteLine($"Executing SQL: {(q as ICacheQueryable).GetFieldsQuery().Sql}");

            var cnt = q.Count();
            Console.WriteLine($"{cnt} records in {sw.Elapsed}");
            var arr = q.Take(10).ToArray();
        }

        private static void PrintStats()
        {
            string str = string.Join(",", GetTimTypes().Select(t => $"{t.Name}: {_caches[t].GetType().GetMethod("GetLocalSize").Invoke(_caches[t], new object[] { null })}/{_caches[t].GetType().GetMethod("GetSize").Invoke(_caches[t], new object[] { null })}"));
            Console.WriteLine(str);
        }

        private static Dictionary<Type, object> _caches = new Dictionary<Type, object>();

        private static ICache<int, T> GetCache<T>()
        {
            return (ICache<int, T>)_caches[typeof(T)];
        }

        private static void LoadCache(Type t)
        {
            var cache = _caches[t];
            cache.GetType().GetMethod("LoadCache").Invoke(cache, new object[] { null, null });
        }

        private static void CreateCaches(IIgnite ignite)
        {
            foreach (var t in GetTimTypes())
            {
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
                _caches[t] = cache;
            }
        }

        private static void LoadCaches()
        {
            var sw = Stopwatch.StartNew();
            foreach (var type in GetTimTypes())
            {
                LoadCache(type);
                Console.WriteLine($"{_caches[type].GetType().GetMethod("GetSize").Invoke(_caches[type], new object[] { null })} {type.Name}s loaded in {sw.Elapsed}");
                sw.Restart();
            }

        }

        private static Type[] GetTimTypes()
        {
            //return typeof(Tim_DB_Entities).Assembly.GetTypes().Where(t => t.IsPublic && t.IsClass && !t.IsAbstract
            //&& !t.Name.StartsWith("Asp") && !t.Name.EndsWith("View") && !t.Name.EndsWith("Tim_DB_Entities")
            //&& !t.Name.EndsWith("Repository")).ToArray();
            return new Type[] {typeof(Contractor),typeof(Month),typeof(Payment),typeof(PaymentRequest),typeof(SalePoint),
                typeof(SellOut),typeof(Contract),typeof(ContractMonth),typeof(PaymentPlan)};
        }
    }
}
