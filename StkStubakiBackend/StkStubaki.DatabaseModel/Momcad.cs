//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StkStubaki.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Momcad
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Momcad()
        {
            this.IgraZzas = new HashSet<IgraZza>();
            this.Natjeces = new HashSet<Natjece>();
            this.Utakmicas = new HashSet<Utakmica>();
            this.Utakmicas1 = new HashSet<Utakmica>();
        }
    
        public int IdMomcad { get; set; }
        public string Naziv { get; set; }
        public Nullable<int> IdVoditelj { get; set; }
    
        public virtual Igrac Igrac { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IgraZza> IgraZzas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Natjece> Natjeces { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Utakmica> Utakmicas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Utakmica> Utakmicas1 { get; set; }
    }
}