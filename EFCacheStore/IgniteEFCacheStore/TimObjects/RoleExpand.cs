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
    
    public partial class RoleExpand
    {
        [Key, Column(Order=0)]
        public int RoleID { get; set; }
        [Key, Column(Order=1)]
        public int ParentRoleID { get; set; }
        public int Lvl { get; set; }
    
        public virtual Role Role { get; set; }
        public virtual Role Role1 { get; set; }
    }
}