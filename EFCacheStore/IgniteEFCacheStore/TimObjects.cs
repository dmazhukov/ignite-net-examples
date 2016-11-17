using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.SqlClient;
using Apache.Ignite.Core.Cache.Configuration;

namespace IgniteEFCacheStore
{
    public class TimDbContext : DbContext
    {
        public TimDbContext() : base(
            new SqlConnectionStringBuilder
            {
                DataSource = "Tim_TEST_AG.spb.local",
                InitialCatalog = "TIM_DB",
                IntegratedSecurity = false,
                UserID = "sa",
                Password = "123QAZwsx/*-"
            }.ConnectionString)
        {
            // No-op.
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<TimDbContext>(null);
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<SalePoint> SalePoints { get; set; }
        public virtual DbSet<PaymentPlanRealView> PaymentPlanRealViews { get; set; }
        public virtual DbSet<Month> Months { get; set; }
        public virtual DbSet<SellOut> SellOuts { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentRequest> PaymentRequests { get; set; }
        public virtual DbSet<Contractor> Contractors { get; set; }
    }

    [Table("PaymentPlanRealView")]
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
    public partial class SalePoint
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SalePoint()
        {
            this.Contractor = new HashSet<Contractor>();
        }

        [QuerySqlField]
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
    public partial class Contractor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Contractor()
        {
            this.SellOut = new HashSet<SellOut>();
        }

        [QuerySqlField]
        public int ID { get; set; }
        [QuerySqlField]
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



}