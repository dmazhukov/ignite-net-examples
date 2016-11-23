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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContractConditionPlan> ContractConditionPlan { get; set; }
        public virtual ContractMatchingCache ContractMatchingCache { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContractMonth> ContractMonth { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContractMonthConditionMatchingCache> ContractMonthConditionMatchingCache { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContractStatusHistory> ContractStatusHistory { get; set; }
        public virtual ContractTotalOutlay ContractTotalOutlay { get; set; }
    }
}
