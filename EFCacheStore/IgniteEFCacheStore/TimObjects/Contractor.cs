//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tim.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class Contractor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Contractor()
        {
            this.ConditionFact = new HashSet<ConditionFact>();
            this.Contract = new HashSet<Contract>();
            this.InvestPercentFact = new HashSet<InvestPercentFact>();
            this.SellOut = new HashSet<SellOut>();
        }
    
        public int ID { get; set; }
        public Nullable<int> SalePointID { get; set; }
        public Nullable<int> TradeNetID { get; set; }
        public int RegionID { get; set; }
        public int DistributorID { get; set; }
        public Nullable<bool> RouteType { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConditionFact> ConditionFact { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Contract> Contract { get; set; }
        public virtual Distributor Distributor { get; set; }
        public virtual Region Region { get; set; }
        public virtual SalePoint SalePoint { get; set; }
        public virtual TradeNet TradeNet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvestPercentFact> InvestPercentFact { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SellOut> SellOut { get; set; }
    }
}