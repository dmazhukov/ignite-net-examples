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
        //public TimDbContext() : base(
        //    new SqlConnectionStringBuilder
        //    {
        //        DataSource = "Tim_TEST_AG.spb.local",
        //        InitialCatalog = "TIM_DB",
        //        IntegratedSecurity = false,
        //        UserID = "sa",
        //        Password = "123QAZwsx/*-"
        //    }.ConnectionString)

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
            return (IDbSet<T>) typeof (TimDbContext).GetProperties().FirstOrDefault(p => p.Name == typeof(T).Name).GetValue(ctx);
        }

        private static int key = int.MinValue;
        public static object GetKey<T>(object entity) where T : class
        {
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

        //public virtual DbSet<SalePoint> SalePoints { get; set; }
        //public virtual DbSet<PaymentPlanRealView> PaymentPlanRealViews { get; set; }
        //public virtual DbSet<Month> Months { get; set; }
        //public virtual DbSet<SellOut> SellOuts { get; set; }
        //public virtual DbSet<Payment> Payments { get; set; }
        //public virtual DbSet<PaymentRequest> PaymentRequests { get; set; }
        //public virtual DbSet<Contractor> Contractors { get; set; }
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

    [Table("SellOut")]
    [Serializable]
    public partial class SellOut
    {
        [QuerySqlField]
        public int ID { get; set; }
        [QuerySqlField]
        public int ContractorID { get; set; }
        [QuerySqlField]
        public int MonthID { get; set; }
        public Nullable<decimal> ValueFact { get; set; }
        public Nullable<decimal> ValuePlan { get; set; }
        public string Comment { get; set; }

        public virtual Contractor Contractor { get; set; }
        public virtual Month Month { get; set; }
    }

    [Table("SalePoint")]
    [Serializable]
    public partial class SalePoint
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SalePoint()
        {
            this.Contractor = new HashSet<Contractor>();
        }

        [QuerySqlField(IsIndexed = true)]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        [QuerySqlField]
        public Nullable<int> SalePointSegmentID { get; set; }
        [QuerySqlField]
        public int SalePointChannelID { get; set; }
        [QuerySqlField]
        public int RegionID { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        [QuerySqlField(IsIndexed = true)]
        public int DistributorID { get; set; }
        public Nullable<int> TradeNetID { get; set; }
        public bool IsIncludedToRoute { get; set; }
        public Nullable<decimal> InvestmentPercent { get; set; }
        public Nullable<int> JeansID { get; set; }
        public Nullable<int> OriginalID { get; set; }
        public string ContactFullName { get; set; }
        public string ContactPhoneMobile { get; set; }
        public string ContactPhoneStationary { get; set; }
        public Nullable<decimal> DoorsCountOfInBevFridge { get; set; }
        public Nullable<decimal> DoorsCountOfEfesSabFridge { get; set; }
        public Nullable<decimal> DoorsCountOfHeinekenFridge { get; set; }
        public Nullable<decimal> DoorsCountOfBalticaFridge { get; set; }
        public Nullable<decimal> DoorsCountOfOthersFridge { get; set; }
        public string CityName { get; set; }
        public string DoorsCountNotBranding { get; set; }
        public int SalePointStatusID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Contractor> Contractor { get; set; }
    }

    [Table("PaymentRequest")]
    [Serializable]
    public partial class PaymentRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PaymentRequest()
        {
            this.Payment = new HashSet<Payment>();
        }

        [QuerySqlField]
        public int ID { get; set; }
        [QuerySqlField]
        public int PaymentPlanID { get; set; }
        public int PaymentRequestTypeID { get; set; }
        public int PaymentRequestStatusID { get; set; }
        public System.DateTime PaymenRequestDate { get; set; }
        public string Comment { get; set; }
        public System.DateTime CreationDateTime { get; set; }
        public int CreationUserID { get; set; }
        public decimal Value { get; set; }
        public Nullable<int> SkuId { get; set; }
        public Nullable<decimal> SkuQuantity { get; set; }
        public Nullable<System.DateTime> LastChangeDateTime { get; set; }
        public Nullable<int> LastChangeUserID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payment { get; set; }
    }


    [Table("Payment")]
    [Serializable]
    public partial class Payment
    {
        [QuerySqlField]
        public int ID { get; set; }
        [QuerySqlField]
        public int PaymentRequestID { get; set; }
        public decimal Value { get; set; }
        public Nullable<int> SkuId { get; set; }
        public Nullable<decimal> SkuQuantity { get; set; }
        public Nullable<System.DateTime> LastChangeDateTime { get; set; }
        public Nullable<int> LastChangeUserID { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public System.DateTime CreationDateTime { get; set; }
        public int PaymentMethodID { get; set; }
        public int CreationUserID { get; set; }

        public virtual PaymentRequest PaymentRequest { get; set; }
    }

    [Table("Month")]
    [Serializable]
    public partial class Month
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Month()
        {
            this.SellOut = new HashSet<SellOut>();
        }

        [QuerySqlField]
        public int ID { get; set; }
        public string Name { get; set; }
        public System.DateTime ActualDate { get; set; }
        public System.DateTime CreationDate { get; set; }
        public Nullable<int> ActualYear { get; set; }
        public Nullable<int> ActualMonth { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SellOut> SellOut { get; set; }
    }


    [Table("Contractor")]
    [Serializable]
    public partial class Contractor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Contractor()
        {
            this.SellOut = new HashSet<SellOut>();
        }

        [QuerySqlField(IsIndexed = true)]
        public int ID { get; set; }
        [QuerySqlField(IsIndexed = true)]
        public Nullable<int> SalePointID { get; set; }
        [QuerySqlField]
        public Nullable<int> TradeNetID { get; set; }
        [QuerySqlField]
        public int RegionID { get; set; }
        [QuerySqlField(IsIndexed = true)]
        public int DistributorID { get; set; }
        public Nullable<bool> RouteType { get; set; }

        public virtual SalePoint SalePoint { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SellOut> SellOut { get; set; }
    }


    public partial class Contract
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Contract()
        {
            this.Contract1 = new HashSet<Contract>();
            this.ContractConditionPlan = new HashSet<ContractConditionPlan>();
            this.ContractMonth = new HashSet<ContractMonth>();
            this.ContractMonthConditionMatchingCache = new HashSet<ContractMonthConditionMatchingCache>();
            this.ContractStatusHistory = new HashSet<ContractStatusHistory>();
        }

        public int ID { get; set; }
        public Nullable<int> CreatedFromContractID { get; set; }
        public int DistributorID { get; set; }
        public bool IsHeinBank { get; set; }
        public int InvestDirectionID { get; set; }
        public System.DateTime ValidFrom { get; set; }
        public System.DateTime ValidTo { get; set; }
        public bool IsAnnul { get; set; }
        public Nullable<System.DateTime> AnnulDate { get; set; }
        public int ContractStatusID { get; set; }
        public int PaymentMethodID { get; set; }
        public int PaymentTermID { get; set; }
        public Nullable<decimal> BonusPercent { get; set; }
        public Nullable<int> PurposeHO { get; set; }
        public int CreationUserId { get; set; }
        public bool IsAuthorized { get; set; }
        public string Comment { get; set; }
        public System.DateTime LastChangeDateTime { get; set; }
        public Nullable<int> ImportContractID { get; set; }
        public bool IsTradeNet { get; set; }
        public string Num { get; set; }
        public Nullable<decimal> InvestPercentPlan { get; set; }
        public Nullable<int> LastChangeUserID { get; set; }
        public System.DateTime CreationDateTime { get; set; }
        public int ContractorID { get; set; }
        public Nullable<decimal> InvestPercentTradeNetPlan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Contract> Contract1 { get; set; }
        public virtual Contract Contract2 { get; set; }
        public virtual Contractor Contractor { get; set; }
        public virtual ContractStatus ContractStatus { get; set; }
        public virtual Distributor Distributor { get; set; }
        public virtual InvestDirection InvestDirection { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual PaymentTerm PaymentTerm { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    
    public partial class Sku
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sku()
        {
            this.Payment = new HashSet<Payment>();
            this.PaymentPlan = new HashSet<PaymentPlan>();
            this.PaymentRequest = new HashSet<PaymentRequest>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentPlan> PaymentPlan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentRequest> PaymentRequest { get; set; }
    }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContractMonth> ContractMonth { get; set; }

    }

    public partial class ContractMonth
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ContractMonth()
        {
            this.PaymentPlan = new HashSet<PaymentPlan>();
        }

        public int ID { get; set; }
        public int ContractID { get; set; }
        public int MonthID { get; set; }

        public virtual Contract Contract { get; set; }
        public virtual Month Month { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentPlan> PaymentPlan { get; set; }
    }


    public partial class PaymentPlan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PaymentPlan()
        {
            this.PaymentRequest = new HashSet<PaymentRequest>();
        }

        public int ID { get; set; }
        public int ContractMonthID { get; set; }
        public decimal Value { get; set; }
        public Nullable<int> SkuId { get; set; }
        public Nullable<decimal> SkuQuantity { get; set; }
        public bool IsReassigned { get; set; }
        public Nullable<System.DateTime> LastChangeDateTime { get; set; }
        public Nullable<int> LastChangeUserID { get; set; }

        public virtual ContractMonth ContractMonth { get; set; }
        public virtual Sku Sku { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentRequest> PaymentRequest { get; set; }
    }

    public partial class Sku
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sku()
        {
            this.Payment = new HashSet<Payment>();
            this.PaymentPlan = new HashSet<PaymentPlan>();
            this.PaymentRequest = new HashSet<PaymentRequest>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentPlan> PaymentPlan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentRequest> PaymentRequest { get; set; }
    }
    */
}