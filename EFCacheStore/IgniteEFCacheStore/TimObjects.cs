using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Apache.Ignite.Core.Cache.Configuration;
using Tim.DataAccess;
using Tim.DataAccess.Configuration;

namespace IgniteEFCacheStore
{
    public class TimDbContext : Tim_DB_ContextWrapper
    {
        static TimDbContext()
        {
            EfConfiguration.SetConfiguration(new DatabaseConfiguration
            {
                ServerName = "Tim_TEST_AG.spb.local",
                DatabaseName = "TIM_DB",
                UserName = "sa",
                Password = "123QAZwsx/*-",
                MaxAttemptsRecommitTransactionDatabase = 3,
                RetryRecommitTransactionDatabaseTimeout = 3
            });

        }
        public TimDbContext()
        {
        }

        public static IDbSet<T> GetDbSet<T>(TimDbContext ctx) where T : class
        {
            var result = (IDbSet<T>)typeof(TimDbContext).GetProperties().FirstOrDefault(p => p.Name == typeof(T).Name)?.GetValue(ctx);
            if (result == null)
            {
                throw new Exception($"Could not find DbSet for type {typeof(T)}");
            }
            return result;
        }

        private static int key = int.MinValue;
        private static Dictionary<Type, PropertyInfo> piMap = new Dictionary<Type, PropertyInfo>();
        public static object GetKey<T>(object entity) where T : class
        {
            // TODO!!!: handle complex keys
            PropertyInfo pi = null;
            if (!piMap.TryGetValue(typeof (T), out pi))
            {
                pi = typeof(T).GetProperty("ID");
                piMap[typeof (T)] = pi;
            }
            
            if (pi != null)
            {
                return pi.GetValue(entity);
            }
            else
                //return Guid.NewGuid();
                return key++;
        }

        public static void SetKey<T>(T entity, object key) where T : class
        {
            return;
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    Database.SetInitializer<TimDbContext>(null);
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //    base.OnModelCreating(modelBuilder);
        //}


    }
    public class PaymentPlanReal
    {
        [QuerySqlField]
        public int ID { get; set; }
        [QuerySqlField]
        public int ContractID { get; set; }
        [QuerySqlField]
        public int ContractStatusID { get; set; }
        [QuerySqlField]
        public int MonthID { get; set; }
        [QuerySqlField]
        public int ContractorID { get; set; }
        [QuerySqlField]
        public int PaymentPlanID { get; set; }
        [QuerySqlField]
        public decimal PaymentPlanValue { get; set; }
        [QuerySqlField]
        public int? PaymentPlanSkuID { get; set; }
        [QuerySqlField]
        public decimal? PaymentPlanSkuQuantity { get; set; }
        [QuerySqlField]
        public int? ActualYear { get; set; }
    }
    /*
        [Table("PaymentPlanRealView")]
        [Serializable]
        public partial class PaymentPlanRealView
        {
            [QuerySqlField]
            public int ID { get; set; }
            [QuerySqlField]
            public int ContractID { get; set; }
            [QuerySqlField]
            public int ContractStatusID { get; set; }
            [QuerySqlField]
            public int MonthID { get; set; }
            [QuerySqlField]
            public int ContractorID { get; set; }
            [QuerySqlField]
            public int PaymentPlanID { get; set; }
            public decimal PaymentPlanValue { get; set; }
            public Nullable<int> PaymentPlanSkuID { get; set; }
            public Nullable<decimal> PaymentPlanSkuQuantity { get; set; }
            public Nullable<int> ActualYear { get; set; }
        }
*/
}