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
        private static ICache<int, SalePoint> _salePoints;
        private static ICache<int, Month> _months;
        private static ICache<int, PaymentRequest> _paymentRequests;
        private static ICache<int, Payment> _payments;
        private static ICache<int, SellOut> _sellouts;
        private static ICache<int, Contractor> _contractors;
        private static ICache<int, object> _all;
        private static IIgnite _ignite;


        public static void Main(string[] args)
        {
            //InitializeDb();



            var td = new TimDbContext() {Configuration = { ProxyCreationEnabled = false}};
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
                //BinaryConfiguration = new BinaryConfiguration(typeof(SalePoint), typeof(Month),
                //typeof(Payment), typeof(PaymentRequest), typeof(SellOut), typeof(Contractor)),
                //Localhost = "127.0.0.1",
                BinaryConfiguration = new BinaryConfiguration(GetTimTypes()),
                GridName = "timtest"
            };
            //using (var ignite = Ignition.StartFromApplicationConfiguration())
            //using (var ignite = Ignition.TryGetIgnite() ?? Ignition.Start(cfg))
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

                if (_salePoints.GetSize() == 0)
                {
                    LoadCaches();
                }

                //var c = _salePoints.AsCacheQueryable().Where(p => p.Value.RegionID == 193);
                //Console.WriteLine(c.Count());
                //var s = _sellouts.AsCacheQueryable().Where(p => p.Value.MonthID == 44);
                //Console.WriteLine(s.Count());

                var sw = Stopwatch.StartNew();
                var rnd = new Random();
                //for (int i = 0; i < 1000; i++)
                //{
                //    int id = rnd.Next(1, 100000);
                //    var z = td.SellOut.FirstOrDefault(ss => ss.ID == id);
                //}
                //Console.WriteLine($"DB rnd read in {sw.Elapsed}");

                //sw.Restart();
                //for (int i = 0; i < 1000; i++)
                //{
                //    int id = rnd.Next(1, 100000);
                //    Contractor so;
                //    var z = _contractors.TryGet(id, out so);
                //}
                //Console.WriteLine($"Ignite rnd read by index in {sw.Elapsed}");

                //sw.Restart();
                //for (int i = 0; i < 1000; i++)
                //{
                //    int id = rnd.Next(1, 100000);
                //    var z = _sellouts.FirstOrDefault(ss => ss.Key == id);
                //}
                //Console.WriteLine($"Ignite rnd read in {sw.Elapsed}");

                //var q = new SqlFieldsQuery(horribleQuery);
                //var f = _months.QueryFields(q);
                //f.GetAll();
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
            var q = //from cm in cms
                    from m in ms
                    //where cm.Value.MonthID == m.Value.ID 
                    where m.Value.CreationDate==DateTime.UtcNow
                    //where m.Value.ActualYear.HasValue && m.Value.ActualYear == 2016
                    //where cm.Value.MonthID == 26
                    select m.Value;

            Console.WriteLine($"Executing SQL: {(q as ICacheQueryable).GetFieldsQuery().Sql}");

            var cnt = q.Count();
            Console.WriteLine($"{cnt} records in {sw.Elapsed}");
            var arr = q.Take(10).ToArray();
        }

        private static void RunQuery()
        {
            var sw = Stopwatch.StartNew();
            //var q = (from s in _sellouts.AsCacheQueryable()
            //    where
            //        !
            //            (from ss in _sellouts.AsCacheQueryable()
            //                select new
            //                {
            //                    ss.Value.MonthID
            //                }).Contains(new {MonthID = s.Value.MonthID})
            //    select new
            //    {
            //        s.Value
            //    }).Concat
            //    (
            //        from sss in _sellouts.AsCacheQueryable()
            //        select new
            //        {
            //            sss.Value
            //        });

            var q = (/*from so in _sellouts.AsCacheQueryable()*/
                     from sp in _salePoints.AsCacheQueryable(new QueryOptions { EnableDistributedJoins = true })
                     from c in _contractors.AsCacheQueryable(new QueryOptions { EnableDistributedJoins = true })
                     where sp.Key == c.Value.SalePointID
                     select sp);
            ////.Union(_sellouts.AsCacheQueryable()).Except(
            ////from so in _sellouts.AsCacheQueryable()
            ////from sp in _salePoints.AsCacheQueryable()
            ////from c in _contractors.AsCacheQueryable()
            ////where so.Value.ContractorID != c.Value.ID && sp.Value.DistributorID == c.Value.DistributorID
            ////select so);

            ////var q = _sellouts.AsCacheQueryable().Join(_contractors.AsCacheQueryable(), =>)
            //var cnt = q.Count();

            //            var q = new SqlQuery("SalePoint", "from SalePoint, \"contractors\".Contractor as c where SalePoint.ID=c.ID")
            //{
            //    EnableDistributedJoins = true
            //};

            var qq = new SqlFieldsQuery("select SalePoint.ID from SalePoint INNER JOIN \"contractors\".Contractor as c ON SalePoint._key=c.SalepointID")
            {
                EnableDistributedJoins = true
            };

            //var r = _salePoints.Query(q);
            //var r = _salePoints.QueryFields(qq);
            var cnt = q.Count();
            Console.WriteLine($"{cnt} records in {sw.Elapsed}");
        }

        private static void PrintStats()
        {
            Console.WriteLine($"Salepoints:{_salePoints.GetLocalSize()}/{_salePoints.GetSize()}," +
                              $"Payments:{_payments.GetLocalSize()}/{_payments.GetSize()}," +
                              $"PaymentRequestss:{_paymentRequests.GetLocalSize()}/{_paymentRequests.GetSize()}," +
                              $"Sellouts:{_sellouts.GetLocalSize()}/{_sellouts.GetSize()}," +
                              $"Contractors:{_contractors.GetLocalSize()}/{_contractors.GetSize()}," +
                              $"Months:{_months.GetLocalSize()}/{_months.GetSize()}");
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
                var ms = typeof(IIgnite).GetMethods().Where(m => m.Name == "GetOrCreateCache").ToArray();
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

            _salePoints = ignite.GetOrCreateCache<int, SalePoint>(new CacheConfiguration("salePoints", typeof(SalePoint))
            {
                Name = "salePoints",
                CacheStoreFactory = new EntityFrameworkCacheStoreFactory<SalePoint, TimDbContext>(() => new TimDbContext() { Configuration = { ProxyCreationEnabled = false } },
        c => c.SalePoint, p => p.ID, (p, o) => p.ID = (int)o),
                ReadThrough = true,
                WriteThrough = true,
                KeepBinaryInStore = false   // Store works with concrete classes.
            });
            _months = ignite.GetOrCreateCache<int, Month>(new CacheConfiguration("months", typeof(Month))
            {
                Name = "months",
                CacheStoreFactory = new EntityFrameworkCacheStoreFactory<Month, TimDbContext>(() => new TimDbContext() { Configuration = { ProxyCreationEnabled = false } },
                    c => c.Month, p => p.ID, (p, o) => p.ID = (int)o),
                ReadThrough = true,
                WriteThrough = true,
                KeepBinaryInStore = false   // Store works with concrete classes.
            });
            _paymentRequests = ignite.GetOrCreateCache<int, PaymentRequest>(new CacheConfiguration("paymentRequests", typeof(PaymentRequest))
            {
                Name = "paymentRequests",
                CacheStoreFactory = new EntityFrameworkCacheStoreFactory<PaymentRequest, TimDbContext>(() => new TimDbContext() { Configuration = { ProxyCreationEnabled = false } },
                    c => c.PaymentRequest, p => p.ID, (p, o) => p.ID = (int)o),
                ReadThrough = true,
                WriteThrough = true,
                KeepBinaryInStore = false   // Store works with concrete classes.
            });
            _payments = ignite.GetOrCreateCache<int, Payment>(new CacheConfiguration("payments", typeof(Payment))
            {
                Name = "payments",
                CacheStoreFactory = new EntityFrameworkCacheStoreFactory<Payment, TimDbContext>(() => new TimDbContext() { Configuration = { ProxyCreationEnabled = false } },
                    c => c.Payment, p => p.ID, (p, o) => p.ID = (int)o),
                ReadThrough = true,
                WriteThrough = true,
                KeepBinaryInStore = false   // Store works with concrete classes.
            });
            _sellouts = ignite.GetOrCreateCache<int, SellOut>(new CacheConfiguration("sellouts", typeof(SellOut))
            {
                Name = "sellouts",
                CacheStoreFactory = new EntityFrameworkCacheStoreFactory<SellOut, TimDbContext>(() => new TimDbContext() { Configuration = { ProxyCreationEnabled = false } },
                    c => c.SellOut, p => p.ID, (p, o) => p.ID = (int)o),
                ReadThrough = true,
                WriteThrough = true,
                KeepBinaryInStore = false   // Store works with concrete classes.
            });
            _contractors = ignite.GetOrCreateCache<int, Contractor>(new CacheConfiguration("contractors", typeof(Contractor))
            {
                Name = "contractors",
                CacheStoreFactory = new EntityFrameworkCacheStoreFactory<Contractor, TimDbContext>(() => new TimDbContext() { Configuration = { ProxyCreationEnabled = false } },
                    c => c.Contractor, p => p.ID, (p, o) => p.ID = (int)o),
                ReadThrough = true,
                WriteThrough = true,
                KeepBinaryInStore = false   // Store works with concrete classes.
            });

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
            //_salePoints.LoadCache(null);
            //Console.WriteLine($"{_salePoints.GetSize()} salePoints loaded in {sw.Elapsed}");
            //sw.Restart();
            //_months.LoadCache(null);
            //Console.WriteLine($"{_months.GetSize()} months loaded in {sw.Elapsed}");
            //sw.Restart();
            //_paymentRequests.LoadCache(null);
            //Console.WriteLine($"{_paymentRequests.GetSize()} paymentRequests loaded in {sw.Elapsed}");
            //sw.Restart();
            //_payments.LoadCache(null);
            //Console.WriteLine($"{_payments.GetSize()} payments loaded in {sw.Elapsed}");
            //sw.Restart();
            //_sellouts.LoadCache(null);
            //Console.WriteLine($"{_sellouts.GetSize()} sellouts loaded in {sw.Elapsed}");
            //sw.Restart();
            //_contractors.LoadCache(null);
            //Console.WriteLine($"{_contractors.GetSize()} contractors loaded in {sw.Elapsed}");
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
