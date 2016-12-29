using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
using LogLevel = Apache.Ignite.Core.Log.LogLevel;

namespace IgniteEFCacheStore
{

    public static class Program
    {
        public static void Main(string[] args)
        {
            Ignition.ClientMode = args.Any(s => s.Contains("client"));

            if (!Ignition.ClientMode)
                Environment.SetEnvironmentVariable("IGNITE_H2_DEBUG_CONSOLE", "true");

            IgniteFactory.GetIgnite();

            if (Ignition.ClientMode)
            {
                IgniteFactory.CreateLocalCaches();
            }

            Console.WriteLine("\n>>> Example started\n\n");
            Console.WriteLine("Press Q to quit, L to reload caches, R to run query, D for DB stress test, S for Ignite stress test, any key to display local stats");

            while (true)
            {
                switch (Console.Read())
                {
                    case 'q':
                        return;
                    case 'l':
                        IgniteFactory.LoadCaches();
                        break;
                    case 'r':
                        RunQueryGetBudgetDataSections();
                        break;
                    case 'p':
                        var actions = new List<Action>();
                        for (int i = 0; i < 10; i++)
                        {
                            actions.Add(RunQueryPPRV);
                        }
                        Parallel.Invoke(actions.ToArray());
                        break;
                    case 's':
                        RunIgniteStressTest();
                        break;
                    case 'd':
                        RunDbStressTest();
                        break;
                    case '\r':
                    case '\n':
                        break;
                    default:
                        PrintStats();
                        break;
                }
            }
        }
        private static void RunQuery2()
        {
            var sw = Stopwatch.StartNew();

            var cms = IgniteFactory.GetCache<ContractMonth>().AsCacheQueryable(new QueryOptions { EnableDistributedJoins = true });
            var ms = IgniteFactory.GetCache<Month>().AsCacheQueryable(new QueryOptions { EnableDistributedJoins = true });
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

        private static void RunQuery()
        {
            var sw = Stopwatch.StartNew();

            //var cfs = GetCache<ConditionFact>().AsCacheQueryable(new QueryOptions { EnableDistributedJoins = true });
            //var cs = GetCache<Condition>().AsCacheQueryable(new QueryOptions { EnableDistributedJoins = true });
            //var q = from cf in cfs
            //        from c in cs
            //        where c.Value.ID == cf.Value.ConditionID
            //        select c.Value.Name;

            var q = from c in IgniteFactory.GetCache<PaymentPlan>().AsCacheQueryable()
                    select c.Value;

            Console.WriteLine($"Executing SQL: {(q as ICacheQueryable).GetFieldsQuery().Sql}");

            var cnt = q.Count();
            Console.WriteLine($"{cnt} records in {sw.Elapsed}");


            var arr = q.Take(10).ToArray();
        }

        private static void RunQueryPPRV()
        {
            var sw = Stopwatch.StartNew();
            var ics = IgniteFactory.GetCache<Contract>().AsCacheQueryable();
            var pps = IgniteFactory.GetCache<PaymentPlan>().AsCacheQueryable();
            var icms = IgniteFactory.GetCache<ContractMonth>().AsCacheQueryable();
            var ims = IgniteFactory.GetCache<Month>().AsCacheQueryable();
            var q = from ic in ics
                    from icm in icms
                    from pp in pps
                    from im in ims

                    where icm.Value.ContractID == ic.Value.ID && pp.Value.ContractMonthID == icm.Value.ID && icm.Value.MonthID == im.Value.ID
                              && (ic.Value.ContractStatusID != 7 || im.Value.ActualDate < ic.Value.AnnulDate)
                              && pp.Value.IsReassigned == false && ic.Value.ContractStatusID != 5
                    select new  { ID = pp.Value.ID, ContractID = ic.Value.ID, ContractStatusID = ic.Value.ContractStatusID, MonthID = icm.Value.MonthID, ContractorID = ic.Value.ContractorID, PaymentPlanID = pp.Value.ID, PaymentPlanValue = pp.Value.Value, PaymentPlanSkuID = pp.Value.SkuId, PaymentPlanSkuQuantity = pp.Value.SkuQuantity, ActualYear = im.Value.ActualYear };

            Console.WriteLine($"Executing SQL: {(q as ICacheQueryable).GetFieldsQuery().Sql}");

            //var cnt = q.Count();
            var arr = q.ToArray();
            var cnt = arr.Length;
            Console.WriteLine($"{cnt} records in {sw.Elapsed}");

        }

        private static void RunQueryGetBudgetDataSections()
        {
            var sw = Stopwatch.StartNew();

            var user = new User {ID = 12021};
            var userId = 12021;
            var regionsToRole = IgniteFactory.GetCache<RegionToRole>().AsCacheQueryable();
            var users = IgniteFactory.GetCache<User>().AsCacheQueryable();
            var currentUserRegionsToRole = users
                .Where(w => w.Value.ID == userId && !w.Value.IsDeleted)
                .Join(regionsToRole, u=>u.Value.ID, r=>r.Value.RoleID,(userr,role) => userr)
                //.SelectMany(sm => sm.Value.RegionToRole);
                .ToList()
                .SelectMany(sm => sm.Value.RegionToRole);



            var q1 = currentUserRegionsToRole.ToList();

            var childRegions = currentUserRegionsToRole
                .SelectMany(rtr => rtr.Region.RegionExpand1)
                .Select(re => re.Region);

            var q2 = childRegions.ToList();

            var regionsAsm = childRegions
                .Where(w => w.RegionToRole.Any(a => a.RoleID == 7));

            var q3 = regionsAsm.ToList();

            var allInvestTitle = IgniteFactory.GetCache<InvestTitle>().AsCacheQueryable();

            var isFutureYear = false;

            var distributorToRegionAsmBindings = !isFutureYear
                ? regionsAsm.SelectMany(s => s.DistributorToRegionAsmBinding)
                : regionsAsm.SelectMany(s => s.DistributorToRegionAsmBinding.Where(b => b.IsActual));

            var q4 = distributorToRegionAsmBindings.ToList();

            var resultAllBudgets = distributorToRegionAsmBindings
                .Select(d => new { Distributor = d.Distributor, RegionAsm = d.Region, d })
                .SelectMany(dr => allInvestTitle, (dr, t) => new { dr.Distributor, dr.RegionAsm, InvestTitle = t, dr.d });

            var q5 = resultAllBudgets.ToList();

            var allBudgets = resultAllBudgets
        .Select(b => new 
        {
            AsmRegionId = b.RegionAsm.ID,
            AsmRegionName = b.RegionAsm.Name,
            DistributorId = b.Distributor.ID,
            DistributorName = b.Distributor.Name,
            RsmRegionId = b.RegionAsm.RegionExpand
                .Where(w => w.Region1.RegionToRole.Any(a => a.RoleID == 5))
                .Select(ss => ss.ParentRegionID)
                .FirstOrDefault(),
            RsmRegionName = b.RegionAsm.RegionExpand
                .Where(w => w.Region1.RegionToRole.Any(a => a.RoleID == 5))
                .Select(ss => ss.Region1.Name)
                .FirstOrDefault(),
            InvestTitleId = b.InvestTitle.Value.ID,
            InvestTitleName = b.InvestTitle.Value.Name,
        });
            var q6 = allBudgets.ToList();
            var q = allBudgets.GroupBy(gb => new { gb.RsmRegionId, gb.RsmRegionName })
                .Select(s => new { RegionId = s.Key.RsmRegionId, RegionName = s.Key.RsmRegionName, Count = s.Count() })
                .ToList();

            Console.WriteLine($"Executing SQL: {(q as ICacheQueryable).GetFieldsQuery().Sql}");

            //var cnt = q.Count();
            var arr = q.ToArray();
            var cnt = arr.Length;
            Console.WriteLine($"{cnt} records in {sw.Elapsed}");

        }

        private static void RunIgniteStressTest()
        {
            Console.WriteLine($"Starting Ignite stress test");
            var sw = Stopwatch.StartNew();
            int counter = 0;
            //foreach (var t in GetTimTypes())
            Parallel.ForEach(ReflectionHelper.GetTimTypes(), t =>
            {
                var cache = IgniteFactory.GetIgnite().GetCache<int, object>(t.Name); ; //GetOrCreateCache(t);
                var rnd = new Random();
                var size = ReflectionHelper.GetCacheSize(cache);
                if (size < 2) return;
                if (t.GetProperty("ID") == null)
                    return;
                if (t.Assembly.GetType($"{t.Name}ComplexPK") != null)
                {
                    return;
                }
                for (int i = 0; i < 1000; i++)
                {
                    var key = rnd.Next(1, size - 1);
                    if (cache.ContainsKey(key))
                    {
                        var item = cache[key];
                        counter++;
                    }
                }
            });
            Console.WriteLine($"Ignite stress test for {counter} items finished in {sw.Elapsed}");
        }

        private static void RunDbStressTest()
        {
            Console.WriteLine($"Starting DB stress test");
            var sw = Stopwatch.StartNew();
            int counter = 0;
            Parallel.ForEach(ReflectionHelper.GetTimTypes(), t =>
            {
                var ctx = new TimDbContext();

                var rnd = new Random();
                var repository = ctx.GetType().GetProperties().FirstOrDefault(p => p.Name == t.Name).GetValue(ctx);
                var cache = IgniteFactory.GetIgnite().GetCache<int, object>(t.Name); ; //GetOrCreateCache(t);
                var size = ReflectionHelper.GetCacheSize(cache);
                if (size < 2) return;
                if (t.GetProperty("ID") == null)
                    return;

                if (t.Assembly.GetType($"{t.Name}ComplexPK") != null)
                {
                    return;
                }

                for (int i = 0; i < 1000; i++)
                {
                    var key = rnd.Next(1, size - 1);
                    try
                    {
                        var item = repository.GetType().GetMethod("Find").Invoke(repository, new object[] { new object[] { key } });
                        if (item != null)
                            counter++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception in {t.Name}");
                        return;
                    }

                }
            });
            Console.WriteLine($"DB stress test for {counter} items finished in {sw.Elapsed}");
        }

        private static void PrintStats()
        {
            string str = string.Join(",", IgniteFactory.GetIgnite().GetCacheNames()
                 .Select(c => IgniteFactory.GetIgnite().GetCache<object, object>(c))
                 .Select(c => $"{c.Name}: {c.GetLocalSize()}/{c.GetSize()}"));
            Console.WriteLine(str);
        }










    }
}
