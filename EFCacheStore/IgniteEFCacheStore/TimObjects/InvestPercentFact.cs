//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tim.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class InvestPercentFact
    {
        [Key, Column(Order=0)]
        public int ContractorID { get; set; }
        [Key, Column(Order=1)]
        public int MonthID { get; set; }
        public Nullable<decimal> InvestSumBefore { get; set; }
        public Nullable<decimal> InvestSumAfter { get; set; }
        public Nullable<decimal> SelloutSum { get; set; }
        public Nullable<decimal> InvestPercentFact1 { get; set; }
        public Nullable<System.DateTime> LastChangeDateTime { get; set; }
    
        public virtual Contractor Contractor { get; set; }
        public virtual Month Month { get; set; }
    }
}
