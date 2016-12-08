using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Compute;
using IgniteEFCacheStore.Actions;
using IgniteEFCacheStore.Serializers;
using Tim.DataAccess;

namespace IgniteEFCacheStore
{
    public static class IgniteFactory
    {
        private static readonly Dictionary<Type, object> _caches = new Dictionary<Type, object>();
        private static IIgnite _ignite;

        public static void LoadCaches()
        {
            var swTotal = Stopwatch.StartNew();
            //Parallel.ForEach(GetTimTypes(), new ParallelOptions {MaxDegreeOfParallelism = 5}, type =>
            //{
            //    var sw = Stopwatch.StartNew();
            //    LoadCache(type);
            //    Console.WriteLine($"{_caches[type].GetType().GetMethod("GetSize").Invoke(_caches[type], new object[] {null})} {type.Name}s loaded in {sw.Elapsed}");
            //    sw.Restart();
            //});

            var tasks = ReflectionHelper.GetTimTypes().Select(type =>
                  Task.Run(() =>
                  {
                      var sw = Stopwatch.StartNew();
                      LoadCache(type);
                      Console.WriteLine($"{_caches[type].GetType().GetMethod("GetSize").Invoke(_caches[type], new object[] { null })} {type.Name}s loaded in {sw.Elapsed}");
                      sw.Restart();
                  })
                  );
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"All caches loaded in {swTotal.Elapsed}");
        }

        public static ICache<int, T> GetCache<T>()
        {
            //return (ICache<int, T>)_caches[typeof(T)];
            return _ignite.GetCache<int, T>(typeof(T).Name);
        }

        public static void LoadCache(Type t)
        {
            object cache = GetOrCreateCache(t);
            cache.GetType().GetMethod("LoadCache").Invoke(cache, new object[] { null, null });
        }

        public static IIgnite GetIgnite()
        {
            if (_ignite == null)
            {
                _ignite = Ignition.Start(CreateConfiguration());
            }
            return _ignite;
        }

        public static IgniteConfiguration CreateConfiguration()
        {
            return new IgniteConfiguration
            {
                BinaryConfiguration = new BinaryConfiguration(ReflectionHelper.GetTimTypes())
                {
                    TypeConfigurations = ReflectionHelper.GetTimTypes().Select(
                    t =>
                    {
                        if (t == typeof(Month))
                        {
                            return new BinaryTypeConfiguration(typeof(Month))
                            {
                                Serializer = new MonthSerializer()
                            };
                        }

                        if (t == typeof(Contract))
                        {
                            return new BinaryTypeConfiguration(typeof(Contract))
                            {
                                Serializer = new ContractSerializer()
                            };
                        }

                        return new BinaryTypeConfiguration(t)
                        {
                            Serializer = new BinaryReflectiveSerializer()
                        };
                    }).ToArray(),
                },



                GridName = "timtest",
                JvmInitialMemoryMb = Ignition.ClientMode ? IgniteConfiguration.DefaultJvmInitMem : 25000,
                JvmMaxMemoryMb = Ignition.ClientMode ? IgniteConfiguration.DefaultJvmMaxMem : 25000,
                //JvmOptions = new []{
                //"-server",
                //"-Xms25g",
                //"-Xmx25g",
                //"-XX:+UseParNewGC ",
                //"-XX:+UseConcMarkSweepGC ",
                //"-XX:+UseTLAB ",
                //"-XX:NewSize=128m ",
                //"-XX:MaxNewSize=128m ",
                //"-XX:MaxTenuringThreshold=0 ",
                //"-XX:SurvivorRatio=1024 ",
                //"-XX:+UseCMSInitiatingOccupancyOnly ",
                //"-XX:CMSInitiatingOccupancyFraction=60",
                //"-XX:+DisableExplicitGC"},
            };
        }

        public static void CreateLocalCaches()
        {
            //foreach (var t in ReflectionHelper.GetTimTypes())
            //{
            //    _caches[t] = GetOrCreateCache(t);
            //}
        }

        public static object GetOrCreateCache(Type t)
        {
            if (_caches.ContainsKey(t))
                return _caches[t];
            else
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
                        CacheMode = CacheMode.Replicated,
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

                var cache = gm.Invoke(_ignite, param);
                _caches[t] = cache;
                return cache;
            }
        }

        public static async void LoadCachesDistributed()
        {
            var funcs = ReflectionHelper.GetTimTypes().Select(t => new LoadCacheAction { Type = t });
            var sw = Stopwatch.StartNew();
            await IgniteFactory.GetIgnite().GetCompute().RunAsync(funcs);
            Console.WriteLine($"LoadCachesDistributed took {sw.Elapsed}");
            await IgniteFactory.GetIgnite().GetCompute().BroadcastAsync(new PrintMessageAction { Message = $"LoadCachesDistributed took {sw.Elapsed}" });
        }
    }
}