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
    
    public partial class DistributorToRegionAsmBinding
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DistributorToRegionAsmBinding()
        {
            this.Budget = new HashSet<Budget>();
            this.TradeNetToDistributorReagionAsmBinding = new HashSet<TradeNetToDistributorReagionAsmBinding>();
        }
    
        public int ID { get; set; }
        public int DistributorID { get; set; }
        public int RegionAsmID { get; set; }
        public bool IsActual { get; set; }
        public System.DateTime LastChangeDateTime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Budget> Budget { get; set; }
        public virtual Distributor Distributor { get; set; }
        public virtual Region Region { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TradeNetToDistributorReagionAsmBinding> TradeNetToDistributorReagionAsmBinding { get; set; }
    }
}
